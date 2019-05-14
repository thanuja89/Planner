using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Planner.Droid.Fragments
{
    public class PasswordResetEmailInputDialogFragment : DialogFragment
    {
        private EditText emailInputEditText;
        private Button okButton;
        private Button cancelButton;
        private Action<string> _onOkButtonClicked;

        public static PasswordResetEmailInputDialogFragment NewInstance(Action<string> onOkButtonClicked)
        {
            PasswordResetEmailInputDialogFragment frag = new PasswordResetEmailInputDialogFragment
            {
                _onOkButtonClicked = onOkButtonClicked
            };
            return frag;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_password_reset_email_input_dialog, container);

            FindViews(view);

            HandleEvents();

            return view;
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
            var text = emailInputEditText.Text;

            _onOkButtonClicked?.Invoke(text);
        }

        private void FindViews(View view)
        {
            emailInputEditText = view.FindViewById<EditText>(Resource.Id.emailInputDialog_EmailInputEditText);
            okButton = view.FindViewById<Button>(Resource.Id.emailInputDialog_OkButton);
            cancelButton = view.FindViewById<Button>(Resource.Id.emailInputDialog_CancelButton);
        }
    }
}