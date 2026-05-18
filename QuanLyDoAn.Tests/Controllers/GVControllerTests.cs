using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using QuanLyDoAn.Controllers.Admin;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using System.Security.Claims;

namespace QuanLyDoAn.Tests.Controllers;

public class GVControllerTests
{
    private AppDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        
        var context = new AppDbContext(options);
        return context;
    }

    private GVController GetController(AppDbContext context, int userIdStr = 1)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userIdStr.ToString()),
            new Claim(ClaimTypes.Role, "Giảng viên")
        }, "mock"));

        var tempDataMock = new Mock<ITempDataDictionary>();

        var controller = new GVController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            },
            TempData = tempDataMock.Object
        };

        return controller;
    }

    [Fact]
    public async Task TienDo_ReturnsViewResult_WithListOfTienDo_WhenUserIsGiangVien()
    {
        // Arrange
        var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        
        var giangVien = new GiangVien { Id = 1, UserId = 1, HoTen = "GV A" };
        context.GiangViens.Add(giangVien);
        
        var deTai = new DeTai { Id = 1, TenDeTai = "Đề tài 1", GiangVienId = 1, SinhVien = new SinhVien { Id = 1, HoTen = "SV 1" } };
        context.DeTais.Add(deTai);
        
        context.TienDos.Add(new TienDo { Id = 1, DeTaiId = 1, TenCongViec = "Tuần 1", NgayCapNhat = DateTime.Now });
        context.TienDos.Add(new TienDo { Id = 2, DeTaiId = 1, TenCongViec = "Tuần 2", NgayCapNhat = DateTime.Now.AddDays(-1) });
        
        await context.SaveChangesAsync();

        var controller = GetController(context, 1);

        // Act
        var result = await controller.TienDo();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<TienDo>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task CapNhatNhanXet_ValidId_UpdatesNhanXetAndRedirects()
    {
        // Arrange
        var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        
        var tienDo = new TienDo { Id = 1, DeTaiId = 1, TenCongViec = "Báo cáo tiến độ", NhanXetCuaGiangVien = "" };
        context.TienDos.Add(tienDo);
        await context.SaveChangesAsync();

        var controller = GetController(context);

        // Act
        var result = await controller.CapNhatNhanXet(1, "Làm tốt lắm");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("TienDo", redirectResult.ActionName);
        
        var dbTienDo = await context.TienDos.FindAsync(1);
        Assert.Equal("Làm tốt lắm", dbTienDo!.NhanXetCuaGiangVien);
    }

    [Fact]
    public async Task DanhSachSinhVien_ReturnsViewResult_WithListOfDeTai()
    {
        // Arrange
        var context = GetInMemoryDbContext(Guid.NewGuid().ToString());
        
        var giangVien = new GiangVien { Id = 1, UserId = 1, HoTen = "GV A" };
        context.GiangViens.Add(giangVien);
        
        var deTai1 = new DeTai { Id = 1, TenDeTai = "Đề tài 1", GiangVienId = 1, SinhVien = new SinhVien { Id = 1, HoTen = "SV 1" }, NgayTao = DateTime.Now };
        var deTai2 = new DeTai { Id = 2, TenDeTai = "Đề tài 2", GiangVienId = 1, SinhVien = new SinhVien { Id = 2, HoTen = "SV 2" }, NgayTao = DateTime.Now.AddDays(-1) };
        
        context.DeTais.AddRange(deTai1, deTai2);
        await context.SaveChangesAsync();

        var controller = GetController(context, 1);

        // Act
        var result = await controller.DanhSachSinhVien();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<DeTai>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }
}
