using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Planner.Android.Controls;
using Planner.Mobile.Core.Services;
using System.Threading.Tasks;

namespace Planner.Android
{
    [Activity(Label = "TasksActivity", MainLauncher = true)]
    public class TasksActivity : Activity
    {
        private RecyclerView recyclerView;
        private readonly RecyclerView.LayoutManager _layoutManager;
        private TaskViewAdapter _adapter;

        private readonly ScheduledTaskDataService _taskDataService;

        public TasksActivity()
        {
            _taskDataService = new ScheduledTaskDataService();
            _layoutManager = new LinearLayoutManager(this);
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TasksView);

            await PrepareRecyclerViewAsync();
        }

        private async Task PrepareRecyclerViewAsync()
        {
            try
            {
                recyclerView = FindViewById<RecyclerView>(Resource.Id.tasksView_RecyclerView);
                recyclerView.SetLayoutManager(_layoutManager);

                var tasks = await _taskDataService.GetAsync();

                _adapter = new TaskViewAdapter(tasks);
                recyclerView.SetAdapter(_adapter);
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
    }
}