using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Fragments;
using Planner.Droid.Helpers;
using Planner.Droid.Receivers;
using Planner.Droid.Services;
using Planner.Droid.Util;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System;

namespace Planner.Droid.Activities
{
    [Activity(Label = "CreateTaskActivity")]
    public class CreateTaskActivity : AppCompatActivity
    {
        private RelativeLayout layout;
        private EditText titleEditText;
        private EditText noteEditText;
        private TextView startDateTextView;
        private TextView endDateTextView;
        private TextView startTimeTextView;
        private TextView endTimeTextView;

        private ImageView startDateImageView;
        private ImageView endDateImageView;
        private ImageView startTimeImageView;
        private ImageView endTimeImageView;

        private RadioGroup importanceRadioGroup;
        private LinearLayout repeatLayout;
        private TextView repeatSelectedTextView;
        private Button saveButton;
        private Android.Support.V7.App.AlertDialog _dialog;

        private Date _startDate;
        private Date _endDate;
        private Time _startTime;
        private Time _endTime;

        private readonly string[] _items;

        private int _selectedRepeatIndex = 0;
        private ProgressBarHelper _progressBarHelper;
        private readonly ScheduledTaskDataHelper _taskDataHelper;
        private readonly DialogHelper _dialogHelper;
        private readonly AlarmHelper _alarmHelper;

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
            _taskDataHelper = new ScheduledTaskDataHelper();
            _dialogHelper = new DialogHelper();
            _alarmHelper = new AlarmHelper();
            _items = Enum.GetNames(typeof(Frequency));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_create_task);

            FindViews();

            HandleEvents();

            _progressBarHelper = new ProgressBarHelper(this, Window, layout);
        }

        public void FindViews()
        {
            layout = FindViewById<RelativeLayout>(Resource.Id.createTask_Layout);
            titleEditText = FindViewById<EditText>(Resource.Id.createTask_TitleEditText);
            noteEditText = FindViewById<EditText>(Resource.Id.createTask_NoteEditText);
            startDateTextView = FindViewById<TextView>(Resource.Id.createTask_StartDateTextView);
            endDateTextView = FindViewById<TextView>(Resource.Id.createTask_EndDateTextView);
            startTimeTextView = FindViewById<TextView>(Resource.Id.createTask_StartTimeTextView);
            endTimeTextView = FindViewById<TextView>(Resource.Id.createTask_EndTimeTextView);

            startDateImageView = FindViewById<ImageView>(Resource.Id.createTask_StartDateImageView);
            startTimeImageView = FindViewById<ImageView>(Resource.Id.createTask_StartTimeImageView);
            endDateImageView = FindViewById<ImageView>(Resource.Id.createTask_EndDateImageView);
            endTimeImageView = FindViewById<ImageView>(Resource.Id.createTask_EndTimeImageView);

            importanceRadioGroup = FindViewById<RadioGroup>(Resource.Id.createTask_ImportanceRadioGroup);
            repeatLayout = FindViewById<LinearLayout>(Resource.Id.createTask_RepeatLayout);
            repeatSelectedTextView = FindViewById<TextView>(Resource.Id.createTask_RepeatSelectedTextView);
            saveButton = FindViewById<Button>(Resource.Id.createTask_SaveButton);
        }

        private void HandleEvents()
        {
            startDateImageView.Click += StartDateImageView_Click;
            startTimeImageView.Click += StartTimeImageView_Click;
            endDateImageView.Click += EndDateImageView_Click;
            endTimeImageView.Click += EndTimeImageView_Click;

            repeatLayout.Click += RepeatLayout_Click;
            saveButton.Click += SaveButton_Click;
        }

        private void EndTimeImageView_Click(object sender, EventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(ev =>
            {
                _endTime = ev.Time;
                endTimeTextView.Text = ev.Time.ToString();
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        private void StartTimeImageView_Click(object sender, EventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(ev =>
            {
                _startTime = ev.Time;
                startTimeTextView.Text = ev.Time.ToString();
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
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

        private void EndDateImageView_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(ev =>
            {
                _endDate = ev.Date;
                endDateTextView.Text = ev.Date.ToString();
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void StartDateImageView_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(ev => 
            {
                _startDate = ev.Date;
                startDateTextView.Text = ev.Date.ToString();
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                _progressBarHelper.Show();

                var task = new ScheduledTask()
                {
                    Id = Guid.NewGuid(),
                    Title = titleEditText.Text,
                    Start = _startDate == default || _startTime == default 
                        ? DateTime.MinValue : Utilities.ToDateTime(_startDate, _startTime),
                    End = _endDate == default || _endTime == default 
                        ? DateTime.MinValue : Utilities.ToDateTime(_endDate, _endTime),
                    Importance = SelectedImportance,
                    Note = noteEditText.Text,
                    Repeat = (Frequency)_selectedRepeatIndex,
                    ApplicationUserId = Utilities.GetUserId(),
                    ClientUpdatedOnTicks = DateTime.UtcNow.Ticks
                };

                await _taskDataHelper.InsertAsync(task);

                if (task.Start > DateTime.Now)
                {
                    _alarmHelper.SetAlarm(task);
                }              

                _ = SyncService.Instance.SyncAsync(); // warning suppressed on purpose

                StartActivity(typeof(TasksActivity));

                _progressBarHelper.Hide();
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                _progressBarHelper.Hide();
                _dialogHelper.ShowError(this, ex);
            }
        }

        private bool ValidateInputs()
        {
            if(_startDate != default 
                && _startTime != default 
                && _endDate != default 
                && _endTime != default)
            {
                var start = Utilities.ToDateTime(_startDate, _startTime);
                var end = Utilities.ToDateTime(_endDate, _endTime);

                if(start > end)
                {
                    _dialogHelper.ShowError(this, "Start Date cannot be after the End Date.");
                    return false;
                }
            }

            if (titleEditText.IsEmpty() && noteEditText.IsEmpty())
            {
                noteEditText.Error = "Both Description and Title can not be empty.";
                return false;
            }

            return true;
        }
    }
}