using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminDeTaiController : Controller
{
    private readonly AppDbContext _context;

    public AdminDeTaiController(AppDbContext context) => _context = context;

    private static readonly string[] DanhSachTrangThai =
    [
        "Chờ duyệt", "Đã duyệt", "Bị từ chối", "Đang thực hiện",
        "Yêu cầu chỉnh sửa", "Đã nộp báo cáo", "Đã hoàn thành", "Đã hủy"
    ];

    // 1. Danh sách đề tài với tìm kiếm + lọc
    public async Task<IActionResult> Manage(string? keyword, string? trangThai, int? dotDoAnId, int? giangVienId)
    {
        var query = _context.DeTais
            .Where(d => !d.IsDeleted)
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .Include(d => d.DotDoAn)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(d =>
                d.TenDeTai.Contains(keyword) ||
                d.MaDeTai.Contains(keyword) ||
                (d.SinhVien != null && d.SinhVien.HoTen.Contains(keyword)) ||
                (d.GiangVien != null && d.GiangVien.HoTen.Contains(keyword)));

        if (!string.IsNullOrEmpty(trangThai))
            query = query.Where(d => d.TrangThai == trangThai);

        if (dotDoAnId.HasValue)
            query = query.Where(d => d.DotDoAnId == dotDoAnId);

        if (giangVienId.HasValue)
            query = query.Where(d => d.GiangVienId == giangVienId);

        ViewBag.Keyword = keyword;
        ViewBag.TrangThaiFilter = trangThai;
        ViewBag.DotDoAnFilter = dotDoAnId;
        ViewBag.GiangVienFilter = giangVienId;
        ViewBag.DanhSachTrangThai = DanhSachTrangThai;
        ViewBag.DotDoAnList = new SelectList(await _context.DotDoAns.OrderByDescending(d => d.NgayTao).ToListAsync(), "Id", "TenDot", dotDoAnId);
        ViewBag.GiangVienList = new SelectList(await _context.GiangViens.OrderBy(g => g.HoTen).ToListAsync(), "Id", "HoTen", giangVienId);

        return View("~/Views/Admin/DeTai/Manage.cshtml", await query.OrderByDescending(d => d.NgayTao).ToListAsync());
    }

    // 2. Thêm đề tài - GET
    public async Task<IActionResult> Create()
    {
        await LoadViewBagForForm();
        return View("~/Views/Admin/DeTai/Create.cshtml");
    }

    // 2. Thêm đề tài - POST
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DeTai model)
    {
        ModelState.Remove("SinhVien");
        ModelState.Remove("GiangVien");
        ModelState.Remove("DotDoAn");
        ModelState.Remove("HoiDong");
        ModelState.Remove("TienDos");

        ValidateDeTai(model);
        await ValidateDeTaiAsync(model, null);

        if (!ModelState.IsValid)
        {
            await LoadViewBagForForm();
            return View("~/Views/Admin/DeTai/Create.cshtml", model);
        }

        model.TrangThai = "Chờ duyệt";
        model.NgayTao = DateTime.Now;
        model.IsDeleted = false;
        _context.DeTais.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thêm đề tài thành công";
        return RedirectToAction(nameof(Manage));
    }

    // 3. Sửa đề tài - GET
    public async Task<IActionResult> Edit(int id)
    {
        var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        if (deTai == null) return NotFound();

        await LoadViewBagForForm();
        ViewBag.DanhSachTrangThai = DanhSachTrangThai;
        return View("~/Views/Admin/DeTai/Edit.cshtml", deTai);
    }

    // 3. Sửa đề tài - POST
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DeTai model)
    {
        var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        if (deTai == null) return NotFound();

        ModelState.Remove("SinhVien");
        ModelState.Remove("GiangVien");
        ModelState.Remove("DotDoAn");
        ModelState.Remove("HoiDong");
        ModelState.Remove("TienDos");

        ValidateDeTai(model);
        await ValidateDeTaiAsync(model, id);

        if (!ModelState.IsValid)
        {
            await LoadViewBagForForm();
            ViewBag.DanhSachTrangThai = DanhSachTrangThai;
            return View("~/Views/Admin/DeTai/Edit.cshtml", model);
        }

        deTai.TenDeTai = model.TenDeTai;
        deTai.MaDeTai = model.MaDeTai;
        deTai.MoTaDeTai = model.MoTaDeTai;
        deTai.LoaiDeTai = model.LoaiDeTai;
        deTai.CongNgheSuDung = model.CongNgheSuDung;
        deTai.NgayBatDau = model.NgayBatDau;
        deTai.NgayKetThuc = model.NgayKetThuc;
        deTai.SinhVienId = model.SinhVienId;
        deTai.GiangVienId = model.GiangVienId;
        deTai.DotDoAnId = model.DotDoAnId;

        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật đề tài thành công";
        return RedirectToAction(nameof(Details), new { id });
    }

    // 4. Chi tiết đề tài
    public async Task<IActionResult> Details(int id)
    {
        var deTai = await _context.DeTais
            .Where(d => d.Id == id && !d.IsDeleted)
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .Include(d => d.DotDoAn)
            .Include(d => d.HoiDong)
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync();

        if (deTai == null) return NotFound();
        return View("~/Views/Admin/DeTai/Details.cshtml", deTai);
    }

    // 5. Xóa mềm
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deTai = await _context.DeTais
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        if (deTai == null) return NotFound();

        bool coDuLieu = deTai.TienDos.Any() ||
                        deTai.DiemHuongDan.HasValue || deTai.DiemPhanBien.HasValue || deTai.DiemHoiDong.HasValue ||
                        !string.IsNullOrEmpty(deTai.FileBaoCao) || !string.IsNullOrEmpty(deTai.LyDoTuChoi) ||
                        deTai.NgayDuyet.HasValue;

        if (coDuLieu)
        {
            // Xóa mềm
            deTai.IsDeleted = true;
            TempData["Success"] = "Đã xóa mềm đề tài";
        }
        else
        {
            // Xóa cứng nếu chưa có dữ liệu liên quan
            _context.DeTais.Remove(deTai);
            TempData["Success"] = "Đã xóa đề tài";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Manage));
    }

    // 11. Duyệt đề tài (Admin cũng có quyền)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DuyetDeTai(int id)
    {
        var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        if (deTai == null) return NotFound();

        if (deTai.TrangThai != "Chờ duyệt")
        {
            TempData["Error"] = "Chỉ có thể duyệt đề tài đang ở trạng thái 'Chờ duyệt'";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Kiểm tra sinh viên chưa có đề tài đã duyệt trong cùng đợt
        if (deTai.SinhVienId.HasValue && deTai.DotDoAnId.HasValue)
        {
            var daDuyet = await _context.DeTais.AnyAsync(d =>
                d.Id != id && !d.IsDeleted &&
                d.SinhVienId == deTai.SinhVienId &&
                d.DotDoAnId == deTai.DotDoAnId &&
                d.TrangThai == "Đã duyệt");

            if (daDuyet)
            {
                TempData["Error"] = "Sinh viên đã có đề tài được duyệt trong đợt này";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // Kiểm tra giảng viên còn khả năng hướng dẫn
        if (deTai.GiangVienId.HasValue)
        {
            var gv = await _context.GiangViens.FindAsync(deTai.GiangVienId.Value);
            if (gv != null && gv.SoLuongSinhVienDangHuongDan >= gv.GioiHanSinhVien)
            {
                TempData["Error"] = $"Giảng viên {gv.HoTen} đã đạt giới hạn hướng dẫn ({gv.GioiHanSinhVien} SV)";
                return RedirectToAction(nameof(Details), new { id });
            }
            if (gv != null) gv.SoLuongSinhVienDangHuongDan++;
        }

        deTai.TrangThai = "Đã duyệt";
        deTai.NgayDuyet = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã duyệt đề tài thành công";
        return RedirectToAction(nameof(Details), new { id });
    }

    // 11. Từ chối đề tài
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoiDeTai(int id, string lyDoTuChoi)
    {
        if (string.IsNullOrWhiteSpace(lyDoTuChoi))
        {
            TempData["Error"] = "Vui lòng nhập lý do từ chối";
            return RedirectToAction(nameof(Details), new { id });
        }

        var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        if (deTai == null) return NotFound();

        if (deTai.TrangThai != "Chờ duyệt")
        {
            TempData["Error"] = "Chỉ có thể từ chối đề tài đang ở trạng thái 'Chờ duyệt'";
            return RedirectToAction(nameof(Details), new { id });
        }

        deTai.TrangThai = "Bị từ chối";
        deTai.LyDoTuChoi = lyDoTuChoi;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã từ chối đề tài";
        return RedirectToAction(nameof(Details), new { id });
    }

    // 15. Cập nhật trạng thái
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id, string trangThaiMoi)
    {
        var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        if (deTai == null) return NotFound();

        if (!DanhSachTrangThai.Contains(trangThaiMoi))
        {
            TempData["Error"] = "Trạng thái không hợp lệ";
            return RedirectToAction(nameof(Details), new { id });
        }

        deTai.TrangThai = trangThaiMoi;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã cập nhật trạng thái thành '{trangThaiMoi}'";
        return RedirectToAction(nameof(Details), new { id });
    }

    // Phân công giảng viên - GET
    public async Task<IActionResult> PhanCong(int? dotDoAnId, int? giangVienId)
    {
        var query = _context.DeTais
            .Where(d => !d.IsDeleted)
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .Include(d => d.DotDoAn)
            .AsQueryable();

        if (dotDoAnId.HasValue)
            query = query.Where(d => d.DotDoAnId == dotDoAnId);

        if (giangVienId.HasValue)
            query = query.Where(d => d.GiangVienId == giangVienId);

        ViewBag.DotDoAnList = new SelectList(await _context.DotDoAns.OrderByDescending(d => d.NgayTao).ToListAsync(), "Id", "TenDot", dotDoAnId);
        ViewBag.GiangVienList = new SelectList(await _context.GiangViens.Where(g => g.TrangThai == "Đang hoạt động").OrderBy(g => g.HoTen).ToListAsync(), "Id", "HoTen", giangVienId);
        ViewBag.GiangViens = await _context.GiangViens.Where(g => g.TrangThai == "Đang hoạt động").OrderBy(g => g.HoTen).ToListAsync();
        ViewBag.DotDoAnFilter = dotDoAnId;
        ViewBag.GiangVienFilter = giangVienId;

        return View("~/Views/Admin/DeTai/PhanCong.cshtml", await query.OrderByDescending(d => d.NgayTao).ToListAsync());
    }

    // Phân công giảng viên - POST
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PhanCongGiangVien(int deTaiId, int giangVienId)
    {
        var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.Id == deTaiId && !d.IsDeleted);
        if (deTai == null) return NotFound();

        var giangVien = await _context.GiangViens.FindAsync(giangVienId);
        if (giangVien == null) return NotFound();

        if (giangVien.SoLuongSinhVienDangHuongDan >= giangVien.GioiHanSinhVien)
        {
            TempData["Error"] = $"Giảng viên {giangVien.HoTen} đã đạt giới hạn hướng dẫn";
            return RedirectToAction(nameof(PhanCong));
        }

        // Giảm đếm GV cũ
        if (deTai.GiangVienId.HasValue && deTai.GiangVienId != giangVienId)
        {
            var gvCu = await _context.GiangViens.FindAsync(deTai.GiangVienId.Value);
            if (gvCu != null && gvCu.SoLuongSinhVienDangHuongDan > 0)
                gvCu.SoLuongSinhVienDangHuongDan--;
        }

        if (deTai.GiangVienId != giangVienId)
            giangVien.SoLuongSinhVienDangHuongDan++;

        deTai.GiangVienId = giangVienId;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã phân công GV {giangVien.HoTen} cho đề tài: {deTai.TenDeTai}";
        return RedirectToAction(nameof(PhanCong));
    }

    private async Task LoadViewBagForForm()
    {
        ViewBag.SinhVienList = new SelectList(await _context.SinhViens.OrderBy(s => s.HoTen).ToListAsync(), "Id", "HoTen");
        ViewBag.GiangVienList = new SelectList(await _context.GiangViens.Where(g => g.TrangThai == "Đang hoạt động").OrderBy(g => g.HoTen).ToListAsync(), "Id", "HoTen");
        ViewBag.DotDoAnList = new SelectList(await _context.DotDoAns.OrderByDescending(d => d.NgayTao).ToListAsync(), "Id", "TenDot");
    }

    // Validate logic thuần (không async)
    private void ValidateDeTai(DeTai model)
    {
        if (string.IsNullOrWhiteSpace(model.TenDeTai))
            ModelState.AddModelError(nameof(model.TenDeTai), "Tên đề tài không được để trống.");
        else if (model.TenDeTai.Trim().Length < 10)
            ModelState.AddModelError(nameof(model.TenDeTai), "Tên đề tài phải có ít nhất 10 ký tự.");

        if (string.IsNullOrWhiteSpace(model.LoaiDeTai))
            ModelState.AddModelError(nameof(model.LoaiDeTai), "Vui lòng chọn loại đề tài.");

        if (model.NgayBatDau == default)
            ModelState.AddModelError(nameof(model.NgayBatDau), "Vui lòng chọn ngày bắt đầu.");

        if (model.NgayKetThuc == default)
            ModelState.AddModelError(nameof(model.NgayKetThuc), "Vui lòng chọn ngày kết thúc.");

        if (model.NgayBatDau != default && model.NgayKetThuc != default)
        {
            if (model.NgayKetThuc <= model.NgayBatDau)
                ModelState.AddModelError(nameof(model.NgayKetThuc), "Ngày kết thúc phải sau ngày bắt đầu.");

            if ((model.NgayKetThuc - model.NgayBatDau).TotalDays < 7)
                ModelState.AddModelError(nameof(model.NgayKetThuc), "Thời gian thực hiện phải ít nhất 7 ngày.");
        }
    }

    // Validate logic cần truy vấn DB (async)
    private async Task ValidateDeTaiAsync(DeTai model, int? excludeId)
    {
        // Mã đề tài không được trùng
        if (!string.IsNullOrWhiteSpace(model.MaDeTai))
        {
            var trung = await _context.DeTais.AnyAsync(d =>
                !d.IsDeleted &&
                d.MaDeTai == model.MaDeTai.Trim() &&
                d.Id != (excludeId ?? 0));
            if (trung)
                ModelState.AddModelError(nameof(model.MaDeTai), "Mã đề tài đã tồn tại trong hệ thống.");
        }

        // Sinh viên không được có 2 đề tài trong cùng đợt
        if (model.SinhVienId.HasValue && model.DotDoAnId.HasValue)
        {
            var svDaTonTai = await _context.DeTais.AnyAsync(d =>
                !d.IsDeleted &&
                d.SinhVienId == model.SinhVienId &&
                d.DotDoAnId == model.DotDoAnId &&
                d.Id != (excludeId ?? 0));
            if (svDaTonTai)
                ModelState.AddModelError(nameof(model.SinhVienId), "Sinh viên này đã có đề tài trong đợt đồ án được chọn.");
        }

        // Giảng viên không được vượt giới hạn hướng dẫn
        if (model.GiangVienId.HasValue)
        {
            var gv = await _context.GiangViens.FindAsync(model.GiangVienId.Value);
            if (gv == null)
            {
                ModelState.AddModelError(nameof(model.GiangVienId), "Giảng viên không tồn tại.");
            }
            else
            {
                // Khi thêm mới: kiểm tra trực tiếp; khi sửa: chỉ kiểm tra nếu đổi GV
                bool doiGV = excludeId.HasValue &&
                    await _context.DeTais.AnyAsync(d => d.Id == excludeId && d.GiangVienId != model.GiangVienId);
                bool isNew = !excludeId.HasValue;

                if ((isNew || doiGV) && gv.SoLuongSinhVienDangHuongDan >= gv.GioiHanSinhVien)
                    ModelState.AddModelError(nameof(model.GiangVienId),
                        $"Giảng viên {gv.HoTen} đã đạt giới hạn hướng dẫn ({gv.GioiHanSinhVien} sinh viên).");
            }
        }

        // Đợt đồ án phải đang mở đăng ký (chỉ áp dụng khi thêm mới)
        if (!excludeId.HasValue && model.DotDoAnId.HasValue)
        {
            var dot = await _context.DotDoAns.FindAsync(model.DotDoAnId.Value);
            if (dot == null)
                ModelState.AddModelError(nameof(model.DotDoAnId), "Đợt đồ án không tồn tại.");
            else if (dot.DaKhoa)
                ModelState.AddModelError(nameof(model.DotDoAnId), "Đợt đồ án này đã bị khóa, không thể thêm đề tài.");
        }
    }
}
