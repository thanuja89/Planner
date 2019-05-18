using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Planner.Droid.Controls;
using Planner.Droid.Helpers;
using Planner.Droid.Services;
using Planner.Mobile.Core;
using Planner.Mobile.Core.Data;
using Planner.Mobile.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using V7 = Android.Support.V7.Widget;

namespace Planner.Droid.Activities
{
    [Activity(Label = "TasksActivity")]
    public class TasksActivity : AppCompatActivity
    {
        private string _userId;
        private RecyclerView recyclerView;
        private readonly RecyclerView.LayoutManager _layoutManager;
        private List<ScheduledTask> _tasks;
        private TaskViewAdapter _adapter;
        private RelativeLayout layout;
        private FloatingActionButton createButton;
        private TextView emptyTextView;
        private V7.SearchView searchView;
        private V7.Toolbar toolbar;
        private View toolbarLayout;
        private Button signoutButton;
        private ProgressBarHelper _progressBarHelper;
        private readonly ScheduledTaskDataHelper _taskDataHelper;
        private readonly DialogHelper _dialogHelper;

        public TasksActivity()
        {
            _taskDataHelper = new ScheduledTaskDataHelper();
            _dialogHelper = new DialogHelper();
            _layoutManager = new LinearLayoutManager(this);
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_tasks);

            PrepareToolbar();

            FindViews();
            HandleEvents();

            await PrepareRecyclerViewAsync();

            _progressBarHelper = new ProgressBarHelper(this, Window, layout);
        }

        private void PrepareToolbar()
        {
            toolbar = FindViewById<V7.Toolbar>(Resource.Id.tasks_toolbar);
            SetSupportActionBar(toolbar);

            toolbarLayout = LayoutInflater.Inflate(Resource.Layout.main_action_bar, null);

            signoutButton = toolbarLayout.FindViewById<Button>(Resource.Id.mainActionBar_SignoutButton);

            toolbar.AddView(toolbarLayout);
        }

        private async Task PrepareRecyclerViewAsync()
        {
            try
            {
                _userId = Helpers.Utilities.GetUserId();

                recyclerView = FindViewById<RecyclerView>(Resource.Id.tasks_RecyclerView);
                recyclerView.SetLayoutManager(_layoutManager);
                recyclerView.HasFixedSize = true;

                _tasks = await _taskDataHelper.GetAsync(_userId);

                _adapter = new TaskViewAdapter(_tasks);
                _adapter.ItemDeleteClick += Adapter_ItemDeleteClick;
                _adapter.ItemClick += Adapter_ItemClick;

                recyclerView.SetAdapter(_adapter);

                ToggleEmptyView();
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
            }
        }

        private void Adapter_ItemClick(object sender, ScheduledTask e)
        {
            var intent = new Intent(this, typeof(EditTaskActivity));
            intent.PutExtra("TaskId", e.Id.ToString());

            StartActivity(intent);
        }

        private async void Adapter_ItemDeleteClick(object sender, ScheduledTask e)
        {
            try
            {
                _progressBarHelper.Show();

                ToggleEmptyView();

                await Task.Yield();

                await _taskDataHelper.MarkAsDeletedAsync(e.Id);

                _ = SyncService.Instance.SyncAsync(this)
                    .ContinueWith(async t => 
                    {
                        try
                        {
                            await _taskDataHelper.DeleteAsync(e.Id);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);
                        }
                    }
                    , TaskContinuationOptions.OnlyOnRanToCompletion); // warning suppressed on purpose

                _progressBarHelper.Hide();
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogPriority.Error, "Planner Error", ex.Message);

                _progressBarHelper.Hide();
                _dialogHelper.ShowError(this, ex);
            }
        }

        private void FindViews()
        {
            layout = FindViewById<RelativeLayout>(Resource.Id.tasks_Layout);
            createButton = FindViewById<FloatingActionButton>(Resource.Id.tasks_CreateButton);
            emptyTextView = FindViewById<TextView>(Resource.Id.tasksView_EmptyText);
            searchView = FindViewById<V7.SearchView>(Resource.Id.tasks_SearchView);
        }

        private void HandleEvents()
        {
            createButton.Click += CreateButton_Click;
            signoutButton.Click += SignoutButton_Click;
            searchView.QueryTextChange += SearchView_QueryTextChange;
        }

        private async void SearchView_QueryTextChange(object sender, V7.SearchView.QueryTextChangeEventArgs e)
        {
            var filteredItems = await _taskDataHelper.SearchAsync(_userId, e.NewText);

            _tasks.Clear();

            _tasks.AddRange(filteredItems);

            _adapter.NotifyDataSetChanged();
        }

        private void SignoutButton_Click(object sender, EventArgs e)
        {
            RemoveUserInfo();

            StartActivity(typeof(SignInActivity));
        }

        private void CreateButton_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(CreateTaskActivity));
        }

        private void RemoveUserInfo()
        {
            var pref = Application.Context
                .GetSharedPreferences(PreferenceKeys.USER_INFO, FileCreationMode.Private);

            var editor = pref.Edit();

            editor.Remove(PreferenceItemKeys.TOKEN);
            editor.Remove(PreferenceItemKeys.USER_ID);
            editor.Remove(PreferenceItemKeys.USERNAME);

            editor.Apply();
        }

        private void ToggleEmptyView()
        {
            if (_tasks == null || _tasks.Count == 0)
            {
                recyclerView.Visibility = ViewStates.Gone;
                emptyTextView.Visibility = ViewStates.Visible;
            }
            else
            {
                recyclerView.Visibility = ViewStates.Visible;
                emptyTextView.Visibility = ViewStates.Gone;
            }
        }
    }
}