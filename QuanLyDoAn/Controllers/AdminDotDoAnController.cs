using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers;

public class AdminDotDoAnController : Controller
{
    private readonly AppDbContext _context;

    public AdminDotDoAnController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Xem danh sách đợt đồ án
    public async Task<IActionResult> Manage()
    {
        var dots = await _context.DotDoAns
            .Include(d => d.DeTais)
            .OrderByDescending(d => d.NgayTao)
            .ToListAsync();
        return View(dots);
    }

    // 2. Thêm đợt mới - GET
    public IActionResult Create()
    {
        return View();
    }

    // 2. Thêm đợt mới - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DotDoAn model)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(model.TenDot))
            ModelState.AddModelError("TenDot", "Tên đợt không được rỗng");
        
        if (string.IsNullOrWhiteSpace(model.LoaiDot))
            ModelState.AddModelError("LoaiDot", "Loại đợt không được rỗng");
        
        if (string.IsNullOrWhiteSpace(model.NamHoc))
            ModelState.AddModelError("NamHoc", "Năm học không được rỗng");
        
        if (model.NgayBatDau >= model.NgayKetThuc)
            ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu");
        
        if (model.HanDangKyDeTai < model.NgayBatDau || model.HanDangKyDeTai > model.NgayKetThuc)
            ModelState.AddModelError("HanDangKyDeTai", "Hạn đăng ký phải nằm trong khoảng ngày bắt đầu - kết thúc");
        
        if (model.HanNopBaoCao <= model.HanDangKyDeTai)
            ModelState.AddModelError("HanNopBaoCao", "Hạn nộp báo cáo phải sau hạn đăng ký");

        // Kiểm tra trùng tên
        var exists = await _context.DotDoAns
            .AnyAsync(d => d.TenDot == model.TenDot && d.NamHoc == model.NamHoc && d.HocKy == model.HocKy);
        if (exists)
            ModelState.AddModelError("TenDot", "Đã tồn tại đợt này trong cùng học kỳ, năm học");

        if (!ModelState.IsValid)
            return View(model);

        model.TrangThai = "Chưa mở";
        model.NgayTao = DateTime.Now;
        model.DangMoDangKy = false;
        model.DaKhoa = false;

        _context.DotDoAns.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thêm đợt đồ án thành công";
        return RedirectToAction(nameof(Manage));
    }

    // 3. Cập nhật thông tin đợt - GET
    public async Task<IActionResult> Edit(int id)
    {
        var dot = await _context.DotDoAns.FindAsync(id);
        if (dot == null)
            return NotFound();

        if (dot.DaKhoa)
        {
            TempData["Error"] = "Không thể sửa đợt đã khóa";
            return RedirectToAction(nameof(Manage));
        }

        return View(dot);
    }

    // 3. Cập nhật thông tin đợt - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DotDoAn model)
    {
        if (id != model.Id)
            return NotFound();

        var dot = await _context.DotDoAns.FindAsync(id);
        if (dot == null)
            return NotFound();

        if (dot.DaKhoa)
        {
            TempData["Error"] = "Không thể sửa đợt đã khóa";
            return RedirectToAction(nameof(Manage));
        }

        // Validate giống Create
        if (string.IsNullOrWhiteSpace(model.TenDot))
            ModelState.AddModelError("TenDot", "Tên đợt không được rỗng");
        
        if (model.NgayBatDau >= model.NgayKetThuc)
            ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu");
        
        if (model.HanDangKyDeTai < model.NgayBatDau || model.HanDangKyDeTai > model.NgayKetThuc)
            ModelState.AddModelError("HanDangKyDeTai", "Hạn đăng ký phải nằm trong khoảng ngày bắt đầu - kết thúc");
        
        if (model.HanNopBaoCao <= model.HanDangKyDeTai)
            ModelState.AddModelError("HanNopBaoCao", "Hạn nộp báo cáo phải sau hạn đăng ký");

        if (!ModelState.IsValid)
            return View(model);

        dot.TenDot = model.TenDot;
        dot.LoaiDot = model.LoaiDot;
        dot.HocKy = model.HocKy;
        dot.NamHoc = model.NamHoc;
        dot.NgayBatDau = model.NgayBatDau;
        dot.NgayKetThuc = model.NgayKetThuc;
        dot.HanDangKyDeTai = model.HanDangKyDeTai;
        dot.HanNopBaoCao = model.HanNopBaoCao;
        dot.MoTa = model.MoTa;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật đợt thành công";
        return RedirectToAction(nameof(Manage));
    }

    // 4. Xem chi tiết đợt
    public async Task<IActionResult> Details(int id)
    {
        var dot = await _context.DotDoAns
            .Include(d => d.DeTais)
                .ThenInclude(dt => dt.SinhVien)
            .Include(d => d.DeTais)
                .ThenInclude(dt => dt.GiangVien)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dot == null)
            return NotFound();

        return View(dot);
    }

    // 5. Mở đợt đăng ký
    [HttpPost]
    public async Task<IActionResult> MoDangKy(int id)
    {
        var dot = await _context.DotDoAns.FindAsync(id);
        if (dot == null)
            return NotFound();

        // Kiểm tra điều kiện
        if (dot.TrangThai != "Chưa mở")
        {
            TempData["Error"] = "Chỉ có thể mở đợt đang ở trạng thái 'Chưa mở'";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (DateTime.Now > dot.HanDangKyDeTai)
        {
            TempData["Error"] = "Đã quá hạn đăng ký đề tài";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (dot.NgayBatDau == default || dot.NgayKetThuc == default)
        {
            TempData["Error"] = "Ngày bắt đầu và kết thúc không hợp lệ";
            return RedirectToAction(nameof(Details), new { id });
        }

        dot.TrangThai = "Đang mở";
        dot.DangMoDangKy = true;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã mở đợt đăng ký";
        return RedirectToAction(nameof(Details), new { id });
    }

    // 5. Đóng đợt đăng ký
    [HttpPost]
    public async Task<IActionResult> DongDangKy(int id)
    {
        var dot = await _context.DotDoAns.FindAsync(id);
        if (dot == null)
            return NotFound();

        if (dot.TrangThai != "Đang mở")
        {
            TempData["Error"] = "Đợt không đang mở";
            return RedirectToAction(nameof(Details), new { id });
        }

        dot.TrangThai = "Đã đóng";
        dot.DangMoDangKy = false;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã đóng đợt đăng ký";
        return RedirectToAction(nameof(Details), new { id });
    }

    // 6. Khóa đợt sau khi kết thúc
    [HttpPost]
    public async Task<IActionResult> KhoaDot(int id)
    {
        var dot = await _context.DotDoAns.FindAsync(id);
        if (dot == null)
            return NotFound();

        if (dot.DaKhoa)
        {
            TempData["Error"] = "Đợt đã được khóa trước đó";
            return RedirectToAction(nameof(Details), new { id });
        }

        dot.DaKhoa = true;
        dot.TrangThai = "Đã khóa";
        dot.DangMoDangKy = false;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã khóa đợt. Không thể thêm/sửa dữ liệu";
        return RedirectToAction(nameof(Details), new { id });
    }

    // 7. Xem danh sách sinh viên trong đợt
    public async Task<IActionResult> SinhVienTrongDot(int id)
    {
        var dot = await _context.DotDoAns
            .Include(d => d.DeTais)
                .ThenInclude(dt => dt.SinhVien)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dot == null)
            return NotFound();

        ViewBag.DotDoAn = dot;
        return View(dot.DeTais.Where(dt => dt.SinhVien != null).Select(dt => dt.SinhVien).Distinct().ToList());
    }

    // 8. Xem danh sách đề tài thuộc đợt
    public async Task<IActionResult> DeTaiTrongDot(int id)
    {
        var dot = await _context.DotDoAns
            .Include(d => d.DeTais)
                .ThenInclude(dt => dt.SinhVien)
            .Include(d => d.DeTais)
                .ThenInclude(dt => dt.GiangVien)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dot == null)
            return NotFound();

        ViewBag.DotDoAn = dot;
        return View(dot.DeTais.ToList());
    }

    // 9. Thống kê số lượng đề tài theo trạng thái
    public async Task<IActionResult> ThongKe(int id)
    {
        var dot = await _context.DotDoAns
            .Include(d => d.DeTais)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dot == null)
            return NotFound();

        var thongKe = dot.DeTais
            .GroupBy(dt => dt.TrangThai)
            .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
            .ToList();

        ViewBag.DotDoAn = dot;
        ViewBag.ThongKe = thongKe;
        ViewBag.TongSo = dot.DeTais.Count;

        return View(dot);
    }
}
