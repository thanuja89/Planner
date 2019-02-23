using Android.Accounts;
using Android.App;
using Android.Content;
using Android.OS;
using System;

namespace Planner.Droid.Extensions.Services
{
    [Service(Name = "Planner.Droid.Planner.Droid.GenericAccountService")]
    [IntentFilter(actions: new string[] { "android.accounts.AccountAuthenticator" })]
    [MetaData(name: "android.accounts.AccountAuthenticator", Resource = "@xml/authenticator")]
    public class GenericAccountService : Service
    {
        private Authenticator _authenticator;

        public static Account GetAccount(string accountType)
        {
            return new Account(AppConstants.SYNC_ADAPTER_ACCOUNT, AppConstants.SYNC_ADAPTER_ACCOUNT_TYPE);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _authenticator = new Authenticator(this);
        }

        public override IBinder OnBind(Intent intent)
        {
            return _authenticator.IBinder;
        }

        public class Authenticator : AbstractAccountAuthenticator
        {
            public Authenticator(Context context) : base(context)
            {
            }

            public override Bundle AddAccount(AccountAuthenticatorResponse response, string accountType, string authTokenType, string[] requiredFeatures, Bundle options)
            {
                return null;
            }

            public override Bundle ConfirmCredentials(AccountAuthenticatorResponse response, Account account, Bundle options)
            {
                return null;
            }

            public override Bundle EditProperties(AccountAuthenticatorResponse response, string accountType)
            {
                throw new NotImplementedException();
            }

            public override Bundle GetAuthToken(AccountAuthenticatorResponse response, Account account, string authTokenType, Bundle options)
            {
                throw new NotImplementedException();
            }

            public override string GetAuthTokenLabel(string authTokenType)
            {
                throw new NotImplementedException();
            }

            public override Bundle HasFeatures(AccountAuthenticatorResponse response, Account account, string[] features)
            {
                throw new NotImplementedException();
            }

            public override Bundle UpdateCredentials(AccountAuthenticatorResponse response, Account account, string authTokenType, Bundle options)
            {
                throw new NotImplementedException();
            }
        }
    }
}