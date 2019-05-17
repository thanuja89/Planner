using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Planner.Droid.Activities;
using System;

namespace Planner.Droid.Callbacks
{
    public class GoogleConnectionFailedCallback : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener
    {
        public SignInActivity Activity { get; set; }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }
    }
}