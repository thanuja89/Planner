using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Services
{
    public class SyncHelper
    {
        private readonly HttpHelper _httpHelper;

        public SyncHelper()
        {
            _httpHelper = new HttpHelper();
        }

        public Task<IEnumerable<ScheduledTask>> PullAsync(DateTime lastSynced)
        {
            return _httpHelper.GetAsync<IEnumerable<ScheduledTask>>($"Syncronization/lastSynced={ lastSynced.ToString("yyyyMMddHHmmss") }"); 
        }

        public Task PushAsync(IEnumerable<ScheduledTask> tasks)
        {
            return _httpHelper.PostAsync("Syncronization", tasks);
        }
    }
}
