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

        private DateTime? _defaultDate;

        Action<DateChangedEventArgs> _dateSelectedHandler;

        public static DatePickerFragment NewInstance(Action<DateChangedEventArgs> onDateSelected, DateTime? defaultDate = null)
        {
            DatePickerFragment frag = new DatePickerFragment
            {
                _dateSelectedHandler = onDateSelected,
                _defaultDate = defaultDate
            };
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime date = _defaultDate ?? DateTime.Now;
            DatePickerDialog dialog = 
                new DatePickerDialog(Activity
                , this
                , date.Year
                , date.Month - 1
                , date.Day);

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