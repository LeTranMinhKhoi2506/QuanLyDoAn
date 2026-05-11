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
}
