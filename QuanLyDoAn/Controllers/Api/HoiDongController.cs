using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class HoiDongController : ControllerBase
{
    private readonly AppDbContext _context;

    public HoiDongController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HoiDong>>> GetHoiDongs()
    {
        return await _context.HoiDongs
            .Include(h => h.ChuTich)
            .Include(h => h.ThuKy)
            .Include(h => h.UyVien)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<HoiDong>> PostHoiDong(HoiDong hoiDong)
    {
        _context.HoiDongs.Add(hoiDong);
        await _context.SaveChangesAsync();

        return Ok(hoiDong);
    }
}