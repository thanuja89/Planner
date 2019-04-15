using Android.Content;
using Android.Util;
using Android.Widget;
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
        private readonly SyncHelper _syncHelper;
        private readonly ScheduledTaskDataHelper _dataHelper;

        private static object _initLock = new object();
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


        public async Task SyncAsync(Context context)
        {
            Toast.MakeText(context, "Connecting", ToastLength.Long);

            bool lockTaken = false;

            try
            {
                lockTaken = await _semaphore.WaitAsync(0);

                if (lockTaken)
                {
                    var lastSynced = Utilities.GetDateTimeFromPreferences(context, "LastSyncedOn");

                    var newTasksFromServer = await _syncHelper.PullAsync(lastSynced);

                    var newTasksInClient = await _dataHelper.GetAllFromDateTimeAsync(lastSynced);

                    if (newTasksInClient != null && newTasksInClient.Count > 0)
                    {
                        await _syncHelper.PushAsync(newTasksInClient); 
                    }

                    if (newTasksFromServer != null && newTasksFromServer.Any())
                    {
                        await _dataHelper.InsertOrUpdateAllAsync(newTasksFromServer); 
                    }

                    Utilities.SaveDateTimeToPreferences(context, "LastSyncedOn", DateTime.UtcNow);
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