using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Planner.Droid.Helpers;
using Planner.Mobile.Core.Helpers;
using Planner.Mobile.Core.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Droid.Services
{
    [Service]
    public class SyncService : Service
    {
        private readonly SyncHelper _syncHelper;
        private readonly ScheduledTaskDataHelper _dataHelper;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public SyncService()
        {
            _syncHelper = new SyncHelper();
            _dataHelper = new ScheduledTaskDataHelper();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _ = SyncAsync(); // Cannot use async await on this method, and we don't want to block the UI thread. Warning suppressed on purpose.

            return StartCommandResult.Sticky;
        }

        private async Task SyncAsync()
        {
            bool lockTaken = false;

            try
            {
                lockTaken = await _semaphore.WaitAsync(0);

                if (lockTaken)
                {
                    var lastSynced = Utilities.GetDateTimeFromPreferences(this, "LastSyncedOn");

                    var newTasksFromServer = await _syncHelper.PullAsync(lastSynced);

                    var newTasksInClient = await _dataHelper.GetAllFromDateTimeAsync(lastSynced);

                    await _syncHelper.PushAsync(newTasksInClient);

                    await _dataHelper.InsertOrUpdateAllAsync(newTasksFromServer);

                    Utilities.SaveDateTimeToPreferences(this, "LastSyncedOn", DateTime.UtcNow);
                }               
            }
            catch (Exception ex)
            {

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