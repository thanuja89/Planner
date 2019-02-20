using Android.App;
using Android.OS;
using Android.Widget;
using System;

namespace Planner.Droid.Fragments
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        public const string TAG = "X:DatePickerFragment";

        Action<DateTime> _dateSelectedHandler;

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
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
            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currently.Year,
                                                           currently.Month - 1,
                                                           currently.Day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            DateTime selectedDate = new DateTime(year, month + 1, dayOfMonth);
            _dateSelectedHandler?.Invoke(selectedDate);
        }
    }
}