using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyDoAn.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin");
        }
        else if (User.IsInRole("Giảng viên"))
        {
            return RedirectToAction("Dashboard", "GV");
        }
        else if (User.IsInRole("Sinh viên"))
        {
            return RedirectToAction("Dashboard", "SV");
        }

        return View();
    }
}