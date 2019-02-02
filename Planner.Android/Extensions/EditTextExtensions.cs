using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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