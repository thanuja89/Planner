using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Services
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<T> PostForResultAsync<T>(string url, object obj)
        {
            var response = await Post(url, obj);

            T result = default;

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponseAsync<T>(response);
            }

            return result;
        }

        public async Task<T> PostForResultAsync<T>(string url, object obj, bool isDeserializeUnsuccesful)
        {
            var response = await Post(url, obj);

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
            var response = await Post(url, obj);

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        public Task<HttpResponseMessage> Post(string url, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return _httpClient.PostAsync($"{ CommonUrls.BASE_URI }{ url}", content);
        }

        private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage httpResponse)
        {
            var cont = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(cont);
        }
    }
}
