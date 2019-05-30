namespace Planner.Mobile.Core
{
    public class CommonUrls
    {
#if DEBUG
        public const string BASE_URI = "http://4c47034c.ngrok.io/api/";
#else
        public const string BASE_URI = "https://myplannerapi.azurewebsites.net/api/";
#endif
    }

    public class PreferenceKeys
    {
        public const string USER_INFO = "userinfo";
    }

    public class PreferenceItemKeys
    {
        public const string TOKEN = "token";
        public const string USER_ID = "user_id";
        public const string USERNAME = "user_name";
        public const string FIREBASE_REG_TOKEN = "fire_token";
    }
}
