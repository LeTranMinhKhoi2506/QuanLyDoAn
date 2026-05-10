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

    public AdminDeTaiController(AppDbContext context)
    {
        _context = context;
    }

    // Danh sách đề tài với filter
    public async Task<IActionResult> Manage(string? trangThai, int? dotDoAnId, string? keyword)
    {
        var query = _context.DeTais
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .Include(d => d.DotDoAn)
            .AsQueryable();

        if (!string.IsNullOrEmpty(trangThai))
            query = query.Where(d => d.TrangThai == trangThai);

        if (dotDoAnId.HasValue)
            query = query.Where(d => d.DotDoAnId == dotDoAnId);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(d => d.TenDeTai.Contains(keyword) || d.MaDeTai.Contains(keyword));

        ViewBag.DotDoAnList = new SelectList(await _context.DotDoAns.OrderByDescending(d => d.NgayTao).ToListAsync(), "Id", "TenDot", dotDoAnId);
        ViewBag.TrangThaiFilter = trangThai;
        ViewBag.DotDoAnFilter = dotDoAnId;
        ViewBag.Keyword = keyword;

        return View("~/Views/Admin/DeTai/Manage.cshtml", await query.OrderByDescending(d => d.Id).ToListAsync());
    }

    // Chi tiết đề tài
    public async Task<IActionResult> Details(int id)
    {
        var deTai = await _context.DeTais
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .Include(d => d.DotDoAn)
            .Include(d => d.HoiDong)
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deTai == null) return NotFound();

        return View("~/Views/Admin/DeTai/Details.cshtml", deTai);
    }

    // Duyệt đề tài
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DuyetDeTai(int id)
    {
        var deTai = await _context.DeTais.FindAsync(id);
        if (deTai == null) return NotFound();

        deTai.TrangThai = "Đã duyệt";
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã duyệt đề tài: {deTai.TenDeTai}";
        return RedirectToAction(nameof(Details), new { id });
    }

    // Từ chối đề tài
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoiDeTai(int id)
    {
        var deTai = await _context.DeTais.FindAsync(id);
        if (deTai == null) return NotFound();

        deTai.TrangThai = "Bị từ chối";
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã từ chối đề tài: {deTai.TenDeTai}";
        return RedirectToAction(nameof(Details), new { id });
    }

    // Trang phân công giảng viên
    public async Task<IActionResult> PhanCong(int? dotDoAnId)
    {
        var query = _context.DeTais
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .Include(d => d.DotDoAn)
            .Where(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Chờ duyệt")
            .AsQueryable();

        if (dotDoAnId.HasValue)
            query = query.Where(d => d.DotDoAnId == dotDoAnId);

        ViewBag.DotDoAnList = new SelectList(await _context.DotDoAns.OrderByDescending(d => d.NgayTao).ToListAsync(), "Id", "TenDot", dotDoAnId);
        ViewBag.DotDoAnFilter = dotDoAnId;
        ViewBag.GiangViens = await _context.GiangViens
            .Where(g => g.TrangThai == "Đang hoạt động")
            .OrderBy(g => g.HoTen)
            .ToListAsync();

        return View("~/Views/Admin/DeTai/PhanCong.cshtml", await query.OrderByDescending(d => d.Id).ToListAsync());
    }

    // Thực hiện phân công giảng viên
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PhanCongGiangVien(int deTaiId, int giangVienId)
    {
        var deTai = await _context.DeTais.FindAsync(deTaiId);
        if (deTai == null) return NotFound();

        var giangVien = await _context.GiangViens.FindAsync(giangVienId);
        if (giangVien == null) return NotFound();

        // Giảm SoLuongSinhVienDangHuongDan của GV cũ nếu có
        if (deTai.GiangVienId.HasValue && deTai.GiangVienId != giangVienId)
        {
            var gvCu = await _context.GiangViens.FindAsync(deTai.GiangVienId.Value);
            if (gvCu != null && gvCu.SoLuongSinhVienDangHuongDan > 0)
                gvCu.SoLuongSinhVienDangHuongDan--;
        }

        deTai.GiangVienId = giangVienId;
        if (deTai.TrangThai == "Chờ duyệt")
            deTai.TrangThai = "Đã duyệt";

        if (!deTai.GiangVienId.HasValue || deTai.GiangVienId != giangVienId)
            giangVien.SoLuongSinhVienDangHuongDan++;

        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã phân công GV {giangVien.HoTen} cho đề tài: {deTai.TenDeTai}";
        return RedirectToAction(nameof(PhanCong));
    }
}
