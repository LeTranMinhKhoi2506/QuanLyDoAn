using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDoAn.Migrations
{
    /// <inheritdoc />
    public partial class AddGiangVienTrangThaiAndLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GioiHanDeTai",
                table: "GiangViens",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GioiHanSinhVien",
                table: "GiangViens",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "GiangViens",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GioiHanDeTai",
                table: "GiangViens");

            migrationBuilder.DropColumn(
                name: "GioiHanSinhVien",
                table: "GiangViens");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "GiangViens");
        }
    }
}
