using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Planner.Droid.Controls;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Droid.Activities
{
    [Activity(Label = "TasksActivity", MainLauncher = true)]
    public class TasksActivity : AppCompatActivity
    {
        private RecyclerView recyclerView;
        private readonly RecyclerView.LayoutManager _layoutManager;
        private List<ScheduledTask> _tasks;
        private TaskViewAdapter _adapter;
        private FloatingActionButton createButton;
        private readonly ScheduledTaskDataHelper _taskDataHelper;
        private readonly ScheduledTaskWebHelper _taskWebHelper;

        public TasksActivity()
        {
            _taskDataHelper = new ScheduledTaskDataHelper();
            _taskWebHelper = new ScheduledTaskWebHelper();
            _layoutManager = new LinearLayoutManager(this);
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_tasks);

            FindViews();
            HandleEvents();

            await PrepareRecyclerViewAsync();
        }

        private async Task PrepareRecyclerViewAsync()
        {
            try
            {
                recyclerView = FindViewById<RecyclerView>(Resource.Id.tasksView_RecyclerView);
                recyclerView.SetLayoutManager(_layoutManager);
                recyclerView.HasFixedSize = true;

                _tasks = await _taskDataHelper.GetAsync();

                _adapter = new TaskViewAdapter(_tasks);
                _adapter.ItemDeleteClick += Adapter_ItemDeleteClick;

                recyclerView.SetAdapter(_adapter);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private async void Adapter_ItemDeleteClick(object sender, Guid e)
        {
            //Guid id = _tasks[e].Id;

            await _taskDataHelper.MarkAsDeletedAsync(e);

            _ = UpdateServerAsync(e); // warning suppressed on purpose
        }

        private Task UpdateServerAsync(Guid id)
        {
            return _taskWebHelper.DeleteScheduledTaskAsync(id)
                .ContinueWith(t =>
                {
                    if (t.Result.IsSuccessStatusCode)
                    {
                        _taskDataHelper.DeleteAsync(id);
                    }
                },
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void FindViews()
        {
            createButton = FindViewById<FloatingActionButton>(Resource.Id.tasksView_CreateButton);
        }

        private void HandleEvents()
        {
            createButton.Click += CreateButton_Click;
        }

        private void CreateButton_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(CreateTaskActivity));
        }
    }
}