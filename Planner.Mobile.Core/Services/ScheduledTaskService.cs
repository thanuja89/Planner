using Planner.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Services
{
    public class ScheduledTaskService
    {
        private readonly HttpService _httpService;

        public ScheduledTaskService()
        {
            _httpService = new HttpService();
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

        public Task<GetScheduledTaskDTO> UpdateScheduledTaskAsync(ScheduledTaskDTO taskDTO)
        {
            return _httpService.PutForResultAsync<GetScheduledTaskDTO>("ScheduledTask", taskDTO);
        }

        public Task DeleteScheduledTaskAsync(int id)
        {
            return _httpService.DeleteAsync("ScheduledTask");
        }
    }
}
