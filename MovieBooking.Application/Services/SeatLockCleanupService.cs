using Microsoft.Extensions.DependencyInjection;
using MovieBooking.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;
namespace MovieBooking.Application.Services
{
    public class SeatLockCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory ScopeFactory;

        public SeatLockCleanupService(IServiceScopeFactory scopeFactory)
        {
            ScopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = ScopeFactory.CreateScope();
                var bookingService =
                    scope.ServiceProvider.GetRequiredService<IBookingService>();

                await bookingService.ReleaseExpiredSeatLocksAsync();

                // Run every 1 minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
