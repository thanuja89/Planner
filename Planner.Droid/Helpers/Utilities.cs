using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Preferences;
using Planner.Droid.Util;
using Planner.Dto;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsAzure.Messaging;

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

        public static void SaveToPreferences(string key, string value)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            var editor = pref.Edit();

            editor.PutString(key, value);

            editor.Apply();
        }

        public static string GetStringFromPreferences(string key)
        {
            var pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);

            var value = pref.GetString(key, string.Empty);

            return value;
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

        public static async Task InitUserAndDeviceInfoAsync(TokenDto dto)
        {
            var pref = Application.Context
                .GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

            var editor = pref.Edit();

            editor.PutString(PreferenceItemKeys.TOKEN, dto.Value);
            editor.PutString(PreferenceItemKeys.USER_ID, dto.ApplicationUserId);
            editor.PutString(PreferenceItemKeys.USERNAME, dto.Username);

            editor.Apply();

            await InitClientAndDeviceInfoAsync(dto.ApplicationUserId, dto.Value);
        }

        public static Task InitClientAndDeviceInfoAsync(string userId, string jwt)
        {
            var hub = new NotificationHub(Keys.AZURE_HUB_NAME,
                                        Keys.AZURE_HUB_CONN_STRING, Application.Context);

            string deviceRegToken = GetStringFromPreferences(PreferenceItemKeys.FIREBASE_REG_TOKEN);

            HttpHelper.Init(jwt, deviceRegToken);

            var tags = new List<string>() { userId };

            return Task.Run(() => hub.Register(deviceRegToken, tags.ToArray()));
        }
    }
}