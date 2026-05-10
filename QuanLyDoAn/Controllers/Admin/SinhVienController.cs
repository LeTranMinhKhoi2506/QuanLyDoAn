using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class SinhVienController : Controller
{
    private readonly AppDbContext _context;

    public SinhVienController(AppDbContext context)
    {
        _context = context;
    }

    // API Get SinhViens (dùng cho AJAX List nếu cần)
    [HttpGet("api/SinhVien")]
    public async Task<ActionResult<IEnumerable<SinhVien>>> GetSinhViens()
    {
        return await _context.SinhViens.ToListAsync();
    }

    // --- MVC Views cho Admin ---

    // GET: /SinhVien/Manage
    public async Task<IActionResult> Manage()
    {
        var dsSinhVien = await _context.SinhViens.Include(s => s.User).ToListAsync();
        return View("~/views/Admin/SinhVien/Manage.cshtml", dsSinhVien);
    }

    // GET: /SinhVien/Create
    public IActionResult Create()
    {
        return View("~/views/Admin/SinhVien/Create.cshtml");
    }

    // POST: /SinhVien/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MaSinhVien,HoTen,Email,SoDienThoai,Lop,KhoaHoc,TrangThai,UserId")] SinhVien sinhVien)
    {
        ModelState.Remove("User");
        ModelState.Remove("DeTais");
        if (ModelState.IsValid)
        {
            _context.SinhViens.Add(sinhVien);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }
        return View("~/views/Admin/SinhVien/Create.cshtml", sinhVien);
    }

    // GET: /SinhVien/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var sinhVien = await _context.SinhViens.FindAsync(id);
        if (sinhVien == null) return NotFound();

        return View("~/views/Admin/SinhVien/Edit.cshtml", sinhVien);
    }

    // POST: /SinhVien/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,MaSinhVien,HoTen,Email,SoDienThoai,Lop,KhoaHoc,TrangThai,UserId")] SinhVien sinhVien)
    {
        if (id != sinhVien.Id) return NotFound();

        ModelState.Remove("User");
        ModelState.Remove("DeTais");
        if (ModelState.IsValid)
        {
            _context.Update(sinhVien);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }
        return View("~/views/Admin/SinhVien/Edit.cshtml", sinhVien);
    }

    // GET: /SinhVien/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var sinhVien = await _context.SinhViens
            .Include(s => s.DeTais).ThenInclude(d => d.TienDos)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sinhVien == null) return NotFound();

        return View("~/views/Admin/SinhVien/Delete.cshtml", sinhVien);
    }

    // POST: /SinhVien/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, string action)
    {
        var sinhVien = await _context.SinhViens
            .Include(s => s.DeTais).ThenInclude(d => d.TienDos)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sinhVien == null) return NotFound();

        bool coDeTai = sinhVien.DeTais.Any();
        bool coDuLieuLienQuan = sinhVien.DeTais.Any(d =>
            d.TienDos.Any() ||
            d.DiemHuongDan.HasValue || d.DiemPhanBien.HasValue || d.DiemHoiDong.HasValue ||
            !string.IsNullOrEmpty(d.FileBaoCao));

        if (action == "deactivate")
        {
            sinhVien.TrangThai = "Ngừng hoạt động";
            await _context.SaveChangesAsync();
        }
        else if (action == "delete")
        {
            // Chỉ cho xoá cứng nếu chưa có dữ liệu liên quan
            if (coDuLieuLienQuan)
                return BadRequest("Không thể xoá sinh viên đã có tiến độ, điểm hoặc file nộp.");
            if (coDeTai)
                return BadRequest("Không thể xoá sinh viên đã đăng ký đề tài.");

            _context.SinhViens.Remove(sinhVien);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Manage));
    }
}