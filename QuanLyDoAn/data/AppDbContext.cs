using Microsoft.EntityFrameworkCore;
using QuanLyDoAn.Models;

namespace QuanLyDoAn.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<SinhVien> SinhViens { get; set; } = null!;
    public DbSet<GiangVien> GiangViens { get; set; } = null!;
    public DbSet<DeTai> DeTais { get; set; } = null!;
    public DbSet<TienDo> TienDos { get; set; } = null!;
    public DbSet<LichGap> LichGaps { get; set; } = null!;
    public DbSet<ThongBao> ThongBaos { get; set; } = null!;
    public DbSet<DotDoAn> DotDoAns { get; set; } = null!;
    public DbSet<HoiDong> HoiDongs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SinhVien>()
            .HasOne(s => s.User)
            .WithOne(u => u.SinhVien)
            .HasForeignKey<SinhVien>(s => s.UserId);

        modelBuilder.Entity<GiangVien>()
            .HasOne(g => g.User)
            .WithOne(u => u.GiangVien)
            .HasForeignKey<GiangVien>(g => g.UserId);

        modelBuilder.Entity<DeTai>()
            .HasOne(d => d.SinhVien)
            .WithMany(s => s.DeTais)
            .HasForeignKey(d => d.SinhVienId);

        modelBuilder.Entity<DeTai>()
            .HasOne(d => d.GiangVien)
            .WithMany(g => g.DeTaisHuongDan)
            .HasForeignKey(d => d.GiangVienId);

        modelBuilder.Entity<TienDo>()
            .HasOne(t => t.DeTai)
            .WithMany(d => d.TienDos)
            .HasForeignKey(t => t.DeTaiId);

        modelBuilder.Entity<LichGap>()
            .HasOne(l => l.SinhVien)
            .WithMany()
            .HasForeignKey(l => l.SinhVienId);

        modelBuilder.Entity<LichGap>()
            .HasOne(l => l.GiangVien)
            .WithMany()
            .HasForeignKey(l => l.GiangVienId);

        modelBuilder.Entity<ThongBao>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<DeTai>()
            .HasOne(d => d.DotDoAn)
            .WithMany(dot => dot.DeTais)
            .HasForeignKey(d => d.DotDoAnId);

        modelBuilder.Entity<DeTai>()
            .HasOne(d => d.HoiDong)
            .WithMany(hd => hd.DeTaisDuocCham)
            .HasForeignKey(d => d.HoiDongId);

        modelBuilder.Entity<HoiDong>()
            .HasOne(hd => hd.ChuTich)
            .WithMany()
            .HasForeignKey(hd => hd.ChuTichId);

        modelBuilder.Entity<HoiDong>()
            .HasOne(hd => hd.ThuKy)
            .WithMany()
            .HasForeignKey(hd => hd.ThuKyId);

        modelBuilder.Entity<HoiDong>()
            .HasOne(hd => hd.UyVien)
            .WithMany()
            .HasForeignKey(hd => hd.UyVienId);
    }
}
