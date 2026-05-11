using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using QuanLyDoAn.Services;

namespace QuanLyDoAn.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TienDoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notification;

    public TienDoController(AppDbContext context, NotificationService notification)
    {
        _context = context;
        _notification = notification;
    }

    [HttpGet("detai/{deTaiId}")]
    public async Task<ActionResult<IEnumerable<TienDo>>> GetTienDoCuaDeTai(int deTaiId)
    {
        return await _context.TienDos
            .Where(t => t.DeTaiId == deTaiId)
            .OrderBy(t => t.NgayCapNhat)
            .ToListAsync();
    }

    // Sinh viên báo cáo tiến độ
    [HttpPost]
    public async Task<ActionResult<TienDo>> PostTienDo(TienDo tienDo)
    {
        tienDo.NgayCapNhat = DateTime.UtcNow;
        _context.TienDos.Add(tienDo);
        await _context.SaveChangesAsync();
        await _notification.ThongBaoChoGiangVien(tienDo.DeTaiId, "Sinh viên vừa cập nhật tiến độ mới.");

        return CreatedAtAction(nameof(GetTienDoCuaDeTai), new { deTaiId = tienDo.DeTaiId }, tienDo);
    }

    // Giảng viên nhận xét tiến độ
    [HttpPut("{id}/nhan-xet")]
    public async Task<IActionResult> NhanXetTienDo(int id, [FromBody] string nhanXet)
    {
        var tienDo = await _context.TienDos.FindAsync(id);
        if (tienDo == null) return NotFound();

        tienDo.NhanXetCuaGiangVien = nhanXet;
        await _context.SaveChangesAsync();
        await _notification.ThongBaoNhanXet(tienDo.DeTaiId);

        return NoContent();
    }
}