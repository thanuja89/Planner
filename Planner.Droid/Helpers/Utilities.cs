using Android.Content;
using Android.Preferences;
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
    }
}