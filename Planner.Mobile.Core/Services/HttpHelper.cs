using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class HttpHelper
    {
        private HttpClient _httpClient;

        private static HttpHelper _helper;

        public HttpHelper() => InitClient();

        public HttpHelper(string token) => InitClient(token);

        public static HttpHelper Instance
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new HttpHelper();    
                }

                return _helper;
            }
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponseAsync<T>(response);
            }

            return default;
        }

        public async Task<T> PostForResultAsync<T>(string url, object obj)
        {
            var response = await PostAsync(url, obj);

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponseAsync<T>(response);
            }

            return default;
        }

        public async Task<T> PostForResultAsync<T>(string url, object obj, bool isDeserializeUnsuccesful)
        {
            var response = await PostAsync(url, obj);

            if (isDeserializeUnsuccesful)
                return await DeserializeResponseAsync<T>(response);

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponseAsync<T>(response);
            }

            return default;
        }

        public async Task<bool> PostForSuccessResultAsync(string url, object obj)
        {
            var response = await PostAsync(url, obj);

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        public Task<HttpResponseMessage> PostAsync(string url, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return _httpClient.PostAsync(url, content);
        }

        public Task<HttpResponseMessage> PutAsync(string url, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return _httpClient.PutAsync(url, content);
        }

        public async Task<T> PutForResultAsync<T>(string url, object obj)
        {
            var response = await PutAsync(url, obj);

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponseAsync<T>(response);
            }

            return default;
        }

        public Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return _httpClient.DeleteAsync(url);
        }

        private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage httpResponse)
        {
            var cont = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(cont);
        }

        private void InitClient(string token = null)
        {
            var handler = new HttpClientHandler()
            {
                UseProxy = false
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(CommonUrls.BASE_URI)
            };

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }
        }
    }
}
