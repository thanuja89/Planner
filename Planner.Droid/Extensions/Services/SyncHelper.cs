using Android.Accounts;
using Android.Content;
using Android.OS;
using System.Linq;
using static Android.Content.ContentResolver;

namespace Planner.Droid.Extensions.Services
{
    public static class SyncHelper
    {
        public static Account GetAccount(Context context)
        {
            AccountManager accountManager = (AccountManager)context.GetSystemService(Context.AccountService);

            var account = accountManager
                .GetAccountsByType(AppConstants.SYNC_ADAPTER_ACCOUNT_TYPE)
                .FirstOrDefault();

            if (account != null)
                return account;

            var syncAccount = new Account(AppConstants.SYNC_ADAPTER_ACCOUNT
                , AppConstants.SYNC_ADAPTER_ACCOUNT_TYPE);

            accountManager.AddAccountExplicitly(syncAccount, null, null);

            return syncAccount;
        }

        public static void SetSyncConfig(Account account)
        {
            SetIsSyncable(account, AppConstants.SYNC_ADAPTER_AUTHORITY, 1);
            SetSyncAutomatically(account, AppConstants.SYNC_ADAPTER_AUTHORITY, true);
            AddPeriodicSync(account, AppConstants.SYNC_ADAPTER_AUTHORITY, Bundle.Empty, 2);
        }

        public static void RequestSync()
        {
            Bundle b = new Bundle();
            // Disable sync backoff and ignore sync preferences. In other words...perform sync NOW!
            b.PutBoolean(SyncExtrasManual, true);
            b.PutBoolean(SyncExtrasExpedited, true);
            ContentResolver.RequestSync(
                    GenericAccountService.GetAccount(AppConstants.SYNC_ADAPTER_ACCOUNT_TYPE),
                    AppConstants.SYNC_ADAPTER_AUTHORITY,           
                    b);
        }
    }
}