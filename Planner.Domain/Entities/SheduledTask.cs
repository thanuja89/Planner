﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Domain.Entities
{
    [Table("ScheduledTasks")]
    public class ScheduledTask : BaseEntity
    {
        [Column(TypeName = "VARCHAR(255)")]
        public string Title { get; set; }

        public string Note { get; set; }
        public Importance Importance { get; set; }
        public Frequency Repeat { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ClientUpdatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
