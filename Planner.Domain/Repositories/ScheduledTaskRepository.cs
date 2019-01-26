using Microsoft.EntityFrameworkCore;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories
{
    public class ScheduledTaskRepository : Repository<ScheduledTask>, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(PlannerDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ScheduledTask>> GetScheduledTasksForUser(string userId)
        {
            return await Entities
                .Where(t => t.ApplicationUserId == userId)
                .ToListAsync();
        }
    }
}
