using Android.App;
using Android.Content;
using Android.Preferences;
using Planner.Droid.Receivers;
using Planner.Mobile.Core.Data;
using System;

namespace Planner.Droid.Helpers
{
    public static class Utilities
    {
        public static DateTime GetDateTimeFromPreferences(Context context, string key)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(context);

            var lastSyncedStr = pref.GetString(key, string.Empty);

            if (string.IsNullOrEmpty(lastSyncedStr))
                return default;

            DateTime.TryParse(lastSyncedStr, out DateTime date);
            return date;
        }

        public static void SaveDateTimeToPreferences(Context context, string key, DateTime dateTime)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(context);
            var editor = pref.Edit();

            editor.PutString(key, dateTime.ToString());

            editor.Apply();
        }

        public static void SetAlarm(Context context, AlarmManager alarmManager, ScheduledTask task)
        {
            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();

            calendar.Set(task.Start.Year, task.Start.Month - 1, task.Start.Day, task.Start.Hour, task.Start.Minute, 0);

            var alarmIntent = new Intent(context, typeof(AlarmReceiver));

            alarmIntent.PutExtra(AlarmReceiver.Constants.TITLE_PARAM_NAME, task.Title);
            alarmIntent.PutExtra(AlarmReceiver.Constants.MESSAGE_PARAM_NAME, task.Note);

            var pending = PendingIntent.GetBroadcast(context, task.ClientSideId, alarmIntent, PendingIntentFlags.UpdateCurrent);

            alarmManager.Set(AlarmType.Rtc, calendar.TimeInMillis, pending);
        }

        public static void CancelAlarm(Context context, AlarmManager alarmManager, ScheduledTask task)
        {           
            var alarmIntent = new Intent(context, typeof(AlarmReceiver));

            var pending = PendingIntent.GetBroadcast(context, task.ClientSideId, alarmIntent, PendingIntentFlags.UpdateCurrent);

            alarmManager.Cancel(pending);
        }
    }
}