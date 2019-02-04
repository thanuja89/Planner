using Android.App;
using Android.OS;
using Android.Widget;
using Planner.Android.Extensions;
using Planner.Dto;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Services;
using System;

namespace Planner.Android
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        private readonly AuthService _authService;
        private EditText usernameEditText;
        private EditText emailEditText;
        private EditText passwordEditText;
        private EditText confirmPasswordEditText;
        private Button signUpButton;

        public SignUpActivity()
        {
            _authService = new AuthService();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SignUpView);

            FindViews();
            HandleEvents();
        }

        private void FindViews()
        {
            usernameEditText = FindViewById<EditText>(Resource.Id.signUp_UsernameEditText);
            emailEditText = FindViewById<EditText>(Resource.Id.signUp_EmailEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.signUp_PasswordEditText);
            confirmPasswordEditText = FindViewById<EditText>(Resource.Id.signUp_ConfirmPasswordEditText);
            signUpButton = FindViewById<Button>(Resource.Id.signUp_SignUpButton);           
        }

        private void HandleEvents()
        {
            signUpButton.Click += SignUpButton_Click;
        }

        private async void SignUpButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            var dto = new CreateAccountDto()
            {
                Username = usernameEditText.Text,
                Email = emailEditText.Text,
                Password = passwordEditText.Text
            };

            await _authService.SignUpAsync(dto);
        }

        #region Validation

        private bool ValidateInputs()
        {
            if (usernameEditText.IsEmpty())
            {
                usernameEditText.Error = "Username can not be empty.";
                return false;
            }

            int requiredUsernameLength = 4;

            if (usernameEditText.Text.Length <= requiredUsernameLength)
            {
                usernameEditText.Error = $"Username must be at least { requiredUsernameLength } characters long.";
                return false;
            }

            if (emailEditText.IsEmpty())
            {
                emailEditText.Error = "Email can not be empty.";
                return false;
            }

            if (!Utilities.IsValidEmail(emailEditText.Text))
            {
                emailEditText.Error = "Email is not valid.";
                return false;
            }

            if (passwordEditText.IsEmpty())
            {
                passwordEditText.Error = "Password can not be empty.";
                return false;
            }

            int requiredPasswordLength = 4;

            if (passwordEditText.Text.Length <= requiredPasswordLength)
            {
                passwordEditText.Error = $"Password must be at least { requiredPasswordLength } characters long.";
                return false;
            }

            if (confirmPasswordEditText.IsEmpty())
            {
                confirmPasswordEditText.Error = "Confirm Password can not be empty.";
                return false;
            }

            if(passwordEditText.Text != confirmPasswordEditText.Text)
            {
                confirmPasswordEditText.Error = "Password and Confirm Password must match.";
                return false;
            }

            return true;
        }

        #endregion
    }
}