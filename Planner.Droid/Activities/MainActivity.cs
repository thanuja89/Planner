using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Helpers;
using System;

namespace Planner.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser; ;

            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            try
            {
                //var token = GetToken();

                //if (token != null)
                //{
                //    HttpHelper.Init(token);
                //    StartActivity(typeof(TasksActivity));
                //}
                //else
                    StartActivity(typeof(SignInActivity));
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
                StartActivity(typeof(SignInActivity));
            }
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private string GetToken()
        {
            var prefs = Application.Context.GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

            var token = prefs.GetString(PreferenceItemKeys.TOKEN, null);

            return token;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}