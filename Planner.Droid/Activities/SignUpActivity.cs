﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Helpers;
using Planner.Dto;
using Planner.Mobile.Core.Helpers;
using System;
using Utilities = Planner.Mobile.Core.Utilities;

namespace Planner.Droid.Activities
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        private readonly AuthHelper _authHelper;
        private readonly DialogHelper _dialogHelper;
        private EditText usernameEditText;
        private EditText emailEditText;
        private EditText passwordEditText;
        private EditText confirmPasswordEditText;
        private Button signUpButton;
        private ProgressBar progressBar;

        public SignUpActivity()
        {
            _authHelper = new AuthHelper();
            _dialogHelper = new DialogHelper();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_sign_up);

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

                var result = await _authHelper.SignUpAsync(dto);

                if (result.Succeeded)
                {
                    _dialogHelper.ShowSuccessDialog(this, "Signing Up was successful. Please Sign In"
                        , (o, ea) => StartActivity(typeof(SignInActivity)));

                    return;
                }

                HandleError(result.ErrorType);               
            }
            catch (Exception ex)
            {
                progressBar.Visibility = ViewStates.Invisible;
                _dialogHelper.ShowError(this, ex);
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
                    _dialogHelper.ShowError(this);
                    break;

                case SignUpErrorType.Other:
                    Toast.MakeText(BaseContext, "Some of the information you entered is incorrect", ToastLength.Short);
                    break;
            }
        }

        #endregion
    }
}