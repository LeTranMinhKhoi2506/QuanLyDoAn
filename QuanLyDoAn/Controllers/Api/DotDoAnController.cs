using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class DotDoAnController : ControllerBase
{
    private readonly AppDbContext _context;

    public DotDoAnController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DotDoAn>>> GetDotDoAns()
    {
        return await _context.DotDoAns.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<DotDoAn>> PostDotDoAn(DotDoAn dotDoAn)
    {
        _context.DotDoAns.Add(dotDoAn);
        await _context.SaveChangesAsync();

        return Ok(dotDoAn);
    }
}