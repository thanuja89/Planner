﻿using Planner.Dto;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<ScheduledTask>> PullAsync(DateTime lastSynced)
        {
            var tasks = await _httpHelper.GetAsync<List<GetScheduledTaskDTO>>($"Syncronization?lastSynced={lastSynced:yyyyMMddHHmmss}");
            
            if(tasks != null)
            {
                var entities = tasks
                    .Select(t => new ScheduledTask()
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Note = t.Note,
                        Start = t.Start,
                        End = t.End,
                        Importance = (Data.Importance) t.Importance,
                        Repeat = (Data.Frequency) t.Repeat,
                        IsAlarm = t.IsAlarm
                    })
                    .ToList();

                return entities;
            }

            return default;
        }

        public Task PushAsync(IEnumerable<ScheduledTask> tasks)
        {
            return _httpHelper.PutAsync("Syncronization", tasks);
        }
    }
}
