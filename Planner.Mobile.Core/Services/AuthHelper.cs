using Planner.Dto;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Helpers
{
    public class AuthHelper
    {
        public Task<TokenDto> SignInAsync(TokenRequestDto loginDto)
        {
            return HttpHelper.Instance.PostForResultAsync<TokenDto>("Auth/CreateToken", loginDto);
        }

        public Task<SignUpResultDTO> SignUpAsync(CreateAccountDto accountDto)
        {
            return HttpHelper.Instance.PostForResultAsync<SignUpResultDTO>("Auth/CreateAccount", accountDto, true);
        }
    }
}
