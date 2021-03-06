﻿using SQLite;
using System;

namespace Planner.Mobile.Core.Data
{
    [Table("ScheduledTask")]
    public class ScheduledTask
    {
        public Guid Id { get; set; }
        [PrimaryKey, AutoIncrement]
        public int ClientSideId { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public Importance Importance { get; set; }
        public Frequency Repeat { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public long ClientUpdatedOnTicks { get; set; }
        public bool IsDeleted { get; set; }
        public string ApplicationUserId { get; set; }
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
