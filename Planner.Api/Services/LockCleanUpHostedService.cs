using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class LockCleanUpHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<LockCleanUpHostedService> _logger;
        private Timer _timer;

        public LockCleanUpHostedService(IServiceProvider provider
            , ILogger<LockCleanUpHostedService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("LockCleanUp Background Service is starting.");

            _timer = new Timer(CleanUpAsync, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("LockCleanUp Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void CleanUpAsync(object state)
        {
            try
            {
                _logger.LogInformation("LockCleanUp Background Service: Clean started.");

                using (var scope = _provider.CreateScope())
                {
                    var lockRepo = scope.ServiceProvider.GetRequiredService<ISyncronizationLockRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var expiredLocks = await lockRepo.GetExpiredSyncronizationLocks();

                    if (!expiredLocks.Any())
                    {
                        _logger.LogInformation("LockCleanUp Background Service: Nothing to clean. Skipping.");
                        return;
                    }

                    lockRepo.DeleteRange(expiredLocks);
                    await unitOfWork.CompleteAsync();

                    _logger.LogInformation("LockCleanUp Background Service: Clean completed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while cleaning locks: { ex }");
                throw;
            }
        }
    }
}
