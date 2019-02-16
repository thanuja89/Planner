using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Extensions.Services;
using Planner.Dto;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Services;
using System;

namespace Planner.Droid
{
    [Activity(Label = "Sign In", MainLauncher = true)]
    public class SignInActivity : Activity
    {
        private readonly AuthService _authService;
        private readonly DialogService _dialogService;

        private EditText usernameEditText;
        private EditText passwordEditText;
        private Button signInButton;
        private CheckBox rememberMeCheckBox;
        private TextView signUpTextView;

        private ProgressBar progressBar;

        public SignInActivity()
        {
            _authService = new AuthService();
            _dialogService = new DialogService();
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
            usernameEditText = FindViewById<EditText>(Resource.Id.signIn_UsernameEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.signIn_PasswordEditText);
            signInButton = FindViewById<Button>(Resource.Id.signIn_SignInButton);
            rememberMeCheckBox = FindViewById<CheckBox>(Resource.Id.signIn_RememberMeCheckBox);
            signUpTextView = FindViewById<TextView>(Resource.Id.signIn_SignUpTextView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.signIn_circularProgressbar);
        }

        private void HandleEvents()
        {
            signInButton.Click += SignInButton_Click;
            signUpTextView.Click += SignUpTextView_Click;
        }

        private void SignUpTextView_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SignUpActivity));
        }

        private async void SignInButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                var dto = new TokenRequestDto()
                {
                    Username = usernameEditText.Text,
                    Password = passwordEditText.Text
                };

                progressBar.Visibility = ViewStates.Visible;

                var tokenDto = await _authService.SignInAsync(dto);

                if (tokenDto != null && tokenDto.Token != null)
                    SaveToken(tokenDto.Token);

                progressBar.Visibility = ViewStates.Invisible;
            }
            catch (Exception ex)
            {
                progressBar.Visibility = ViewStates.Invisible;
                _dialogService.ShowError(this, ex);
            }
        }

        private void SaveToken(string token)
        {
            var pref = Application.Context
                .GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

            var editor = pref.Edit();

            editor.PutString(PreferenceItemKeys.TOKEN, token);
            editor.Apply();
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
    }
}