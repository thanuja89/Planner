using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories.Interfaces
{
    public interface IScheduledTaskRepository : IRepository<ScheduledTask>
    {
        Task<IEnumerable<ScheduledTask>> GetScheduledTasksForUser(string userId);
        Task<IEnumerable<ScheduledTask>> GetNewScheduledTasksForUserAsync(string userId, DateTime lastSyncedOn);
        Task AddNewScheduledTasksAsync(IEnumerable<ScheduledTask> scheduledTasks, string userId);
    }
}
