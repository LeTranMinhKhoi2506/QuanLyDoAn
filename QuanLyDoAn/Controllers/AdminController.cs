using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    // API endpoint for dashboard stats
    [HttpGet]
    public async Task<IActionResult> GetDashboardStats()
    {
        var totalGiangVien = await _context.GiangViens.CountAsync();
        var totalSinhVien = await _context.SinhViens.CountAsync();

        var deTaiChoDuyet = await _context.DeTais.CountAsync(d => d.TrangThai == "Chờ duyệt");
        var deTaiDangThucHien = await _context.DeTais.CountAsync(d => d.TrangThai == "Đang thực hiện");
        var deTaiHoanThanh = await _context.DeTais.CountAsync(d => d.TrangThai == "Hoàn thành");

        var sinhVienCoDeTaiIds = await _context.DeTais.Where(d => d.SinhVienId != null).Select(d => d.SinhVienId).ToListAsync();
        var sinhVienChuaDangKy = await _context.SinhViens.CountAsync(s => !sinhVienCoDeTaiIds.Contains(s.Id));

        return Json(new {
            totalGiangVien,
            totalSinhVien,
            deTaiChoDuyet,
            deTaiDangThucHien,
            deTaiHoanThanh,
            sinhVienChuaDangKy
        });
    }
}