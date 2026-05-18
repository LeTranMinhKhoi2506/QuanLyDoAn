using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class GiangVienController : Controller
{
    private readonly AppDbContext _context;

    public GiangVienController(AppDbContext context) => _context = context;

    // GET: /GiangVien/Manage
    public async Task<IActionResult> Manage(string search)
    {
        var query = _context.GiangViens
            .Include(g => g.User)
            .Include(g => g.DeTaisHuongDan)
            .ThenInclude(d => d.SinhVien)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(g =>
                g.MaGiangVien.Contains(search) ||
                g.HoTen.Contains(search) ||
                g.Email.Contains(search) ||
                g.BoMon.Contains(search));

        ViewBag.Search = search;
        return View("~/views/Admin/GiangVien/Manage.cshtml", await query.ToListAsync());
    }

    // GET: /GiangVien/Create
    public IActionResult Create() => View("~/views/Admin/GiangVien/Create.cshtml");

    // POST: /GiangVien/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MaGiangVien,HoTen,Email,SoDienThoai,BoMon,HocVi,TrangThai,GioiHanSinhVien,GioiHanDeTai,UserId")] GiangVien gv)
    {
        ModelState.Remove("User");
        ModelState.Remove("DeTaisHuongDan");
        if (!ModelState.IsValid) return View("~/views/Admin/GiangVien/Create.cshtml", gv);

        _context.GiangViens.Add(gv);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Manage));
    }

    // GET: /GiangVien/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var gv = await _context.GiangViens.FindAsync(id);
        if (gv == null) return NotFound();
        return View("~/views/Admin/GiangVien/Edit.cshtml", gv);
    }

    // POST: /GiangVien/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,MaGiangVien,HoTen,Email,SoDienThoai,BoMon,HocVi,TrangThai,GioiHanSinhVien,GioiHanDeTai,SoLuongSinhVienDangHuongDan,UserId")] GiangVien gv)
    {
        if (id != gv.Id) return NotFound();
        ModelState.Remove("User");
        ModelState.Remove("DeTaisHuongDan");
        if (!ModelState.IsValid) return View("~/views/Admin/GiangVien/Edit.cshtml", gv);

        _context.Update(gv);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Manage));
    }

    // GET: /GiangVien/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var gv = await _context.GiangViens
            .Include(g => g.User)
            .Include(g => g.DeTaisHuongDan).ThenInclude(d => d.SinhVien)
            .FirstOrDefaultAsync(g => g.Id == id);
        if (gv == null) return NotFound();
        return View("~/views/Admin/GiangVien/Details.cshtml", gv);
    }

    // GET: /GiangVien/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var gv = await _context.GiangViens
            .Include(g => g.DeTaisHuongDan).ThenInclude(d => d.TienDos)
            .FirstOrDefaultAsync(g => g.Id == id);
        if (gv == null) return NotFound();
        return View("~/views/Admin/GiangVien/Delete.cshtml", gv);
    }

    // POST: /GiangVien/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, string action)
    {
        var gv = await _context.GiangViens
            .Include(g => g.DeTaisHuongDan).ThenInclude(d => d.TienDos)
            .FirstOrDefaultAsync(g => g.Id == id);
        if (gv == null) return NotFound();

        bool coDeTai = gv.DeTaisHuongDan.Any();
        bool coDuLieu = gv.DeTaisHuongDan.Any(d =>
            d.TienDos.Any() || d.DiemHuongDan.HasValue || d.DiemPhanBien.HasValue ||
            d.DiemHoiDong.HasValue || !string.IsNullOrEmpty(d.FileBaoCao));

        if (action == "deactivate")
        {
            gv.TrangThai = "Ngừng hoạt động";
            var user = await _context.Users.FindAsync(gv.UserId);
            if (user != null) user.Role = "Inactive";
            await _context.SaveChangesAsync();
        }
        else if (action == "delete")
        {
            if (coDuLieu || coDeTai)
                return BadRequest("Không thể xoá giảng viên đang có dữ liệu liên quan.");
            _context.GiangViens.Remove(gv);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Manage));
    }

    // POST: /GiangVien/ToggleLock/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(int id)
    {
        var gv = await _context.GiangViens.Include(g => g.User).FirstOrDefaultAsync(g => g.Id == id);
        if (gv == null || gv.User == null) return NotFound();

        if (gv.User.Role == "Inactive")
        {
            gv.User.Role = "Giảng viên";
            gv.TrangThai = "Đang hoạt động";
        }
        else
        {
            gv.User.Role = "Inactive";
            gv.TrangThai = "Bị khóa";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Manage));
    }
}
