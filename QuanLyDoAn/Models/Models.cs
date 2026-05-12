using System.ComponentModel.DataAnnotations;

namespace QuanLyDoAn.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 - 50 ký tự")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty; // Admin, Giảng viên, Sinh viên

    public SinhVien? SinhVien { get; set; }
    public GiangVien? GiangVien { get; set; }
}

public class SinhVien
{
    public int Id { get; set; }
    public string MaSinhVien { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string Lop { get; set; } = string.Empty;
    public string KhoaHoc { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<DeTai> DeTais { get; set; } = new List<DeTai>();
}

public class GiangVien
{
    public int Id { get; set; }
    public string MaGiangVien { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string BoMon { get; set; } = string.Empty;
    public string HocVi { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Đang hoạt động";
    public int GioiHanSinhVien { get; set; } = 5;
    public int GioiHanDeTai { get; set; } = 5;
    public int SoLuongSinhVienDangHuongDan { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<DeTai> DeTaisHuongDan { get; set; } = new List<DeTai>();
}

public class DeTai
{
    public int Id { get; set; }
    public string MaDeTai { get; set; } = string.Empty;
    public string TenDeTai { get; set; } = string.Empty;
    public string MoTaDeTai { get; set; } = string.Empty;
    public string LoaiDeTai { get; set; } = string.Empty; // Đồ án, Luận văn
    public string CongNgheSuDung { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Chờ duyệt"; // Chờ duyệt, Đã duyệt, Bị từ chối, Đang thực hiện, Yêu cầu chỉnh sửa, Đã nộp báo cáo, Đã hoàn thành, Đã hủy
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.Now;
    public DateTime? NgayDuyet { get; set; }
    public string LyDoTuChoi { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;

    public int? SinhVienId { get; set; }
    public SinhVien? SinhVien { get; set; }

    public int? GiangVienId { get; set; }
    public GiangVien? GiangVien { get; set; }

    public string FileBaoCao { get; set; } = string.Empty;
    public string FileSlide { get; set; } = string.Empty;
    public string FileSourceCode { get; set; } = string.Empty;
    public string LinkGitHub { get; set; } = string.Empty;
    public string LinkDemo { get; set; } = string.Empty;

    public double? DiemHuongDan { get; set; }
    public double? DiemPhanBien { get; set; }
    public double? DiemHoiDong { get; set; }

    public int? DotDoAnId { get; set; }
    public DotDoAn? DotDoAn { get; set; }

    public int? HoiDongId { get; set; }
    public HoiDong? HoiDong { get; set; }

    public ICollection<TienDo> TienDos { get; set; } = new List<TienDo>();
}

public class TienDo
{
    public int Id { get; set; }
    public string TenCongViec { get; set; } = string.Empty;
    public string NoiDungDaLam { get; set; } = string.Empty;
    public DateTime NgayCapNhat { get; set; }
    public int PhanTramHoanThanh { get; set; }
    public string FileMinhChung { get; set; } = string.Empty;
    public string NhanXetCuaGiangVien { get; set; } = string.Empty;

    public int DeTaiId { get; set; }
    public DeTai DeTai { get; set; } = null!;
}

public class LichGap
{
    public int Id { get; set; }
    public DateTime NgayGioGap { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public string HinhThuc { get; set; } = string.Empty; // Online / Trực tiếp
    public string TrangThai { get; set; } = string.Empty; // Chờ duyệt, Chấp nhận, Từ chối, Đổi lịch
    public string GhiChu { get; set; } = string.Empty;

    public int SinhVienId { get; set; }
    public SinhVien SinhVien { get; set; } = null!;

    public int GiangVienId { get; set; }
    public GiangVien GiangVien { get; set; } = null!;
}

public class ThongBao
{
    public int Id { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; }
    public bool DaDoc { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

public class DotDoAn
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Tên đợt không được rỗng")]
    public string TenDot { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Loại đợt không được rỗng")]
    public string LoaiDot { get; set; } = string.Empty; // Đồ án, Luận văn
    
    public string HocKy { get; set; } = string.Empty; // HK1, HK2, HK3
    
    [Required(ErrorMessage = "Năm học không được rỗng")]
    public string NamHoc { get; set; } = string.Empty; // 2024-2025
    
    [Required(ErrorMessage = "Ngày bắt đầu không được rỗng")]
    public DateTime NgayBatDau { get; set; }
    
    [Required(ErrorMessage = "Ngày kết thúc không được rỗng")]
    public DateTime NgayKetThuc { get; set; }
    
    [Required(ErrorMessage = "Hạn đăng ký đề tài không được rỗng")]
    public DateTime HanDangKyDeTai { get; set; }
    
    [Required(ErrorMessage = "Hạn nộp báo cáo không được rỗng")]
    public DateTime HanNopBaoCao { get; set; }
    
    public string TrangThai { get; set; } = "Chưa mở"; // Chưa mở, Đang mở, Đã đóng, Đã khóa
    
    public string MoTa { get; set; } = string.Empty;
    
    public DateTime NgayTao { get; set; } = DateTime.Now;
    
    public bool DangMoDangKy { get; set; } = false;
    
    public bool DaKhoa { get; set; } = false;

    public ICollection<DeTai> DeTais { get; set; } = new List<DeTai>();
}

public class HoiDong
{
    public int Id { get; set; }
    public string TenHoiDong { get; set; } = string.Empty;
    public string? MoTa { get; set; } = string.Empty;
    public bool DaKhoa { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateTime NgayTao { get; set; } = DateTime.Now;

    public int? DotDoAnId { get; set; }
    public DotDoAn? DotDoAn { get; set; }

    public ICollection<HoiDongThanhVien> ThanhViens { get; set; } = new List<HoiDongThanhVien>();
    public ICollection<DeTai> DeTaisDuocCham { get; set; } = new List<DeTai>();
    public ICollection<LichBaoVe> LichBaoVes { get; set; } = new List<LichBaoVe>();
}

public class HoiDongThanhVien
{
    public int Id { get; set; }
    public string VaiTro { get; set; } = string.Empty; // Chủ tịch, Thư ký, Ủy viên, Phản biện

    public int HoiDongId { get; set; }
    public HoiDong HoiDong { get; set; } = null!;

    public int GiangVienId { get; set; }
    public GiangVien GiangVien { get; set; } = null!;
}

public class LichBaoVe
{
    public int Id { get; set; }
    public DateTime ThoiGianBaoVe { get; set; }
    public string PhongBaoVe { get; set; } = string.Empty;
    public string GhiChu { get; set; } = string.Empty;

    public int HoiDongId { get; set; }
    public HoiDong HoiDong { get; set; } = null!;

    public int DeTaiId { get; set; }
    public DeTai DeTai { get; set; } = null!;
}

public class DiemHoiDong
{
    public int Id { get; set; }
    public double DiemBaoCao { get; set; }      // 30%
    public double DiemThuyetTrinh { get; set; } // 20%
    public double DiemSanPham { get; set; }     // 30%
    public double DiemTraLoiCauHoi { get; set; }// 20%
    public double DiemTong => DiemBaoCao * 0.3 + DiemThuyetTrinh * 0.2 + DiemSanPham * 0.3 + DiemTraLoiCauHoi * 0.2;
    public string NhanXet { get; set; } = string.Empty;
    public DateTime NgayNhap { get; set; } = DateTime.Now;

    public int DeTaiId { get; set; }
    public DeTai DeTai { get; set; } = null!;

    public int GiangVienId { get; set; }
    public GiangVien GiangVien { get; set; } = null!;

    public int HoiDongId { get; set; }
    public HoiDong HoiDong { get; set; } = null!;
}
