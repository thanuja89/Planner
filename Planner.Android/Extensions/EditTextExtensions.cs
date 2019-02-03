using Android.Widget;
using System;

namespace Planner.Android.Extensions
{
    public static class EditTextExtensions
    {
        public static bool IsEmpty(this EditText editText)
        {
            if (editText == null)
                throw new ArgumentNullException(nameof(editText));

            return string.IsNullOrWhiteSpace(editText.Text);
        }
    }
}