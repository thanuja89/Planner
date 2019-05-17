using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Planner.Droid.Activities;

namespace Planner.Droid.Callbacks
{
    public class SignInResultCallback : Java.Lang.Object, IResultCallback
    {
        public SignInActivity Activity { get; set; }

        public void OnResult(Java.Lang.Object result)
        {
            var googleSignInResult = result as GoogleSignInResult;
        }
    }
}