﻿using Android.App;
using Android.App.Job;
using Android.Net;
using Android.Runtime;
using Planner.Droid.Callbacks;
using Planner.Droid.Helpers;
using Planner.Droid.Jobs;
using Planner.Droid.Services;
using System;

namespace Planner.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class PlannerApp : Application
    {
#if !DEBUG
        private SyncNetworkCallback _networkCallback;
#endif
        public PlannerApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

#if !DEBUG

            var jobScheduler = (JobScheduler) GetSystemService(JobSchedulerService);

            // Sync

            var jobBuilder = Utilities.CreateJobBuilderUsingJobId<SyncJobService>(Context, 1);

            var millisecs = (long) TimeSpan.FromMinutes(45).TotalMilliseconds;

            var jobInfo = jobBuilder
                .SetRequiredNetworkType(NetworkType.Any)
                .SetPeriodic(millisecs)
                .Build();  // creates a JobInfo object.
           
            jobScheduler.Schedule(jobInfo);


            // Db Clean

            var cleanJobBuilder = Utilities.CreateJobBuilderUsingJobId<CleanDbJobService>(Context, 1);

            var cleanJobInfo = cleanJobBuilder
                .SetPeriodic((long)TimeSpan.FromHours(24).TotalMilliseconds)
                .Build();  // creates a JobInfo object.

            jobScheduler.Schedule(cleanJobInfo);

            // Register Network Callback

            var cm = (ConnectivityManager) GetSystemService(ConnectivityService);

            var builder = new NetworkRequest.Builder();

            _networkCallback = new SyncNetworkCallback();

            if (cm != null)
            {
                cm.RegisterNetworkCallback(builder.Build(), _networkCallback);
            }
#endif
        }

        public override void OnTerminate()
        {
#if !DEBUG
            //var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            //cm.UnregisterNetworkCallback(_networkCallback);
#endif
            base.OnTerminate();
        }
    }
}