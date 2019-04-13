using Microsoft.AspNetCore.Identity;
using Planner.Domain.Entities;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class NumericTokenProvider : IUserTwoFactorTokenProvider<ApplicationUser>
    {
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            var token = await manager.CreateSecurityTokenAsync(user);

            var modifier = await GetUserModifierAsync(purpose, manager, user);

            return Rfc6238AuthenticationService.GenerateCode(new SecurityToken(token)
                , modifier).ToString("D6", CultureInfo.InvariantCulture);
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (!int.TryParse(token, out int code))
            {
                return false;
            }

            var securityToken = await manager.CreateSecurityTokenAsync(user);
            var modifier = await GetUserModifierAsync(purpose, manager, user);

            return securityToken != null && Rfc6238AuthenticationService.ValidateCode(new SecurityToken(securityToken)
                , code, modifier);
        }

        public async Task<string> GetUserModifierAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            var userId = await manager.GetUserIdAsync(user);

            return "Totp:" + purpose + ":" + userId;
        }

    }
}
