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
    //[Activity(Label = "CreateTaskActivity", MainLauncher = true)]
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
        private LinearLayout repeatLayout;
        private TextView repeatSelectedTextView;
        private Button saveButton;

        private bool _isAlarm;
        private bool _isNotify;

        private readonly string[] _items;

        private int _selectedRepeatIndex = 0;
        private Android.Support.V7.App.AlertDialog _dialog;
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
            startDateText = FindViewById<TextView>(Resource.Id.createTask_StartDateText);
            endDateText = FindViewById<TextView>(Resource.Id.createTask_EndDateText);
            alarmButton = FindViewById<ImageButton>(Resource.Id.createTask_AlarmButton);
            notifyButton = FindViewById<ImageButton>(Resource.Id.createTask_NotifyButton);
            importanceRadioGroup = FindViewById<RadioGroup>(Resource.Id.createTask_ImportanceRadioGroup);
            repeatLayout = FindViewById<LinearLayout>(Resource.Id.createTask_RepeatLayout);
            repeatSelectedTextView = FindViewById<TextView>(Resource.Id.createTask_RepeatSelectedTextView);
            saveButton = FindViewById<Button>(Resource.Id.createTask_SaveButton);
        }

        private void HandleEvents()
        {
            startDateRow.Click += _startDateButton_Click;
            endDateRow.Click += _endDateButton_Click;
            alarmButton.Click += AlarmButton_Click;
            notifyButton.Click += AlertButton_Click;
            repeatLayout.Click += RepeatLayout_Click;
            saveButton.Click += _saveButton_Click;
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
                Note = noteEditText.Text,
                Repeat = (Frequency) _selectedRepeatIndex
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