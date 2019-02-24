using Planner.Dto;
using System.Collections.Generic;
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

        public Task<IEnumerable<GetScheduledTaskDTO>> GetScheduledTasksAsync()
        {
            return _httpService.GetAsync<IEnumerable<GetScheduledTaskDTO>>("ScheduledTask");
        }

        public Task<IEnumerable<GetScheduledTaskDTO>> GetScheduledTaskAsync(int id)
        {
            return _httpService.GetAsync<IEnumerable<GetScheduledTaskDTO>>($"ScheduledTask/{ id }");
        }

        public Task<GetScheduledTaskDTO> CreateScheduledTaskAsync(ScheduledTaskDTO taskDTO)
        {
            return _httpService.PostForResultAsync<GetScheduledTaskDTO>("ScheduledTask", taskDTO);
        }

        public Task<GetScheduledTaskDTO> UpdateScheduledTaskAsync(int id, ScheduledTaskDTO taskDTO)
        {
            return _httpService.PutForResultAsync<GetScheduledTaskDTO>($"ScheduledTask/{ id }", taskDTO);
        }

        public Task DeleteScheduledTaskAsync(int id)
        {
            return _httpService.DeleteAsync($"ScheduledTask/{ id }");
        }
    }
}
