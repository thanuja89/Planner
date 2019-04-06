using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Planner.Api.Controllers;
using Planner.Domain.Entities;
using Planner.Dto;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Planner.Api.Tests
{
    public class AuthenticationControllerTests
    {
        private ApplicationUser _user;
        private Mock<ILogger<AuthenticationController>> _mockLogger;
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
                , _mockLogger.Object);
        }
        #endregion
    }
}
