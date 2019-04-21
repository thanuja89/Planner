using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Planner.Api.Services;
using Planner.Domain.Entities;
using Planner.Dto;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IEmailSender _emailSender;

        public AuthenticationController(IConfiguration config
            , SignInManager<ApplicationUser> signInManager
            , UserManager<ApplicationUser> userManager
            , ILogger<AuthenticationController> logger
            , IEmailSender emailSender)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
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
                _logger.LogError("Threw exception while creating Token: {@ex}", ex);
                return new StatusCodeResult(500);
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
                        await SendConfirmationEmaiAsync(appUser);

                        return Ok(new SignUpResultDTO()
                        {
                            Succeeded = true,
                            UserId = appUser.Id
                        });
                    }

                    var user = await _userManager.FindByNameAsync(register.Username);

                    if (user != null)
                    {
                        if (user.Email == register.Email)
                        {
                            await SendConfirmationEmaiAsync(appUser);

                            return Ok(new SignUpResultDTO()
                            {
                                Succeeded = true,
                                UserId = user.Id
                            });
                        }
                        else
                        {
                            return BadRequest(new SignUpResultDTO()
                            {
                                Succeeded = false,
                                ErrorType = SignUpErrorType.UsernameExists
                            });
                        }
                    }

                    if ((await _userManager.FindByEmailAsync(register.Email)) != null)
                        return BadRequest(new SignUpResultDTO()
                        {
                            Succeeded = false,
                            ErrorType = SignUpErrorType.EmailExists
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Threw exception while creating User: {@ex}", ex);

                return StatusCode((int)HttpStatusCode.InternalServerError, new SignUpResultDTO()
                {
                    Succeeded = false,
                    ErrorType = SignUpErrorType.ServerError
                });
            }

            return BadRequest(new SignUpResultDTO()
            {
                Succeeded = false,
                ErrorType = SignUpErrorType.Other
            });
        }

        [HttpPost("{action}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmationRequestDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(dto.UserId);

                    if (user == null)
                        return BadRequest();

                    var result = await _userManager.ConfirmEmailAsync(user, dto.Code);

                    if (result.Succeeded)
                    {
                        return Ok(); 
                    }
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("Threw exception while confirming email for user: {@ex}", ex);
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("{action}/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationEmail(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return BadRequest();

                await SendConfirmationEmaiAsync(user);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Threw exception while confirming emai for user: {@ex}", ex);
                return new StatusCodeResult(500);
            }
        }

# if DEBUG
        [AllowAnonymous]
        [HttpGet("{action}")]
        public IActionResult Test()
        {
            try
            {
                throw null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Test ex {@ex}", ex);
                return StatusCode(500);
            }
        }
#endif

        #region Private Methods
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

        private async Task SendConfirmationEmaiAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailSender.SendEmailAsync(user.Email, "Email Confirmation Code", $"Your Confirmation Code is {token}");
        }

        #endregion
    }
}
