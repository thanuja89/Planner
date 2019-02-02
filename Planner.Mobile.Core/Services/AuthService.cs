using Newtonsoft.Json;
using Planner.Dto;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<TokenDto> SignInAsync(TokenRequestDto loginDto)
        {
            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"{ CommonUrls.BASE_URI }Auth/CreateToken";

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
