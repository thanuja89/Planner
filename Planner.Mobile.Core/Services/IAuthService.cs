using System.Threading.Tasks;
using Planner.Dto;

namespace Planner.Mobile.Core.Services
{
    public interface IAuthService
    {
        Task<TokenDto> SignIn(LoginDto loginDto);
    }
}