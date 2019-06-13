using System;
using System.ComponentModel.DataAnnotations;

namespace Planner.Dto
{
    public class ScheduledTaskDTO
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public Importance Importance { get; set; }
        public Frequency Repeat { get; set; }
        public long ClientUpdatedOnTicks { get; set; }
        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }
    }

    public class GetScheduledTaskDTO : ScheduledTaskDTO
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public string ApplicationUserId { get; set; }
    }

    public class PostScheduledTaskDTO : ScheduledTaskDTO
    {
        public Guid Id { get; set; }
    }

    public class PutScheduledTaskDTO : ScheduledTaskDTO
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
