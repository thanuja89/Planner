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

            var login = new TokenRequestDto();

            _mockSignInManager
                .Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(_user);

            // Act
            var result = await _sut.CreateToken(login);

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.IsAssignableFrom<TokenDto>(okResult.Value);
        }

        [Fact]
        public async Task CreateToken_WhenCalledWithInValidCreds_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            var login = new TokenRequestDto();

            _mockSignInManager
                .Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _sut.CreateToken(login);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        #endregion

        #region CreateAccount Method Tests

        [Fact]
        public async Task CreateAccount_WhenCalledWithValidArgs_CallsEmailSenderAndReturnsOkWithSuccess()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto();
            string code = "12346";

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(code);

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var dto = Assert.IsAssignableFrom<SignUpResultDTO>(okResult.Value);
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
                Username = "BBBBBBBBBBB",
                Password = "password"
            };

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(_user);

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var badRequestResult = Assert.IsAssignableFrom<BadRequestObjectResult>(result);
            var dto = Assert.IsAssignableFrom<SignUpResultDTO>(badRequestResult.Value);
            Assert.False(dto.Succeeded);
        }

        [Fact]
        public async Task CreateAccount_WhenUsernameExistsForSameEmail_ReturnsOk()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto()
            {
                Email = "ddad@dawd.com",
                Username = "BBBBBBBBBBB",
                Password = "password"
            };

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(_user);

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var okResult = Assert.IsAssignableFrom<OkObjectResult>(result);
            var dto = Assert.IsAssignableFrom<SignUpResultDTO>(okResult.Value);
            Assert.True(dto.Succeeded);
        }

        [Fact]
        public async Task CreateAccount_WhenEmailExistsForDifferentUsername_ReturnsBadRequest()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto()
            {
                Email = "ddad@dawd.com",
                Username = "CCCCCCCC",
                Password = "password"
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
            var dto = Assert.IsAssignableFrom<SignUpResultDTO>(badRequestResult.Value);
            Assert.False(dto.Succeeded);
        }

        [Fact]
        public async Task CreateAccount_WhenExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            SetUp();

            var acc = new CreateAccountDto()
            {
                Email = "ddad@dawd.com",
                Username = "CCCCCCCC",
                Password = "password"
            };

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException());

            // Act
            var result = await _sut.CreateAccount(acc);

            // Assert
            var internalErrorResult = Assert.IsAssignableFrom<ObjectResult>(result);

            Assert.Equal(internalErrorResult.StatusCode, (int?) HttpStatusCode.InternalServerError);

            var dto = Assert.IsAssignableFrom<SignUpResultDTO>(internalErrorResult.Value);

            Assert.False(dto.Succeeded);
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
                Code = "12345"
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

            var req = new ConfirmationRequestDto()
            {
                UserId = "aaaaaaa",
                Code = "12345"
            };

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
                Code = "12345"
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

        #region Private Methods
        private void SetUp()
        {
            _user = new ApplicationUser()
            {
                Id = "AAAAAAAAAA",
                UserName = "BBBBBBBBBBB",
                Email = "ddad@dawd.com"
            };

            _mockLogger = new Mock<ILogger<AuthenticationController>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AuthenticationController>>();
            //IEmailSender emailSender
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
