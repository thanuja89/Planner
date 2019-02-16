using Android.App;
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
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        private readonly AuthService _authService;
        private readonly DialogService _dialogService;
        private EditText usernameEditText;
        private EditText emailEditText;
        private EditText passwordEditText;
        private EditText confirmPasswordEditText;
        private Button signUpButton;
        private ProgressBar progressBar;

        public SignUpActivity()
        {
            _authService = new AuthService();
            _dialogService = new DialogService();
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
            progressBar = FindViewById<ProgressBar>(Resource.Id.signUp_circularProgressbar);
        }

        private void HandleEvents()
        {
            signUpButton.Click += SignUpButton_Click;
        }

        private async void SignUpButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                var dto = new CreateAccountDto()
                {
                    Username = usernameEditText.Text,
                    Email = emailEditText.Text,
                    Password = passwordEditText.Text
                };

                progressBar.Visibility = ViewStates.Visible;

                var result = await _authService.SignUpAsync(dto);

                if (result.Succeeded)
                {
                    _dialogService.ShowSuccessDialog(this, "Signing Up was successful. Please Sign In"
                        , (o, ea) => StartActivity(typeof(SignInActivity)));

                    return;
                }

                HandleError(result.ErrorType);               
            }
            catch (Exception ex)
            {
                progressBar.Visibility = ViewStates.Invisible;
                _dialogService.ShowError(this, ex);
            }
            finally
            {
                progressBar.Visibility = ViewStates.Invisible;
            }
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

        #region ErrorHandling

        private void HandleError(SignUpErrorType errorType)
        {
            switch (errorType)
            {
                case SignUpErrorType.UsernameExists:
                    usernameEditText.Error = "Username already exists";
                    break;

                case SignUpErrorType.EmailExists:
                    emailEditText.Error = "Email already exists";
                    break;

                case SignUpErrorType.ServerError:
                    _dialogService.ShowError(this);
                    break;

                case SignUpErrorType.Other:
                    Toast.MakeText(BaseContext, "Some of the information you entered is incorrect", ToastLength.Short);
                    break;
            }
        }

        #endregion
    }
}