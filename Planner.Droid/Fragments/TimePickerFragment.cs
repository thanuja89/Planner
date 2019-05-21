using Android.App;
using Android.OS;
using Android.Widget;
using Planner.Droid.Util;
using System;

namespace Planner.Droid.Fragments
{
    public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        public const string TAG = "X:TimePickerFragment";

        private DateTime _defaultDate;

        Action<TimeChangedEventArgs> _timeSelectedHandler;

        public static TimePickerFragment NewInstance(Action<TimeChangedEventArgs> onDateSelected, DateTime defaultDate = default)
        {
            TimePickerFragment frag = new TimePickerFragment
            {
                _timeSelectedHandler = onDateSelected,
                _defaultDate = defaultDate
            };
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime date = _defaultDate == default ? DateTime.Now : _defaultDate;
            TimePickerDialog dialog = new TimePickerDialog(Activity
                , this
                , date.Hour
                , date.Minute
                , true);

            return dialog;
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            _timeSelectedHandler?.Invoke(new TimeChangedEventArgs()
            {
                Time = new Time(hourOfDay, minute)
            });
        }
    }
}