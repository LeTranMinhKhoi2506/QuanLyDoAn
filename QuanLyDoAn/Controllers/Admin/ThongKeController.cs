using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;

namespace QuanLyDoAn.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class ThongKeController : Controller
{
    private readonly AppDbContext _context;

    public ThongKeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ThongKeTrangThai()
    {
        var data = await _context.DeTais
            .Where(d => !d.IsDeleted)
            .GroupBy(d => d.TrangThai)
            .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
            .ToListAsync();

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> ThongKeTheoDot()
    {
        var data = await _context.DeTais
            .Where(d => !d.IsDeleted && d.DotDoAnId != null)
            .Include(d => d.DotDoAn)
            .GroupBy(d => d.DotDoAn!.TenDot)
            .Select(g => new { TenDot = g.Key, SoLuong = g.Count() })
            .ToListAsync();

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> ThongKeTheoGiangVien()
    {
        var data = await _context.DeTais
            .Where(d => !d.IsDeleted && d.GiangVienId != null)
            .Include(d => d.GiangVien)
            .GroupBy(d => d.GiangVien!.HoTen)
            .Select(g => new { GiangVien = g.Key, SoLuong = g.Count() })
            .OrderByDescending(x => x.SoLuong)
            .Take(10)
            .ToListAsync();

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> ThongKeoBoMon()
    {
        var data = await _context.DeTais
            .Where(d => !d.IsDeleted && d.GiangVienId != null)
            .Include(d => d.GiangVien)
            .GroupBy(d => d.GiangVien!.BoMon)
            .Select(g => new { BoMon = g.Key, SoLuong = g.Count() })
            .OrderByDescending(x => x.SoLuong)
            .ToListAsync();

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> ThongKeTienDo()
    {
        var deTais = await _context.DeTais
            .Where(d => !d.IsDeleted && d.TrangThai == "Đang thực hiện")
            .Include(d => d.TienDos)
            .ToListAsync();

        var data = deTais.Select(d => new
        {
            MaDeTai = d.MaDeTai,
            TenDeTai = d.TenDeTai,
            TienDo = d.TienDos.Any() ? d.TienDos.Average(t => t.PhanTramHoanThanh) : 0
        }).ToList();

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> ThongKeTongQuan()
    {
        var tongDeTai = await _context.DeTais.CountAsync(d => !d.IsDeleted);
        var tongGiangVien = await _context.GiangViens.CountAsync();
        var tongSinhVien = await _context.SinhViens.CountAsync();
        var tongDot = await _context.DotDoAns.CountAsync();

        var deTaiTheoTrangThai = await _context.DeTais
            .Where(d => !d.IsDeleted)
            .GroupBy(d => d.TrangThai)
            .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
            .ToListAsync();

        return Json(new
        {
            tongDeTai,
            tongGiangVien,
            tongSinhVien,
            tongDot,
            deTaiTheoTrangThai
        });
    }
}
