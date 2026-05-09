using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDoAn.Services;

public class DotDoAnService
{
    private readonly AppDbContext _context;

    public DotDoAnService(AppDbContext context)
    {
        _context = context;
    }

    // Kiểm tra sinh viên có thể đăng ký đề tài không
    public async Task<(bool CanRegister, string Message)> CanSinhVienDangKyDeTai(int dotDoAnId, int sinhVienId)
    {
        var dot = await _context.DotDoAns.FindAsync(dotDoAnId);
        if (dot == null)
            return (false, "Đợt đồ án không tồn tại");

        // Kiểm tra đợt có đang mở đăng ký không
        if (!dot.DangMoDangKy || dot.TrangThai != "Đang mở")
            return (false, "Đợt đăng ký chưa mở hoặc đã đóng");

        // Kiểm tra đã quá hạn đăng ký chưa
        if (DateTime.Now > dot.HanDangKyDeTai)
            return (false, "Đã quá hạn đăng ký đề tài");

        // Kiểm tra sinh viên đã có đề tài trong đợt này chưa
        var daCoDeTai = await _context.DeTais
            .AnyAsync(dt => dt.SinhVienId == sinhVienId && dt.DotDoAnId == dotDoAnId);
        
        if (daCoDeTai)
            return (false, "Bạn đã đăng ký đề tài trong đợt này");

        return (true, "Có thể đăng ký");
    }

    // Kiểm tra giảng viên có thể nhận thêm sinh viên không
    public async Task<(bool CanAccept, string Message)> CanGiangVienNhanSinhVien(int giangVienId)
    {
        var gv = await _context.GiangViens.FindAsync(giangVienId);
        if (gv == null)
            return (false, "Giảng viên không tồn tại");

        if (gv.TrangThai != "Đang hoạt động")
            return (false, "Giảng viên không còn hoạt động");

        // Đếm số sinh viên đang hướng dẫn
        var soLuongDangHuongDan = await _context.DeTais
            .Where(dt => dt.GiangVienId == giangVienId && 
                        (dt.TrangThai == "Đã duyệt" || dt.TrangThai == "Đang thực hiện"))
            .CountAsync();

        if (soLuongDangHuongDan >= gv.GioiHanSinhVien)
            return (false, $"Giảng viên đã đạt giới hạn hướng dẫn ({gv.GioiHanSinhVien} sinh viên)");

        return (true, "Có thể nhận sinh viên");
    }

    // Kiểm tra có thể thêm/sửa đề tài trong đợt không
    public async Task<(bool CanModify, string Message)> CanModifyDeTaiInDot(int dotDoAnId)
    {
        var dot = await _context.DotDoAns.FindAsync(dotDoAnId);
        if (dot == null)
            return (false, "Đợt đồ án không tồn tại");

        if (dot.DaKhoa)
            return (false, "Đợt đã khóa, không thể thêm/sửa đề tài");

        return (true, "Có thể thêm/sửa");
    }

    // Kiểm tra có thể sửa điểm không
    public async Task<(bool CanModify, string Message)> CanModifyDiem(int deTaiId)
    {
        var deTai = await _context.DeTais
            .Include(dt => dt.DotDoAn)
            .FirstOrDefaultAsync(dt => dt.Id == deTaiId);

        if (deTai == null)
            return (false, "Đề tài không tồn tại");

        if (deTai.DotDoAn != null && deTai.DotDoAn.DaKhoa)
            return (false, "Đợt đã khóa, không thể sửa điểm");

        return (true, "Có thể sửa điểm");
    }

    // Kiểm tra có thể sửa tiến độ không
    public async Task<(bool CanModify, string Message)> CanModifyTienDo(int deTaiId)
    {
        var deTai = await _context.DeTais
            .Include(dt => dt.DotDoAn)
            .FirstOrDefaultAsync(dt => dt.Id == deTaiId);

        if (deTai == null)
            return (false, "Đề tài không tồn tại");

        if (deTai.DotDoAn != null && deTai.DotDoAn.DaKhoa)
            return (false, "Đợt đã khóa, không thể cập nhật tiến độ");

        return (true, "Có thể cập nhật tiến độ");
    }

    // Kiểm tra có thể đổi đề tài không (sau khi đóng đăng ký)
    public async Task<(bool CanChange, string Message)> CanSinhVienDoiDeTai(int dotDoAnId, string userRole)
    {
        var dot = await _context.DotDoAns.FindAsync(dotDoAnId);
        if (dot == null)
            return (false, "Đợt đồ án không tồn tại");

        // Nếu đợt đã đóng đăng ký
        if (dot.TrangThai == "Đã đóng" || !dot.DangMoDangKy)
        {
            // Chỉ admin hoặc giảng viên mới được đổi
            if (userRole == "Admin" || userRole == "Giảng viên")
                return (true, "Có quyền đổi đề tài");
            else
                return (false, "Đợt đã đóng đăng ký, sinh viên không thể đổi đề tài");
        }

        return (true, "Có thể đổi đề tài");
    }
}
