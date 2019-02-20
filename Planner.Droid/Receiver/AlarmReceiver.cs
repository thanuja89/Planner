using Android.App;
using Android.Content;
using Android.Support.V4.App;

namespace Planner.Droid.Receiver
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public const int NOTIFICATION_ID = 1000;        
        public const string URGENT_CHANNEL = "URGENT";

        public override void OnReceive(Context context, Intent intent)
        {
            string chanName = "Channel1";
            var importance = NotificationImportance.High;
            NotificationChannel chan = new NotificationChannel(URGENT_CHANNEL, chanName, importance);

            chan.EnableVibration(true);
            chan.LockscreenVisibility = NotificationVisibility.Public;

            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(chan);

            var message = intent.GetStringExtra("message");
            var title = intent.GetStringExtra("title");

            var resultIntent = new Intent(context, typeof(TasksActivity));
            resultIntent.SetFlags(ActivityFlags.ClearTop);

            var pending = PendingIntent.GetActivity(context, 0,
                resultIntent,
                PendingIntentFlags.CancelCurrent);

            var notification = new NotificationCompat.Builder(context, URGENT_CHANNEL)
                .SetContentIntent(pending)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetColor(Resource.Color.colorPrimary)
                .SetSmallIcon(Resource.Drawable.notify_panel_notification_icon_bg)
                .Build();


            notificationManager.Notify(NOTIFICATION_ID, notification);
        }
    }

}