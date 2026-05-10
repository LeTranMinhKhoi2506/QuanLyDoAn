using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using System.Security.Claims;

namespace QuanLyDoAn.Controllers.SinhVien;

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
            return Json(new {
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

        return Json(new {
            hasDeTai = true,
            tenDeTai = deTai.TenDeTai,
            giangVienHuongDan = deTai.GiangVien?.HoTen ?? "Chưa phân công",
            trangThai = deTai.TrangThai,
            phanTramTienDo = phanTramTienDo,
            hanNop = deTai.DotDoAn?.HanNopBaoCao.ToString("dd/MM/yyyy") ?? "Chưa xác định",
            nhanXetMoiNhat = nhanXetMoiNhat ?? "Chưa có nhận xét nào"
        });
    }
}