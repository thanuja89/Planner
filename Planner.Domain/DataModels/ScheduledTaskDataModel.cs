using Planner.Domain.Entities;
using System;

namespace Planner.Domain.DataModels
{
    public class ScheduledTaskDataModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public Importance Importance { get; set; }
        public Frequency Repeat { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAlarm { get; set; }
        public DateTime ClientUpdatedOn { get; set; }
    }
}
