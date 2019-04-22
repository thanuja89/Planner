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

        Action<TimeChangedEventArgs> _timeSelectedHandler;

        public static TimePickerFragment NewInstance(Action<TimeChangedEventArgs> onDateSelected)
        {
            TimePickerFragment frag = new TimePickerFragment
            {
                _timeSelectedHandler = onDateSelected
            };
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currentDateTime = DateTime.Now;
            TimePickerDialog dialog = new TimePickerDialog(Activity
                , this
                , currentDateTime.Hour
                , currentDateTime.Minute
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