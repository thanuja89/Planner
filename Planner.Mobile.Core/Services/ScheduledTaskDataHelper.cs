using Planner.Mobile.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class ScheduledTaskDataHelper
    {
        public Task<List<ScheduledTask>> GetAllAsync(string userId)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .Where(t => t.ApplicationUserId == userId && !t.IsDeleted)
                .ToListAsync();
        }

        public Task<List<ScheduledTask>> GetAsync(string userId, int take = 10)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .Where(t => t.ApplicationUserId == userId && !t.IsDeleted)
                .OrderByDescending(t => t.Start)
                .Take(take)
                .ToListAsync();
        }

        public Task<List<ScheduledTask>> GetSortedListAsync(string userId, Expression<Func<ScheduledTask, object>> orderBy, int take = 10)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .Where(t => t.ApplicationUserId == userId && !t.IsDeleted)
                .OrderBy(orderBy)
                .Take(take)
                .ToListAsync();
        }

        public Task<List<ScheduledTask>> SearchAsync(string userId, string keyword, int take = 10)
        {
            return PlannerDatabase.Instance
                .QueryAllAsync<ScheduledTask>(@"
                    SELECT * FROM ScheduledTask 
                    WHERE ApplicationUserId = ?3 AND (Title LIKE ?1
                        OR Note LIKE ?1)
                    ORDER BY Start DESC
                    LIMIT ?2"
                    , $"%{ keyword }%", take, userId);
        }

        public Task<List<ScheduledTask>> SearchAndOrderAsync(string userId
            , string keyword
            , string sortBy = "Start"
            , string ascOrDesc = "DESC"
            , int skip = 0
            , int take = 10)
        {
            return PlannerDatabase.Instance
                .QueryAllAsync<ScheduledTask>($@"
                    SELECT * FROM ScheduledTask 
                    WHERE ApplicationUserId = ?4 AND (Title LIKE ?1
                        OR Note LIKE ?1)
                    ORDER BY {sortBy} {ascOrDesc}
                    LIMIT ?3 OFFSET ?2"
                    , $"%{ keyword }%", skip, take, userId);
        }

        public Task<int> CountSearchResultsAsync(string userId
            , string keyword)
        {
            return PlannerDatabase.Instance
                .ExecuteScalarAsync<int>($@"SELECT COUNT(*) FROM ScheduledTask 
                    WHERE ApplicationUserId = ? 2 AND(Title LIKE ? 1
                        OR Note LIKE ? 1)", $"%{ keyword }%", userId);
        }

        public Task<List<ScheduledTask>> GetAllForRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return PlannerDatabase.Instance
                .QueryAllAsync<ScheduledTask>(@"
                    SELECT * FROM ScheduledTask 
                    WHERE ApplicationUserId = ?3 AND (Start BETWEEN ?1 AND ?2) 
                        OR (End BETWEEN ?1 AND ?2) 
                        OR (Start < ?1 AND End > ?2)"
                    , startDate.Ticks, endDate.Ticks, userId);
        }

        public Task<List<ScheduledTask>> GetAllFromDateTimeAsync(string userId, long ticks)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .Where(t => t.ApplicationUserId == userId && t.ClientUpdatedOnTicks >= ticks)
                .ToListAsync();
        }

        public Task<ScheduledTask> GetByIdAsync(Guid id)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task<ScheduledTask> GetByClientSideIdAsync(int id)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .FirstOrDefaultAsync(t => t.ClientSideId == id);
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

        public Task UpdateAsync(ScheduledTask task)
        {
            return PlannerDatabase.Instance
                .UpdateAsync(task);
        }

        public Task MarkAsDeletedAsync(Guid id)
        {
            return PlannerDatabase.Instance
                .ExecuteCommandAsync(@"UPDATE ScheduledTask
                                        SET IsDeleted = 1,
                                            ClientUpdatedOnTicks = ?2
                                       WHERE Id = ?1", id, DateTime.UtcNow.Ticks);
        }

        public Task DeleteAsync(Guid id)
        {
            return PlannerDatabase.Instance
                .ExecuteCommandAsync(@"DELETE 
                                       FROM ScheduledTask
                                       WHERE Id = ?", id);
        }

        public Task DeleteTasksAfterTicksAsync(long ticks)
        {
            return PlannerDatabase.Instance
                .ExecuteCommandAsync(@"DELETE 
                                       FROM ScheduledTask
                                       WHERE IsDeleted = 1
                                            AND ClientUpdatedOnTicks < ?", ticks);
        }
    }
}
