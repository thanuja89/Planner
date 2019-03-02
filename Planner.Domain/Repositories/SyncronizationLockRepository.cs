using Microsoft.EntityFrameworkCore;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories
{
    public class SyncronizationLockRepository : Repository<SyncronizationLock>, ISyncronizationLockRepository
    {
        public SyncronizationLockRepository(PlannerDbContext context) : base(context)
        {
        }

        public Task<SyncronizationLock> GetSyncronizationLockByUserId(string userId)
        {
            return Entities.FirstOrDefaultAsync(l => l.ApplicationUserId == userId);
        }

        public async Task<IEnumerable<SyncronizationLock>> GetExpiredSyncronizationLocks()
        {
            return await Entities
                .Where(l => l.ExpiresOn <= DateTime.UtcNow)
                .ToListAsync();
        }
    }
}
