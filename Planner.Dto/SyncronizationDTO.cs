using System;
using System.Collections.Generic;

namespace Planner.Dto
{
    public class SyncronizationDTO
    {
        public IEnumerable<GetScheduledTaskDTO> PutScheduledTasks { get; set; }
        public Guid LockId { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
