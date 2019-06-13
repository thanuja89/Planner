using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Planner.Api.Controllers;
using Planner.Api.Services;
using Planner.Domain.DataModels;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using Planner.Dto;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Planner.Api.Tests.ControllerTests
{
    public class SyncronizationControllerTests
    {
        private readonly string _userId;
        private readonly DateTime _lastSynced;
        private readonly Mock<IScheduledTaskRepository> _mockRepo;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<SyncronizationController>> _mockLogger;
        private readonly SyncronizationController _sut;

        public SyncronizationControllerTests()
        {
            _userId = "abcadasda";
            _lastSynced = DateTime.UtcNow.AddDays(-30);

            _mockRepo = new Mock<IScheduledTaskRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<SyncronizationController>>();

            var claimsPrinc = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.NameIdentifier, _userId) }));

            _sut = new SyncronizationController(_mockRepo.Object
                , _mockNotificationService.Object
                , _mockMapper.Object
                , _mockLogger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrinc
                    }
                },
            };
        }

        #region Get Method Tests
        [Fact]
        public async Task Get_WhenCalled_CallsRepoAndMapperWithCorrectArgs()
        {
            // Arrange
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
        public async Task Get_WhenCalled_ReturnsOkWithTaskDTOs()
        {
            // Arrange

            // Act
            var result = await _sut.Get(_lastSynced);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<GetScheduledTaskDTO>>(okResult.Value);
        }
        #endregion

        #region Put Method Tests
        [Fact]
        public async Task Put_WhenCalled_CallsRepoAndMapperWithCorrectArgs()
        {
            // Arrange
            var tasks = new ScheduledTaskDataModel[] {
                new ScheduledTaskDataModel()
                {
                    //Id = 1,
                    Start = DateTime.UtcNow,
                    End = DateTime.UtcNow
                }
            };

            var taskDTOs = new PutScheduledTaskDTO[] {
                new PutScheduledTaskDTO()
                {
                    //Id = 1,
                    Start = DateTime.UtcNow,
                    End = DateTime.UtcNow
                }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<ScheduledTaskDataModel>>(It.IsAny<IEnumerable<PutScheduledTaskDTO>>()))
                .Returns(tasks);

            // Act
            var result = await _sut.Put(taskDTOs);

            // Assert
            _mockMapper.Verify(m => m.Map<IEnumerable<ScheduledTaskDataModel>>(taskDTOs));
            _mockRepo.Verify(m => m.AddOrUpdateScheduledTasksAsync(tasks, _userId));
        }

        [Fact]
        public async Task Put_WhenCalledWithValidArgs_ReturnsNoContent()
        {
            // Arrange
            var taskDTOs = new PutScheduledTaskDTO[] {
                new PutScheduledTaskDTO()
                {
                    //Id = 1,
                    Start = DateTime.UtcNow,
                    End = DateTime.UtcNow
                }
            };

            // Act
            var result = await _sut.Put(taskDTOs);

            // Assert
            Assert.IsAssignableFrom<NoContentResult>(result);
        }

        [Fact]
        public async Task Put_WhenCalledWithInValidArgs_ReturnsBadRequest()
        {
            // Arrange

            // Act
            var result = await _sut.Put(null);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }
        #endregion
    }
}
