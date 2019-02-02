using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Planner.Domain.Entities;
using Planner.Dto;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config
            , SignInManager<ApplicationUser> signInManager
            , UserManager<ApplicationUser> userManager
            , ILogger<AuthController> logger)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("{action}")]
        public async Task<IActionResult> CreateToken([FromBody]TokenRequestDto login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await AuthenticateAsync(login);

                    if (user != null)
                    {
                        var tokenString = BuildToken(user);
                        return Ok(new TokenDto()
                        {
                            Token = tokenString
                        });
                    } 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while creating Token: { ex }");
                throw;
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("{action}")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountDto register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var appUser = new ApplicationUser()
                    {
                        UserName = register.Username,
                        Email = register.Email
                    };

                    var result = await _userManager.CreateAsync(appUser, register.Password);

                    if (result.Succeeded)
                    {
                        return Ok();
                    } 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception while creating User: { ex }");
                throw;
            }

            return BadRequest();
        }

        private string BuildToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ApplicationUser> AuthenticateAsync(TokenRequestDto login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, false, false);

            if (result.Succeeded)
            {
                return await _userManager.FindByNameAsync(login.Username);
            }
            
            return null;
        }
    }
}
