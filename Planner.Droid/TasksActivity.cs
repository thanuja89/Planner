using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Planner.Droid.Controls;
using Planner.Mobile.Core.Services;
using System.Threading.Tasks;

namespace Planner.Droid
{
    [Activity(Label = "TasksActivity", MainLauncher = true)]
    public class TasksActivity : AppCompatActivity
    {
        private RecyclerView recyclerView;
        private readonly RecyclerView.LayoutManager _layoutManager;
        private TaskViewAdapter _adapter;
        private FloatingActionButton createButton;
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

                var tasks = await _taskDataService.GetAsync();

                _adapter = new TaskViewAdapter(tasks);
                recyclerView.SetAdapter(_adapter);
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
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