using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using System.Security.Claims;

namespace QuanLyDoAn.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ThongBaoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ThongBaoController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID");
        }
        return userId;
    }

    // GET: api/ThongBao
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ThongBaoDto>>> GetThongBaos()
    {
        try
        {
            int userId = GetCurrentUserId();
            
            var thongBaos = await _context.ThongBaos
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.NgayTao)
                .Select(t => new ThongBaoDto
                {
                    Id = t.Id,
                    NoiDung = t.NoiDung,
                    NgayTao = t.NgayTao,
                    DaDoc = t.DaDoc
                })
                .ToListAsync();

            return Ok(thongBaos);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // GET: api/ThongBao/latest?count=5 - for dropdown
    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<ThongBaoDto>>> GetLatestThongBaos([FromQuery] int count = 5)
    {
        try
        {
            int userId = GetCurrentUserId();
            
            var thongBaos = await _context.ThongBaos
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.NgayTao)
                .Take(count)
                .Select(t => new ThongBaoDto
                {
                    Id = t.Id,
                    NoiDung = t.NoiDung,
                    NgayTao = t.NgayTao,
                    DaDoc = t.DaDoc
                })
                .ToListAsync();

            return Ok(thongBaos);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // GET: api/ThongBao/unread-count
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        try
        {
            int userId = GetCurrentUserId();
            
            var count = await _context.ThongBaos
                .Where(t => t.UserId == userId && !t.DaDoc)
                .CountAsync();

            return Ok(count);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // PUT: api/ThongBao/{id}/read
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            int userId = GetCurrentUserId();
            
            var thongBao = await _context.ThongBaos
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (thongBao == null)
                return NotFound();

            thongBao.DaDoc = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    // PUT: api/ThongBao/read-all
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            int userId = GetCurrentUserId();
            
            var thongBaos = await _context.ThongBaos
                .Where(t => t.UserId == userId && !t.DaDoc)
                .ToListAsync();

            foreach (var tb in thongBaos)
            {
                tb.DaDoc = true;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}

public class ThongBaoDto
{
    public int Id { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; }
    public bool DaDoc { get; set; }
}
