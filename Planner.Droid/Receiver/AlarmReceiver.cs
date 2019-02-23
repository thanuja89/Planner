using Android.App;
using Android.Content;
using Android.Support.V4.App;

namespace Planner.Droid.Receiver
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var channel = BuildChannel();

            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            var notification = BuildNotification(context, intent);

            notificationManager.Notify(Constants.NOTIFICATION_ID, notification);
        }

        private NotificationChannel BuildChannel()
        {
            var channel = new NotificationChannel(Constants.CHANNEL_ID
                , Constants.CHANNEL_NAME, NotificationImportance.High)
            {
                LockscreenVisibility = NotificationVisibility.Public,
            };

            channel.EnableVibration(true);

            return channel;
        }

        private Notification BuildNotification(Context context, Intent sourceIntent)
        {
            var message = sourceIntent.GetStringExtra(Constants.MESSAGE_PARAM_NAME);
            var title = sourceIntent.GetStringExtra(Constants.TITLE_PARAM_NAME);

            var intent = new Intent(context, typeof(TasksActivity));
            intent.SetFlags(ActivityFlags.ClearTop);

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);

            return new NotificationCompat.Builder(context, Constants.CHANNEL_ID)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetColor(Resource.Color.colorPrimary)
                .SetSmallIcon(Resource.Drawable.notify_panel_notification_icon_bg)
                .Build();
        }

        public static class Constants
        {
            public const int NOTIFICATION_ID = 1000;
            public const string CHANNEL_ID = "Urgent";
            public const string CHANNEL_NAME = "Urgent";
            public const string TITLE_PARAM_NAME = "Title";
            public const string MESSAGE_PARAM_NAME = "Message";
            public const string IS_ALARM_PARAM_NAME = "IsAlarm";
        } 
    }

}