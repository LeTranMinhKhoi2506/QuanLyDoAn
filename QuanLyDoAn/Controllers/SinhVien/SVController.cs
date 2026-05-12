using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using System.Security.Claims;

namespace QuanLyDoAn.Controllers.Admin;

[Authorize(Roles = "Sinh viên")]
public class SVController : Controller
{
    private readonly AppDbContext _context;

    public SVController(AppDbContext context)
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

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais
            .Include(d => d.GiangVien)
            .Include(d => d.TienDos)
            .Include(d => d.DotDoAn)
            .FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        if (deTai == null)
        {
            return Json(new
            {
                hasDeTai = false
            });
        }

        // Tính % tiến độ (Ví dụ: Tổng % của các task / số tasks hoặc lấy max tùy logic)
        // Ở đây lấy trung bình cộng làm ví dụ
        int phanTramTienDo = deTai.TienDos.Any() ? (int)deTai.TienDos.Average(t => t.PhanTramHoanThanh) : 0;

        var nhanXetMoiNhat = deTai.TienDos
            .Where(t => !string.IsNullOrEmpty(t.NhanXetCuaGiangVien))
            .OrderByDescending(t => t.NgayCapNhat)
            .Select(t => t.NhanXetCuaGiangVien)
            .FirstOrDefault();

        return Json(new
        {
            hasDeTai = true,
            tenDeTai = deTai.TenDeTai,
            giangVienHuongDan = deTai.GiangVien?.HoTen ?? "Chưa phân công",
            trangThai = deTai.TrangThai,
            phanTramTienDo = phanTramTienDo,
            hanNop = deTai.DotDoAn?.HanNopBaoCao.ToString("dd/MM/yyyy") ?? "Chưa xác định",
            nhanXetMoiNhat = nhanXetMoiNhat ?? "Chưa có nhận xét nào"
        });
    }

    // Hiển thị trang thông báo cho sinh viên
    public IActionResult ThongBao()
    {
        return View();
    }

    // 1. Đăng ký / Đề xuất đề tài
    [HttpGet]
    public async Task<IActionResult> DangKyDeXuat()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var currentDoAn = await _context.DeTais.FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        // Nếu đã có đề tài, không cho đăng ký nữa
        if (currentDoAn != null)
        {
            TempData["Message"] = "Bạn đã đăng ký đề tài rồi.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        // Lấy danh sách giảng viên để chọn
        ViewBag.GiangViens = await _context.GiangViens.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DangKyDeXuat(DeTai model)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var currentDoAn = await _context.DeTais.FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);
        if (currentDoAn != null)
        {
            TempData["Message"] = "Bạn đã đăng ký đề tài rồi.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        if (ModelState.IsValid)
        {
            model.SinhVienId = sinhVien.Id;
            model.TrangThai = "Chờ duyệt";
            model.NgayTao = DateTime.Now;

            // Generate a random MaDeTai or logic based
            model.MaDeTai = "DT" + DateTime.Now.Ticks.ToString().Substring(10); 

            _context.DeTais.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký đề tài thành công, chờ duyệt.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        ViewBag.GiangViens = await _context.GiangViens.ToListAsync();
        return View(model);
    }

    // 2. Đề tài của tôi
    public async Task<IActionResult> DeTaiCuaToi()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais
            .Include(d => d.GiangVien)
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        return View(deTai);
    }

    // 3. Cập nhật tiến độ
    public IActionResult CapNhatTienDo()
    {
        return View();
    }

    // 4. Nộp báo cáo
    public IActionResult NopBaoCao()
    {
        return View();
    }

    // 5. Nhận xét từ giảng viên
    public IActionResult NhanXetTuGV()
    {
        return View();
    }

    // 6. Lịch gặp giảng viên
    public IActionResult LichGapGiangVien()
    {
        return View();
    }
}