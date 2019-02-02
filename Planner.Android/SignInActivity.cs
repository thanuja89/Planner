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
using Planner.Dto;
using Planner.Mobile.Core.Services;

namespace Planner.Android
{
    [Activity(Label = "Sign In", MainLauncher = true)]
    public class SignInActivity : Activity
    {
        private readonly AuthService authService;
        private EditText usernameEditText;
        private EditText passwordEditText;
        private Button signInButton;

        public SignInActivity()
        {
            authService = new AuthService();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SignInView);

            FindViews();
            HandleEvents();
        }

        private void FindViews()
        {
            usernameEditText = FindViewById<EditText>(Resource.Id.usernameEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            signInButton = FindViewById<Button>(Resource.Id.signInButton);
        }

        private void HandleEvents()
        {
            signInButton.Click += SignInButton_Click;
        }

        private async void SignInButton_Click(object sender, EventArgs e)
        {
            var dto = new LoginDto()
            {
                Username = usernameEditText.Text,
                Password = passwordEditText.Text
            };

            var tokenDto = await authService.SignIn(dto);

            Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        }
    }
}