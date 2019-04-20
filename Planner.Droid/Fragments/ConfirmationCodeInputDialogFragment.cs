using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Planner.Droid.Fragments
{
    public class ConfirmationCodeInputDialogFragment : DialogFragment
    {
        public const string TAG = "X:ConfirmationCodeInputDialogFragment";
        private Action<string> _onOkButtonClicked;
        private Action _resendClicked;

        private EditText inputEditText;
        private TextView resendTextView;
        private Button okButton;
        private Button cancelButton;

        public static ConfirmationCodeInputDialogFragment NewInstance(Action<string> onOkButtonClicked, Action resendClicked)
        {
            ConfirmationCodeInputDialogFragment frag = new ConfirmationCodeInputDialogFragment
            {
                _onOkButtonClicked = onOkButtonClicked,
                _resendClicked = resendClicked
            };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_confirmation_code_input_dialog, container);

            FindViews(view);

            HandleEvents();

            return view;
        }

        private void FindViews(View view)
        {
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

        private void ResendTextView_Click(object sender, EventArgs e)
        {
            _resendClicked?.Invoke();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var text = inputEditText.Text;

            _onOkButtonClicked?.Invoke(text);
        }
    }
}