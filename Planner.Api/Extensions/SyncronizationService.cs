using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Api.Extensions
{
    public class SyncronizationService
    {
        private readonly IRepository<SyncronizationLock> _lockRepository;
        private readonly IScheduledTaskRepository _scheduledTaskRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SyncronizationService> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public SyncronizationService(IRepository<SyncronizationLock> lockRepository
            , IScheduledTaskRepository scheduledTaskRepo
            , IUnitOfWork unitOfWork
            , ILogger<SyncronizationService> logger)
        {
            _lockRepository = lockRepository;
            _scheduledTaskRepo = scheduledTaskRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SyncronizationLock> TakeLockAsync(string userId)
        {
            try
            {
                await _semaphore.WaitAsync(1000);

                var lk = await _lockRepository.GetAll()
                    .FirstOrDefaultAsync(l => l.ApplicationUserId == userId);

                if (lk != null)
                    return null;

                var newLock = new SyncronizationLock()
                {
                    ApplicationUserId = userId,
                    CreatedOnUtc = DateTime.UtcNow
                };

                await _lockRepository.AddAsync(newLock);
                await _unitOfWork.CompleteAsync();

                return newLock;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ReleaseLockAsync(string userId)
        {
            var lk = await _lockRepository.GetAll()
                    .FirstOrDefaultAsync(l => l.ApplicationUserId == userId);

            if(lk != null)
            {
                _lockRepository.Delete(lk);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
