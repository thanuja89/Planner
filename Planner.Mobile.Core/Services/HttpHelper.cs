using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class HttpHelper
    {
        private readonly HttpClient _httpClient;

        public HttpHelper()
        {
            _httpClient = new HttpClient();
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync($"{ CommonUrls.BASE_URI }{ url}");

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

            return _httpClient.PostAsync($"{ CommonUrls.BASE_URI }{ url}", content);
        }

        public Task<HttpResponseMessage> PutAsync(string url, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return _httpClient.PutAsync($"{ CommonUrls.BASE_URI }{ url}", content);
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
            return _httpClient.DeleteAsync($"{ CommonUrls.BASE_URI }{ url}");
        }

        private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage httpResponse)
        {
            var cont = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(cont);
        }
    }
}
