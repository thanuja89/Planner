using Microsoft.EntityFrameworkCore;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        public DeviceRepository(PlannerDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<string>> GetDeviceIdsForUserAsync(string userId)
        {
            return await Entities
                .Where(d => d.ApplicationUserId == userId)
                .Select(d => d.RegistrationId)
                .ToListAsync();
        }
    }
}
