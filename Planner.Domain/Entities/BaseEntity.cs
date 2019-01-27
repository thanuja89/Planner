using System;

namespace Planner.Domain.Entities
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}