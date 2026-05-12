using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using QuanLyDoAn.Services;

namespace QuanLyDoAn.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class DeTaiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notification;

    public DeTaiController(AppDbContext context, NotificationService notification)
    {
        _context = context;
        _notification = notification;
    }

    // 1. Xem danh sách đề tài
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeTai>>> GetDeTais()
    {
        return await _context.DeTais
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .ToListAsync();
    }

    // 2. Lấy thông tin chi tiết đề tài
    [HttpGet("{id}")]
    public async Task<ActionResult<DeTai>> GetDeTai(int id)
    {
        var deTai = await _context.DeTais
            .Include(d => d.SinhVien)
            .Include(d => d.GiangVien)
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deTai == null)
        {
            return NotFound();
        }

        return deTai;
    }

    // 3. Sinh viên đăng ký / đề xuất đề tài mới
    [HttpPost]
    public async Task<ActionResult<DeTai>> PostDeTai(DeTai deTai)
    {
        deTai.TrangThai = "Chờ duyệt";
        deTai.NgayBatDau = DateTime.UtcNow;

        _context.DeTais.Add(deTai);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeTai), new { id = deTai.Id }, deTai);
    }

    // 4. Giảng viên duyệt/từ chối đề tài
    [HttpPut("{id}/duyet")]
    public async Task<IActionResult> DuyetDeTai(int id, [FromQuery] string trangThaiMoi)
    {
        var deTai = await _context.DeTais.FindAsync(id);
        if (deTai == null)
        {
            return NotFound();
        }

        // trangThaiMoi có thể là "Đã duyệt", "Bị từ chối", "Đang thực hiện"
        deTai.TrangThai = trangThaiMoi;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 5. Nộp file báo cáo / source code
    [HttpPut("{id}/nop-bai")]
    public async Task<IActionResult> NopBai(int id, [FromBody] NopBaiDto dto)
    {
        var deTai = await _context.DeTais.FindAsync(id);
        if (deTai == null) return NotFound();

        deTai.FileBaoCao = dto.FileBaoCao ?? deTai.FileBaoCao;
        deTai.FileSlide = dto.FileSlide ?? deTai.FileSlide;
        deTai.FileSourceCode = dto.FileSourceCode ?? deTai.FileSourceCode;
        deTai.LinkGitHub = dto.LinkGitHub ?? deTai.LinkGitHub;
        deTai.LinkDemo = dto.LinkDemo ?? deTai.LinkDemo;

        await _context.SaveChangesAsync();

        // Thông báo cho giảng viên hướng dẫn khi sinh viên nộp file mới
        if (!string.IsNullOrEmpty(dto.FileBaoCao) || !string.IsNullOrEmpty(dto.FileSlide) || !string.IsNullOrEmpty(dto.FileSourceCode))
        {
            await _notification.ThongBaoChoGiangVien(id, "Sinh viên vừa nộp file mới cho đề tài.");
        }

        return Ok(deTai);
    }
}

public class NopBaiDto
{
    public string? FileBaoCao { get; set; }
    public string? FileSlide { get; set; }
    public string? FileSourceCode { get; set; }
    public string? LinkGitHub { get; set; }
    public string? LinkDemo { get; set; }
}