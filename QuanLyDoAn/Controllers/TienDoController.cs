using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TienDoController : ControllerBase
{
    private readonly AppDbContext _context;

    public TienDoController(AppDbContext context)
    {
        _context = context;
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

        return NoContent();
    }
}