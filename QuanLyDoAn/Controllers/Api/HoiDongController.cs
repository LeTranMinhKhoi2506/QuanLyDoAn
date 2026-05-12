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
            .Include(h => h.ThanhViens).ThenInclude(tv => tv.GiangVien)
            .Where(h => !h.IsDeleted)
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
