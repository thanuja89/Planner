using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Helpers;
using Planner.Dto;
using Planner.Mobile.Core.Helpers;
using System;
using System.Net;

namespace Planner.Droid.Activities
{
    [Activity(Label = "ResetPasswordActivity")]
    public class ResetPasswordActivity : AppCompatActivity
    {
        private EditText passwordEditText;
        private EditText confirmPasswordEditText;
        private EditText codeEditText;
        private Button resetButton;
        private readonly AuthHelper _authHelper;
        private readonly DialogHelper _dialogHelper;

        public ResetPasswordActivity()
        {
            _authHelper = new AuthHelper();
            _dialogHelper = new DialogHelper();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_reset_password);

            FindViews();

            HandleEvents();
        }

        private void HandleEvents()
        {
            resetButton.Click += ResetButton_Click;
        }

        private async void ResetButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                var dto = new ResetPasswordRequestDto()
                {
                    Code = codeEditText.Text,
                    Password = passwordEditText.Text,
                    UserId = Utilities.GetUserId()
                };

                var res = await _authHelper.ResetPasswordAsync(dto);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    _dialogHelper.ShowSuccessDialog(this, "Resetting Password was successful. Please Sign In"
                                    , (o, ea) => StartActivity(typeof(SignInActivity)));
                }
                else if (res.StatusCode == HttpStatusCode.BadRequest)
                    _dialogHelper.ShowError(this, "The code entered is incorrect.");
                else
                    _dialogHelper.ShowError(this);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                _dialogHelper.ShowError(this, ex);
            }
        }

        private void FindViews()
        {
            passwordEditText = FindViewById<EditText>(Resource.Id.resetPassword_PasswordEditText);
            confirmPasswordEditText = FindViewById<EditText>(Resource.Id.resetPassword_ConfirmPasswordEditText);
            codeEditText = FindViewById<EditText>(Resource.Id.resetPassword_CodeEditText);
            resetButton = FindViewById<Button>(Resource.Id.resetPassword_ResetButton);
        }

        #region Validation

        private bool ValidateInputs()
        {
            if (codeEditText.IsEmpty())
            {
                codeEditText.Error = "Code can not be empty.";
                return false;
            }

            if (passwordEditText.IsEmpty())
            {
                passwordEditText.Error = "Password can not be empty.";
                return false;
            }

            int requiredPasswordLength = 4;

            if (passwordEditText.Text.Length < requiredPasswordLength)
            {
                passwordEditText.Error = $"Password must be at least { requiredPasswordLength } characters long.";
                return false;
            }

            if (confirmPasswordEditText.IsEmpty())
            {
                confirmPasswordEditText.Error = "Confirm Password can not be empty.";
                return false;
            }

            if (passwordEditText.Text != confirmPasswordEditText.Text)
            {
                confirmPasswordEditText.Error = "Password and Confirm Password must match.";
                return false;
            }

            return true;
        }

        #endregion
    }
}