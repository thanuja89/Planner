using Android.App;
using Android.App.Job;
using Android.Util;
using Planner.Droid.Helpers;
using Planner.Mobile.Core.Helpers;
using System;
using System.Threading.Tasks;

namespace Planner.Droid.Jobs
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE")]
    public class CleanDbJobService : JobService
    {
        private readonly ScheduledTaskDataHelper _dataHelper;

        public CleanDbJobService()
        {
            _dataHelper = new ScheduledTaskDataHelper();
        }

        public override bool OnStartJob(JobParameters @params)
        {
            var lastSyncedTicks = Utilities.GetLongFromPreferences(Application.Context, "LastSyncedOn");

            Task.Run(() => DeleteSyncedTasks(lastSyncedTicks));

            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            return true;
        }

        private async Task DeleteSyncedTasks(long lastSyncedTicks)
        {
            try
            {
                await _dataHelper.DeleteTasksAfterTicksAsync(lastSyncedTicks);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
            }
        }
    }
}