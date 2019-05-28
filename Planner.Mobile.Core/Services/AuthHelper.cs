using Planner.Dto;
using System.Net.Http;
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

        public Task<TokenCreationResultDto> SignInAsync(TokenRequestDto loginDto)
        {
            return _httpService.PostForResultAsync<TokenCreationResultDto>("Auth/CreateToken", loginDto, true);
        }

        public Task<TokenDto> ExternalSignInAsync(ExternalSignInDto loginDto)
        {
            return _httpService.PostForResultAsync<TokenDto>("Auth/ExternalSignIn", loginDto);
        }

        public Task<AccountCreationResultDto> SignUpAsync(CreateAccountDto accountDto)
        {
            return _httpService.PostForResultAsync<AccountCreationResultDto>("Auth/CreateAccount", accountDto, true);
        }

        public Task<HttpResponseMessage> ConfirmEmailAsync(ConfirmationRequestDto requestDto)
        {
            return _httpService.PostAsync("Auth/ConfirmEmail", requestDto);
        }

        public Task<HttpResponseMessage> ResendConfirmationEmailAsync(string userId)
        {
            return _httpService.PostAsync($"Auth/ResendConfirmationEmail/{userId}");
        }

        public Task<HttpResponseMessage> SendPasswordResetEmailAsync(SendPasswordResetEmailDto dto)
        {
            return _httpService.PostAsync($"Auth/SendPasswordResetEmail", dto);
        }

        public Task<HttpResponseMessage> ResetPasswordAsync(ResetPasswordRequestDto requestDto)
        {
            return _httpService.PostAsync("Auth/ResetPassword", requestDto);
        }

        public Task<HttpResponseMessage> RegisterDeviceAsync(string id)
        {
            return _httpService.PostAsync($"Auth/RegisterDevice/{id}");
        }
    }
}
