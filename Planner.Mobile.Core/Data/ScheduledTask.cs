﻿using SQLite;
using System;

namespace Planner.Mobile.Core.Data
{
    [Table("ScheduledTask")]
    public class ScheduledTask
    {
        [PrimaryKey]
        public int Id { get; set; }
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

    public enum Importance
    {
        Low,
        Medium,
        High
    }

    public enum Frequency
    {
        Never,
        EveryDay,
        Weekdays,
        Weekends,
        Weekly,
        Monthly,
        Yearly
    }
}