﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
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

            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var token = GetToken();

            if (token != null)
            {
                HttpHelper.Init(token);
                StartActivity(typeof(TasksActivity));
            }              
            else
                StartActivity(typeof(SignInActivity));
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
    }
}