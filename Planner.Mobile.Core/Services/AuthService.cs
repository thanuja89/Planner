using Planner.Dto;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Services
{
    public class AuthService
    {
        private readonly HttpService _httpService;

        public AuthService()
        {
            _httpService = new HttpService();
        }

        public Task<TokenDto> SignInAsync(TokenRequestDto loginDto)
        {
            return _httpService.PostForResultAsync<TokenDto>("Auth/CreateToken", loginDto);
        }

        public Task SignUpAsync(CreateAccountDto accountDto)
        {
            return _httpService.PostAsync("Auth/CreateAccount", accountDto);
        }
    }
}
