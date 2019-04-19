using Planner.Dto;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class AuthHelper
    {
        private readonly HttpHelper _httpService;

        public AuthHelper()
        {
            _httpService = new HttpHelper();
        }

        public Task<TokenDto> SignInAsync(TokenRequestDto loginDto)
        {
            return _httpService.PostForResultAsync<TokenDto>("Auth/CreateToken", loginDto);
        }

        public Task<SignUpResultDTO> SignUpAsync(CreateAccountDto accountDto)
        {
            return _httpService.PostForResultAsync<SignUpResultDTO>("Auth/CreateAccount", accountDto, true);
        }
    }
}
