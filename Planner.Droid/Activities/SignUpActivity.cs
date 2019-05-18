using Android.App;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Fragments;
using Planner.Droid.Helpers;
using Planner.Dto;
using Planner.Mobile.Core.Helpers;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities = Planner.Mobile.Core.Utilities;

namespace Planner.Droid.Activities
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : AppCompatActivity
    {
        private readonly AuthHelper _authHelper;
        private readonly DialogHelper _dialogHelper;
        private RelativeLayout layout;
        private EditText usernameEditText;
        private EditText emailEditText;
        private EditText passwordEditText;
        private EditText confirmPasswordEditText;
        private Button signUpButton;
        private ProgressBarHelper _progressBarHelper;

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

            _progressBarHelper = new ProgressBarHelper(this, Window, layout);
        }

        private void FindViews()
        {
            layout = FindViewById<RelativeLayout>(Resource.Id.signUp_Layout);
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
            try
            {
                if (!ValidateInputs())
                    return;

                var dto = new CreateAccountDto()
                {
                    Username = usernameEditText.Text.Trim(),
                    Email = emailEditText.Text.Trim(),
                    Password = passwordEditText.Text
                };

                _progressBarHelper.Show();

                var result = await _authHelper.SignUpAsync(dto);

                _progressBarHelper.Hide();

                if (result.Succeeded)
                {
                    ShowConfirmationCodeDialog(result.UserId);

                    return;
                }

                HandleError(result.ErrorType);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                _progressBarHelper.Hide();
                _dialogHelper.ShowError(this, ex);
            }
        }

        private void ShowConfirmationCodeDialog(string userId)
        {
            ConfirmationCodeInputDialogFragment frag = ConfirmationCodeInputDialogFragment.NewInstance(s =>
            {
                _ = ConfirmEmailAsync(userId, s);
            },
            () =>
            {
                _ = ResendConfirmationEmailAsync(userId);
            });

            frag.Show(FragmentManager, ConfirmationCodeInputDialogFragment.TAG);
        }

        private async Task ResendConfirmationEmailAsync(string userId)
        {
            try
            {
                _progressBarHelper.Show();

                await _authHelper.ResendConfirmationEmailAsync(userId);

                _progressBarHelper.Hide();
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                _progressBarHelper.Hide();

                _dialogHelper.ShowError(this, ex);
            }
        }

        private async Task ConfirmEmailAsync(string userId, string s)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    _dialogHelper.ShowError(this, "The code cannot be empty.");
                    return;
                }

                if(!Regex.IsMatch(s, "[0-9]{4,}"))
                {
                    _dialogHelper.ShowError(this, "The code entered is incorrect.");
                    return;
                }

                _progressBarHelper.Show();

                var res = await _authHelper.ConfirmEmailAsync(new ConfirmationRequestDto
                {
                    Code = s,
                    UserId = userId
                });

                _progressBarHelper.Hide();

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    _dialogHelper.ShowSuccessDialog(this, "Signing Up was successful. Please Sign In"
                                    , (o, ea) => StartActivity(typeof(SignInActivity))); 
                }
                else if(res.StatusCode == HttpStatusCode.BadRequest)
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