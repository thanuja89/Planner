using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace Planner.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    //[Activity(Label = "SplashActivity")]
    public class SplashActivity : Activity
    {
        private ImageView imageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_splash);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            StartAnimation();
        }

        private async void StartAnimation()
        {
            imageView = FindViewById<ImageView>(Resource.Id.splash_ImageView);

            var set = new AnimationSet(true)
            {
                Duration = 5_000,
                FillAfter = true,
                FillEnabled = true
            };

            set.AddAnimation(new RotateAnimation(0f, 360f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.50f));

            set.AddAnimation(new ScaleAnimation(1f, 3f, 1f, 3f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f));

            imageView.StartAnimation(set);

            await Task.Delay(TimeSpan.FromSeconds(5));

            StartActivity(typeof(MainActivity));         
        }
    }
}