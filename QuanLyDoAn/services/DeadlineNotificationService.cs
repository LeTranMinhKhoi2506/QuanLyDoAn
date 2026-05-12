using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuanLyDoAn.Data;
using QuanLyDoAn.Models;
using QuanLyDoAn.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLyDoAn.Services;

public class DeadlineNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeadlineNotificationService> _logger;
    private const int DaysBeforeDeadlineToNotify = 3; // Notify 3 days before deadline

    public DeadlineNotificationService(IServiceProvider serviceProvider, ILogger<DeadlineNotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Deadline Notification Service is starting.");

        // Wait a bit before starting to ensure app is fully started
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                    // Get current date
                    var now = DateTime.Now;
                    var deadlineStart = now.Date.AddDays(DaysBeforeDeadlineToNotify);
                    var deadlineEnd = now.Date.AddDays(DaysBeforeDeadlineToNotify + 1);

                    // Find all DeTai with deadline in the next X days that are not yet completed
                    var upcomingDeTais = await db.DeTais
                        .Include(d => d.SinhVien)
                        .Include(d => d.DotDoAn)
                        .Where(d => 
                            d.SinhVien != null &&
                            d.DotDoAn != null &&
                            d.TrangThai != "Đã hoàn thành" &&
                            d.TrangThai != "Bị từ chối" &&
                            d.TrangThai != "Đã hủy" &&
                            d.DotDoAn.HanNopBaoCao.Date >= deadlineStart &&
                            d.DotDoAn.HanNopBaoCao.Date < deadlineEnd &&
                            !d.IsDeleted)
                        .ToListAsync();

                    foreach (var deTai in upcomingDeTais)
                    {
                        if (deTai.SinhVien == null) continue;

                        // Check if we already sent a notification for this deadline recently
                        // (within the last 24 hours to avoid spam)
                        var lastNotification = await db.ThongBaos
                            .Where(t => 
                                t.UserId == deTai.SinhVien.UserId &&
                                t.NoiDung.Contains("sắp tới hạn nộp") &&
                                t.NgayTao > now.AddDays(-1))
                            .FirstOrDefaultAsync();

                        if (lastNotification == null)
                        {
                            await notificationService.ThongBaoSapHanNop(deTai.Id);
                            _logger.LogInformation($"Sent deadline reminder for DeTai ID {deTai.Id} to student {deTai.SinhVien.HoTen}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking deadline notifications");
            }

            // Wait for 24 hours before checking again
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
