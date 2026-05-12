using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using System.Security.Claims;

namespace QuanLyDoAn.Controllers.Admin;

[Authorize(Roles = "Giảng viên")]
public class GVController : Controller
{
    private readonly AppDbContext _context;

    public GVController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardStats()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return BadRequest("Invalid User ID");
        }

        var giangVien = await _context.GiangViens.FirstOrDefaultAsync(g => g.UserId == userId);
        if (giangVien == null) return NotFound("Giảng viên profile not found.");

        int svDangHuongDan = giangVien.SoLuongSinhVienDangHuongDan;

        var dsDeTai = await _context.DeTais
            .Where(d => d.GiangVienId == giangVien.Id)
            .ToListAsync();

        int dtChoDuyet = dsDeTai.Count(d => d.TrangThai == "Chờ duyệt");
        int dtHoanThanh = dsDeTai.Count(d => d.TrangThai == "Hoàn thành");

        // Giả sử lấy số tiến độ chờ nhận xét (Ví dụ: chưa có nhận xét)
        var soTienDoMoi = await _context.TienDos
            .Include(t => t.DeTai)
            .Where(t => t.DeTai.GiangVienId == giangVien.Id && string.IsNullOrEmpty(t.NhanXetCuaGiangVien))
            .CountAsync();

        return Json(new {
            svDangHuongDan,
            dtChoDuyet,
            soTienDoMoi,
            soBaoCaoCanNhanXet = soTienDoMoi, // Có thể bằng nhau tùy logic
            dtHoanThanh
        });
    }

    // Hiển thị trang thông báo cho giảng viên
    public IActionResult ThongBao()
    {
        return View();
    }

    public async Task<IActionResult> DanhSachSinhVien()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out int userId))
        {
            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(g => g.UserId == userId);
            if (giangVien != null)
            {
                var dsDeTai = await _context.DeTais
                    .Include(d => d.SinhVien)
                    .Where(d => d.GiangVienId == giangVien.Id && d.SinhVien != null)
                    .OrderByDescending(d => d.NgayTao)
                    .ToListAsync();
                
                return View(dsDeTai);
            }
        }
        return View(new List<DeTai>());
    }

    public async Task<IActionResult> DeTaiChoDuyet()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out int userId))
        {
            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(g => g.UserId == userId);
            if (giangVien != null)
            {
                var dsDeTai = await _context.DeTais
                    .Include(d => d.SinhVien)
                    .Where(d => d.GiangVienId == giangVien.Id && d.TrangThai == "Chờ duyệt")
                    .OrderByDescending(d => d.NgayTao)
                    .ToListAsync();
                
                return View(dsDeTai);
            }
        }
        return View(new List<DeTai>());
    }

    [HttpPost]
    public async Task<IActionResult> DuyetDeTai(int id)
    {
        var deTai = await _context.DeTais.FindAsync(id);
        if (deTai != null)
        {
            deTai.TrangThai = "Đang thực hiện"; // Hoặc trạng thái tiếp theo phù hợp
            deTai.NgayDuyet = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã duyệt đề tài thành công!";
        }
        return RedirectToAction(nameof(DeTaiChoDuyet));
    }

    [HttpPost]
    public async Task<IActionResult> TuChoiDeTai(int id, string lyDoTuChoi)
    {
        var deTai = await _context.DeTais.FindAsync(id);
        if (deTai != null)
        {
            deTai.TrangThai = "Bị từ chối";
            deTai.LyDoTuChoi = lyDoTuChoi;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã từ chối đề tài thành công!";
        }
        return RedirectToAction(nameof(DeTaiChoDuyet));
    }

    public async Task<IActionResult> DeTaiDangHuongDan()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out int userId))
        {
            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(g => g.UserId == userId);
            if (giangVien != null)
            {
                var trangThaiDangHuongDan = new[] { "Đã duyệt", "Đang thực hiện", "Yêu cầu chỉnh sửa", "Đã nộp báo cáo" };
                
                var dsDeTai = await _context.DeTais
                    .Include(d => d.SinhVien)
                    .Where(d => d.GiangVienId == giangVien.Id && trangThaiDangHuongDan.Contains(d.TrangThai))
                    .OrderByDescending(d => d.NgayDuyet)
                    .ToListAsync();
                
                return View(dsDeTai);
            }
        }
        return View(new List<DeTai>());
    }

    public async Task<IActionResult> TienDo()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out int userId))
        {
            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(g => g.UserId == userId);
            if (giangVien != null)
            {
                var dsTienDo = await _context.TienDos
                    .Include(t => t.DeTai)
                    .ThenInclude(d => d.SinhVien)
                    .Where(t => t.DeTai.GiangVienId == giangVien.Id)
                    .OrderByDescending(t => t.NgayCapNhat)
                    .ToListAsync();
                return View(dsTienDo);
            }
        }
        return View(new List<TienDo>());
    }

    [HttpPost]
    public async Task<IActionResult> CapNhatNhanXet(int id, string nhanXetCuaGiangVien)
    {
        var tienDo = await _context.TienDos.FindAsync(id);
        if (tienDo != null)
        {
            tienDo.NhanXetCuaGiangVien = nhanXetCuaGiangVien;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Nhận xét thành công!";
        }
        return RedirectToAction(nameof(TienDo));
    }

    public async Task<IActionResult> LichGap()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out int userId))
        {
            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(g => g.UserId == userId);
            if (giangVien != null)
            {
                var dsLichGap = await _context.LichGaps
                    .Include(l => l.SinhVien)
                    .Where(l => l.GiangVienId == giangVien.Id)
                    .OrderBy(l => l.NgayGioGap)
                    .ToListAsync();
                return View(dsLichGap);
            }
        }
        return View(new List<LichGap>());
    }

    [HttpPost]
    public async Task<IActionResult> DuyetLichGap(int id, string trangThai)
    {
        var lichGap = await _context.LichGaps.FindAsync(id);
        if (lichGap != null)
        {
            lichGap.TrangThai = trangThai; // Chấp nhận / Từ chối / Đổi lịch
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Lịch hẹn đã được đổi sang: {trangThai}";
        }
        return RedirectToAction(nameof(LichGap));
    }

    public async Task<IActionResult> ChamDiem()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out int userId))
        {
            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(g => g.UserId == userId);
            if (giangVien != null)
            {
                var trangThaiDangHuongDan = new[] { "Đã nộp báo cáo", "Hoàn thành" };
                
                var dsDeTai = await _context.DeTais
                    .Include(d => d.SinhVien)
                    .Where(d => d.GiangVienId == giangVien.Id && trangThaiDangHuongDan.Contains(d.TrangThai))
                    .OrderByDescending(d => d.NgayBatDau)
                    .ToListAsync();
                
                return View(dsDeTai);
            }
        }
        return View(new List<DeTai>());
    }

    [HttpPost]
    public async Task<IActionResult> LuuDiemHuongDan(int id, double diemHuongDan)
    {
        var deTai = await _context.DeTais.FindAsync(id);
        if (deTai != null)
        {
            deTai.DiemHuongDan = diemHuongDan;
            if(deTai.TrangThai == "Đã nộp báo cáo") 
            {
                deTai.TrangThai = "Hoàn thành";
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã lưu điểm hướng dẫn và hoàn tất đề tài!";
        }
        return RedirectToAction(nameof(ChamDiem));
    }

    public async Task<IActionResult> DeTaiChiTiet(int id)
    {
        var deTai = await _context.DeTais
            .Include(d => d.SinhVien)
            .Include(d => d.DotDoAn)
            .Include(d => d.HoiDong)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deTai == null)
        {
            return NotFound();
        }

        return View(deTai);
    }

    public async Task<IActionResult> TienDoChiTiet(int id)
    {
        var deTai = await _context.DeTais
            .Include(d => d.SinhVien)
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deTai == null)
        {
            return NotFound();
        }

        // Tùy chọn: lấy tất cả tiến độ của đề tài này và truyền qua model
        var dsTienDo = deTai.TienDos.OrderByDescending(t => t.NgayCapNhat).ToList();

        ViewBag.DeTai = deTai;
        return View(dsTienDo);
    }
}
