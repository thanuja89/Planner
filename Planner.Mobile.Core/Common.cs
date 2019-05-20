﻿namespace Planner.Mobile.Core
{
    public class CommonUrls
    {
#if DEBUG
        public const string BASE_URI = "http://2e75a4f5.ngrok.io/api/";
#else
        public const string BASE_URI = "https://myplannerapi.azurewebsites.net";
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
    }
}
