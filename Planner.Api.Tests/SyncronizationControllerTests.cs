using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Planner.Api.Controllers;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using Planner.Dto;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Planner.Api.Tests
{
    public class SyncronizationControllerTests
    {
        private string _userId;
        private DateTime _lastSynced;
        private Mock<IScheduledTaskRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<SyncronizationController>> _mockLogger;
        private Mock<IUnitOfWork> _mockLUOW;
        //private Mock<IUrlHelper> _urlHelper;
        private SyncronizationController _sut;

        [Fact]
        public async void Get_WhenCalled_CallsRepoAndMapperWithCorrectArgs()
        {
            // Arrange
            SetUp();

            var tasks = new ScheduledTask[] {
                new ScheduledTask()
                {
                    //Id = 1,
                    Start = DateTime.UtcNow,
                    End = DateTime.UtcNow
                }
            };

            _mockRepo.Setup(r => r.GetNewScheduledTasksForUserAsync(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(tasks);

            // Act
            var result = await _sut.Get(_lastSynced);

            // Assert
            _mockRepo.Verify(m => m.GetNewScheduledTasksForUserAsync(_userId, _lastSynced));
            _mockMapper.Verify(m => m.Map<IEnumerable<GetScheduledTaskDTO>>(tasks));
        }

        [Fact]
        public async void Get_WhenCalled_ReturnsOkWithTaskDTOs()
        {
            // Arrange
            SetUp();

            // Act
            var result = await _sut.Get(_lastSynced);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<GetScheduledTaskDTO>>(okResult.Value);
        }

        #region Private Methods
        private void SetUp()
        {
            _userId = "abcadasda";
            _lastSynced = DateTime.UtcNow.AddDays(-30);

            _mockRepo = new Mock<IScheduledTaskRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<SyncronizationController>>();
            _mockLUOW = new Mock<IUnitOfWork>();

            var claimsPrinc = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.NameIdentifier, _userId) }));

            //_urlHelper = new Mock<IUrlHelper>();
            //_urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost/api/ScheduledTask/1");

            _sut = new SyncronizationController(_mockRepo.Object, _mockLUOW.Object, _mockMapper.Object, _mockLogger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrinc
                    }
                },
                //Url = _urlHelper.Object
            };
        }
        #endregion
    }
}
