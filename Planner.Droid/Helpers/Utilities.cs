using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Preferences;
using Planner.Droid.Receivers;
using Planner.Droid.Util;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Data;
using System;

namespace Planner.Droid.Helpers
{
    public static class Utilities
    {
        public static long GetLongFromPreferences(Context context, string key)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(context);

            var ticks = pref.GetLong(key, 0);

            return ticks;
        }

        public static void SaveLongToPreferences(Context context, string key, long ticks)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(context);
            var editor = pref.Edit();

            editor.PutLong(key, ticks);

            editor.Apply();
        }

        //public static void SetAlarm(Context context, AlarmManager alarmManager, ScheduledTask task)
        //{
        //    Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
        //    calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();

        //    calendar.Set(task.Start.Year, task.Start.Month - 1, task.Start.Day, task.Start.Hour, task.Start.Minute, 0);

        //    var alarmIntent = new Intent(context, typeof(AlarmReceiver));

        //    alarmIntent.PutExtra(AlarmReceiver.Constants.TITLE_PARAM_NAME, task.Title);
        //    alarmIntent.PutExtra(AlarmReceiver.Constants.MESSAGE_PARAM_NAME, task.Note);

        //    var pending = PendingIntent.GetBroadcast(context, task.ClientSideId, alarmIntent, PendingIntentFlags.UpdateCurrent);

        //    alarmManager.Set(AlarmType.Rtc, calendar.TimeInMillis, pending);
        //}

        public static JobInfo.Builder CreateJobBuilderUsingJobId<T>(this Context context, int jobId) where T : JobService
        {
            var javaClass = Java.Lang.Class.FromType(typeof(T));
            var componentName = new ComponentName(context, javaClass);
            return new JobInfo.Builder(jobId, componentName);
        }

        public static string GetUserId()
        {
            var prefs = Application.Context.GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

            var userId = prefs.GetString(PreferenceItemKeys.USER_ID, null);

            return userId;
        }

        public static DateTime ToDateTime(Date date, Time time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);
        }
    }
}