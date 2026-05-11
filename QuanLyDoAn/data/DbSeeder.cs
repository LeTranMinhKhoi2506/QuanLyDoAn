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
            TenDot = "Đồ án tốt nghiệp HK2 2024-2025",
            LoaiDot = "Đồ án",
            HocKy = "HK2",
            NamHoc = "2024-2025",
            NgayBatDau = new DateTime(2025, 1, 15),
            NgayKetThuc = new DateTime(2025, 6, 30),
            HanDangKyDeTai = new DateTime(2025, 2, 15),
            HanNopBaoCao = new DateTime(2025, 6, 15),
            TrangThai = "Đang mở",
            DangMoDangKy = true,
            MoTa = "Đợt đồ án tốt nghiệp học kỳ 2 năm học 2024-2025"
        };

        var dotCu = new DotDoAn
        {
            TenDot = "Đồ án tốt nghiệp HK1 2024-2025",
            LoaiDot = "Đồ án",
            HocKy = "HK1",
            NamHoc = "2024-2025",
            NgayBatDau = new DateTime(2024, 8, 1),
            NgayKetThuc = new DateTime(2024, 12, 31),
            HanDangKyDeTai = new DateTime(2024, 8, 31),
            HanNopBaoCao = new DateTime(2024, 12, 15),
            TrangThai = "Đã đóng",
            DaKhoa = true,
            MoTa = "Đợt đồ án tốt nghiệp học kỳ 1 năm học 2024-2025"
        };

        await db.DotDoAns.AddRangeAsync(dotHienTai, dotCu);
        await db.SaveChangesAsync();

        // === ĐỀ TÀI ===
        var deTais = new List<DeTai>
        {
            new() {
                MaDeTai = "DT001", TenDeTai = "Xây dựng hệ thống quản lý bán hàng online",
                MoTaDeTai = "Phát triển website thương mại điện tử với ASP.NET Core và React",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "ASP.NET Core, React, SQL Server",
                TrangThai = "Đang thực hiện",
                NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 6, 30),
                NgayDuyet = new DateTime(2025, 2, 10),
                SinhVienId = sinhViens[0].Id, GiangVienId = giangViens[0].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                MaDeTai = "DT002", TenDeTai = "Ứng dụng quản lý thư viện số",
                MoTaDeTai = "Xây dựng hệ thống quản lý thư viện với tính năng mượn/trả sách online",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Spring Boot, Angular, MySQL",
                TrangThai = "Đã nộp báo cáo",
                NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 6, 30),
                NgayDuyet = new DateTime(2025, 2, 10),
                SinhVienId = sinhViens[1].Id, GiangVienId = giangViens[0].Id, DotDoAnId = dotHienTai.Id,
                FileBaoCao = "baocao_dt002.pdf", FileSlide = "slide_dt002.pptx",
                LinkGitHub = "https://github.com/sv02/library-app"
            },
            new() {
                MaDeTai = "DT003", TenDeTai = "Hệ thống nhận diện khuôn mặt điểm danh",
                MoTaDeTai = "Ứng dụng AI nhận diện khuôn mặt để điểm danh tự động",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Python, OpenCV, Flask, React",
                TrangThai = "Chờ duyệt",
                NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 6, 30),
                SinhVienId = sinhViens[2].Id, GiangVienId = giangViens[3].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                MaDeTai = "DT004", TenDeTai = "Chatbot hỗ trợ sinh viên bằng NLP",
                MoTaDeTai = "Xây dựng chatbot tư vấn học vụ sử dụng xử lý ngôn ngữ tự nhiên",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Python, Rasa, FastAPI, Vue.js",
                TrangThai = "Bị từ chối", LyDoTuChoi = "Đề tài quá rộng, cần thu hẹp phạm vi nghiên cứu",
                NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 6, 30),
                SinhVienId = sinhViens[3].Id, GiangVienId = giangViens[1].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                MaDeTai = "DT005", TenDeTai = "Ứng dụng đặt lịch khám bệnh trực tuyến",
                MoTaDeTai = "Hệ thống đặt lịch khám bệnh với tích hợp thanh toán online",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "ASP.NET Core, Blazor, PostgreSQL",
                TrangThai = "Yêu cầu chỉnh sửa",
                NgayBatDau = new DateTime(2025, 2, 1), NgayKetThuc = new DateTime(2025, 6, 30),
                NgayDuyet = new DateTime(2025, 2, 12),
                SinhVienId = sinhViens[4].Id, GiangVienId = giangViens[2].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                MaDeTai = "DT006", TenDeTai = "Hệ thống quản lý nhân sự doanh nghiệp",
                MoTaDeTai = "Phần mềm quản lý nhân sự, chấm công, tính lương",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Java EE, JSF, Oracle DB",
                TrangThai = "Đã hoàn thành",
                NgayBatDau = new DateTime(2024, 8, 15), NgayKetThuc = new DateTime(2024, 12, 20),
                NgayDuyet = new DateTime(2024, 8, 20),
                SinhVienId = sinhViens[5].Id, GiangVienId = giangViens[1].Id, DotDoAnId = dotCu.Id,
                DiemHuongDan = 8.5, DiemPhanBien = 8.0, DiemHoiDong = 8.2,
                FileBaoCao = "baocao_dt006.pdf", FileSlide = "slide_dt006.pptx",
                LinkGitHub = "https://github.com/sv06/hrm-system"
            },
            new() {
                MaDeTai = "DT007", TenDeTai = "Website học trực tuyến E-Learning",
                MoTaDeTai = "Nền tảng học trực tuyến với video bài giảng và bài kiểm tra",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Node.js, Express, MongoDB, React",
                TrangThai = "Đang thực hiện",
                NgayBatDau = new DateTime(2025, 2, 5), NgayKetThuc = new DateTime(2025, 6, 30),
                NgayDuyet = new DateTime(2025, 2, 15),
                SinhVienId = sinhViens[6].Id, GiangVienId = giangViens[2].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                MaDeTai = "DT008", TenDeTai = "Ứng dụng mobile quản lý chi tiêu cá nhân",
                MoTaDeTai = "App theo dõi thu chi, lập ngân sách và phân tích tài chính cá nhân",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Flutter, Dart, Firebase",
                TrangThai = "Đã duyệt",
                NgayBatDau = new DateTime(2025, 3, 1), NgayKetThuc = new DateTime(2025, 6, 30),
                NgayDuyet = new DateTime(2025, 2, 20),
                SinhVienId = sinhViens[7].Id, GiangVienId = giangViens[4].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                MaDeTai = "DT009", TenDeTai = "Hệ thống IoT giám sát môi trường",
                MoTaDeTai = "Thu thập và hiển thị dữ liệu cảm biến nhiệt độ, độ ẩm qua dashboard",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Arduino, MQTT, Node-RED, InfluxDB, Grafana",
                TrangThai = "Đang thực hiện",
                NgayBatDau = new DateTime(2025, 2, 10), NgayKetThuc = new DateTime(2025, 6, 30),
                NgayDuyet = new DateTime(2025, 2, 18),
                SinhVienId = sinhViens[8].Id, GiangVienId = giangViens[3].Id, DotDoAnId = dotHienTai.Id
            },
            new() {
                MaDeTai = "DT010", TenDeTai = "Phân tích cảm xúc bình luận mạng xã hội",
                MoTaDeTai = "Áp dụng Machine Learning phân tích sentiment từ dữ liệu mạng xã hội",
                LoaiDeTai = "Đồ án", CongNgheSuDung = "Python, scikit-learn, BERT, FastAPI",
                TrangThai = "Chờ duyệt",
                NgayBatDau = new DateTime(2025, 3, 5), NgayKetThuc = new DateTime(2025, 6, 30),
                SinhVienId = sinhViens[9].Id, GiangVienId = giangViens[4].Id, DotDoAnId = dotHienTai.Id
            }
        };

        await db.DeTais.AddRangeAsync(deTais);
        await db.SaveChangesAsync();

        // === TIẾN ĐỘ ===
        var tienDos = new List<TienDo>
        {
            new() { DeTaiId = deTais[0].Id, TenCongViec = "Phân tích yêu cầu", NoiDungDaLam = "Hoàn thành tài liệu SRS", NgayCapNhat = new DateTime(2025, 2, 15), PhanTramHoanThanh = 100, NhanXetCuaGiangVien = "Tốt, tiếp tục thiết kế" },
            new() { DeTaiId = deTais[0].Id, TenCongViec = "Thiết kế CSDL", NoiDungDaLam = "Hoàn thành ERD và schema", NgayCapNhat = new DateTime(2025, 3, 1), PhanTramHoanThanh = 100, NhanXetCuaGiangVien = "Cần bổ sung index cho các bảng lớn" },
            new() { DeTaiId = deTais[0].Id, TenCongViec = "Lập trình backend", NoiDungDaLam = "Hoàn thành API quản lý sản phẩm và đơn hàng", NgayCapNhat = new DateTime(2025, 4, 10), PhanTramHoanThanh = 70 },
            new() { DeTaiId = deTais[1].Id, TenCongViec = "Hoàn thiện báo cáo", NoiDungDaLam = "Nộp báo cáo đầy đủ", NgayCapNhat = new DateTime(2025, 5, 20), PhanTramHoanThanh = 100, NhanXetCuaGiangVien = "Báo cáo đạt yêu cầu" },
            new() { DeTaiId = deTais[6].Id, TenCongViec = "Thiết kế UI/UX", NoiDungDaLam = "Hoàn thành wireframe và prototype", NgayCapNhat = new DateTime(2025, 3, 5), PhanTramHoanThanh = 100 },
            new() { DeTaiId = deTais[6].Id, TenCongViec = "Phát triển tính năng video", NoiDungDaLam = "Tích hợp player và upload video", NgayCapNhat = new DateTime(2025, 4, 20), PhanTramHoanThanh = 60 },
            new() { DeTaiId = deTais[8].Id, TenCongViec = "Lắp đặt phần cứng", NoiDungDaLam = "Kết nối cảm biến với Arduino", NgayCapNhat = new DateTime(2025, 3, 10), PhanTramHoanThanh = 100, NhanXetCuaGiangVien = "Hoàn thành đúng tiến độ" },
            new() { DeTaiId = deTais[8].Id, TenCongViec = "Xây dựng dashboard", NoiDungDaLam = "Cấu hình Grafana hiển thị dữ liệu realtime", NgayCapNhat = new DateTime(2025, 4, 15), PhanTramHoanThanh = 80 },
        };

        await db.TienDos.AddRangeAsync(tienDos);

        // === LỊCH GẶP ===
        var lichGaps = new List<LichGap>
        {
            new() { SinhVienId = sinhViens[0].Id, GiangVienId = giangViens[0].Id, NgayGioGap = new DateTime(2025, 5, 20, 9, 0, 0), NoiDung = "Báo cáo tiến độ tháng 5", HinhThuc = "Trực tiếp", TrangThai = "Chấp nhận", GhiChu = "Phòng A201" },
            new() { SinhVienId = sinhViens[1].Id, GiangVienId = giangViens[0].Id, NgayGioGap = new DateTime(2025, 5, 21, 14, 0, 0), NoiDung = "Review báo cáo trước khi nộp", HinhThuc = "Online", TrangThai = "Chấp nhận", GhiChu = "Google Meet" },
            new() { SinhVienId = sinhViens[2].Id, GiangVienId = giangViens[3].Id, NgayGioGap = new DateTime(2025, 5, 22, 10, 0, 0), NoiDung = "Thảo luận hướng tiếp cận", HinhThuc = "Trực tiếp", TrangThai = "Chờ duyệt" },
            new() { SinhVienId = sinhViens[6].Id, GiangVienId = giangViens[2].Id, NgayGioGap = new DateTime(2025, 5, 23, 15, 0, 0), NoiDung = "Demo tính năng video", HinhThuc = "Online", TrangThai = "Chờ duyệt" },
            new() { SinhVienId = sinhViens[8].Id, GiangVienId = giangViens[3].Id, NgayGioGap = new DateTime(2025, 5, 19, 9, 0, 0), NoiDung = "Kiểm tra phần cứng", HinhThuc = "Trực tiếp", TrangThai = "Từ chối", GhiChu = "GV bận họp, đổi lịch khác" },
        };

        await db.LichGaps.AddRangeAsync(lichGaps);

        // === HỘI ĐỒNG ===
        var hoiDong1 = new HoiDong
        {
            TenHoiDong = "Hội đồng 1 - CNTT",
            MoTa = "Hội đồng chấm đồ án nhóm CNTT",
            DotDoAnId = dotHienTai.Id
        };
        var hoiDong2 = new HoiDong
        {
            TenHoiDong = "Hội đồng 2 - HTTT",
            MoTa = "Hội đồng chấm đồ án nhóm HTTT",
            DotDoAnId = dotHienTai.Id
        };

        await db.HoiDongs.AddRangeAsync(hoiDong1, hoiDong2);
        await db.SaveChangesAsync();

        // Gán đề tài đã nộp báo cáo vào hội đồng
        deTais[1].HoiDongId = hoiDong1.Id;
        db.DeTais.Update(deTais[1]);

        // === THÀNH VIÊN HỘI ĐỒNG ===
        var thanhViens = new List<HoiDongThanhVien>
        {
            new() { HoiDongId = hoiDong1.Id, GiangVienId = giangViens[0].Id, VaiTro = "Chủ tịch" },
            new() { HoiDongId = hoiDong1.Id, GiangVienId = giangViens[1].Id, VaiTro = "Thư ký" },
            new() { HoiDongId = hoiDong1.Id, GiangVienId = giangViens[2].Id, VaiTro = "Phản biện" },
            new() { HoiDongId = hoiDong2.Id, GiangVienId = giangViens[3].Id, VaiTro = "Chủ tịch" },
            new() { HoiDongId = hoiDong2.Id, GiangVienId = giangViens[4].Id, VaiTro = "Thư ký" },
            new() { HoiDongId = hoiDong2.Id, GiangVienId = giangViens[1].Id, VaiTro = "Ủy viên" },
        };

        await db.HoiDongThanhViens.AddRangeAsync(thanhViens);

        // === LỊCH BẢO VỆ ===
        var lichBaoVe = new LichBaoVe
        {
            HoiDongId = hoiDong1.Id,
            DeTaiId = deTais[1].Id,
            ThoiGianBaoVe = new DateTime(2025, 6, 20, 8, 0, 0),
            PhongBaoVe = "B301",
            GhiChu = "Sinh viên chuẩn bị slide và demo"
        };

        await db.LichBaoVes.AddAsync(lichBaoVe);

        // === THÔNG BÁO ===
        var thongBaos = new List<ThongBao>
        {
            new() { UserId = svUsers[0].Id, NoiDung = "Đề tài DT001 của bạn đã được duyệt", NgayTao = new DateTime(2025, 2, 10), DaDoc = true },
            new() { UserId = svUsers[1].Id, NoiDung = "Đề tài DT002 của bạn đã được duyệt", NgayTao = new DateTime(2025, 2, 10), DaDoc = true },
            new() { UserId = svUsers[1].Id, NoiDung = "Bạn đã được xếp vào Hội đồng 1, lịch bảo vệ ngày 20/06/2025", NgayTao = new DateTime(2025, 5, 15), DaDoc = false },
            new() { UserId = svUsers[3].Id, NoiDung = "Đề tài DT004 bị từ chối. Lý do: Đề tài quá rộng, cần thu hẹp phạm vi", NgayTao = new DateTime(2025, 2, 14), DaDoc = true },
            new() { UserId = svUsers[4].Id, NoiDung = "Đề tài DT005 yêu cầu chỉnh sửa, vui lòng xem phản hồi của giảng viên", NgayTao = new DateTime(2025, 3, 1), DaDoc = false },
            new() { UserId = gvUsers[0].Id, NoiDung = "Có sinh viên mới đăng ký đề tài cần duyệt", NgayTao = new DateTime(2025, 3, 5), DaDoc = false },
        };

        await db.ThongBaos.AddRangeAsync(thongBaos);
        await db.SaveChangesAsync();
    }
}
