﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Planner.Droid.Fragments
{
    public class DateTimePickerFragment : DialogFragment
    {
        public const string TAG = "X:DateTimePickerFragment";

        private readonly EventHandler<DateTime> _okButtonHandler;
        private DatePicker datePicker;
        private TimePicker timePicker;
        private Button cancelButton;
        private Button okButton;

        public DateTimePickerFragment(EventHandler<DateTime> okButtonHandler)
        {
            _okButtonHandler = okButtonHandler;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.DateTimePickerView, container);

            datePicker = view.FindViewById<DatePicker>(Resource.Id.dateTimePickerView_DatePicker);

            timePicker = view.FindViewById<TimePicker>(Resource.Id.dateTimePickerView_TimePicker);

            cancelButton = view.FindViewById<Button>(Resource.Id.dateTimePickerView_CancelButton);
            okButton = view.FindViewById<Button>(Resource.Id.dateTimePickerView_OkButton);

            okButton.Click += OkButton_Click;
            cancelButton.Click += CancelButton_Click;

            return view;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if(_okButtonHandler != null)
            {
                var date = new DateTime(datePicker.Year, datePicker.Month, datePicker.DayOfMonth, timePicker.Hour, timePicker.Minute, 0);
                _okButtonHandler(sender, date);
            }

            Dismiss();
        }
    }
}