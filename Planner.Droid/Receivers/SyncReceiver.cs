using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Net;
using Android.Widget;
using Planner.Droid.Helpers;
using Planner.Droid.Jobs;
using Planner.Droid.Services;

namespace Planner.Droid.Receivers
{
    //[BroadcastReceiver(Enabled = true)]
    //[IntentFilter(new[] { Intent.ActionBootCompleted })]
    //public class SyncReceiver : BroadcastReceiver
    //{
    //    public override void OnReceive(Context context, Intent intent)
    //    {
    //        Toast.MakeText(context, "Boot", ToastLength.Long);

    //        var jobBuilder = Utilities.CreateJobBuilderUsingJobId<SyncJobService>(context, 1);

    //        var jobInfo = jobBuilder
    //            .SetRequiredNetworkType(NetworkType.Any)
    //            .Build();  // creates a JobInfo object.

    //        var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
    //        var scheduleResult = jobScheduler.Schedule(jobInfo);

    //        var cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);

    //         var builder = new NetworkRequest.Builder();

    //        if (cm != null)
    //        {
    //            cm.RegisterNetworkCallback(builder.Build(), new SyncNetworkCallback());
    //        }
    //    }
    //}
}