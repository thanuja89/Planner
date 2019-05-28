using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Messaging;
using Planner.Mobile.Core;
using System;
using System.Threading.Tasks;

namespace Planner.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PlannerFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            Task.Run(() => 
            {
                try
                {
                    string token = Helpers.Utilities.GetStringFromPreferences(PreferenceItemKeys.FIREBASE_REG_TOKEN);

                    message.Data.TryGetValue("fromDeviceId", out string fromDeviceId);

                    if (fromDeviceId != token)
                        _ = SyncService.Instance.SyncAsync(); // Warning suppressed on purpose
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
                }
            });
        }
    }
}