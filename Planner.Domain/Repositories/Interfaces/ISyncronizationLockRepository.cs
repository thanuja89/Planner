using Planner.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories.Interfaces
{
    public interface ISyncronizationLockRepository : IRepository<SyncronizationLock>
    {
        Task<SyncronizationLock> GetSyncronizationLockByUserId(string userId);
        Task<IEnumerable<SyncronizationLock>> GetExpiredSyncronizationLocks();
    }
}
