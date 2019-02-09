using Planner.Mobile.Core.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Services
{
    public class ScheduledTaskDataService
    {
        public Task<List<ScheduledTask>> GetScheduledTasksAsync()
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .ToListAsync();
        }

        public Task<ScheduledTask> GetScheduledTaskAsync(int id)
        {
            return PlannerDatabase.Instance
                .GetAll<ScheduledTask>()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task InsertScheduledTaskAsync(ScheduledTask task)
        {
            return PlannerDatabase.Instance
                .InsertAsync(task);
        }
    }
}
