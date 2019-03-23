using Planner.Mobile.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class ScheduledTaskDataHelper
    {
        public Task<List<ScheduledTask>> GetAllAsync()
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .ToListAsync();
        }

        public Task<List<ScheduledTask>> GetAsync(int take = 10)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .Take(take)
                .ToListAsync();
        }

        public Task<List<ScheduledTask>> GetSortedListAsync(Expression<Func<ScheduledTask, object>> orderBy, int take = 10)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .OrderBy(orderBy)
                .Take(take)
                .ToListAsync();
        }

        public Task<List<ScheduledTask>> SearchAsync(string keyword, int take = 10)
        {
            return PlannerDatabase.Instance
                .QueryAll<ScheduledTask>(@"
                    SELECT * FROM ScheduledTask 
                    WHERE Title LIKE ?1
                        OR Note LIKE ?1
                    ORDER BY Start
                    LIMIT ?2"
                    , $"%{ keyword }%"
                    , take);
        }

        public Task<List<ScheduledTask>> GetAllForRangeAsync(DateTime startDate, DateTime endDate)
        {
            return PlannerDatabase.Instance
                .QueryAll<ScheduledTask>(@"
                    SELECT * FROM ScheduledTask 
                    WHERE (Start BETWEEN ?1 AND ?2) 
                        OR (End BETWEEN ?1 AND ?2) 
                        OR (Start < ?1 AND End > ?2)"
                    , startDate.Ticks, endDate.Ticks);
        }

        public Task<List<ScheduledTask>> GetAllFromDateTimeAsync(DateTime dateTime)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .Where(t => t.ClientUpdatedOn >= dateTime)
                .ToListAsync();
        }

        public Task<ScheduledTask> GetByIdAsync(Guid id)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task InsertAsync(ScheduledTask task)
        {
            return PlannerDatabase.Instance
                .InsertAsync(task);
        }

        public Task InsertAllAsync(IEnumerable<ScheduledTask> tasks)
        {
            return PlannerDatabase.Instance
                .InsertAllAsync(tasks);
        }

        public Task InsertOrUpdateAllAsync(IEnumerable<ScheduledTask> tasks)
        {
            return PlannerDatabase.Instance
                .InsertOrUpdateAllAsync(tasks);
        }

        public Task UpdateAsync(ScheduledTask task)
        {
            return PlannerDatabase.Instance
                .UpdateAsync(task);
        }

        public Task UpdateSyncStatusAsync(Guid id)
        {
            return PlannerDatabase.Instance
                .ExecuteCommandAsync(@"UPDATE ScheduledTask
                                        SET IsChangesSynced = 1
                                       WHERE Id = ?", id);
        }

        public Task DeleteAsync(ScheduledTask task)
        {
            return PlannerDatabase.Instance
                .DeleteAsync(task);
        }
    }
}
