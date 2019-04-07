using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Planner.Droid.Extensions;
using Planner.Droid.Fragments;
using Planner.Droid.Helpers;
using Planner.Droid.Receivers;
using Planner.Droid.Services;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System;
using System.Threading.Tasks;

namespace Planner.Droid.Activities
{
    [Activity(Label = "CreateTaskActivity")]
    public class EditTaskActivity : AppCompatActivity
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
        private Guid __scheduledTaskId;
        private ScheduledTask _scheduledTask;
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

            set
            {
                switch (value)
                {
                    case Mobile.Core.Data.Importance.Low:
                        importanceRadioGroup.Check(Resource.Id.createTask_ImportanceRadio_Low);
                        break;
                    case Mobile.Core.Data.Importance.High:
                        importanceRadioGroup.Check(Resource.Id.createTask_ImportanceRadio_High);
                        break;
                    default:
                        importanceRadioGroup.Check(Resource.Id.createTask_ImportanceRadio_Medium);
                        break;
                }
            }
        }

        public EditTaskActivity()
        {
            _taskDataHelper = new ScheduledTaskDataHelper();
            _taskWebHelper = new ScheduledTaskWebHelper();

            _items = Enum.GetNames(typeof(Frequency));
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            __scheduledTaskId = new Guid(Intent.GetStringExtra("TaskId"));

            _scheduledTask = await _taskDataHelper.GetByIdAsync(__scheduledTaskId);

            SetContentView(Resource.Layout.activity_create_task);

            FindViews();

            SetValues();

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

        private void SetValues()
        {
            titleEditText.Text = _scheduledTask.Title;
            noteEditText.Text = _scheduledTask.Note;

            startDateTextView.Text = _scheduledTask.Start == DateTime.MinValue ? "-" : _scheduledTask.Start.ToShortDateString();
            startTimeTextView.Text = _scheduledTask.Start == DateTime.MinValue ? "-" : _scheduledTask.Start.ToShortTimeString();

            endDateTextView.Text = _scheduledTask.End == DateTime.MinValue ? "-" : _scheduledTask.End.ToShortDateString();
            endTimeTextView.Text = _scheduledTask.End == DateTime.MinValue ? "-" : _scheduledTask.End.ToShortTimeString();

            alarmCheckBox.Selected = _scheduledTask.IsAlarm;

            SelectedImportance = _scheduledTask.Importance;

            _selectedRepeatIndex = (int)_scheduledTask.Repeat;
            repeatSelectedTextView.Text = _scheduledTask.Repeat.ToString();
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

            _scheduledTask.Title = titleEditText.Text;
            _scheduledTask.Start = _startDate;
            _scheduledTask.End = _endDate;
            _scheduledTask.IsAlarm = alarmCheckBox.Checked;
            _scheduledTask.Importance = SelectedImportance;
            _scheduledTask.Note = noteEditText.Text;
            _scheduledTask.Repeat = (Frequency)_selectedRepeatIndex;
            _scheduledTask.ClientUpdatedOn = DateTime.UtcNow;

            await _taskDataHelper.UpdateAsync(_scheduledTask);

            //if (task.Start > DateTime.Now)
            //{
            //    SetAlarm(task.Start, task);
            //}

            //StartSyncService();

            //_ = PostToServerAsync(_scheduledTask); // warning suppressed on purpose

            StartActivity(typeof(TasksActivity));
        }

        private Task PostToServerAsync(ScheduledTask task)
        {
            return _taskWebHelper.UpdateScheduledTaskAsync(task.Id, task)
                .ContinueWith(t =>
                {
                    if (t.Result.IsSuccessStatusCode)
                    {
                        _taskDataHelper.UpdateSyncStatusAsync(task.Id);
                    }
                },
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void StartSyncService()
        {
            var intent = new Intent(this, typeof(SyncService));
            StartService(intent);
        }

        //private void SetAlarm(ScheduledTask task)
        //{
        //    var alarmManager = (AlarmManager)GetSystemService(AlarmService);

        //    Utilities.SetAlarm(ApplicationContext, alarmManager, task);
        //}

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