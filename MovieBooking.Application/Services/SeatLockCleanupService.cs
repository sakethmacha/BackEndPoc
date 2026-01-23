using Microsoft.Extensions.DependencyInjection;
using MovieBooking.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;
namespace MovieBooking.Application.Services
{
    public class SeatLockCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SeatLockCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var bookingService =
                    scope.ServiceProvider.GetRequiredService<IBookingService>();

                await bookingService.ReleaseExpiredSeatLocksAsync();

                // Run every 1 minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
