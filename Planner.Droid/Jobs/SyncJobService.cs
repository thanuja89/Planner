
using Android.App;
using Android.App.Job;
using Planner.Droid.Services;
using Planner.Mobile.Core.Helpers;

namespace Planner.Droid.Jobs
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE")]
    public class SyncJobService : JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            if (!HttpHelper.IsInitialized)
                return true;

            _ = SyncService.Instance.SyncAsync(this); // Warning suppressed on purpuse

            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            return true;
        }
    }
}