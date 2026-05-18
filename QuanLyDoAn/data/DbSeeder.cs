using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        // === USERS ===
        var adminUser = new User { Username = "admin", PasswordHash = "admin123", Role = "Admin" };

        var gvUsers = Enumerable.Range(1, 5).Select(i => new User
        {
            Username = $"gv{i:D2}",
            PasswordHash = "gv123",
            Role = "Giảng viên"
        }).ToList();

        var svUsers = Enumerable.Range(1, 10).Select(i => new User
        {
            Username = $"sv{i:D2}",
            PasswordHash = "sv123",
            Role = "Sinh viên"
        }).ToList();

        await db.Users.AddAsync(adminUser);
        await db.Users.AddRangeAsync(gvUsers);
        await db.Users.AddRangeAsync(svUsers);
        await db.SaveChangesAsync();

        // === GIẢNG VIÊN ===
        string[] hocVis = ["TS", "ThS", "PGS.TS", "GS.TS", "ThS"];
        string[] boMons = ["Công nghệ phần mềm", "Hệ thống thông tin", "Mạng máy tính", "Trí tuệ nhân tạo", "Khoa học máy tính"];
        string[] tenGVs = ["Nguyễn Văn An", "Trần Thị Bình", "Lê Văn Cường", "Phạm Thị Dung", "Hoàng Văn Em"];

        var giangViens = gvUsers.Select((u, i) => new GiangVien
        {
            MaGiangVien = $"GV{(i + 1):D3}",
            HoTen = tenGVs[i],
            Email = $"gv{i + 1}@university.edu.vn",
            SoDienThoai = $"090{1000000 + i}",
            BoMon = boMons[i],
            HocVi = hocVis[i],
            TrangThai = "Đang hoạt động",
            GioiHanSinhVien = 5,
            GioiHanDeTai = 5,
            UserId = u.Id
        }).ToList();

        await db.GiangViens.AddRangeAsync(giangViens);
        await db.SaveChangesAsync();

        // === SINH VIÊN ===
        string[] tenSVs = [
            "Nguyễn Minh Anh", "Trần Quốc Bảo", "Lê Thị Cẩm", "Phạm Văn Dũng", "Hoàng Thị Ế",
            "Vũ Minh Fong", "Đặng Thị Giang", "Bùi Văn Hùng", "Ngô Thị Iris", "Đinh Văn Khoa"
        ];
        string[] lops = ["CNTT01", "CNTT02", "HTTT01", "HTTT02", "MMT01", "CNTT01", "CNTT02", "HTTT01", "HTTT02", "MMT01"];

        var sinhViens = svUsers.Select((u, i) => new SinhVien
        {
            MaSinhVien = $"SV{(i + 1):D4}",
            HoTen = tenSVs[i],
            Email = $"sv{i + 1}@student.edu.vn",
            SoDienThoai = $"098{2000000 + i}",
            Lop = lops[i],
            KhoaHoc = "2021-2025",
            TrangThai = "Đang học",
            UserId = u.Id
        }).ToList();

        await db.SinhViens.AddRangeAsync(sinhViens);
        await db.SaveChangesAsync();

        // === ĐỢT ĐỒ ÁN ===
        var dotHienTai = new DotDoAn
        {
            TenDot = "Đồ án (Còn hạn đăng ký)",
            LoaiDot = "Đồ án",
            HocKy = "HK2",
            NamHoc = $"{DateTime.Now.Year}-{DateTime.Now.Year + 1}",
            NgayBatDau = DateTime.Now.AddDays(-10),
            NgayKetThuc = DateTime.Now.AddDays(90),
            HanDangKyDeTai = DateTime.Now.AddDays(15),
            HanNopBaoCao = DateTime.Now.AddDays(75),
            TrangThai = "Đang mở",
            DangMoDangKy = true,
            MoTa = "Đợt đồ án đang trong thời gian đăng ký đề tài (Sinh viên có thể đăng ký, rút đề tài)"
        };

        var dotQuaHanDangKy = new DotDoAn
        {
            TenDot = "Đồ án (Hết hạn đăng ký)",
            LoaiDot = "Đồ án",
            HocKy = "HK1",
            NamHoc = $"{DateTime.Now.Year}-{DateTime.Now.Year + 1}",
            NgayBatDau = DateTime.Now.AddDays(-40),
            NgayKetThuc = DateTime.Now.AddDays(60),
            HanDangKyDeTai = DateTime.Now.AddDays(-10),
            HanNopBaoCao = DateTime.Now.AddDays(45),
            TrangThai = "Đang mở",
            DangMoDangKy = true,
            MoTa = "Đợt đồ án đã qua thời hạn đăng ký (Không thể rút đề tài chờ duyệt, không thể nộp lại nếu bị từ chối)"
        };

        var dotDaDong = new DotDoAn
        {
            TenDot = "Đồ án (Đã đóng)",
            LoaiDot = "Đồ án",
            HocKy = "HK3",
            NamHoc = $"{DateTime.Now.Year - 1}-{DateTime.Now.Year}",
            NgayBatDau = DateTime.Now.AddDays(-200),
            NgayKetThuc = DateTime.Now.AddDays(-100),
            HanDangKyDeTai = DateTime.Now.AddDays(-180),
            HanNopBaoCao = DateTime.Now.AddDays(-120),
            TrangThai = "Đã đóng",
            DaKhoa = true,
            MoTa = "Đợt đồ án cũ đã kết thúc"
        };

        await db.DotDoAns.AddRangeAsync(dotHienTai, dotQuaHanDangKy, dotDaDong);
        await db.SaveChangesAsync();

        // === ĐỀ TÀI ===
        var deTais = new List<DeTai>
        {
            new() {
                // SV01: Đang thực hiện, Dot còn hạn đăng ký
                MaDeTai = "DT001", TenDeTai = "Hệ thống quản lý bán hàng (SV01)",
                MoTaDeTai = "Phát triển website thương mại điện tử với ASP.NET Core",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "ASP.NET Core",
                TrangThai = "Đang thực hiện",
                NgayBatDau = dotHienTai.NgayBatDau, NgayKetThuc = dotHienTai.NgayKetThuc,
                NgayDuyet = DateTime.Now.AddDays(-2),
                SinhVienId = sinhViens[0].Id, GiangVienId = giangViens[0].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                // SV02: Đã nộp báo cáo, Dot hết hạn đăng ký
                MaDeTai = "DT002", TenDeTai = "Ứng dụng thư viện (SV02)",
                MoTaDeTai = "Hệ thống quản lý thư viện số",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Spring Boot",
                TrangThai = "Đã nộp báo cáo",
                NgayBatDau = dotQuaHanDangKy.NgayBatDau, NgayKetThuc = dotQuaHanDangKy.NgayKetThuc,
                NgayDuyet = DateTime.Now.AddDays(-30),
                SinhVienId = sinhViens[1].Id, GiangVienId = giangViens[0].Id, DotDoAnId = dotQuaHanDangKy.Id,
                FileBaoCao = "baocao_dt002.pdf", FileSlide = "slide_dt002.pptx",
                LinkGitHub = "https://github.com/sv02/library-app"
            },
            new() {
                // SV03: Chờ duyệt, Dot còn hạn đăng ký -> Có thể RÚT đề tài
                MaDeTai = "DT003", TenDeTai = "AI điểm danh (SV03 - Có thể rút)",
                MoTaDeTai = "Nhận diện khuôn mặt điểm danh",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Python, OpenCV",
                TrangThai = "Chờ duyệt",
                NgayBatDau = dotHienTai.NgayBatDau, NgayKetThuc = dotHienTai.NgayKetThuc,
                SinhVienId = sinhViens[2].Id, GiangVienId = giangViens[3].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                // SV04: Chờ duyệt, Dot HẾT hạn đăng ký -> KHÔNG THỂ rút đề tài
                MaDeTai = "DT004", TenDeTai = "Chatbot NLP (SV04 - Không thể rút)",
                MoTaDeTai = "Chatbot tư vấn sinh viên",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Python, Rasa",
                TrangThai = "Chờ duyệt",
                NgayBatDau = dotQuaHanDangKy.NgayBatDau, NgayKetThuc = dotQuaHanDangKy.NgayKetThuc,
                SinhVienId = sinhViens[3].Id, GiangVienId = giangViens[1].Id, DotDoAnId = dotQuaHanDangKy.Id
            },
            new() {
                // SV05: Yêu cầu chỉnh sửa, duyệt cách đây 1 ngày -> CÒN HẠN SỬA (3 ngày)
                MaDeTai = "DT005", TenDeTai = "Đặt lịch khám (SV05 - Còn hạn sửa)",
                MoTaDeTai = "Hệ thống đặt lịch y tế",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "ASP.NET Core",
                TrangThai = "Yêu cầu chỉnh sửa",
                NgayBatDau = dotQuaHanDangKy.NgayBatDau, NgayKetThuc = dotQuaHanDangKy.NgayKetThuc,
                NgayDuyet = DateTime.Now.AddDays(-1), // Mới duyệt hôm qua
                SinhVienId = sinhViens[4].Id, GiangVienId = giangViens[2].Id, DotDoAnId = dotQuaHanDangKy.Id
            },
            new() {
                // SV06: Yêu cầu chỉnh sửa, duyệt cách đây 5 ngày -> QUÁ HẠN SỬA (3 ngày)
                MaDeTai = "DT006", TenDeTai = "Quản lý nhân sự (SV06 - Hết hạn sửa)",
                MoTaDeTai = "Phần mềm HR",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Java",
                TrangThai = "Yêu cầu chỉnh sửa",
                NgayBatDau = dotQuaHanDangKy.NgayBatDau, NgayKetThuc = dotQuaHanDangKy.NgayKetThuc,
                NgayDuyet = DateTime.Now.AddDays(-5), // Đã duyệt 5 ngày trước
                SinhVienId = sinhViens[5].Id, GiangVienId = giangViens[1].Id, DotDoAnId = dotQuaHanDangKy.Id
            },
            new() {
                // SV07: Bị từ chối, Dot HẾT hạn đăng ký -> KHÔNG THỂ gửi lại
                MaDeTai = "DT007", TenDeTai = "E-Learning (SV07 - Hết hạn đăng ký)",
                MoTaDeTai = "Web học trực tuyến",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Node.js",
                TrangThai = "Bị từ chối", LyDoTuChoi = "Đề tài trùng lặp",
                NgayBatDau = dotQuaHanDangKy.NgayBatDau, NgayKetThuc = dotQuaHanDangKy.NgayKetThuc,
                NgayDuyet = DateTime.Now.AddDays(-1),
                SinhVienId = sinhViens[6].Id, GiangVienId = giangViens[2].Id, DotDoAnId = dotQuaHanDangKy.Id
            },
            new() {
                // SV08: Bị từ chối, Dot CÒN hạn đăng ký -> CÓ THỂ gửi lại (sửa đổi)
                MaDeTai = "DT008", TenDeTai = "App chi tiêu (SV08 - Có thể gửi lại)",
                MoTaDeTai = "Quản lý tài chính",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Flutter",
                TrangThai = "Bị từ chối", LyDoTuChoi = "Cần thêm tính năng AI",
                NgayBatDau = dotHienTai.NgayBatDau, NgayKetThuc = dotHienTai.NgayKetThuc,
                NgayDuyet = DateTime.Now.AddDays(-1),
                SinhVienId = sinhViens[7].Id, GiangVienId = giangViens[4].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                // SV09: Đã duyệt (chưa có tiến độ)
                MaDeTai = "DT009", TenDeTai = "IoT Môi trường (SV09)",
                MoTaDeTai = "Giám sát bằng Arduino",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Arduino",
                TrangThai = "Đã duyệt",
                NgayBatDau = dotHienTai.NgayBatDau, NgayKetThuc = dotHienTai.NgayKetThuc,
                NgayDuyet = DateTime.Now.AddDays(-1),
                SinhVienId = sinhViens[8].Id, GiangVienId = giangViens[3].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                // SV10: Đã hoàn thành, đợt cũ
                MaDeTai = "DT010", TenDeTai = "Phân tích cảm xúc (SV10)",
                MoTaDeTai = "NLP Model",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Python",
                TrangThai = "Đã hoàn thành",
                NgayBatDau = dotDaDong.NgayBatDau, NgayKetThuc = dotDaDong.NgayKetThuc,
                NgayDuyet = dotDaDong.NgayBatDau.AddDays(10),
                SinhVienId = sinhViens[9].Id, GiangVienId = giangViens[4].Id, DotDoAnId = dotDaDong.Id,
                DiemHuongDan = 8.5, DiemPhanBien = 8.0, DiemHoiDong = 8.2,
                FileBaoCao = "baocao.pdf"
            }
        };

        await db.DeTais.AddRangeAsync(deTais);
        await db.SaveChangesAsync();

        foreach (var gv in giangViens)
        {
            gv.SoLuongSinhVienDangHuongDan = await db.DeTais.CountAsync(d => d.GiangVienId == gv.Id && d.TrangThai == "Đang thực hiện" && !d.IsDeleted);
        }
        await db.SaveChangesAsync();

        // === TIẾN ĐỘ ===
        var tienDos = new List<TienDo>
        {
            new() { DeTaiId = deTais[0].Id, TenCongViec = "Phân tích yêu cầu", NoiDungDaLam = "Hoàn thành tài liệu SRS", NgayCapNhat = DateTime.Now.AddDays(-2), PhanTramHoanThanh = 20, NhanXetCuaGiangVien = "Tốt, tiếp tục thiết kế" },
            new() { DeTaiId = deTais[1].Id, TenCongViec = "Lập trình backend", NoiDungDaLam = "Xong API", NgayCapNhat = DateTime.Now.AddDays(-15), PhanTramHoanThanh = 100, NhanXetCuaGiangVien = "Đã đạt" },
            new() { DeTaiId = deTais[9].Id, TenCongViec = "Báo cáo cuối cùng", NoiDungDaLam = "Nộp báo cáo đầy đủ", NgayCapNhat = dotDaDong.HanNopBaoCao.AddDays(-5), PhanTramHoanThanh = 100, NhanXetCuaGiangVien = "Báo cáo đạt yêu cầu" }
        };

        await db.TienDos.AddRangeAsync(tienDos);

        // === LỊCH GẶP ===
        var lichGaps = new List<LichGap>
        {
            new() { SinhVienId = sinhViens[0].Id, GiangVienId = giangViens[0].Id, NgayGioGap = DateTime.Now.AddDays(2), NoiDung = "Báo cáo tuần 1", HinhThuc = "Trực tiếp", TrangThai = "Chấp nhận", GhiChu = "Phòng A201" },
            new() { SinhVienId = sinhViens[2].Id, GiangVienId = giangViens[3].Id, NgayGioGap = DateTime.Now.AddDays(3), NoiDung = "Hỏi về đề cương", HinhThuc = "Online", TrangThai = "Chờ duyệt" }
        };

        await db.LichGaps.AddRangeAsync(lichGaps);

        // === HỘI ĐỒNG ===
        var hoiDong1 = new HoiDong
        {
            TenHoiDong = "Hội đồng 1 - CNTT",
            MoTa = "Hội đồng chấm đồ án nhóm CNTT",
            DotDoAnId = dotQuaHanDangKy.Id
        };

        await db.HoiDongs.AddAsync(hoiDong1);
        await db.SaveChangesAsync();

        // Gán đề tài đã nộp báo cáo vào hội đồng
        deTais[1].HoiDongId = hoiDong1.Id;
        db.DeTais.Update(deTais[1]);

        // === THÀNH VIÊN HỘI ĐỒNG ===
        var thanhViens = new List<HoiDongThanhVien>
        {
            new() { HoiDongId = hoiDong1.Id, GiangVienId = giangViens[0].Id, VaiTro = "Chủ tịch" },
            new() { HoiDongId = hoiDong1.Id, GiangVienId = giangViens[1].Id, VaiTro = "Thư ký" },
            new() { HoiDongId = hoiDong1.Id, GiangVienId = giangViens[2].Id, VaiTro = "Phản biện" }
        };

        await db.HoiDongThanhViens.AddRangeAsync(thanhViens);

        // === LỊCH BẢO VỆ ===
        var lichBaoVe = new LichBaoVe
        {
            HoiDongId = hoiDong1.Id,
            DeTaiId = deTais[1].Id,
            ThoiGianBaoVe = dotQuaHanDangKy.HanNopBaoCao.AddDays(15),
            PhongBaoVe = "B301",
            GhiChu = "Sinh viên chuẩn bị slide và demo"
        };

        await db.LichBaoVes.AddAsync(lichBaoVe);

        // === THÔNG BÁO ===
        var thongBaos = new List<ThongBao>
        {
            new() { UserId = svUsers[0].Id, NoiDung = "Đề tài của bạn đã được duyệt", NgayTao = DateTime.Now.AddDays(-2), DaDoc = true },
            new() { UserId = svUsers[4].Id, NoiDung = "Đề tài yêu cầu chỉnh sửa, hạn chót 3 ngày", NgayTao = DateTime.Now.AddDays(-1), DaDoc = false }
        };

        await db.ThongBaos.AddRangeAsync(thongBaos);
        await db.SaveChangesAsync();
    }
}
