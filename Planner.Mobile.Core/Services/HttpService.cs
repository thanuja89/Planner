using Newtonsoft.Json;
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
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{ CommonUrls.BASE_URI }{ url}", content);

            T result = default;

            if (response.IsSuccessStatusCode)
            {
                var cont = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<T>(cont);
            }

            return result;
        }

        public async Task PostAsync(string url, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync($"{ CommonUrls.BASE_URI }{ url}", content);
        }
    }
}
