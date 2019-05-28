using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Iid;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Helpers;
using System;
using System.Collections.Generic;
using WindowsAzure.Messaging;

namespace Planner.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";

        public override void OnTokenRefresh()
        {
            try
            {
                var refreshedToken = FirebaseInstanceId.Instance.Token;

                Log.WriteLine(LogPriority.Info, TAG, refreshedToken);

                SaveTokenToPreferences(refreshedToken);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
            }
        }

        private void SaveTokenToPreferences(string token)
        {
            Log.Debug(TAG, $"Successful registration of ID {token}");
            Helpers.Utilities.SaveToPreferences(PreferenceItemKeys.FIREBASE_REG_TOKEN, token);          
        }
    }
}