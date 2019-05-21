using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ScheduledTask>SheduledTasks { get; set; }

        public static ApplicationUser NewExternalUser(string email)
        {
            return new ApplicationUser()
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
        }
    }
}
