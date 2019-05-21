using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Util;
using Planner.Droid.Activities;
using Planner.Droid.Helpers;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System;
using System.Threading.Tasks;

namespace Planner.Droid.Receivers
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        private ScheduledTaskDataHelper _taskDataHelper;
        private AlarmHelper _alarmHelper;

        public AlarmReceiver()
        {
            _taskDataHelper = new ScheduledTaskDataHelper();
            _alarmHelper = new AlarmHelper();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Task.Run(async () =>
            {
                try
                {
                    var taskId = intent.GetIntExtra(Constants.CLIENT_SIDE_ID_PARAM_NAME, 0);

                    var task = await _taskDataHelper.GetByClientSideIdAsync(taskId);

                    if (task == null)
                        return;

                    var channel = BuildChannel();

                    var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                    notificationManager.CreateNotificationChannel(channel);

                    var notification = BuildNotification(context, task);

                    notificationManager.Notify(Constants.NOTIFICATION_ID, notification);

                    if(task.Repeat != Frequency.Never)
                    {
                        _alarmHelper.SetNextRepeatingAlarm(task);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
                }
            });
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

        private Notification BuildNotification(Context context, ScheduledTask task)
        {
            var intent = new Intent(context, typeof(TasksActivity));
            intent.SetFlags(ActivityFlags.ClearTop);

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);

            return new NotificationCompat.Builder(context, Constants.CHANNEL_ID)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(task.Title)
                .SetContentText(task.Note)
                .SetColor(Resource.Color.colorPrimary)
                .SetSmallIcon(Resource.Drawable.notify_panel_notification_icon_bg)
                .Build();
        }

        public static class Constants
        {
            public const int NOTIFICATION_ID = 1000;
            public const string CHANNEL_ID = "Urgent";
            public const string CHANNEL_NAME = "Urgent";
            public const string CLIENT_SIDE_ID_PARAM_NAME = "ClientSideId";
        }
    }

}