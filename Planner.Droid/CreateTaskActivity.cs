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
        //private Spinner importanceSpinner;
        //private Spinner repeatSpinner;
        private Button addNoteButton;
        private Button saveButton;
        private Button startDateButton;
        private Button endDateButton;
        private TextView startDateText;
        private TextView endDateText;
        private readonly ScheduledTaskDataService _taskDataService;

        public CreateTaskActivity()
        {
            _taskDataService = new ScheduledTaskDataService();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateTaskView);

            FindViews();

            SetUpSpinners();

            HandleEvents();
        }

        private void SetUpSpinners()
        {
            //var importanceItems = Enum
            //    .GetNames(typeof(Mobile.Core.Data.Importance));

            //var repeatItems = Enum
            //    .GetNames(typeof(Frequency));

            //var importanceAdapter = new ArrayAdapter<string>(this,
            //    Android.Resource.Layout.SimpleSpinnerItem, importanceItems);

            //var repeatAdapter = new ArrayAdapter<string>(this,
            //    Android.Resource.Layout.SimpleSpinnerItem, repeatItems);

            //importanceSpinner.Adapter = importanceAdapter;
            //repeatSpinner.Adapter = repeatAdapter;
        }

        public void FindViews()
        {
            titleEditText = FindViewById<EditText>(Resource.Id.createTask_TitleEditText);
            descriptionEditText = FindViewById<EditText>(Resource.Id.createTask_DescriptionEditText);
            //importanceSpinner = FindViewById<Spinner>(Resource.Id.createTask_ImportanceSpinner);
            //repeatSpinner = FindViewById<Spinner>(Resource.Id.createTask_RepeatSpinner);
            addNoteButton = FindViewById<Button>(Resource.Id.createTask_AddNoteButton);
            saveButton = FindViewById<Button>(Resource.Id.createTask_SaveButton);
            startDateButton = FindViewById<Button>(Resource.Id.createTask_StartDateButton);
            endDateButton = FindViewById<Button>(Resource.Id.createTask_EndDateButton);
            startDateText = FindViewById<TextView>(Resource.Id.createTask_StartDateText);
            endDateText = FindViewById<TextView>(Resource.Id.createTask_EndDateText);
        }

        private void HandleEvents()
        {
            startDateButton.Click += _startDateButton_Click;
            endDateButton.Click += _endDateButton_Click;
            saveButton.Click += _saveButton_Click;
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
                //Importance = (Mobile.Core.Data.Importance) importanceSpinner.SelectedItemPosition,
                //Repeat = (Frequency) repeatSpinner.SelectedItemPosition
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

            if (descriptionEditText.IsEmpty())
            {
                descriptionEditText.Error = "Description can not be empty.";
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