using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Fragments;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Models;
using Planner.Mobile.Core.Services;
using System;

namespace Planner.Droid
{
    [Activity(Label = "CreateTaskActivity")]
    public class CreateTaskActivity : AppCompatActivity
    {
        private EditText titleEditText;
        private EditText descriptionEditText;
        private EditText noteEditText;
        private TableRow startDateRow;
        private TableRow endDateRow;
        private TextView startDateTextView;
        private TextView endDateTextView;
        private TextView startTimeTextView;
        private TextView endTimeTextView;
        private CheckBox alarmCheckBox;
        private CheckBox notifyCheckBox;
        private RadioGroup importanceRadioGroup;
        private LinearLayout repeatLayout;
        private TextView repeatSelectedTextView;
        private Button saveButton;
        private Android.Support.V7.App.AlertDialog _dialog;

        private DateTime _startDate;
        private DateTime _endDate;

        private readonly string[] _items;

        private int _selectedRepeatIndex = 0;
        private readonly ScheduledTaskDataService _taskDataService;

        public Mobile.Core.Data.Importance SelectedImportance
        {
            get
            {
                if (importanceRadioGroup == null)
                    return Mobile.Core.Data.Importance.Medium;

                switch (importanceRadioGroup.CheckedRadioButtonId)
                {
                    case Resource.Id.createTask_ImportanceRadio_Low:
                        return Mobile.Core.Data.Importance.Low;

                    case Resource.Id.createTask_ImportanceRadio_High:
                        return Mobile.Core.Data.Importance.High;

                    default:
                        return Mobile.Core.Data.Importance.Medium;
                }
            }
        }

        public CreateTaskActivity()
        {
            _taskDataService = new ScheduledTaskDataService();

            _items = Enum.GetNames(typeof(Frequency));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateTaskView);

            FindViews();

            HandleEvents();
        }

        public void FindViews()
        {
            titleEditText = FindViewById<EditText>(Resource.Id.createTask_TitleEditText);
            descriptionEditText = FindViewById<EditText>(Resource.Id.createTask_DescriptionEditText);
            noteEditText = FindViewById<EditText>(Resource.Id.createTask_NoteEditText);
            startDateRow = FindViewById<TableRow>(Resource.Id.createTask_StartDateRow);
            endDateRow = FindViewById<TableRow>(Resource.Id.createTask_EndDateRow);
            startDateTextView = FindViewById<TextView>(Resource.Id.createTask_StartDateTextView);
            endDateTextView = FindViewById<TextView>(Resource.Id.createTask_EndDateTextView);
            startTimeTextView = FindViewById<TextView>(Resource.Id.createTask_StartTimeTextView);
            endTimeTextView = FindViewById<TextView>(Resource.Id.createTask_EndTimeTextView);
            alarmCheckBox = FindViewById<CheckBox>(Resource.Id.createTask_AlarmCheckBox);
            notifyCheckBox = FindViewById<CheckBox>(Resource.Id.createTask_AlertCheckBox);
            importanceRadioGroup = FindViewById<RadioGroup>(Resource.Id.createTask_ImportanceRadioGroup);
            repeatLayout = FindViewById<LinearLayout>(Resource.Id.createTask_RepeatLayout);
            repeatSelectedTextView = FindViewById<TextView>(Resource.Id.createTask_RepeatSelectedTextView);
            saveButton = FindViewById<Button>(Resource.Id.createTask_SaveButton);
        }

        private void HandleEvents()
        {
            startDateRow.Click += StartDateRow_Click;
            endDateRow.Click += EndDateRow_Click;
            repeatLayout.Click += RepeatLayout_Click;
            saveButton.Click += SaveButton_Click;
        }

        private void RepeatLayout_Click(object sender, EventArgs e)
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle("Choose a repeat interval");
            builder.SetSingleChoiceItems(_items, _selectedRepeatIndex, (s, ev) =>
            {
                _selectedRepeatIndex = ev.Which;
                repeatSelectedTextView.Text = _items[ev.Which];
                _dialog.Dismiss();
            });

            _dialog = builder.Create();
            _dialog.Show();
        }

        private void EndDateRow_Click(object sender, EventArgs e)
        {
            DateTimePickerFragment frag = DateTimePickerFragment.NewInstance((o, date) =>
            {
                _endDate = date;
                endDateTextView.Text = date.ToShortDateString();
                endTimeTextView.Text = date.ToShortTimeString();
            });

            frag.Show(FragmentManager, DateTimePickerFragment.TAG);
        }

        private void StartDateRow_Click(object sender, EventArgs e)
        {
            DateTimePickerFragment frag = DateTimePickerFragment.NewInstance((o, date) => 
            {
                _startDate = date;
                startDateTextView.Text = date.ToShortDateString();
                startTimeTextView.Text = date.ToShortTimeString();
            });

            frag.Show(FragmentManager, DateTimePickerFragment.TAG);
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            var task = new ScheduledTask()
            {
                Id = Guid.NewGuid(),
                Title = titleEditText.Text,
                Description = descriptionEditText.Text,
                Start = _startDate,
                End = _endDate,
                IsAlarm = alarmCheckBox.Checked,
                IsNotify = notifyCheckBox.Checked,
                Importance = SelectedImportance,
                Note = noteEditText.Text,
                Repeat = (Frequency) _selectedRepeatIndex
            };

            await _taskDataService.InsertAsync(task);

            StartActivity(typeof(TasksActivity));
        }

        private bool ValidateInputs()
        {
            if (titleEditText.IsEmpty())
            {
                titleEditText.Error = "Title can not be empty.";
                return false;
            }

            if (_startDate == default)
            {
                startDateTextView.Error = "Please pick the Start Date";
                return false;
            }

            if (_endDate == default)
            {
                endDateTextView.Error = "Please pick the End Date";
                return false;
            }

            return true;
        }
    }
}