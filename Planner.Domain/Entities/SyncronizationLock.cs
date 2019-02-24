using System.ComponentModel.DataAnnotations;

namespace Planner.Domain.Entities
{
    public class SyncronizationLock : BaseEntity
    {
        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
