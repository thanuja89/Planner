using Planner.Dto;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Services
{
    public class SyncHelper
    {
        public Task<IEnumerable<GetScheduledTaskDTO>> PullAsync(long lastSynced)
        {
            return HttpHelper.Instance.GetAsync<IEnumerable<GetScheduledTaskDTO>>($"Syncronization?lastSynced={lastSynced}");

        }

        public Task PushAsync(IEnumerable<ScheduledTask> tasks)
        {
            return HttpHelper.Instance.PutAsync("Syncronization", tasks);
        }
    }
}
