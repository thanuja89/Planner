using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.DefaultRequestHeaders
                .Authorization = new AuthenticationHeaderValue("bearer"
                    , "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy" +
                    "8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjlhZGY1YTA3LTQ3ZTUtNGRjZS1hMDdhL" +
                    "Tg3YWMyMTRiZTM5NiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL" +
                    "25hbWUiOiJ0aGFudWphODl4IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9j" +
                    "bGFpbXMvZW1haWxhZGRyZXNzIjoidGhhbnVqYUBnbWFpbC5jb20iLCJleHAiOjE1NTMzMTkzNzAsImlzcyI6Imh0dHA" +
                    "6Ly9sb2NhbGhvc3Q6NDQzNjMvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo0NDM2My8ifQ.4LAqhlLrUHAjLGpO7w0YsrZ4cdALG3kaAffVJIjB1Vw");
        }

        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ CommonUrls.BASE_URI }{ url}");

                if (response.IsSuccessStatusCode)
                {
                    return await DeserializeResponseAsync<T>(response);
                }

                return default;
            }
            catch (System.Exception ex)
            {

                throw;
            }
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
