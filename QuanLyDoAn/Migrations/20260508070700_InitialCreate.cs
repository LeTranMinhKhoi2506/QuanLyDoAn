using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDoAn.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiangViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaGiangVien = table.Column<string>(type: "TEXT", nullable: false),
                    HoTen = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    SoDienThoai = table.Column<string>(type: "TEXT", nullable: false),
                    BoMon = table.Column<string>(type: "TEXT", nullable: false),
                    HocVi = table.Column<string>(type: "TEXT", nullable: false),
                    SoLuongSinhVienDangHuongDan = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiangViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiangViens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SinhViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaSinhVien = table.Column<string>(type: "TEXT", nullable: false),
                    HoTen = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    SoDienThoai = table.Column<string>(type: "TEXT", nullable: false),
                    Lop = table.Column<string>(type: "TEXT", nullable: false),
                    KhoaHoc = table.Column<string>(type: "TEXT", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SinhViens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeTais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaDeTai = table.Column<string>(type: "TEXT", nullable: false),
                    TenDeTai = table.Column<string>(type: "TEXT", nullable: false),
                    MoTaDeTai = table.Column<string>(type: "TEXT", nullable: false),
                    LoaiDeTai = table.Column<string>(type: "TEXT", nullable: false),
                    CongNgheSuDung = table.Column<string>(type: "TEXT", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SinhVienId = table.Column<int>(type: "INTEGER", nullable: true),
                    GiangVienId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeTais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeTais_GiangViens_GiangVienId",
                        column: x => x.GiangVienId,
                        principalTable: "GiangViens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeTais_SinhViens_SinhVienId",
                        column: x => x.SinhVienId,
                        principalTable: "SinhViens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TienDos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenCongViec = table.Column<string>(type: "TEXT", nullable: false),
                    NoiDungDaLam = table.Column<string>(type: "TEXT", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PhanTramHoanThanh = table.Column<int>(type: "INTEGER", nullable: false),
                    FileMinhChung = table.Column<string>(type: "TEXT", nullable: false),
                    NhanXetCuaGiangVien = table.Column<string>(type: "TEXT", nullable: false),
                    DeTaiId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TienDos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TienDos_DeTais_DeTaiId",
                        column: x => x.DeTaiId,
                        principalTable: "DeTais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeTais_GiangVienId",
                table: "DeTais",
                column: "GiangVienId");

            migrationBuilder.CreateIndex(
                name: "IX_DeTais_SinhVienId",
                table: "DeTais",
                column: "SinhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_GiangViens_UserId",
                table: "GiangViens",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_UserId",
                table: "SinhViens",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TienDos_DeTaiId",
                table: "TienDos",
                column: "DeTaiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TienDos");

            migrationBuilder.DropTable(
                name: "DeTais");

            migrationBuilder.DropTable(
                name: "GiangViens");

            migrationBuilder.DropTable(
                name: "SinhViens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
