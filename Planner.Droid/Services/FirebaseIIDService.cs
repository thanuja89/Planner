using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Iid;
using Planner.Mobile.Core.Helpers;
using System;

namespace Planner.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        private readonly AuthHelper _authHelper;

        public FirebaseIIDService()
        {
            _authHelper = new AuthHelper();
        }

        public async override void OnTokenRefresh()
        {
            try
            {
                var refreshedToken = FirebaseInstanceId.Instance.Token;

                Log.WriteLine(LogPriority.Info, TAG, refreshedToken);

                await _authHelper.RegisterDeviceAsync(refreshedToken);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
            }
        }
    }
}