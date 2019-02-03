using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Planner.Android.Extensions;
using Planner.Dto;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Services;
using System;

namespace Planner.Android
{
    [Activity(Label = "Sign In", MainLauncher = true)]
    public class SignInActivity : Activity
    {
        private readonly AuthService _authService;
        private EditText usernameEditText;
        private EditText passwordEditText;
        private Button signInButton;
        private CheckBox rememberMeCheckBox;

        public SignInActivity()
        {
            _authService = new AuthService();
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
            rememberMeCheckBox = FindViewById<CheckBox>(Resource.Id.rememberMeCheckBox);
        }

        private void HandleEvents()
        {
            signInButton.Click += SignInButton_Click;
        }

        private async void SignInButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            var dto = new TokenRequestDto()
            {
                Username = usernameEditText.Text,
                Password = passwordEditText.Text
            };

            var tokenDto = await _authService.SignInAsync(dto);

            if (tokenDto != null && tokenDto.Token != null)
                SaveToken(tokenDto.Token);
        }

        private bool ValidateInputs()
        {
            if (usernameEditText.IsEmpty())
            {
                usernameEditText.Error = "Username can not be empty.";
                return false;
            }

            if (passwordEditText.IsEmpty())
            {
                passwordEditText.Error = "Password can not be empty.";
                return false;
            }

            return true;
        }

        private void SaveToken(string token)
        {
            var pref = Application.Context
                .GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

            var editor = pref.Edit();

            editor.PutString(PreferenceItemKeys.TOKEN, token);
            editor.Apply();
        }
    }
}