using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using System.Security.Claims;

namespace QuanLyDoAn.Controllers.Admin;

[Authorize(Roles = "Sinh viên")]
public class SVController : Controller
{
    private readonly AppDbContext _context;

    public SVController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardStats()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return BadRequest("Invalid User ID");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais
            .Include(d => d.GiangVien)
            .Include(d => d.TienDos)
            .Include(d => d.DotDoAn)
            .FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        if (deTai == null)
        {
            return Json(new
            {
                hasDeTai = false
            });
        }

        // Tính % tiến độ (Ví dụ: Tổng % của các task / số tasks hoặc lấy max tùy logic)
        // Ở đây lấy trung bình cộng làm ví dụ
        int phanTramTienDo = deTai.TienDos.Any() ? (int)deTai.TienDos.Average(t => t.PhanTramHoanThanh) : 0;

        var nhanXetMoiNhat = deTai.TienDos
            .Where(t => !string.IsNullOrEmpty(t.NhanXetCuaGiangVien))
            .OrderByDescending(t => t.NgayCapNhat)
            .Select(t => t.NhanXetCuaGiangVien)
            .FirstOrDefault();

        return Json(new
        {
            hasDeTai = true,
            tenDeTai = deTai.TenDeTai,
            giangVienHuongDan = deTai.GiangVien?.HoTen ?? "Chưa phân công",
            trangThai = deTai.TrangThai,
            phanTramTienDo = phanTramTienDo,
            hanNop = deTai.DotDoAn?.HanNopBaoCao.ToString("dd/MM/yyyy") ?? "Chưa xác định",
            nhanXetMoiNhat = nhanXetMoiNhat ?? "Chưa có nhận xét nào"
        });
    }

    // Hiển thị trang thông báo cho sinh viên
    public IActionResult ThongBao()
    {
        return View();
    }

    // 1. Đăng ký / Đề xuất đề tài
    [HttpGet]
    public async Task<IActionResult> DangKyDeXuat()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var currentDoAn = await _context.DeTais.FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        // Nếu đã có đề tài, không cho đăng ký nữa
        if (currentDoAn != null)
        {
            TempData["Message"] = "Bạn đã đăng ký đề tài rồi.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        // Lấy danh sách giảng viên để chọn
        ViewBag.GiangViens = await _context.GiangViens.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DangKyDeXuat(DeTai model)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var currentDoAn = await _context.DeTais.FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);
        if (currentDoAn != null)
        {
            TempData["Message"] = "Bạn đã đăng ký đề tài rồi.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        if (ModelState.IsValid)
        {
            model.SinhVienId = sinhVien.Id;
            model.TrangThai = "Chờ duyệt";
            model.NgayTao = DateTime.Now;

            // Generate a random MaDeTai or logic based
            model.MaDeTai = "DT" + DateTime.Now.Ticks.ToString().Substring(10); 

            _context.DeTais.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký đề tài thành công, chờ duyệt.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        ViewBag.GiangViens = await _context.GiangViens.ToListAsync();
        return View(model);
    }

    // 2. Đề tài của tôi
    public async Task<IActionResult> DeTaiCuaToi()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais
            .Include(d => d.GiangVien)
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        return View(deTai);
    }

    // 3. Cập nhật tiến độ
    public async Task<IActionResult> CapNhatTienDo()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        if (deTai == null)
        {
            TempData["Message"] = "Bạn chưa có đề tài, không thể cập nhật tiến độ.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        // Truyền Id của đề tài hiện tại sang View để có thể thêm mới tiến độ
        ViewBag.DeTaiId = deTai.Id;
        return View(deTai.TienDos.OrderByDescending(t => t.NgayCapNhat).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> CapNhatTienDo(TienDo model, IFormFile? uploadFile)
    {
        var deTai = await _context.DeTais.FindAsync(model.DeTaiId);
        if (deTai == null) return NotFound("Đề tài không tồn tại.");

        // Xử lý upload file minh chứng (nếu có)
        if (uploadFile != null && uploadFile.Length > 0)
        {
            var fileName = Path.GetFileName(uploadFile.FileName);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var uniqueFileName = $"{timestamp}_{fileName}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "minhchung");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await uploadFile.CopyToAsync(fileStream);
            }
            model.FileMinhChung = $"/uploads/minhchung/{uniqueFileName}";
        }

        // Cập nhật các trường bắt buộc
        model.NgayCapNhat = DateTime.Now;

        _context.TienDos.Add(model);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật tiến độ thành công.";
        return RedirectToAction(nameof(CapNhatTienDo));
    }

    [HttpPost]
    public async Task<IActionResult> XoaTienDo(int id)
    {
        var tienDo = await _context.TienDos.FindAsync(id);
        if (tienDo != null)
        {
            _context.TienDos.Remove(tienDo);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bạn đã xóa 1 bản ghi tiến độ thành công.";
        }
        return RedirectToAction(nameof(CapNhatTienDo));
    }

    // 4. Nộp báo cáo
    [HttpGet]
    public async Task<IActionResult> NopBaoCao()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais
            .FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        if (deTai == null)
        {
            TempData["Message"] = "Bạn chưa có đề tài, không thể nộp báo cáo.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        return View(deTai);
    }

    [HttpPost]
    public async Task<IActionResult> NopBaoCao(
        string? LinkGitHub, 
        string? LinkDemo, 
        IFormFile? FileBaoCao, 
        IFormFile? FileSlide, 
        IFormFile? FileSourceCode)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);
        if (deTai == null) return NotFound("Đề tài không tồn tại.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "baocao");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        // Upload File báo cáo
        if (FileBaoCao != null && FileBaoCao.Length > 0)
        {
            var fileName = $"BaoCao_{sinhVien.MaSinhVien}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}{Path.GetExtension(FileBaoCao.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await FileBaoCao.CopyToAsync(stream);
            }
            deTai.FileBaoCao = $"/uploads/baocao/{fileName}";
        }

        // Upload File Slide
        if (FileSlide != null && FileSlide.Length > 0)
        {
            var fileName = $"Slide_{sinhVien.MaSinhVien}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}{Path.GetExtension(FileSlide.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await FileSlide.CopyToAsync(stream);
            }
            deTai.FileSlide = $"/uploads/baocao/{fileName}";
        }

        // Upload File Source Code (zip/rar)
        if (FileSourceCode != null && FileSourceCode.Length > 0)
        {
            var fileName = $"SourceCode_{sinhVien.MaSinhVien}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}{Path.GetExtension(FileSourceCode.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await FileSourceCode.CopyToAsync(stream);
            }
            deTai.FileSourceCode = $"/uploads/baocao/{fileName}";
        }

        if (LinkGitHub != null) deTai.LinkGitHub = LinkGitHub;
        if (LinkDemo != null) deTai.LinkDemo = LinkDemo;

        // Cập nhật trạng thái
        deTai.TrangThai = "Đã nộp báo cáo";

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Nộp báo cáo thành công!";
        return RedirectToAction(nameof(NopBaoCao));
    }

    // 5. Nhận xét từ giảng viên
    [HttpGet]
    public async Task<IActionResult> NhanXetTuGV()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais
            .Include(d => d.GiangVien)
            .Include(d => d.HoiDong)
            .Include(d => d.TienDos)
            .FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        if (deTai == null)
        {
            TempData["Message"] = "Bạn chưa có đề tài, không thể xem nhận xét.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        return View(deTai);
    }

    // 6. Lịch gặp giảng viên
    [HttpGet]
    public async Task<IActionResult> LichGapGiangVien()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais
            .Include(d => d.GiangVien)
            .FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);

        if (deTai == null || deTai.GiangVienId == null)
        {
            TempData["Message"] = "Bạn chưa có đề tài hoặc chưa được phân công giảng viên hướng dẫn.";
            return RedirectToAction(nameof(DeTaiCuaToi));
        }

        // Lấy danh sách lịch hẹn của sinh viên này
        var lichGaps = await _context.LichGaps
            .Include(l => l.GiangVien)
            .Where(l => l.SinhVienId == sinhVien.Id)
            .OrderByDescending(l => l.NgayGioGap)
            .ToListAsync();

        ViewBag.DeTai = deTai;
        return View(lichGaps);
    }

    [HttpPost]
    public async Task<IActionResult> XinLichGap([FromForm] LichGap model)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
             return RedirectToAction("Login", "Account");
        }

        var sinhVien = await _context.SinhViens.FirstOrDefaultAsync(s => s.UserId == userId);
        if (sinhVien == null) return NotFound("Student profile not found.");

        var deTai = await _context.DeTais.FirstOrDefaultAsync(d => d.SinhVienId == sinhVien.Id);
        if (deTai == null || deTai.GiangVienId == null) return NotFound("Không tìm thấy giảng viên.");

        // Remove those fields from modelstate checks because we assign them programatically
        ModelState.Remove("SinhVien");
        ModelState.Remove("GiangVien");

        if (ModelState.IsValid)
        {
            model.SinhVienId = sinhVien.Id;
            model.GiangVienId = deTai.GiangVienId.Value;
            model.TrangThai = "Chờ duyệt"; // Mới tạo mặc định là Chờ duyệt

            _context.LichGaps.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký xin hẹn lịch thành công. Vui lòng chờ Giảng viên xác nhận.";
        }
        else
        {
            // Trả về lỗi model validation chi tiết
            var errorMsgs = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            TempData["Message"] = "Vui lòng nhập đầy đủ thông tin hợp lệ. Chi tiết: " + errorMsgs;
        }

        return RedirectToAction(nameof(LichGapGiangVien));
    }

    [HttpPost]
    public async Task<IActionResult> HuyLichGap(int id)
    {
        var lich = await _context.LichGaps.FindAsync(id);
        if (lich == null) return NotFound();

        // Chỉ cho phép sinh viên xoá/hủy các lịch đang chờ duyệt. Lịch đã duyệt không được tự ý xóa
        if (lich.TrangThai == "Chờ duyệt")
        {
            _context.LichGaps.Remove(lich);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã hủy yêu cầu hẹn thành công.";
        }
        else
        {
            TempData["Message"] = "Không thể xóa lịch đã được xử lý. Hãy liên hệ trực tiếp giảng viên.";
        }

        return RedirectToAction(nameof(LichGapGiangVien));
    }
}