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
using System.Text.RegularExpressions;

namespace Planner.Droid.Activities
{
    [Activity(Label = "ResetPasswordActivity")]
    public class ResetPasswordActivity : AppCompatActivity
    {
        private RelativeLayout layout;
        private EditText passwordEditText;
        private EditText confirmPasswordEditText;
        private EditText codeEditText;
        private Button resetButton;
        private string _email;
        private ProgressBarHelper _progressBarHelper;
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

            _email = Intent.GetStringExtra("Email");

            SetContentView(Resource.Layout.activity_reset_password);

            FindViews();

            HandleEvents();

            _progressBarHelper = new ProgressBarHelper(this, Window, layout);
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

                _progressBarHelper.Show();

                var dto = new ResetPasswordRequestDto()
                {
                    Code = codeEditText.Text,
                    Password = passwordEditText.Text,
                    Email = _email
                };

                var res = await _authHelper.ResetPasswordAsync(dto);

                _progressBarHelper.Hide();

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    _dialogHelper.ShowSuccessDialog(this, "Resetting Password was successful. Please Sign In"
                                    , (o, ea) =>
                                    {
                                        StartActivity(typeof(SignInActivity));
                                        Finish();
                                    });
                }
                else if (res.StatusCode == HttpStatusCode.BadRequest)
                    _dialogHelper.ShowError(this, "The code entered is incorrect.");
                else
                    _dialogHelper.ShowError(this);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                _progressBarHelper.Hide();
                _dialogHelper.ShowError(this, ex);
            }
        }

        private void FindViews()
        {
            layout = FindViewById<RelativeLayout>(Resource.Id.resetPassword_Layout);
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

            if (!Regex.IsMatch(codeEditText.Text, "[0-9]{4,}"))
            {
                codeEditText.Error = "Code entered is incorrect";
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