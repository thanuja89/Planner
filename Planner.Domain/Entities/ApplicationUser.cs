using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ScheduledTask>SheduledTasks { get; set; }
    }
}
