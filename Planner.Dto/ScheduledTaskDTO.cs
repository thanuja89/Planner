using Planner.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Planner.Dto
{
    public class ScheduledTaskDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Importance Importance { get; set; }
        public Frequency Repeat { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public bool IsNotify { get; set; }
        public bool IsAlarm { get; set; }
    }

    public class GetScheduledTaskDTO : ScheduledTaskDTO
    {
        public long Id { get; set; }
    }

    public class PostScheduledTaskDTO : ScheduledTaskDTO
    {
        
    }

    public class PutScheduledTaskDTO : ScheduledTaskDTO
    {

    }
}
