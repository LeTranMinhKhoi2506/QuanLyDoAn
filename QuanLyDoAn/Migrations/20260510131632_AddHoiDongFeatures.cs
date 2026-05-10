using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDoAn.Migrations
{
    /// <inheritdoc />
    public partial class AddHoiDongFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoiDongs_GiangViens_ChuTichId",
                table: "HoiDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoiDongs_GiangViens_ThuKyId",
                table: "HoiDongs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoiDongs_GiangViens_UyVienId",
                table: "HoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoiDongs_ChuTichId",
                table: "HoiDongs");

            migrationBuilder.DropIndex(
                name: "IX_HoiDongs_ThuKyId",
                table: "HoiDongs");

            migrationBuilder.DropColumn(
                name: "ChuTichId",
                table: "HoiDongs");

            migrationBuilder.DropColumn(
                name: "ThuKyId",
                table: "HoiDongs");

            migrationBuilder.RenameColumn(
                name: "UyVienId",
                table: "HoiDongs",
                newName: "DotDoAnId");

            migrationBuilder.RenameColumn(
                name: "PhongBaoVe",
                table: "HoiDongs",
                newName: "NgayTao");

            migrationBuilder.RenameColumn(
                name: "NgayBaoVe",
                table: "HoiDongs",
                newName: "MoTa");

            migrationBuilder.RenameIndex(
                name: "IX_HoiDongs_UyVienId",
                table: "HoiDongs",
                newName: "IX_HoiDongs_DotDoAnId");

            migrationBuilder.AddColumn<bool>(
                name: "DaKhoa",
                table: "HoiDongs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HoiDongs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DiemHoiDongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiemBaoCao = table.Column<double>(type: "REAL", nullable: false),
                    DiemThuyetTrinh = table.Column<double>(type: "REAL", nullable: false),
                    DiemSanPham = table.Column<double>(type: "REAL", nullable: false),
                    DiemTraLoiCauHoi = table.Column<double>(type: "REAL", nullable: false),
                    NhanXet = table.Column<string>(type: "TEXT", nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeTaiId = table.Column<int>(type: "INTEGER", nullable: false),
                    GiangVienId = table.Column<int>(type: "INTEGER", nullable: false),
                    HoiDongId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemHoiDongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiemHoiDongs_DeTais_DeTaiId",
                        column: x => x.DeTaiId,
                        principalTable: "DeTais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiemHoiDongs_GiangViens_GiangVienId",
                        column: x => x.GiangVienId,
                        principalTable: "GiangViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiemHoiDongs_HoiDongs_HoiDongId",
                        column: x => x.HoiDongId,
                        principalTable: "HoiDongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoiDongThanhViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VaiTro = table.Column<string>(type: "TEXT", nullable: false),
                    HoiDongId = table.Column<int>(type: "INTEGER", nullable: false),
                    GiangVienId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoiDongThanhViens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoiDongThanhViens_GiangViens_GiangVienId",
                        column: x => x.GiangVienId,
                        principalTable: "GiangViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoiDongThanhViens_HoiDongs_HoiDongId",
                        column: x => x.HoiDongId,
                        principalTable: "HoiDongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichBaoVes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ThoiGianBaoVe = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PhongBaoVe = table.Column<string>(type: "TEXT", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", nullable: false),
                    HoiDongId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeTaiId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichBaoVes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichBaoVes_DeTais_DeTaiId",
                        column: x => x.DeTaiId,
                        principalTable: "DeTais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichBaoVes_HoiDongs_HoiDongId",
                        column: x => x.HoiDongId,
                        principalTable: "HoiDongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiemHoiDongs_DeTaiId",
                table: "DiemHoiDongs",
                column: "DeTaiId");

            migrationBuilder.CreateIndex(
                name: "IX_DiemHoiDongs_GiangVienId",
                table: "DiemHoiDongs",
                column: "GiangVienId");

            migrationBuilder.CreateIndex(
                name: "IX_DiemHoiDongs_HoiDongId",
                table: "DiemHoiDongs",
                column: "HoiDongId");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongThanhViens_GiangVienId",
                table: "HoiDongThanhViens",
                column: "GiangVienId");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongThanhViens_HoiDongId",
                table: "HoiDongThanhViens",
                column: "HoiDongId");

            migrationBuilder.CreateIndex(
                name: "IX_LichBaoVes_DeTaiId",
                table: "LichBaoVes",
                column: "DeTaiId");

            migrationBuilder.CreateIndex(
                name: "IX_LichBaoVes_HoiDongId",
                table: "LichBaoVes",
                column: "HoiDongId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoiDongs_DotDoAns_DotDoAnId",
                table: "HoiDongs",
                column: "DotDoAnId",
                principalTable: "DotDoAns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoiDongs_DotDoAns_DotDoAnId",
                table: "HoiDongs");

            migrationBuilder.DropTable(
                name: "DiemHoiDongs");

            migrationBuilder.DropTable(
                name: "HoiDongThanhViens");

            migrationBuilder.DropTable(
                name: "LichBaoVes");

            migrationBuilder.DropColumn(
                name: "DaKhoa",
                table: "HoiDongs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HoiDongs");

            migrationBuilder.RenameColumn(
                name: "NgayTao",
                table: "HoiDongs",
                newName: "PhongBaoVe");

            migrationBuilder.RenameColumn(
                name: "MoTa",
                table: "HoiDongs",
                newName: "NgayBaoVe");

            migrationBuilder.RenameColumn(
                name: "DotDoAnId",
                table: "HoiDongs",
                newName: "UyVienId");

            migrationBuilder.RenameIndex(
                name: "IX_HoiDongs_DotDoAnId",
                table: "HoiDongs",
                newName: "IX_HoiDongs_UyVienId");

            migrationBuilder.AddColumn<int>(
                name: "ChuTichId",
                table: "HoiDongs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThuKyId",
                table: "HoiDongs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_ChuTichId",
                table: "HoiDongs",
                column: "ChuTichId");

            migrationBuilder.CreateIndex(
                name: "IX_HoiDongs_ThuKyId",
                table: "HoiDongs",
                column: "ThuKyId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoiDongs_GiangViens_ChuTichId",
                table: "HoiDongs",
                column: "ChuTichId",
                principalTable: "GiangViens",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HoiDongs_GiangViens_ThuKyId",
                table: "HoiDongs",
                column: "ThuKyId",
                principalTable: "GiangViens",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HoiDongs_GiangViens_UyVienId",
                table: "HoiDongs",
                column: "UyVienId",
                principalTable: "GiangViens",
                principalColumn: "Id");
        }
    }
}
