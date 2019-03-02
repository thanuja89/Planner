using Microsoft.Extensions.Logging;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class SyncronizationService : ISyncronizationService
    {
        private readonly ISyncronizationLockRepository _lockRepo;
        private readonly IScheduledTaskRepository _scheduledTaskRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SyncronizationService> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public SyncronizationService(ISyncronizationLockRepository lockRepo
            , IScheduledTaskRepository scheduledTaskRepo
            , IUnitOfWork unitOfWork
            , ILogger<SyncronizationService> logger)
        {
            _lockRepo = lockRepo;
            _scheduledTaskRepo = scheduledTaskRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SyncronizationLock> TakeLockAsync(string userId)
        {
            try
            {
                await _semaphore.WaitAsync(1000);

                var lk = await _lockRepo.GetSyncronizationLockByUserId(userId);

                if (lk != null)
                    return null;

                var currentDate = DateTime.UtcNow;

                var newLock = new SyncronizationLock()
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = userId,
                    CreatedOnUtc = currentDate,
                    ExpiresOn = currentDate.AddMinutes(10) // To be taken from appsettings.json
                };

                await _lockRepo.AddAsync(newLock);
                await _unitOfWork.CompleteAsync();

                return newLock;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public Task<SyncronizationLock> GetLockAsync(Guid id)
        {
            return _lockRepo
                .FindAsync(id);
        }

        public Task ReleaseLockAsync(SyncronizationLock syncLock)
        {
            _lockRepo.Delete(syncLock);
            return _unitOfWork.CompleteAsync();
        }

        public async Task ReleaseLockAsync(string userId)
        {
            var lk = await _lockRepo.GetSyncronizationLockByUserId(userId);

            if (lk != null)
            {
                _lockRepo.Delete(lk);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
