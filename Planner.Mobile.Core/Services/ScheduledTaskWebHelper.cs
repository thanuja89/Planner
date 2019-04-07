using Planner.Dto;
using Planner.Mobile.Core.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class ScheduledTaskWebHelper
    {
        private readonly HttpHelper _httpService;

        public ScheduledTaskWebHelper()
        {
            _httpService = new HttpHelper();
        }

        public Task<IEnumerable<ScheduledTask>> GetScheduledTasksAsync()
        {
            return _httpService.GetAsync<IEnumerable<ScheduledTask>>("ScheduledTask");
        }

        public Task<IEnumerable<ScheduledTask>> GetScheduledTaskAsync(Guid id)
        {
            return _httpService.GetAsync<IEnumerable<ScheduledTask>>($"ScheduledTask/{ id }");
        }

        public Task<HttpResponseMessage> CreateScheduledTaskAsync(ScheduledTask task)
        {
            return _httpService.PostAsync("ScheduledTask", task);
        }

        public Task<HttpResponseMessage> UpdateScheduledTaskAsync(Guid id, ScheduledTask task)
        {
            return _httpService.PutAsync($"ScheduledTask/{ id }", task);
        }

        public Task<HttpResponseMessage> DeleteScheduledTaskAsync(Guid id)
        {
            return _httpService.DeleteAsync($"ScheduledTask/{ id }");
        }
    }
}
