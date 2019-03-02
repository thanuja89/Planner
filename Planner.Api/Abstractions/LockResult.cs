using Planner.Domain.Entities;

namespace Planner.Api.Abstractions
{
    public class LockResult
    {
        public bool IsSucceeded { get; set; }
        public SyncronizationLock Lock { get; set; }

        public static LockResult FailedResult { get; } = new LockResult();
    }
}
