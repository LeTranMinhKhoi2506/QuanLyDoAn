using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDoAn.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiemHoiDong",
                table: "DeTais",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiemHuongDan",
                table: "DeTais",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiemPhanBien",
                table: "DeTais",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DotDoAnId",
                table: "DeTais",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileBaoCao",
                table: "DeTais",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileSlide",
                table: "DeTais",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileSourceCode",
                table: "DeTais",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HoiDongId",
                table: "DeTais",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkDemo",
                table: "DeTais",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkGitHub",
                table: "DeTais",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DotDoAns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenDot = table.Column<string>(type: "TEXT", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HanDangKyDeTai = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HanNopBaoCao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotDoAns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HoiDongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenHoiDong = table.Column<string>(type: "TEXT", nullable: false),
                    NgayBaoVe = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PhongBaoVe = table.Column<string>(type: "TEXT", nullable: false),
                    ChuTichId = table.Column<int>(type: "INTEGER", nullable: true),
                    ThuKyId = table.Column<int>(type: "INTEGER", nullable: true),
                    UyVienId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoiDongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoiDongs_GiangViens_ChuTichId",
                        column: x => x.ChuTichId,
                        principalTable: "GiangViens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoiDongs_GiangViens_ThuKyId",
                        column: x => x.ThuKyId,
                        principalTable: "GiangViens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HoiDongs_GiangViens_UyVienId",
                        column: x => x.UyVienId,
                        principalTable: "GiangViens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LichGaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NgayGioGap = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoiDung = table.Column<string>(type: "TEXT", nullable: false),
                    HinhThuc = table.Column<string>(type: "TEXT", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", nullable: false),
                    SinhVienId = table.Column<int>(type: "INTEGER", nullable: false),
                    GiangVienId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichGaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichGaps_GiangViens_GiangVienId",
                        column: x => x.GiangVienId,
                        principalTable: "GiangViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichGaps_SinhViens_SinhVienId",
                        column: x => x.SinhVienId,
                        principalTable: "SinhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoiDung = table.Column<string>(type: "TEXT", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DaDoc = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThongBaos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeTais_DotDoAnId",
                table: "DeTais",
                column: "DotDoAnId");

            migrationBuilder.CreateIndex(
                name: "IX_DeTais_HoiDongId",
                table: "DeTais",
                column: "HoiDongId");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_ChuTichId",
                table: "HoiDongs",
                column: "ChuTichId");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_ThuKyId",
                table: "HoiDongs",
                column: "ThuKyId");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_UyVienId",
                table: "HoiDongs",
                column: "UyVienId");

            migrationBuilder.CreateIndex(
                name: "IX_LichGaps_GiangVienId",
                table: "LichGaps",
                column: "GiangVienId");

            migrationBuilder.CreateIndex(
                name: "IX_LichGaps_SinhVienId",
                table: "LichGaps",
                column: "SinhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_UserId",
                table: "ThongBaos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeTais_DotDoAns_DotDoAnId",
                table: "DeTais",
                column: "DotDoAnId",
                principalTable: "DotDoAns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeTais_HoiDongs_HoiDongId",
                table: "DeTais",
                column: "HoiDongId",
                principalTable: "HoiDongs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeTais_DotDoAns_DotDoAnId",
                table: "DeTais");

            migrationBuilder.DropForeignKey(
                name: "FK_DeTais_HoiDongs_HoiDongId",
                table: "DeTais");

            migrationBuilder.DropTable(
                name: "DotDoAns");

            migrationBuilder.DropTable(
                name: "HoiDongs");

            migrationBuilder.DropTable(
                name: "LichGaps");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropIndex(
                name: "IX_DeTais_DotDoAnId",
                table: "DeTais");

            migrationBuilder.DropIndex(
                name: "IX_DeTais_HoiDongId",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "DiemHoiDong",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "DiemHuongDan",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "DiemPhanBien",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "DotDoAnId",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "FileBaoCao",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "FileSlide",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "FileSourceCode",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "HoiDongId",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "LinkDemo",
                table: "DeTais");

            migrationBuilder.DropColumn(
                name: "LinkGitHub",
                table: "DeTais");
        }
    }
}
