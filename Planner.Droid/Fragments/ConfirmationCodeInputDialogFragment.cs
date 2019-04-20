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

        private EditText inputEditText;
        private Button okButton;
        private Button cancelButton;

        public static ConfirmationCodeInputDialogFragment NewInstance(Action<string> onOkButtonClicked)
        {
            ConfirmationCodeInputDialogFragment frag = new ConfirmationCodeInputDialogFragment
            {
                _onOkButtonClicked = onOkButtonClicked
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

            okButton = view.FindViewById<Button>(Resource.Id.inputDialog_OkButton);
            cancelButton = view.FindViewById<Button>(Resource.Id.inputDialog_CancelButton);
        }

        private void HandleEvents()
        {
            okButton.Click += OkButton_Click;
            cancelButton.Click += CancelButton_Click;
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