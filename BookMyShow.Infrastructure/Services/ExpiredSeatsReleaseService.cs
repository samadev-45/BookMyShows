using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BookMyShow.Application.Interfaces;

namespace BookMyShow.Infrastructure.Services
{
    public class ExpiredSeatsReleaseService : BackgroundService
    {
        private readonly ILogger<ExpiredSeatsReleaseService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExpiredSeatsReleaseService(ILogger<ExpiredSeatsReleaseService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Expired Seat Release Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Expired Seat Release Service checking for expired held seats.");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    await bookingService.ReleaseExpiredHeldSeatsAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Check every 5 minutes
            }

            _logger.LogInformation("Expired Seat Release Service stopped.");
        }
    }
}