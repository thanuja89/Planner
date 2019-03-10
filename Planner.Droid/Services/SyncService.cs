using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Planner.Droid.Helpers;
using Planner.Mobile.Core.Helpers;
using Planner.Mobile.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Droid.Services
{
    [Service]
    public class SyncService : Service
    {
        private readonly SyncHelper _syncHelper;
        private readonly ScheduledTaskDataHelper _dataHelper;

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
            Task.Run(SyncAsync);

            return StartCommandResult.Sticky;
        }

        private async Task SyncAsync()
        {
            try
            {
                await RetriveAndSaveNewTasksAsync();

                var lastSynced = Utilities.GetDateTimeFromPreferences(this ,"LastPushedOn");

                var newTasksToPush = await _dataHelper.GetAllFromDateTimeAsync(lastSynced);

                await _syncHelper.PushAsync(newTasksToPush);

                Utilities.SaveDateTimeToPreferences(this, "LastPushedOn", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task RetriveAndSaveNewTasksAsync()
        {
            var lastSynced = Utilities.GetDateTimeFromPreferences(this, "LastPulledOn");

            var tasks = await _syncHelper.PullAsync(lastSynced);

            if (tasks == null || tasks.Count() < 1)
                return;

            await _dataHelper.InsertAllAsync(tasks);

            Utilities.SaveDateTimeToPreferences(this, "LastPulledOn", DateTime.UtcNow);
        }      
    }
}