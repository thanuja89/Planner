using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Planner.Api.Controllers;
using Planner.Api.Services;
using Planner.Domain.Entities;
using Planner.Dto;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Planner.Api.Tests
{
    public class AuthenticationControllerTests
    {
        private ApplicationUser _user;
        private ApplicationUser _userWithNotConfirmedEmail;
        private Mock<ILogger<AuthenticationController>> _mockLogger;
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IConfiguration> _mockConfiguration;

        private AuthenticationController _sut;

        #region CreateToken Method Tests

        [Fact]
        public async Task CreateToken_WhenCalledWithValidArgs_ReturnsOkWithToken()
        {
            // Arrange
            SetUp();

            var login = new TokenRequestDto() { Username = "AAAAAA", Password = "AAAAAAA" };

            _mockSignInManager
                .Setup(m => m.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(_user);

            // Act
            var result = await _sut.CreateToken(login);

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var cResult = Assert.IsAssignableFrom<TokenCreationResultDto>(okResult.Value);

            Assert.NotNull(cResult);
            Assert.NotNull(cResult.Token);
            Assert.NotNull(cResult.Token.Value);
        }

        [Fact]
        public async Task CreateToken_WhenCalledWithInvalidCreds_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            var login = new TokenRequestDto();

            _mockSignInManager
                .Setup(m => m.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _sut.CreateToken(login);

            // Assert
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateToken_WhenEmailNotConfirmed_ReturnsBadRequestWithUserId()
        {
            // Arrange
            SetUp();

            var login = new TokenRequestDto();

            _mockSignInManager
                .Setup(m => m.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(_userWithNotConfirmedEmail);

            // Act
            var result = await _sut.CreateToken(login);

            // Assert
            var brResult = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            var cResult = Assert.IsAssignableFrom<TokenCreationResultDto>(brResult.Value);
            Assert.Equal(_userWithNotConfirmedEmail.Id, cResult.ValidationData);
        }

        #endregion

        #region CreateAccount Method Tests

        [Fact]
        public async Task CreateAccount_WhenCalledWithValidArgs_CallsEmailSenderAndReturnsOkWithSuccess()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto();
            string code = "0";

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(code);

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var dto = Assert.IsAssignableFrom<AccountCreationResultDto>(okResult.Value);
            Assert.True(dto.Succeeded);
        }

        [Fact]
        public async Task CreateAccount_WhenUsernameExistsForAnotherEmail_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto()
            {
                Email = "aaa@aa.com",
                Username = _user.UserName
            };

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(_user);

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var badRequestResult = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            var dto = Assert.IsAssignableFrom<AccountCreationResultDto>(badRequestResult.Value);
            Assert.False(dto.Succeeded);
        }

        [Fact]
        public async Task CreateAccount_WhenUsernameExistsForSameEmail_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto()
            {
                Email = _user.Email,
                Username = _user.UserName
            };

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(_user);

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var okResult = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            var dto = Assert.IsAssignableFrom<AccountCreationResultDto>(okResult.Value);
            Assert.False(dto.Succeeded);
        }

        [Fact]
        public async Task CreateAccount_WhenEmailExistsForDifferentUsername_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto()
            {
                Email = _user.Email,
                Username = "AAAAAAA"
            };

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(null as ApplicationUser);
            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(_user);

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var badRequestResult = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            var dto = Assert.IsAssignableFrom<AccountCreationResultDto>(badRequestResult.Value);
            Assert.False(dto.Succeeded);
        }

        [Fact]
        public async Task CreateAccount_WhenExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto();

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var internalErrorResult = Assert.IsAssignableFrom<ObjectResult>(result);

            Assert.Equal(internalErrorResult.StatusCode, (int?) HttpStatusCode.InternalServerError);

            var dto = Assert.IsAssignableFrom<AccountCreationResultDto>(internalErrorResult.Value);

            Assert.False(dto.Succeeded);
        }

        #endregion

        #region ExternalSignIn Method Tests

        [Fact]
        public async Task ExternalSignIn_WhenEmailNotExists_CallsCreateAndReturnsOkWithToken()
        {
            // Arrange
            SetUp();

            var login = new ExternalSignInDto() { Email = "aaa@aaa.com" };

            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(null as ApplicationUser);

            _mockSignInManager
                .Setup(m => m.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, s) => 
                {
                    u.Id = "AAAAAAAA";
                    u.Email = "aaa@aaa.com";
                    u.UserName = "aaaaaaaaa";
                })
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.ExternalSignIn(login);

            // Assert

            _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));

            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var token = Assert.IsAssignableFrom<TokenDto>(okResult.Value);

            Assert.NotNull(token);
            Assert.NotNull(token.Value);
        }

        [Fact]
        public async Task ExternalSignIn_WhenEmailExists_ReturnsOkWithToken()
        {
            // Arrange
            SetUp();

            var login = new ExternalSignInDto() { Email = "aaa@aaa.com" };

            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(_user);
            _mockSignInManager.Setup(m => m.CanSignInAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(true);

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, s) =>
                {
                    u.Id = "AAAAAAAA";
                })
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.ExternalSignIn(login);

            // Assert

            _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
            _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);

            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var token = Assert.IsAssignableFrom<TokenDto>(okResult.Value);

            Assert.NotNull(token);
            Assert.NotNull(token.Value);
        }

        [Fact]
        public async Task ExternalSignIn_WhenEmailExistsAndUserNotActive_CallsCreateDeleteAndReturnsOkWithToken()
        {
            // Arrange
            SetUp();

            var login = new ExternalSignInDto() { Email = "aaa@aaa.com" };

            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(_userWithNotConfirmedEmail);
            _mockSignInManager.Setup(m => m.CanSignInAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(false);

            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, s) =>
                {
                    u.Id = "AAAAAAAA";
                })
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.ExternalSignIn(login);

            // Assert

            _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<ApplicationUser>()));
            _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));

            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var token = Assert.IsAssignableFrom<TokenDto>(okResult.Value);

            Assert.NotNull(token);
            Assert.NotNull(token.Value);
        }

        #endregion

        #region Confirm Email Tests

        [Fact]
        public async Task ConfirmEmail_WhenCalledWithValidArgs_ReturnsOk()
        {
            // Arrange
            SetUp();

            var req = new ConfirmationRequestDto()
            {
                UserId = "aaaaaaa",
                Code = "0"
            };

            _mockUserManager
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(m => m.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.ConfirmEmail(req);

            // Assert
            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task ConfirmEmail_WhenCalledWithInvalidUserId_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            var req = new ConfirmationRequestDto();

            _mockUserManager
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            // Act
            var result = await _sut.ConfirmEmail(req);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }


        [Fact]
        public async Task ConfirmEmail_WhenCalledWithInvalidCode_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            var req = new ConfirmationRequestDto()
            {
                UserId = "aaaaaaa",
                Code = "0"
            };

            _mockUserManager
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(m => m.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _sut.ConfirmEmail(req);

            // Assert

            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        #endregion

        #region ResendConfirmationEmail Method Tests

        [Fact]
        public async Task ResendConfirmationEmail_WhenCalledWithValidArgs_GeneratesTokenAndCallsEmailSenderWithToken()
        {
            // Arrange
            SetUp();

            string userId = "AAAAAA";
            string code = "0";

            _mockUserManager
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(code);

            // Act
            var result = await _sut.ResendConfirmationEmail(userId);

            // Assert
            _mockUserManager.Verify(m => m.GenerateEmailConfirmationTokenAsync(_user));
            _mockEmailSender.Verify(s => s.SendEmailAsync(It.IsAny<string>()
                , It.IsAny<string>()
                , It.Is<string>(m => m.Contains(code))));
        }

        [Fact]
        public async Task ResendConfirmationEmail_WhenCalledWithValidArgs_ReturnsOk()
        {
            // Arrange
            SetUp();

            string userId = "AAAAAA";
            string code = "0";

            _mockUserManager
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(code);

            // Act
            var result = await _sut.ResendConfirmationEmail(userId);

            // Assert
            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task ResendConfirmationEmail_WhenCalledWithInvalidArgs_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            string userId = "AAAAAA";

            _mockUserManager
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            // Act
            var result = await _sut.ResendConfirmationEmail(userId);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        #endregion

        #region SendPasswordResetEmail Method Tests

        [Fact]
        public async Task SendPasswordResetEmail_WhenCalledWithValidArgs_GeneratesTokenAndCallsEmailSenderWithToken()
        {
            // Arrange
            SetUp();

            string email = "aaaa@aaa.com";
            string code = "0";

            var dto = new SendPasswordResetEmailDto()
            {
                Email = email
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(m => m.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(code);

            // Act
            var result = await _sut.SendPasswordResetEmail(dto);

            // Assert
            _mockUserManager.Verify(m => m.GeneratePasswordResetTokenAsync(_user));
            _mockEmailSender.Verify(s => s.SendEmailAsync(It.IsAny<string>()
                , It.IsAny<string>()
                , It.Is<string>(m => m.Contains(code))));

            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task SendPasswordResetEmail_WhenInvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            string email = "aaaa@aaa.com";
            string code = "0";

            var dto = new SendPasswordResetEmailDto()
            {
                Email = email
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            // Act
            var result = await _sut.SendPasswordResetEmail(dto);

            // Assert
            _mockUserManager.Verify(m => m.GeneratePasswordResetTokenAsync(_user), Times.Never);
            _mockEmailSender.Verify(s => s.SendEmailAsync(It.IsAny<string>()
                , It.IsAny<string>()
                , It.Is<string>(m => m.Contains(code))), Times.Never);

            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        #endregion

        #region ResetPassword Method Tests

        [Fact]
        public async Task ResetPassword_WhenCalledWithValidArgs_ReturnsOk()
        {
            // Arrange
            SetUp();

            string email = "aaaa@aaa.com";

            var req = new ResetPasswordRequestDto()
            {
                Email = email,
                Code = "0"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(m => m.ResetPasswordAsync(It.IsAny<ApplicationUser>(), req.Code, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.ResetPassword(req);

            // Assert
            Assert.IsAssignableFrom<OkResult>(result);
        }

        [Fact]
        public async Task ResetPassword_WhenCalledWithInValidEmail_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            string email = "aaaa@aaa.com";

            var req = new ResetPasswordRequestDto()
            {
                Email = email,
                Code = "0"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            // Act
            var result = await _sut.ResetPassword(req);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        [Fact]
        public async Task ResetPassword_WhenCalledWithInValidCode_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            string email = "aaaa@aaa.com";

            var req = new ResetPasswordRequestDto()
            {
                Email = email,
                Code = "0"
            };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(m => m.ResetPasswordAsync(It.IsAny<ApplicationUser>(), req.Code, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _sut.ResetPassword(req);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        #endregion

        #region Private Methods
        private void SetUp()
        {
            _user = new ApplicationUser()
            {
                Id = "AAAAAAAAAA",
                UserName = "BBBBBBBBBBB",
                Email = "ddad@dawd.com",
                EmailConfirmed = true
            };

            _userWithNotConfirmedEmail = new ApplicationUser()
            {
                Id = "AAAAAAAAAA",
                UserName = "BBBBBBBBBBB",
                Email = "ddad@dawd.com",
                EmailConfirmed = false
            };

            _mockLogger = new Mock<ILogger<AuthenticationController>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AuthenticationController>>();
            _mockEmailSender = new Mock<IEmailSender>();

            _mockUserManager = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object,
               new Mock<IOptions<IdentityOptions>>().Object,
               new Mock<IPasswordHasher<ApplicationUser>>().Object,
               new IUserValidator<ApplicationUser>[0],
               new IPasswordValidator<ApplicationUser>[0],
               new Mock<ILookupNormalizer>().Object,
               new Mock<IdentityErrorDescriber>().Object,
               new Mock<IServiceProvider>().Object,
               new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(_mockUserManager.Object,
                 new Mock<IHttpContextAccessor>().Object,
                 new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                 new Mock<IOptions<IdentityOptions>>().Object,
                 new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                 new Mock<IAuthenticationSchemeProvider>().Object);           

            _mockConfiguration.Setup(c => c[It.IsAny<string>()]).Returns("somedummytextxxxxxxxxx");

            _sut = new AuthenticationController(_mockConfiguration.Object
                , _mockSignInManager.Object
                , _mockUserManager.Object
                , _mockLogger.Object
                , _mockEmailSender.Object);
        }
        #endregion
    }
}
