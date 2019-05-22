using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Preferences;
using Planner.Droid.Util;
using Planner.Mobile.Core;
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

        public static string GetUsername()
        {
            var prefs = Application.Context.GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

            var userId = prefs.GetString(PreferenceItemKeys.USERNAME, null);

            return userId;
        }

        public static DateTime ToDateTime(Date date, Time time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);
        }
    }
}