using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views.Animations;
using Android.Widget;
using Planner.Droid.Services;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Helpers;
using System;
using System.Threading.Tasks;

namespace Planner.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        private ImageView imageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_splash);
        }

        public override async void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                try
                {
                    var initTask = InitAsync();

                    StartAnimation();

                    await Task.WhenAll(initTask, Task.Delay(TimeSpan.FromSeconds(5)));

                    if (initTask.Result) // Task is already complete, so we won't block the thread
                        StartActivity(typeof(TasksActivity));
                    else
                        StartActivity(typeof(SignInActivity));
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
                    StartActivity(typeof(SignInActivity));
                }
            }
        }

        private void StartAnimation()
        {
            imageView = FindViewById<ImageView>(Resource.Id.splash_ImageView);

            var set = new AnimationSet(true)
            {
                Duration = 5_000,
                FillAfter = true,
                FillEnabled = true
            };

            set.AddAnimation(
                new RotateAnimation(0f, 360f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.50f));

            set.AddAnimation(
                new ScaleAnimation(1f, 3f, 1f, 3f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f));

            imageView.StartAnimation(set);
        }

        private Task<bool> InitAsync()
        {
            return Task.Run(async () =>
            {
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;

                var prefs = Application.Context
                    .GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

                var token = prefs.GetString(PreferenceItemKeys.TOKEN, null);
                string deviceRegToken = Helpers.Utilities
                    .GetStringFromPreferences(PreferenceItemKeys.FIREBASE_REG_TOKEN);

                if (token != null)
                {
                    HttpHelper.Init(token, deviceRegToken);

                    await SyncService.Instance.SyncAsync();

                    return true;
                }
                else
                    return false;
            });
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}