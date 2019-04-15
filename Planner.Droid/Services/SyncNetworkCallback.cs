﻿
using Android.App;
using Android.Net;
using Planner.Mobile.Core.Helpers;
using System.Threading.Tasks;
using static Android.Net.ConnectivityManager;

namespace Planner.Droid.Services
{
    public class SyncNetworkCallback : NetworkCallback
    {
        public override void OnAvailable(Network network)
        {
            try
            {
                base.OnAvailable(network);

                if (!AuthenticatedHttpHelper.IsInitialized)
                    return;

                Task.Run(() => SyncService.Instance.SyncAsync(Application.Context));
            }
            catch (System.Exception ex)
            {

            }
        }
    }
}