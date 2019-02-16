using Android.Widget;
using System;

namespace Planner.Droid.Extensions
{
    public static class ControlExtensions
    {
        public static bool IsEmpty(this EditText editText)
        {
            if (editText == null)
                throw new ArgumentNullException(nameof(editText));

            return string.IsNullOrWhiteSpace(editText.Text);
        }

        public static bool IsEmpty(this TextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));

            return string.IsNullOrWhiteSpace(textView.Text);
        }
    }
}