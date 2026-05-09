using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDoAn.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDotDoAnModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaKhoa",
                table: "DotDoAns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DangMoDangKy",
                table: "DotDoAns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HocKy",
                table: "DotDoAns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoaiDot",
                table: "DotDoAns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "DotDoAns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NamHoc",
                table: "DotDoAns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "DotDoAns",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaKhoa",
                table: "DotDoAns");

            migrationBuilder.DropColumn(
                name: "DangMoDangKy",
                table: "DotDoAns");

            migrationBuilder.DropColumn(
                name: "HocKy",
                table: "DotDoAns");

            migrationBuilder.DropColumn(
                name: "LoaiDot",
                table: "DotDoAns");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "DotDoAns");

            migrationBuilder.DropColumn(
                name: "NamHoc",
                table: "DotDoAns");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "DotDoAns");
        }
    }
}
