using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Planner.Droid.Activities
{
    [Activity(Label = "CreateTaskActivity")]
    public class ResetPasswordActivity : AppCompatActivity
    {
        private EditText passwordEditText;
        private EditText confirmPasswordEditText;
        private Button resetButton;

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

        private void ResetButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FindViews()
        {
            passwordEditText = FindViewById<EditText>(Resource.Id.resetPassword_PasswordEditText);
            confirmPasswordEditText = FindViewById<EditText>(Resource.Id.resetPassword_ConfirmPasswordEditText);
            resetButton = FindViewById<Button>(Resource.Id.resetPassword_ResetButton);
        }
    }
}