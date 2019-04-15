using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class AuthenticatedHttpHelper
    {
        private static AuthenticatedHttpHelper _instance;

        private readonly HttpHelper _helper;

        public static AuthenticatedHttpHelper Instance
            => _instance ?? throw new InvalidOperationException("Helper not intialized.");

        public static bool IsInitialized => _instance != null;

        public static void Init(string token) => _instance = new AuthenticatedHttpHelper(token);

        private AuthenticatedHttpHelper(string token)
        {
            _helper = new HttpHelper(token);
        }

        public Task<T> GetAsync<T>(string url)
        {
            return _helper.GetAsync<T>(url);
        }

        public Task<HttpResponseMessage> PostAsync(string url, object obj)
        {
            return _helper.PostAsync(url, obj);
        }

        public Task<HttpResponseMessage> PutAsync(string url, object obj)
        {          
            return _helper.PutAsync(url, obj);
        }

        public Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return _helper.DeleteAsync(url);
        }
    }
}
