using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Java.Lang;
using Planner.Droid.Activities;

namespace Planner.Droid.Services
{
    public class SignInResultCallback : Java.Lang.Object, IResultCallback
    {
        public SignUpActivity Activity { get; set; }

        public void OnResult(Java.Lang.Object result)
        {
            var googleSignInResult = result as GoogleSignInResult;
        }
    }
}