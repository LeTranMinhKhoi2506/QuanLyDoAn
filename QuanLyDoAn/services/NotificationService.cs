using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Data;
using QuanLyDoAn.Hubs;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Services;

public class NotificationService(AppDbContext db, IHubContext<NotificationHub> hub)
{
    public async Task GuiThongBao(int userId, string noiDung)
    {
        var tb = new ThongBao { UserId = userId, NoiDung = noiDung, NgayTao = DateTime.Now, DaDoc = false };
        db.ThongBaos.Add(tb);
        await db.SaveChangesAsync();
        await hub.Clients.Group($"user_{userId}").SendAsync("NhanThongBao", new { tb.Id, tb.NoiDung, tb.NgayTao });
    }

    // Đề tài được duyệt / bị từ chối → Sinh viên
    public async Task ThongBaoDeTai(int deTaiId, string noiDung)
    {
        var deTai = await db.DeTais.Include(d => d.SinhVien).ThenInclude(sv => sv!.User)
            .FirstOrDefaultAsync(d => d.Id == deTaiId);
        if (deTai?.SinhVien?.UserId is int userId)
            await GuiThongBao(userId, noiDung);
    }

    // Có nhận xét mới → Sinh viên
    public async Task ThongBaoNhanXet(int deTaiId)
    {
        await ThongBaoDeTai(deTaiId, "Giảng viên vừa thêm nhận xét mới cho đề tài của bạn.");
    }

    // Có tiến độ mới / file nộp mới → Giảng viên
    public async Task ThongBaoChoGiangVien(int deTaiId, string noiDung)
    {
        var deTai = await db.DeTais.Include(d => d.GiangVien).ThenInclude(gv => gv!.User)
            .FirstOrDefaultAsync(d => d.Id == deTaiId);
        if (deTai?.GiangVien?.UserId is int userId)
            await GuiThongBao(userId, noiDung);
    }

    // Sắp tới hạn nộp → Sinh viên (gọi từ background job hoặc thủ công)
    public async Task ThongBaoSapHanNop(int deTaiId)
    {
        await ThongBaoDeTai(deTaiId, "Đề tài của bạn sắp tới hạn nộp báo cáo. Vui lòng hoàn thành đúng hạn.");
    }

    // Được thêm vào hội đồng → Giảng viên
    public async Task ThongBaoThemVaoHoiDong(int giangVienId, string tenHoiDong)
    {
        var gv = await db.GiangViens.Include(g => g.User).FirstOrDefaultAsync(g => g.Id == giangVienId);
        if (gv?.UserId is int userId)
            await GuiThongBao(userId, $"Bạn đã được thêm vào hội đồng \"{tenHoiDong}\".");
    }

    // Hội đồng bị thay đổi → Tất cả thành viên hội đồng
    public async Task ThongBaoHoiDongThayDoi(int hoiDongId, string noiDung)
    {
        var thanhViens = await db.HoiDongThanhViens
            .Include(tv => tv.GiangVien).ThenInclude(gv => gv.User)
            .Where(tv => tv.HoiDongId == hoiDongId)
            .ToListAsync();
        foreach (var tv in thanhViens)
            await GuiThongBao(tv.GiangVien.UserId, noiDung);
    }

    // Có lịch bảo vệ → Sinh viên + Giảng viên hội đồng
    public async Task ThongBaoLichBaoVe(int lichBaoVeId)
    {
        var lich = await db.LichBaoVes
            .Include(l => l.DeTai).ThenInclude(d => d.SinhVien).ThenInclude(sv => sv!.User)
            .Include(l => l.HoiDong).ThenInclude(hd => hd.ThanhViens).ThenInclude(tv => tv.GiangVien).ThenInclude(gv => gv.User)
            .FirstOrDefaultAsync(l => l.Id == lichBaoVeId);
        if (lich == null) return;

        var noiDung = $"Lịch bảo vệ đề tài \"{lich.DeTai.TenDeTai}\" vào {lich.ThoiGianBaoVe:dd/MM/yyyy HH:mm} tại {lich.PhongBaoVe}.";

        if (lich.DeTai.SinhVien?.UserId is int svUserId)
            await GuiThongBao(svUserId, noiDung);

        foreach (var tv in lich.HoiDong.ThanhViens)
            await GuiThongBao(tv.GiangVien.UserId, noiDung);
    }
}
