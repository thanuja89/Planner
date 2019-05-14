using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Fragments;
using Planner.Droid.Helpers;
using Planner.Dto;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Helpers;
using System;
using System.Net;
using System.Threading.Tasks;
using CoreHelper = Planner.Mobile.Core;

namespace Planner.Droid.Activities
{
    [Activity(Label = "Sign In")]
    public class SignInActivity : Activity
    {
        private readonly AuthHelper _authHelper;
        private readonly DialogHelper _dialogHelper;

        private EditText usernameEditText;
        private EditText passwordEditText;
        private Button signInButton;
        private TextView signUpTextView;
        private TextView forgotPasswordTextView;
        private ProgressBar progressBar;

        public SignInActivity()
        {
            _authHelper = new AuthHelper();
            _dialogHelper = new DialogHelper();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_sign_in);

            FindViews();
            HandleEvents();
        }

        private void FindViews()
        {
            usernameEditText = FindViewById<EditText>(Resource.Id.signIn_UsernameEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.signIn_PasswordEditText);
            signInButton = FindViewById<Button>(Resource.Id.signIn_SignInButton);
            signUpTextView = FindViewById<TextView>(Resource.Id.signIn_SignUpTextView);
            forgotPasswordTextView = FindViewById<TextView>(Resource.Id.signIn_ForgotPasswordTextView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.signIn_circularProgressbar);
        }

        private void HandleEvents()
        {
            signInButton.Click += SignInButton_Click;
            signUpTextView.Click += SignUpTextView_Click;
            forgotPasswordTextView.Click += ForgotPasswordTextView_Click;
        }

        private void ForgotPasswordTextView_Click(object sender, EventArgs e)
        {
            PasswordResetEmailInputDialogFragment frag = PasswordResetEmailInputDialogFragment.NewInstance(s =>
            {
                _ = SendPasswordResetEmailAsync(s);
            });

            frag.Show(FragmentManager, ConfirmationCodeInputDialogFragment.TAG);
        }

        private async Task SendPasswordResetEmailAsync(string s)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s) || !CoreHelper.Utilities.IsValidEmail(s))
                    return;

                var res = await _authHelper.SendPasswordResetEmailAsync(new SendPasswordResetEmailDto() { Email = s });

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    _dialogHelper.ShowSuccessDialog(this, "Email Sent."
                          , (o, ea) => 
                          {
                              var intent = new Intent(this, typeof(ResetPasswordActivity));
                              intent.PutExtra("Email", s);

                              StartActivity(intent);
                          });
                }
                else if (res.StatusCode == HttpStatusCode.BadRequest)
                    _dialogHelper.ShowError(this, "The email entered is incorrect.");
                else
                    _dialogHelper.ShowError(this);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                _dialogHelper.ShowError(this, ex);
            }
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

                var tokenDto = await _authHelper.SignInAsync(dto);

                progressBar.Visibility = ViewStates.Invisible;

                if (tokenDto != null && tokenDto.Token != null)
                {
                    SaveUserInfo(tokenDto);
                    HttpHelper.Init(tokenDto.Token);

                    StartActivity(typeof(TasksActivity));
                }
                else
                {
                    _dialogHelper.ShowError(this, "Username or password is incorrect.");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                progressBar.Visibility = ViewStates.Invisible;
                _dialogHelper.ShowError(this, ex);
            }
        }

        private void SaveUserInfo(TokenDto dto)
        {
            var pref = Application.Context
                .GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

            var editor = pref.Edit();

            editor.PutString(PreferenceItemKeys.TOKEN, dto.Token);
            editor.PutString(PreferenceItemKeys.USER_ID, dto.ApplicationUserId);
            editor.PutString(PreferenceItemKeys.USERNAME, dto.Username);

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