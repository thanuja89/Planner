using Planner.Domain.Entities;
using System;

namespace Planner.Dto
{
    public class SheduledTaskDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public Importance Importance { get; set; }
        public Frequency Repeat { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsNotify { get; set; }
        public bool IsAlarm { get; set; }
    }
}
