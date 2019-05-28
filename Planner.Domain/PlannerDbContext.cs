using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Planner.Domain.Entities;

namespace Planner.Domain
{
    public class PlannerDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ScheduledTask> SheduledTasks { get; set; }

        public PlannerDbContext(DbContextOptions<PlannerDbContext> options) : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var importanceConverter = new EnumToStringConverter<Importance>();
            var frequencyConverter = new EnumToStringConverter<Frequency>();

            builder.Entity<ScheduledTask>()
                .Property(t => t.Importance)
                .HasConversion(importanceConverter);

            builder.Entity<ScheduledTask>()
                .Property(t => t.Repeat)
                .HasConversion(frequencyConverter);

            base.OnModelCreating(builder);
        }
    }
}
