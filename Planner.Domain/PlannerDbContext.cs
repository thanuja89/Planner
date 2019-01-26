using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Planner.Domain.Entities;

namespace Planner.Domain
{
    public class PlannerDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ScheduledTask> SheduledTasks { get; set; }

        public PlannerDbContext(DbContextOptions<PlannerDbContext> options) : base(options)
        {

        }
    }
}
