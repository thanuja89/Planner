using Android.App;
using Android.Util;
using Planner.Droid.Helpers;
using Planner.Dto;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using Planner.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Droid.Services
{
    public class SyncService
    {
        public event EventHandler NewTasksAvailable;

        private readonly SyncHelper _syncHelper;
        private readonly ScheduledTaskDataHelper _dataHelper;
        private readonly AlarmHelper _alarmHelper;

        private static readonly object _initLock = new object();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public SyncService()
        {
            _syncHelper = new SyncHelper();
            _dataHelper = new ScheduledTaskDataHelper();
            _alarmHelper = new AlarmHelper();
        }

        private static SyncService _syncService;

        public static SyncService Instance
        {
            get
            {
                if (_syncService == null)
                {
                    lock (_initLock)
                    {
                        if (_syncService == null)
                        {
                            _syncService = new SyncService();
                            return _syncService;
                        }
                    }
                }

                return _syncService;
            }
        }


        public async Task SyncAsync()
        {
            bool lockTaken = false;

            try
            {
                lockTaken = await _semaphore.WaitAsync(0);

                if (lockTaken)
                {
                    string userId = Utilities.GetUserId();

                    var lastSyncedTicks = Utilities.GetLongFromPreferences(Application.Context, $"{userId}_LastSyncedOn");

                    var newTasksFromServer = await _syncHelper.PullAsync(lastSyncedTicks);

                    var newTasksInClient = await _dataHelper.GetAllFromDateTimeAsync(userId, lastSyncedTicks);

                    if (newTasksInClient != null && newTasksInClient.Count > 0)
                    {
                        await _syncHelper.PushAsync(newTasksInClient);
                    }

                    if (newTasksFromServer != null && newTasksFromServer.Any())
                    {
                        await UpdateTasksAsync(newTasksFromServer);

                        // Invoke event
                        NewTasksAvailable?.Invoke(this, new EventArgs());
                    }

                    Utilities.SaveLongToPreferences(Application.Context, $"{userId}_LastSyncedOn", DateTime.UtcNow.Ticks);                       
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
            }
            finally
            {
                if (lockTaken)
                {
                    _semaphore.Release();
                }
            }
        }

        #region Helper Methods
        private async Task UpdateTasksAsync(IEnumerable<GetScheduledTaskDTO> tasks)
        {
            foreach (var task in tasks)
            {
                var dbTask = await _dataHelper.GetByIdAsync(task.Id);

                if (task.IsDeleted)
                {
                    await _dataHelper.DeleteAsync(task.Id);
                    
                    if(dbTask != null)
                        _alarmHelper.CancelAlarm(dbTask);
                }
                else if (dbTask == null)
                {
                    var newTask = MapToEntity(task);

                    await _dataHelper.InsertAsync(newTask);

                    _alarmHelper.SetAlarm(newTask);
                }
                else
                {
                    var updatedTask = MapToEntity(task, dbTask);

                    await _dataHelper.UpdateAsync(updatedTask);

                    _alarmHelper.UpdateAlarm(updatedTask);
                }
            }
        }

        #endregion

        #region Mapping Methods

        private ScheduledTask MapToEntity(GetScheduledTaskDTO task)
        {
            return new ScheduledTask()
            {
                Id = task.Id,
                Title = task.Title,
                Note = task.Note,
                Start = task.Start,
                End = task.End,
                Importance = (Mobile.Core.Data.Importance)task.Importance,
                Repeat = (Mobile.Core.Data.Frequency)task.Repeat,
                ApplicationUserId = task.ApplicationUserId

            };
        }

        private ScheduledTask MapToEntity(GetScheduledTaskDTO task, ScheduledTask dbTask)
        {
            return new ScheduledTask()
            {
                Id = task.Id,
                Title = task.Title,
                Note = task.Note,
                Start = task.Start,
                End = task.End,
                Importance = (Mobile.Core.Data.Importance)task.Importance,
                Repeat = (Mobile.Core.Data.Frequency)task.Repeat,
                ClientSideId = dbTask.ClientSideId,
                ApplicationUserId = dbTask.ApplicationUserId,
                IsDeleted = dbTask.IsDeleted,
                ClientUpdatedOnTicks = dbTask.ClientUpdatedOnTicks
            };
        }

        #endregion
    }
}