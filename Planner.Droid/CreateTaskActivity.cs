using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Fragments;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Services;
using System;

namespace Planner.Droid
{
    [Activity(Label = "CreateTaskActivity", MainLauncher = true)]
    public class CreateTaskActivity : AppCompatActivity
    {
        private EditText titleEditText;
        private EditText descriptionEditText;
        private EditText noteEditText;
        private TableRow startDateRow;
        private TableRow endDateRow;
        private TextView startDateText;
        private TextView endDateText;
        private ImageButton alarmButton;
        private ImageButton notifyButton;
        private RadioGroup importanceRadioGroup;
        private Button saveButton;

        private bool _isAlarm;
        private bool _isNotify;

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
            startDateText = FindViewById<TextView>(Resource.Id.createTask_StartDateText);
            endDateText = FindViewById<TextView>(Resource.Id.createTask_EndDateText);
            alarmButton = FindViewById<ImageButton>(Resource.Id.createTask_AlarmButton);
            notifyButton = FindViewById<ImageButton>(Resource.Id.createTask_NotifyButton);
            importanceRadioGroup = FindViewById<RadioGroup>(Resource.Id.createTask_ImportanceRadioGroup);
            saveButton = FindViewById<Button>(Resource.Id.createTask_SaveButton);
        }

        private void HandleEvents()
        {
            startDateRow.Click += _startDateButton_Click;
            endDateRow.Click += _endDateButton_Click;
            alarmButton.Click += AlarmButton_Click;
            notifyButton.Click += AlertButton_Click;
            saveButton.Click += _saveButton_Click;
        }

        private void AlertButton_Click(object sender, EventArgs e)
        {
            _isNotify = !_isNotify;
        }

        private void AlarmButton_Click(object sender, EventArgs e)
        {
            _isAlarm = !_isAlarm;
        }

        private void _endDateButton_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance((time) =>
            {
                endDateText.Text = time.ToLongDateString();
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void _startDateButton_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance((time) =>
            {
                startDateText.Text = time.ToLongDateString();
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private async void _saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            var task = new ScheduledTask()
            {
                Id = Guid.NewGuid(),
                Title = titleEditText.Text,
                Description = descriptionEditText.Text,
                Start = DateTime.Parse(startDateText.Text),
                End = DateTime.Parse(endDateText.Text),
                IsAlarm = _isAlarm,
                IsNotify = _isNotify,
                Importance = SelectedImportance,
                Note = noteEditText.Text
            };

            await _taskDataService.InsertAsync(task);
        }

        private bool ValidateInputs()
        {
            if (titleEditText.IsEmpty())
            {
                titleEditText.Error = "Title can not be empty.";
                return false;
            }

            if (startDateText.IsEmpty())
            {
                startDateText.Error = "Start Date can not be empty.";
                return false;
            }

            if (endDateText.IsEmpty())
            {
                endDateText.Error = "End Date can not be empty.";
                return false;
            }

            return true;
        }
    }
}