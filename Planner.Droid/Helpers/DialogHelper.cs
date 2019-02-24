
using Android.App;
using Android.Content;
using System;

namespace Planner.Droid.Helpers
{
    public class DialogHelper
    {
        public void ShowError(Context context, string message)
        {
            ShowDialog(context, "Error", message);
        }

        public void ShowError(Context context, Exception ex)
        {
            switch (ex)
            {
                case System.Net.WebException web:
                    ShowError(context, "An error occured, please check the internet connection");
                    break;

                default:
                    ShowError(context);
                    break;
            }
        }

        public void ShowError(Context context)
        {
            ShowError(context, "Something went wrong. Try again later.");
        }

        public void ShowSuccessDialog(Context context, string message)
        {
            ShowDialog(context, "Success", message);
        }

        public void ShowSuccessDialog(Context context, string message, EventHandler<DialogClickEventArgs> positiveCallback)
        {
            ShowDialog(context, "Success", message, positiveCallback);
        }

        private void ShowDialog(Context context, string title, string message, EventHandler<DialogClickEventArgs> positiveCallback = null)
        {
            AlertDialog.Builder builder;
            builder = new AlertDialog.Builder(context);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetCancelable(false);

            builder.SetPositiveButton("OK", positiveCallback);

            Dialog dialog = builder.Create();
            dialog.Show();
        }
    }
}