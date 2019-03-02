using System;
using System.Threading.Tasks;
using Planner.Domain.Entities;

namespace Planner.Api.Abstractions
{
    public interface ISyncronizationService
    {
        Task<SyncronizationLock> GetLockAsync(Guid id);
        Task ReleaseLockAsync(string userId);
        Task ReleaseLockAsync(SyncronizationLock syncLock);
        Task<LockResult> TakeLockAsync(string userId);
    }
}