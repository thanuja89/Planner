
using Android.App;
using Android.Content;

namespace Planner.Android.Extensions.Services
{
    public class DialogService
    {
        public void ShowError(Context context, string message)
        {
            AlertDialog.Builder builder;
            builder = new AlertDialog.Builder(context);
            builder.SetTitle("Error");
            builder.SetMessage(message);
            builder.SetCancelable(false);
            builder.SetPositiveButton("OK", delegate { });
            Dialog dialog = builder.Create();
            dialog.Show();
        }
    }
}