using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Planner.Droid.Callbacks;
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
    public class SignInActivity : AppCompatActivity
    {
        const int RC_GOOGLE_SIGN_IN = 9001;

        private readonly AuthHelper _authHelper;
        private readonly DialogHelper _dialogHelper;

        private EditText usernameEditText;
        private EditText passwordEditText;
        private Button signInButton;
        private Button signUpButton;
        private Button forgotPasswordButton;
        private ProgressBar progressBar;
        private GoogleApiClient _googleApiClient;
        private SignInButton googleSignInButton;

        public SignInActivity()
        {
            _authHelper = new AuthHelper();
            _dialogHelper = new DialogHelper();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_sign_in);

            PrepareGoogleSignIn();

            FindViews();
            HandleEvents();
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == RC_GOOGLE_SIGN_IN)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                await HandleSignInResult(result);
            }
        }

        private void FindViews()
        {
            usernameEditText = FindViewById<EditText>(Resource.Id.signIn_UsernameEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.signIn_PasswordEditText);
            signInButton = FindViewById<Button>(Resource.Id.signIn_SignInButton);
            signUpButton = FindViewById<Button>(Resource.Id.signIn_SignUpTextView);
            forgotPasswordButton = FindViewById<Button>(Resource.Id.signIn_ForgotPasswordTextView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.signIn_circularProgressbar);
        }

        private void HandleEvents()
        {
            signInButton.Click += SignInButton_Click;
            signUpButton.Click += SignUpTextView_Click;
            forgotPasswordButton.Click += ForgotPasswordTextView_Click;
            googleSignInButton.Click += GoogleSignInButton_Click;
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

        private void GoogleSignInButton_Click(object sender, EventArgs e)
        {
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(_googleApiClient);
            StartActivityForResult(signInIntent, RC_GOOGLE_SIGN_IN);
        }

        private async void SignInButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                var dto = new TokenRequestDto()
                {
                    Username = usernameEditText.Text.Trim(),
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

        private void PrepareGoogleSignIn()
        {
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestEmail()
                    .Build();

            _googleApiClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this, new GoogleConnectionFailedCallback() { Activity = this })
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();

            googleSignInButton = FindViewById<SignInButton>(Resource.Id.signIn_GoogleSignInButton);
        }

        private async Task HandleSignInResult(GoogleSignInResult result)
        {
            try
            {
                if (result.IsSuccess && result.SignInAccount?.Email != null)
                {
                    var tokenDto = await _authHelper.ExternalSignInAsync(new ExternalSignInDto
                    {
                        Email = result.SignInAccount.Email
                    });

                    SaveUserInfo(tokenDto);

                    StartActivity(typeof(TasksActivity));
                }
                else
                {
                    _dialogHelper.ShowError(this);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
                _dialogHelper.ShowError(this, ex);
            }
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