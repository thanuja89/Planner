﻿using Android.App;
using Android.Util;
using Planner.Droid.Helpers;
using Planner.Mobile.Core.Helpers;
using Planner.Mobile.Core.Services;
using System;
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

        private static readonly object _initLock = new object();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public SyncService()
        {
            _syncHelper = new SyncHelper();
            _dataHelper = new ScheduledTaskDataHelper();
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

                    bool isNewTasksAvailableFromServer = false;

                    if (newTasksFromServer != null && newTasksFromServer.Any())
                    {
                        await _dataHelper.InsertOrUpdateAllAsync(newTasksFromServer);
                        isNewTasksAvailableFromServer = true;
                    }

                    Utilities.SaveLongToPreferences(Application.Context, $"{userId}_LastSyncedOn", DateTime.UtcNow.Ticks);

                    if(isNewTasksAvailableFromServer)
                        NewTasksAvailable?.Invoke(this, new EventArgs());
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
    }
}