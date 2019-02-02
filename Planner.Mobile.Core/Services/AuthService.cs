using Newtonsoft.Json;
using Planner.Dto;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<TokenDto> SignIn(LoginDto loginDto)
        {
            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{ Common.BASE_URI }Token/CreateToken";

            var response = await _httpClient.PostAsync(url, content);

            TokenDto result = null;

            if (response.IsSuccessStatusCode)
            {
                var cont = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<TokenDto>(cont);
            }

            return result;
        } 
    }
}
