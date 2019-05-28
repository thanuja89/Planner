using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Planner.Api.Extensions;
using Planner.Api.Services;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
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
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationController(IConfiguration config
            , SignInManager<ApplicationUser> signInManager
            , UserManager<ApplicationUser> userManager
            , ILogger<AuthenticationController> logger
            , IEmailSender emailSender
            , IUnitOfWork unitOfWork)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpPost("{action}")]
        public async Task<IActionResult> CreateToken([FromBody]TokenRequestDto login)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(TokenCreationResultDto.Failed(TokenCreationErrorType.InvalidUsernameOrPassword));

                var user = await _userManager.FindByNameAsync(login.Username);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);

                    if (result.Succeeded)
                    {
                        var tokenString = BuildToken(user);
                        return Ok(TokenCreationResultDto.Success(new TokenDto()
                        {
                            Value = tokenString,
                            ApplicationUserId = user.Id,
                            Username = user.UserName
                        }));
                    }

                    if (!user.EmailConfirmed)
                    {
                        return BadRequest(TokenCreationResultDto.Failed(TokenCreationErrorType.EmailNotConfirmed, user.Id));
                    }
                }

                return BadRequest(TokenCreationResultDto.Failed(TokenCreationErrorType.InvalidUsernameOrPassword));

            }
            catch (Exception ex)
            {
                _logger.LogError("Threw exception while creating Token: {@ex}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, TokenCreationResultDto.Failed(TokenCreationErrorType.ServerError));
            }
        }

        [AllowAnonymous]
        [HttpPost("{action}")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountDto register)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(AccountCreationResultDto.Failed(AccountCreationErrorType.Other));

                var userForEmail = await _userManager.FindByEmailAsync(register.Email);

                if (userForEmail != null)
                {
                    // An user exists for the email
                    if (!userForEmail.EmailConfirmed)
                    {
                        // The existing user's email is not confirmed, so delete the user.
                        await _userManager.DeleteAsync(userForEmail);
                    }
                    else
                    {
                        return BadRequest(AccountCreationResultDto.Failed(AccountCreationErrorType.EmailExists));
                    }
                }

                var appUser = new ApplicationUser()
                {
                    UserName = register.Username,
                    Email = register.Email
                };

                var result = await _userManager.CreateAsync(appUser, register.Password);

                if (result.Succeeded)
                {
                    await SendConfirmationEmaiAsync(appUser);

                    return Ok(AccountCreationResultDto.Success(appUser.Id));
                }

                var userForUsername = await _userManager.FindByNameAsync(register.Username);

                if (userForUsername != null)
                {
                    //Username exists 
                    return BadRequest(AccountCreationResultDto.Failed(AccountCreationErrorType.UsernameExists));
                }

                return BadRequest(AccountCreationResultDto.Failed(AccountCreationErrorType.Other));
            }
            catch (Exception ex)
            {
                _logger.LogError("Threw exception while creating User: {@ex}", ex);

                return StatusCode((int)HttpStatusCode.InternalServerError, AccountCreationResultDto.Failed(AccountCreationErrorType.ServerError));
            }
        }

        [AllowAnonymous]
        [HttpPost("{action}")]
        public async Task<IActionResult> ExternalSignIn([FromBody]ExternalSignInDto register)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ApplicationUser user;

                user = await _userManager.FindByEmailAsync(register.Email);

                if (user == null)
                {
                    user = ApplicationUser.NewExternalUser(register.Email);

                    var result = await _userManager.CreateAsync(user, GetRandomString());

                    if (!result.Succeeded)
                        return BadRequest();
                }
                else
                {
                    if (!await _signInManager.CanSignInAsync(user))
                    {                      
                        await _userManager.DeleteAsync(user);

                        var newUser = ApplicationUser.NewExternalUser(register.Email);
                        await _userManager.CreateAsync(newUser, GetRandomString());

                        return OkWithToken(newUser);
                    }
                }

                return OkWithToken(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Threw exception while Signing In External User: {@ex}", ex);

                return new StatusCodeResult(500);
            }
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
                _logger.LogError("Threw exception while resending confirmation email for user: {@ex}", ex);
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("{action}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendPasswordResetEmail([FromBody] SendPasswordResetEmailDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var user = await _userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                    return BadRequest();

                await SendPasswordResetEmaiAsync(user);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Threw exception while sending password reset code for user: {@ex}", ex);
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("{action}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(dto.Email);

                    if (user == null)
                        return BadRequest();

                    var result = await _userManager.ResetPasswordAsync(user, dto.Code, dto.Password);

                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("Threw exception while resetting password for user: {@ex}", ex);
                return new StatusCodeResult(500);
            }
        }

        private async Task SendPasswordResetEmaiAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSender.SendEmailAsync(user.Email, "Password Reset Code", $"Your Password Reset Code is {token}");
        }

#if DEBUG
        [AllowAnonymous]
        [HttpGet("{action}")]
        public async Task<IActionResult> Test()
        {
            try
            {
                await _emailSender.SendEmailAsync("thanujadilhan@gmail.com", "Test", "This is a test email.");
                return Ok();
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

        private async Task SendConfirmationEmaiAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailSender.SendEmailAsync(user.Email, "Email Confirmation Code", $"Your Confirmation Code is {token}");
        }

        private string GetRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        private IActionResult OkWithToken(ApplicationUser user)
        {
            string token = BuildToken(user);
            return Ok(new TokenDto()
            {
                Value = token,
                ApplicationUserId = user.Id,
                Username = user.UserName
            });
        }

        #endregion
    }
}
