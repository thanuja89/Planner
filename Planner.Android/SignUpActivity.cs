using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Planner.Mobile.Core.Services;

namespace Planner.Android
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        private readonly AuthService _authService;

        public SignUpActivity()
        {
            _authService = new AuthService();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
    }
}