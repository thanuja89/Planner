
using Android.App;
using Android.Content;
using Android.OS;

namespace Planner.Droid.Extensions.Services
{
    [Service(Name = "Planner.Droid.Planner.Droid.SyncService", Exported = true)]
    [IntentFilter(actions: new string[] { "android.content.SyncAdapter" })]
    [MetaData(name: "android.content.SyncAdapter", Resource = "@xml/sync_adapter")]
    public class SyncService : Service
    {
        private readonly object _syncLock = new object();
        private SyncAdapter _syncAdapter;

        public override void OnCreate()
        {
            base.OnCreate();

            lock (_syncLock)
            {
                _syncAdapter = new SyncAdapter(ApplicationContext, true);
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return _syncAdapter?.SyncAdapterBinder;
        }
    }
}