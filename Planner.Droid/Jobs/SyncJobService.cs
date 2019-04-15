
using Android.App;
using Android.App.Job;
using Planner.Droid.Services;
using Planner.Mobile.Core.Helpers;
using System.Threading.Tasks;

namespace Planner.Droid.Jobs
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE")]
    public class SyncJobService : JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            if (!HttpHelper.IsInitialized)
                return true;

            Task.Run(() => SyncService.Instance.SyncAsync(Application.Context));

            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            return true;
        }
    }
}