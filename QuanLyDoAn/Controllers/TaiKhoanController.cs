using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Controllers;

[Authorize(Roles = "Admin")]
public class TaiKhoanController : Controller
{
    private readonly AppDbContext _context;

    public TaiKhoanController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /TaiKhoan/Manage
    public async Task<IActionResult> Manage()
    {
        var dsUser = await _context.Users.ToListAsync();
        return View("~/views/Admin/TaiKhoan/Manage.cshtml", dsUser);
    }

    // GET: /TaiKhoan/Create
    public IActionResult Create()
    {
        return View("~/views/Admin/TaiKhoan/Create.cshtml");
    }

    // POST: /TaiKhoan/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Username,PasswordHash,Role")] User user)
    {
        if (ModelState.IsValid)
        {
            var usernameClean = user.Username.Trim();
            if (await _context.Users.AnyAsync(u => u.Username == usernameClean))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại trong hệ thống.");
                return View("~/views/Admin/TaiKhoan/Create.cshtml", user);
            }

            user.Username = usernameClean;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }
        return View("~/views/Admin/TaiKhoan/Create.cshtml", user);
    }

    // GET: /TaiKhoan/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return View("~/views/Admin/TaiKhoan/Edit.cshtml", user);
    }

    // POST: /TaiKhoan/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Username,PasswordHash,Role")] User user)
    {
        if (id != user.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            var usernameClean = user.Username.Trim();

            // Check nếu sửa username trùng với người khác
            if (usernameClean != existingUser?.Username && await _context.Users.AnyAsync(u => u.Username == usernameClean))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại.");
                return View("~/views/Admin/TaiKhoan/Edit.cshtml", user);
            }

            user.Username = usernameClean;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }
        return View("~/views/Admin/TaiKhoan/Edit.cshtml", user);
    }

    // GET: /TaiKhoan/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return View("~/views/Admin/TaiKhoan/Delete.cshtml", user);
    }

    // POST: /TaiKhoan/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Manage));
    }
}