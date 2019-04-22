using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Fragments;
using Planner.Droid.Receivers;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System;
using System.Threading.Tasks;

namespace Planner.Droid.Activities
{
    [Activity(Label = "CreateTaskActivity")]
    public class CreateTaskActivity : AppCompatActivity
    {
        private EditText titleEditText;
        private EditText noteEditText;
        private TableRow startDateRow;
        private TableRow endDateRow;
        private TextView startDateTextView;
        private TextView endDateTextView;
        private TextView startTimeTextView;
        private TextView endTimeTextView;
        private CheckBox alarmCheckBox;
        private RadioGroup importanceRadioGroup;
        private LinearLayout repeatLayout;
        private TextView repeatSelectedTextView;
        private Button saveButton;
        private Android.Support.V7.App.AlertDialog _dialog;

        private DateTime _startDate;
        private DateTime _endDate;

        private readonly string[] _items;

        private int _selectedRepeatIndex = 0;
        private readonly ScheduledTaskDataHelper _taskDataHelper;
        private readonly ScheduledTaskWebHelper _taskWebHelper;

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
            _taskWebHelper = new ScheduledTaskWebHelper();

            _items = Enum.GetNames(typeof(Frequency));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_create_task);

            FindViews();

            HandleEvents();
        }

        public void FindViews()
        {
            titleEditText = FindViewById<EditText>(Resource.Id.createTask_TitleEditText);
            noteEditText = FindViewById<EditText>(Resource.Id.createTask_NoteEditText);
            startDateRow = FindViewById<TableRow>(Resource.Id.createTask_StartDateRow);
            endDateRow = FindViewById<TableRow>(Resource.Id.createTask_EndDateRow);
            startDateTextView = FindViewById<TextView>(Resource.Id.createTask_StartDateTextView);
            endDateTextView = FindViewById<TextView>(Resource.Id.createTask_EndDateTextView);
            startTimeTextView = FindViewById<TextView>(Resource.Id.createTask_StartTimeTextView);
            endTimeTextView = FindViewById<TextView>(Resource.Id.createTask_EndTimeTextView);
            alarmCheckBox = FindViewById<CheckBox>(Resource.Id.createTask_AlarmCheckBox);
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
            try
            {
                if (!ValidateInputs())
                    return;

                var task = new ScheduledTask()
                {
                    Id = Guid.NewGuid(),
                    Title = titleEditText.Text,
                    Start = _startDate,
                    End = _endDate,
                    IsAlarm = alarmCheckBox.Checked,
                    Importance = SelectedImportance,
                    Note = noteEditText.Text,
                    Repeat = (Frequency)_selectedRepeatIndex,
                    ClientUpdatedOn = DateTime.UtcNow
                };

                await _taskDataHelper.InsertAsync(task);

                if (task.Start > DateTime.Now)
                {
                    SetAlarm(task.Start, task);
                }

                _ = UpdateToServerAsync(task); // warning suppressed on purpose

                StartActivity(typeof(TasksActivity));
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
            }
        }

        private async Task UpdateToServerAsync(ScheduledTask task)
        {
            try
            {
                var result = await _taskWebHelper.CreateScheduledTaskAsync(task);

                if (result.IsSuccessStatusCode)
                {
                    await _taskDataHelper.UpdateSyncStatusAsync(task.Id);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
            }
        }

        private void SetAlarm(DateTime time, ScheduledTask task)
        {
            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();

            calendar.Set(time.Year, time.Month - 1, time.Day, time.Hour, time.Minute, 0);

            var alarmIntent = new Intent(this, typeof(AlarmReceiver));
            alarmIntent.PutExtra(AlarmReceiver.Constants.TITLE_PARAM_NAME, task.Title);
            alarmIntent.PutExtra(AlarmReceiver.Constants.MESSAGE_PARAM_NAME, task.Note);

            var pending = PendingIntent.GetBroadcast(this, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);

            var alarmManager = (AlarmManager) GetSystemService(AlarmService);

            alarmManager.Set(AlarmType.Rtc, calendar.TimeInMillis, pending);
        }

        private bool ValidateInputs()
        {
            if (!titleEditText.IsEmpty())
            {
                return true;
            }

            if (!noteEditText.IsEmpty())
            {
                noteEditText.Error = "Description can not be empty.";
                return true;
            }

            return false;
        }
    }
}