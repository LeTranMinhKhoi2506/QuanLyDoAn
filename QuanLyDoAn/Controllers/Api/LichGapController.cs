using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class LichGapController : ControllerBase
{
    private readonly AppDbContext _context;

    public LichGapController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("sinhvien/{sinhVienId}")]
    public async Task<ActionResult<IEnumerable<LichGap>>> GetLichGapTheoSinhVien(int sinhVienId)
    {
        return await _context.LichGaps
            .Where(l => l.SinhVienId == sinhVienId)
            .OrderByDescending(l => l.NgayGioGap)
            .ToListAsync();
    }

    [HttpGet("giangvien/{giangVienId}")]
    public async Task<ActionResult<IEnumerable<LichGap>>> GetLichGapTheoGiangVien(int giangVienId)
    {
        return await _context.LichGaps
            .Where(l => l.GiangVienId == giangVienId)
            .OrderByDescending(l => l.NgayGioGap)
            .ToListAsync();
    }

    // Sinh viên đề xuất lịch gặp
    [HttpPost]
    public async Task<ActionResult<LichGap>> PostLichGap(LichGap lichGap)
    {
        lichGap.TrangThai = "Chờ duyệt";
        _context.LichGaps.Add(lichGap);
        await _context.SaveChangesAsync();

        return Ok(lichGap);
    }

    // Giảng viên duyệt/từ chối lịch
    [HttpPut("{id}/duyet")]
    public async Task<IActionResult> DuyetLichGap(int id, [FromQuery] string trangThai, [FromQuery] string? ghiChu)
    {
        var lichGap = await _context.LichGaps.FindAsync(id);
        if (lichGap == null) return NotFound();

        lichGap.TrangThai = trangThai; // Chấp nhận, Từ chối, Đổi lịch
        if (ghiChu != null)
        {
            lichGap.GhiChu = ghiChu;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}