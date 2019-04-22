using Android.App;
using Android.OS;
using Android.Widget;
using Planner.Droid.Util;
using System;

namespace Planner.Droid.Fragments
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        public const string TAG = "X:DatePickerFragment";

        Action<DateChangedEventArgs> _dateSelectedHandler;

        public static DatePickerFragment NewInstance(Action<DateChangedEventArgs> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment
            {
                _dateSelectedHandler = onDateSelected
            };
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog = 
                new DatePickerDialog(Activity
                , this
                , currently.Year
                , currently.Month - 1
                , currently.Day);

            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            _dateSelectedHandler?.Invoke(new DateChangedEventArgs()
            {
                Date = new Date(year, month + 1, dayOfMonth)
            });
        }
    }
}