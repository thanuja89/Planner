using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Planner.Droid.Fragments
{
    public class DateTimePickerFragment : DialogFragment
    {
        public const string TAG = "X:DateTimePickerFragment";

        private EventHandler<DateTime> _okButtonHandler;
        private DatePicker datePicker;
        private TimePicker timePicker;
        private Button cancelButton;
        private Button okButton;

        public static DateTimePickerFragment NewInstance(EventHandler<DateTime> okButtonHandler)
        {
            DateTimePickerFragment frag = new DateTimePickerFragment
            {
                _okButtonHandler = okButtonHandler
            };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_date_time_picker, container);

            FindViews(view);

            SetDefaultValues();

            HandleEvents();

            return view;
        }

        private void SetDefaultValues()
        {
            var tommorow = DateTime.Now.AddDays(1);

            datePicker.UpdateDate(tommorow.Year, tommorow.Month - 1, tommorow.Day);
        }

        private void HandleEvents()
        {
            okButton.Click += OkButton_Click;
            cancelButton.Click += CancelButton_Click;
        }

        private void FindViews(View view)
        {
            datePicker = view.FindViewById<DatePicker>(Resource.Id.dateTimePickerView_DatePicker);

            timePicker = view.FindViewById<TimePicker>(Resource.Id.dateTimePickerView_TimePicker);

            cancelButton = view.FindViewById<Button>(Resource.Id.dateTimePickerView_CancelButton);
            okButton = view.FindViewById<Button>(Resource.Id.dateTimePickerView_OkButton);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if(_okButtonHandler != null)
            {
                var date = new DateTime(datePicker.Year
                    , datePicker.Month + 1
                    , datePicker.DayOfMonth
                    , timePicker.Hour
                    , timePicker.Minute
                    , 0);

                _okButtonHandler(sender, date);
            }

            Dismiss();
        }
    }
}