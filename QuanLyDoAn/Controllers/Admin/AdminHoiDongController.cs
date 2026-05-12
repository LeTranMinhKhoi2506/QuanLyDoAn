using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using QuanLyDoAn.Services;

namespace QuanLyDoAn.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminHoiDongController : Controller
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notification;

    public AdminHoiDongController(AppDbContext context, NotificationService notification)
    {
        _context = context;
        _notification = notification;
    }

    // 1. Danh sách hội đồng
    public async Task<IActionResult> Manage()
    {
        var list = await _context.HoiDongs
            .Include(h => h.DotDoAn)
            .Include(h => h.ThanhViens).ThenInclude(tv => tv.GiangVien)
            .Include(h => h.DeTaisDuocCham)
            .Where(h => !h.IsDeleted)
            .OrderByDescending(h => h.NgayTao)
            .ToListAsync();
        return View("~/Views/Admin/HoiDong/Manage.cshtml", list);
    }

    // 2. Thêm hội đồng - GET
    public async Task<IActionResult> Create()
    {
        ViewBag.DotDoAns = await _context.DotDoAns.Where(d => !d.DaKhoa).ToListAsync();
        return View("~/Views/Admin/HoiDong/Create.cshtml");
    }

    // 2. Thêm hội đồng - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HoiDong model)
    {
        if (string.IsNullOrWhiteSpace(model.TenHoiDong))
            ModelState.AddModelError("TenHoiDong", "Tên hội đồng không được rỗng");

        var dot = await _context.DotDoAns.FindAsync(model.DotDoAnId);
        if (dot == null)
            ModelState.AddModelError("DotDoAnId", "Đợt đồ án không tồn tại");

        if (!ModelState.IsValid)
        {
            ViewBag.DotDoAns = await _context.DotDoAns.Where(d => !d.DaKhoa).ToListAsync();
            return View("~/Views/Admin/HoiDong/Create.cshtml", model);
        }

        model.NgayTao = DateTime.Now;
        _context.HoiDongs.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thêm hội đồng thành công";
        return RedirectToAction(nameof(Manage));
    }

    // 3. Sửa hội đồng - GET
    public async Task<IActionResult> Edit(int id)
    {
        var hd = await _context.HoiDongs.FindAsync(id);
        if (hd == null || hd.IsDeleted) return NotFound();
        if (hd.DaKhoa) { TempData["Error"] = "Hội đồng đã khóa, không thể sửa"; return RedirectToAction(nameof(Manage)); }

        ViewBag.DotDoAns = await _context.DotDoAns.Where(d => !d.DaKhoa).ToListAsync();
        return View("~/Views/Admin/HoiDong/Edit.cshtml", hd);
    }

    // 3. Sửa hội đồng - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HoiDong model)
    {
        var hd = await _context.HoiDongs.FindAsync(id);
        if (hd == null || hd.IsDeleted) return NotFound();
        if (hd.DaKhoa) { TempData["Error"] = "Hội đồng đã khóa, không thể sửa"; return RedirectToAction(nameof(Manage)); }

        if (string.IsNullOrWhiteSpace(model.TenHoiDong))
            ModelState.AddModelError("TenHoiDong", "Tên hội đồng không được rỗng");

        var dot = await _context.DotDoAns.FindAsync(model.DotDoAnId);
        if (dot == null)
            ModelState.AddModelError("DotDoAnId", "Đợt đồ án không tồn tại");

        if (!ModelState.IsValid)
        {
            ViewBag.DotDoAns = await _context.DotDoAns.Where(d => !d.DaKhoa).ToListAsync();
            return View("~/Views/Admin/HoiDong/Edit.cshtml", model);
        }

        hd.TenHoiDong = model.TenHoiDong;
        hd.MoTa = model.MoTa ?? string.Empty;
        hd.DotDoAnId = model.DotDoAnId;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật hội đồng thành công";
        return RedirectToAction(nameof(Manage));
    }

    // 4. Chi tiết hội đồng
    public async Task<IActionResult> Details(int id)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.DotDoAn)
            .Include(h => h.ThanhViens).ThenInclude(tv => tv.GiangVien)
            .Include(h => h.DeTaisDuocCham).ThenInclude(dt => dt.SinhVien)
            .Include(h => h.DeTaisDuocCham).ThenInclude(dt => dt.GiangVien)
            .Include(h => h.LichBaoVes).ThenInclude(l => l.DeTai)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);

        if (hd == null) return NotFound();
        return View("~/Views/Admin/HoiDong/Details.cshtml", hd);
    }

    // 5. Xóa mềm hội đồng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var hd = await _context.HoiDongs.FindAsync(id);
        if (hd == null || hd.IsDeleted) return NotFound();

        hd.IsDeleted = true;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã xóa hội đồng";
        return RedirectToAction(nameof(Manage));
    }

    // 6 & 7. Thêm thành viên + phân vai trò - GET
    public async Task<IActionResult> QuanLyThanhVien(int id)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.ThanhViens).ThenInclude(tv => tv.GiangVien)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (hd == null) return NotFound();
        if (hd.DaKhoa) { TempData["Error"] = "Hội đồng đã khóa"; return RedirectToAction(nameof(Details), new { id }); }

        ViewBag.GiangViens = await _context.GiangViens
            .Where(g => g.TrangThai == "Đang hoạt động")
            .ToListAsync();
        return View("~/Views/Admin/HoiDong/QuanLyThanhVien.cshtml", hd);
    }

    // 6 & 7. Thêm thành viên - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemThanhVien(int hoiDongId, int giangVienId, string vaiTro)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.ThanhViens)
            .FirstOrDefaultAsync(h => h.Id == hoiDongId && !h.IsDeleted);
        if (hd == null) return NotFound();
        if (hd.DaKhoa) { TempData["Error"] = "Hội đồng đã khóa"; return RedirectToAction(nameof(QuanLyThanhVien), new { id = hoiDongId }); }

        var gv = await _context.GiangViens.FindAsync(giangVienId);
        if (gv == null || gv.TrangThai != "Đang hoạt động")
        {
            TempData["Error"] = "Giảng viên không hợp lệ hoặc không đang hoạt động";
            return RedirectToAction(nameof(QuanLyThanhVien), new { id = hoiDongId });
        }

        if (hd.ThanhViens.Any(tv => tv.GiangVienId == giangVienId))
        {
            TempData["Error"] = "Giảng viên đã có trong hội đồng";
            return RedirectToAction(nameof(QuanLyThanhVien), new { id = hoiDongId });
        }

        if (vaiTro == "Chủ tịch" && hd.ThanhViens.Any(tv => tv.VaiTro == "Chủ tịch"))
        {
            TempData["Error"] = "Hội đồng đã có Chủ tịch";
            return RedirectToAction(nameof(QuanLyThanhVien), new { id = hoiDongId });
        }

        if (vaiTro == "Thư ký" && hd.ThanhViens.Any(tv => tv.VaiTro == "Thư ký"))
        {
            TempData["Error"] = "Hội đồng đã có Thư ký";
            return RedirectToAction(nameof(QuanLyThanhVien), new { id = hoiDongId });
        }

        _context.HoiDongThanhViens.Add(new HoiDongThanhVien
        {
            HoiDongId = hoiDongId,
            GiangVienId = giangVienId,
            VaiTro = vaiTro
        });
        await _context.SaveChangesAsync();
        await _notification.ThongBaoThemVaoHoiDong(giangVienId, hd.TenHoiDong);
        await _notification.ThongBaoHoiDongThayDoi(hoiDongId, $"Hội đồng \"{hd.TenHoiDong}\" vừa có thành viên mới.");

        TempData["Success"] = "Thêm thành viên thành công";
        return RedirectToAction(nameof(QuanLyThanhVien), new { id = hoiDongId });
    }

    // Xóa thành viên khỏi hội đồng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaThanhVien(int thanhVienId, int hoiDongId)
    {
        var hd = await _context.HoiDongs.FindAsync(hoiDongId);
        if (hd == null || hd.DaKhoa) { TempData["Error"] = "Không thể xóa thành viên"; return RedirectToAction(nameof(QuanLyThanhVien), new { id = hoiDongId }); }

        var tv = await _context.HoiDongThanhViens.FindAsync(thanhVienId);
        if (tv == null) return NotFound();

        _context.HoiDongThanhViens.Remove(tv);
        await _context.SaveChangesAsync();
        await _notification.ThongBaoHoiDongThayDoi(hoiDongId, $"Thành phần hội đồng vừa được thay đổi.");

        TempData["Success"] = "Đã xóa thành viên";
        return RedirectToAction(nameof(QuanLyThanhVien), new { id = hoiDongId });
    }

    // 8. Gán đề tài vào hội đồng - GET
    public async Task<IActionResult> GanDeTai(int id)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.DotDoAn)
            .Include(h => h.DeTaisDuocCham)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (hd == null) return NotFound();
        if (hd.DaKhoa) { TempData["Error"] = "Hội đồng đã khóa"; return RedirectToAction(nameof(Details), new { id }); }

        var deTaiCoThe = await _context.DeTais
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .Where(d => d.DotDoAnId == hd.DotDoAnId
                && !d.IsDeleted
                && (d.TrangThai == "Hoàn thành" || d.TrangThai == "Đã nộp báo cáo")
                && d.HoiDongId == null)
            .ToListAsync();

        ViewBag.HoiDong = hd;
        return View("~/Views/Admin/HoiDong/GanDeTai.cshtml", deTaiCoThe);
    }

    // 8. Gán đề tài - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GanDeTaiPost(int hoiDongId, int deTaiId)
    {
        var hd = await _context.HoiDongs.FindAsync(hoiDongId);
        if (hd == null || hd.IsDeleted || hd.DaKhoa)
        {
            TempData["Error"] = "Hội đồng không hợp lệ hoặc đã khóa";
            return RedirectToAction(nameof(GanDeTai), new { id = hoiDongId });
        }

        var dt = await _context.DeTais.FindAsync(deTaiId);
        if (dt == null || dt.IsDeleted)
        {
            TempData["Error"] = "Đề tài không tồn tại";
            return RedirectToAction(nameof(GanDeTai), new { id = hoiDongId });
        }

        if (dt.DotDoAnId != hd.DotDoAnId)
        {
            TempData["Error"] = "Đề tài không thuộc cùng đợt đồ án với hội đồng";
            return RedirectToAction(nameof(GanDeTai), new { id = hoiDongId });
        }

        if (dt.TrangThai != "Hoàn thành" && dt.TrangThai != "Đã nộp báo cáo")
        {
            TempData["Error"] = "Đề tài phải ở trạng thái Hoàn thành hoặc Đã nộp báo cáo";
            return RedirectToAction(nameof(GanDeTai), new { id = hoiDongId });
        }

        if (dt.HoiDongId != null)
        {
            TempData["Error"] = "Đề tài đã được gán vào hội đồng khác";
            return RedirectToAction(nameof(GanDeTai), new { id = hoiDongId });
        }

        dt.HoiDongId = hoiDongId;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Gán đề tài thành công";
        return RedirectToAction(nameof(GanDeTai), new { id = hoiDongId });
    }

    // Hủy gán đề tài
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HuyGanDeTai(int deTaiId, int hoiDongId)
    {
        var hd = await _context.HoiDongs.FindAsync(hoiDongId);
        if (hd == null || hd.DaKhoa) { TempData["Error"] = "Không thể hủy gán"; return RedirectToAction(nameof(Details), new { id = hoiDongId }); }

        var dt = await _context.DeTais.FindAsync(deTaiId);
        if (dt == null) return NotFound();

        dt.HoiDongId = null;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã hủy gán đề tài";
        return RedirectToAction(nameof(Details), new { id = hoiDongId });
    }

    // 9. Sắp lịch bảo vệ - GET
    public async Task<IActionResult> LichBaoVe(int id)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.DotDoAn)
            .Include(h => h.DeTaisDuocCham).ThenInclude(dt => dt.SinhVien)
            .Include(h => h.LichBaoVes).ThenInclude(l => l.DeTai)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (hd == null) return NotFound();
        if (hd.DaKhoa) { TempData["Error"] = "Hội đồng đã khóa"; return RedirectToAction(nameof(Details), new { id }); }

        return View("~/Views/Admin/HoiDong/LichBaoVe.cshtml", hd);
    }

    // 9. Lưu lịch bảo vệ - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuLichBaoVe(int hoiDongId, int deTaiId, DateTime thoiGianBaoVe, string phongBaoVe, string ghiChu)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.DotDoAn)
            .FirstOrDefaultAsync(h => h.Id == hoiDongId && !h.IsDeleted);
        if (hd == null || hd.DaKhoa)
        {
            TempData["Error"] = "Hội đồng không hợp lệ";
            return RedirectToAction(nameof(LichBaoVe), new { id = hoiDongId });
        }

        if (hd.DotDoAn != null && (thoiGianBaoVe < hd.DotDoAn.NgayBatDau || thoiGianBaoVe > hd.DotDoAn.NgayKetThuc))
        {
            TempData["Error"] = "Ngày bảo vệ phải nằm trong thời gian của đợt đồ án";
            return RedirectToAction(nameof(LichBaoVe), new { id = hoiDongId });
        }

        // Kiểm tra phòng trùng
        var trungPhong = await _context.LichBaoVes
            .AnyAsync(l => l.PhongBaoVe == phongBaoVe && l.ThoiGianBaoVe == thoiGianBaoVe && l.HoiDongId != hoiDongId);
        if (trungPhong)
        {
            TempData["Error"] = "Phòng bảo vệ đã được sử dụng vào thời gian này";
            return RedirectToAction(nameof(LichBaoVe), new { id = hoiDongId });
        }

        var existing = await _context.LichBaoVes.FirstOrDefaultAsync(l => l.HoiDongId == hoiDongId && l.DeTaiId == deTaiId);
        if (existing != null)
        {
            existing.ThoiGianBaoVe = thoiGianBaoVe;
            existing.PhongBaoVe = phongBaoVe;
            existing.GhiChu = ghiChu;
        }
        else
        {
            _context.LichBaoVes.Add(new LichBaoVe
            {
                HoiDongId = hoiDongId,
                DeTaiId = deTaiId,
                ThoiGianBaoVe = thoiGianBaoVe,
                PhongBaoVe = phongBaoVe,
                GhiChu = ghiChu
            });
        }

        await _context.SaveChangesAsync();
        var savedLich = await _context.LichBaoVes
            .FirstOrDefaultAsync(l => l.HoiDongId == hoiDongId && l.DeTaiId == deTaiId);
        if (savedLich != null)
            await _notification.ThongBaoLichBaoVe(savedLich.Id);
        TempData["Success"] = "Đã lưu lịch bảo vệ";
        return RedirectToAction(nameof(LichBaoVe), new { id = hoiDongId });
    }

    // 10 & 11. Nhập điểm và nhận xét - GET
    public async Task<IActionResult> NhapDiem(int id)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.ThanhViens).ThenInclude(tv => tv.GiangVien)
            .Include(h => h.DeTaisDuocCham).ThenInclude(dt => dt.SinhVien)
            .Include(h => h.LichBaoVes)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (hd == null) return NotFound();
        if (hd.DaKhoa) { TempData["Error"] = "Hội đồng đã khóa, không thể nhập điểm"; return RedirectToAction(nameof(Details), new { id }); }

        var dsDiem = await _context.DiemHoiDongs
            .Where(d => d.HoiDongId == id)
            .ToListAsync();

        ViewBag.DsDiem = dsDiem;
        return View("~/Views/Admin/HoiDong/NhapDiem.cshtml", hd);
    }

    // 10 & 11. Lưu điểm - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuDiem(int hoiDongId, int deTaiId, int giangVienId,
        double diemBaoCao, double diemThuyetTrinh, double diemSanPham, double diemTraLoiCauHoi, string nhanXet)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.ThanhViens)
            .Include(h => h.LichBaoVes)
            .FirstOrDefaultAsync(h => h.Id == hoiDongId && !h.IsDeleted);
        if (hd == null) return NotFound();
        if (hd.DaKhoa) { TempData["Error"] = "Hội đồng đã khóa"; return RedirectToAction(nameof(NhapDiem), new { id = hoiDongId }); }

        if (!hd.ThanhViens.Any(tv => tv.GiangVienId == giangVienId))
        {
            TempData["Error"] = "Chỉ thành viên hội đồng mới được nhập điểm";
            return RedirectToAction(nameof(NhapDiem), new { id = hoiDongId });
        }

        var daBaoVe = hd.LichBaoVes.Any(l => l.DeTaiId == deTaiId && l.ThoiGianBaoVe <= DateTime.Now);
        if (!daBaoVe)
        {
            TempData["Error"] = "Chỉ được nhập điểm sau khi đề tài đã bảo vệ";
            return RedirectToAction(nameof(NhapDiem), new { id = hoiDongId });
        }

        double[] scores = { diemBaoCao, diemThuyetTrinh, diemSanPham, diemTraLoiCauHoi };
        if (scores.Any(s => s < 0 || s > 10))
        {
            TempData["Error"] = "Điểm phải nằm trong khoảng 0 đến 10";
            return RedirectToAction(nameof(NhapDiem), new { id = hoiDongId });
        }

        var existing = await _context.DiemHoiDongs
            .FirstOrDefaultAsync(d => d.HoiDongId == hoiDongId && d.DeTaiId == deTaiId && d.GiangVienId == giangVienId);

        if (existing != null)
        {
            existing.DiemBaoCao = diemBaoCao;
            existing.DiemThuyetTrinh = diemThuyetTrinh;
            existing.DiemSanPham = diemSanPham;
            existing.DiemTraLoiCauHoi = diemTraLoiCauHoi;
            existing.NhanXet = nhanXet;
            existing.NgayNhap = DateTime.Now;
        }
        else
        {
            _context.DiemHoiDongs.Add(new DiemHoiDong
            {
                HoiDongId = hoiDongId,
                DeTaiId = deTaiId,
                GiangVienId = giangVienId,
                DiemBaoCao = diemBaoCao,
                DiemThuyetTrinh = diemThuyetTrinh,
                DiemSanPham = diemSanPham,
                DiemTraLoiCauHoi = diemTraLoiCauHoi,
                NhanXet = nhanXet,
                NgayNhap = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã lưu điểm thành công";
        return RedirectToAction(nameof(NhapDiem), new { id = hoiDongId });
    }

    // 12. Xem kết quả chấm
    public async Task<IActionResult> KetQua(int id)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.DotDoAn)
            .Include(h => h.ThanhViens).ThenInclude(tv => tv.GiangVien)
            .Include(h => h.DeTaisDuocCham).ThenInclude(dt => dt.SinhVien)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (hd == null) return NotFound();

        var dsDiem = await _context.DiemHoiDongs
            .Include(d => d.GiangVien)
            .Include(d => d.DeTai).ThenInclude(dt => dt.SinhVien)
            .Where(d => d.HoiDongId == id)
            .ToListAsync();

        ViewBag.DsDiem = dsDiem;
        return View("~/Views/Admin/HoiDong/KetQua.cshtml", hd);
    }

    // Khóa hội đồng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhoaHoiDong(int id)
    {
        var hd = await _context.HoiDongs
            .Include(h => h.ThanhViens)
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (hd == null) return NotFound();

        if (hd.ThanhViens.Count < 3)
        {
            TempData["Error"] = "Hội đồng phải có ít nhất 3 thành viên trước khi khóa";
            return RedirectToAction(nameof(Details), new { id });
        }

        hd.DaKhoa = true;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã khóa hội đồng";
        return RedirectToAction(nameof(Details), new { id });
    }
}
