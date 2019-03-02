using System;
using System.ComponentModel.DataAnnotations;

namespace Planner.Domain.Entities
{
    public class SyncronizationLock : BaseEntity
    {
        public DateTime ExpiresOn { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
