using Android.App;
using Android.Content;
using Android.Net;
using Planner.Droid.Services;

namespace Planner.Droid.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { ConnectivityManager.ConnectivityAction })]
    public class SyncReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {   
            if (IsOnline(context))
            {
                _ = SyncService.Instance.SyncAsync(context);
            }
        }

        public bool IsOnline(Context context)
        {
            var cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }
    }
}