using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Planner.Droid.Activities;
using Planner.Droid.Helpers;
using Planner.Dto;
using Planner.Mobile.Core.Helpers;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Planner.Droid.Fragments
{
    public class ConfirmationCodeInputDialogFragment : DialogFragment
    {
        public const string TAG = "X:ConfirmationCodeInputDialogFragment";

        private string _userId;
        private ProgressBarHelper _progressBarHelper;
        private RelativeLayout layout;
        private EditText inputEditText;
        private TextView resendTextView;
        private Button okButton;
        private Button cancelButton;
        private AuthHelper _authHelper;
        private DialogHelper _dialogHelper;

        public static ConfirmationCodeInputDialogFragment NewInstance(string userId)
        {
            ConfirmationCodeInputDialogFragment frag = new ConfirmationCodeInputDialogFragment
            {
                _userId = userId,
                _authHelper = new AuthHelper(),
                _dialogHelper = new DialogHelper()
        };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_confirmation_code_input_dialog, container);

            FindViews(view);

            HandleEvents();

            _progressBarHelper = new ProgressBarHelper(Activity, Activity.Window, layout);

            return view;
        }

        private void FindViews(View view)
        {
            layout = view.FindViewById<RelativeLayout>(Resource.Id.inputDialog_RelativeLayout);
            inputEditText = view.FindViewById<EditText>(Resource.Id.inputDialog_InputEditText);
            resendTextView = view.FindViewById<TextView>(Resource.Id.inputDialog_ResendTextView);

            okButton = view.FindViewById<Button>(Resource.Id.inputDialog_OkButton);
            cancelButton = view.FindViewById<Button>(Resource.Id.inputDialog_CancelButton);
        }

        private void HandleEvents()
        {
            okButton.Click += OkButton_Click;
            cancelButton.Click += CancelButton_Click;
            resendTextView.Click += ResendTextView_Click;
        }

        private async void ResendTextView_Click(object sender, EventArgs e)
        {
            await ResendConfirmationEmailAsync(_userId);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private async void OkButton_Click(object sender, EventArgs e)
        {
            await ConfirmEmailAsync(_userId, inputEditText.Text);
        }

        private async Task ConfirmEmailAsync(string userId, string s)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    _dialogHelper.ShowError(Activity, "The code cannot be empty.");
                    return;
                }

                if (!Regex.IsMatch(s, "[0-9]{4,}"))
                {
                    _dialogHelper.ShowError(Activity, "The code entered is incorrect.");
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
                    Dismiss();

                    var activity = Activity;

                    _dialogHelper.ShowSuccessDialog(Activity, "Signing Up was successful. Please Sign In"
                                    , (o, ea) => activity.StartActivity(typeof(SignInActivity)));
                }
                else if (res.StatusCode == HttpStatusCode.BadRequest)
                    _dialogHelper.ShowError(Activity, "The code entered is incorrect.");
                else
                    _dialogHelper.ShowError(Activity);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                _progressBarHelper.Hide();

                _dialogHelper.ShowError(Activity, ex);
            }
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

                _dialogHelper.ShowError(Activity, ex);
            }
        }
    }
}