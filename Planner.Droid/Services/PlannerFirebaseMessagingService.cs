using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Messaging;
using System.Threading.Tasks;

namespace Planner.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PlannerFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            message.Data.TryGetValue("key", out string val);
            Log.Debug(TAG, val);

            Task.Run(() => SyncService.Instance.SyncAsync());
        }
    }
}