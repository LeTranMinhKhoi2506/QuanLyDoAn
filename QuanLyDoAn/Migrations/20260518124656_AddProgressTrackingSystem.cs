using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDoAn.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressTrackingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MilestoneId",
                table: "TienDos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CanhBaosTienDo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoiDung = table.Column<string>(type: "TEXT", nullable: false),
                    LoaiCanhBao = table.Column<string>(type: "TEXT", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DaXem = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeTaiId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanhBaosTienDo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanhBaosTienDo_DeTais_DeTaiId",
                        column: x => x.DeTaiId,
                        principalTable: "DeTais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CongViecsDeTai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenCongViec = table.Column<string>(type: "TEXT", nullable: false),
                    MoTa = table.Column<string>(type: "TEXT", nullable: false),
                    ThuTuThuc = table.Column<int>(type: "INTEGER", nullable: false),
                    HanKyVong = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DaHoanThanh = table.Column<bool>(type: "INTEGER", nullable: false),
                    NgayHoanThanh = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeTaiId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongViecsDeTai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CongViecsDeTai_DeTais_DeTaiId",
                        column: x => x.DeTaiId,
                        principalTable: "DeTais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Milestones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenMilestone = table.Column<string>(type: "TEXT", nullable: false),
                    MoTa = table.Column<string>(type: "TEXT", nullable: false),
                    HanKyVong = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PercentageExpected = table.Column<int>(type: "INTEGER", nullable: false),
                    TrangThai = table.Column<string>(type: "TEXT", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeTaiId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Milestones_DeTais_DeTaiId",
                        column: x => x.DeTaiId,
                        principalTable: "DeTais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TienDos_MilestoneId",
                table: "TienDos",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_CanhBaosTienDo_DeTaiId",
                table: "CanhBaosTienDo",
                column: "DeTaiId");

            migrationBuilder.CreateIndex(
                name: "IX_CongViecsDeTai_DeTaiId",
                table: "CongViecsDeTai",
                column: "DeTaiId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_DeTaiId",
                table: "Milestones",
                column: "DeTaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_TienDos_Milestones_MilestoneId",
                table: "TienDos",
                column: "MilestoneId",
                principalTable: "Milestones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TienDos_Milestones_MilestoneId",
                table: "TienDos");

            migrationBuilder.DropTable(
                name: "CanhBaosTienDo");

            migrationBuilder.DropTable(
                name: "CongViecsDeTai");

            migrationBuilder.DropTable(
                name: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_TienDos_MilestoneId",
                table: "TienDos");

            migrationBuilder.DropColumn(
                name: "MilestoneId",
                table: "TienDos");
        }
    }
}
