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

        private DateTime? _defaultDate;

        Action<TimeChangedEventArgs> _timeSelectedHandler;

        public static TimePickerFragment NewInstance(Action<TimeChangedEventArgs> onDateSelected, DateTime? defaultDate = null)
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
            DateTime defaultDate = _defaultDate ?? DateTime.Now;
            TimePickerDialog dialog = new TimePickerDialog(Activity
                , this
                , defaultDate.Hour
                , defaultDate.Minute
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