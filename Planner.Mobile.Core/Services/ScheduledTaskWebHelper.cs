using Planner.Mobile.Core.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class ScheduledTaskWebHelper
    {
        public Task<IEnumerable<ScheduledTask>> GetScheduledTasksAsync()
        {
            return HttpHelper.Instance.GetAsync<IEnumerable<ScheduledTask>>("ScheduledTask");
        }

        public Task<IEnumerable<ScheduledTask>> GetScheduledTaskAsync(Guid id)
        {
            return HttpHelper.Instance.GetAsync<IEnumerable<ScheduledTask>>($"ScheduledTask/{ id }");
        }

        public Task<HttpResponseMessage> CreateScheduledTaskAsync(ScheduledTask task)
        {
            return HttpHelper.Instance.PostAsync("ScheduledTask", task);
        }

        public Task<HttpResponseMessage> UpdateScheduledTaskAsync(Guid id, ScheduledTask task)
        {
            return HttpHelper.Instance.PutAsync($"ScheduledTask/{ id }", task);
        }

        public Task<HttpResponseMessage> DeleteScheduledTaskAsync(Guid id)
        {
            return HttpHelper.Instance.DeleteAsync($"ScheduledTask/{ id }");
        }
    }
}
