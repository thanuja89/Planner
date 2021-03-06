﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Planner.Droid.Helpers;
using System;

namespace Planner.Droid.Fragments
{
    public class PasswordResetEmailInputDialogFragment : DialogFragment
    {
        private RelativeLayout layout;
        private EditText emailInputEditText;
        private Button okButton;
        private Button cancelButton;
        private Action<string> _onOkButtonClicked;
        private ProgressBarHelper _progressBarHelper;

        public static PasswordResetEmailInputDialogFragment NewInstance(Action<string> onOkButtonClicked)
        {
            PasswordResetEmailInputDialogFragment frag = new PasswordResetEmailInputDialogFragment
            {
                _onOkButtonClicked = onOkButtonClicked
            };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_password_reset_email_input_dialog, container);

            FindViews(view);

            HandleEvents();

            _progressBarHelper = new ProgressBarHelper(Activity, Activity.Window, layout);

            return view;
        }

        public void ShowProgressBar()
        {
            _progressBarHelper.Show();
        }

        public void HideProgressBar()
        {
            _progressBarHelper.Hide();
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
            layout = view.FindViewById<RelativeLayout>(Resource.Id.emailInputDialog_Layout);
            emailInputEditText = view.FindViewById<EditText>(Resource.Id.emailInputDialog_EmailInputEditText);
            okButton = view.FindViewById<Button>(Resource.Id.emailInputDialog_OkButton);
            cancelButton = view.FindViewById<Button>(Resource.Id.emailInputDialog_CancelButton);
        }
    }
}