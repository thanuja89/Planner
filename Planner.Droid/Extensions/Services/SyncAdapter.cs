using Android.Accounts;
using Android.Content;
using Android.OS;
using Android.Util;
using Planner.Mobile.Core.Services;
using System;

namespace Planner.Droid.Extensions.Services
{
    public class SyncAdapter : AbstractThreadedSyncAdapter
    {
        private readonly AuthService _auth;

        public SyncAdapter(Context context, bool autoInitialize, bool allowParallelSyncs = false) : base(context, autoInitialize, allowParallelSyncs)
        {
            _auth = new AuthService();
        }

        public async override void OnPerformSync(Account account, Bundle extras, string authority, ContentProviderClient provider, SyncResult syncResult)
        {
            Log.Info("Sync", "Called");
            await _auth.SignInAsync(new Dto.TokenRequestDto()
            {
                Username = "thanuja",
                Password = "password"
            });
        }
    }
}