using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Planner.Api.Controllers;
using Planner.Api.Services;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using Planner.Dto;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Planner.Api.Tests.ControllerTests
{
    public class ScheduledTaskControllerTests
    {
        private string _userId;
        private Mock<IScheduledTaskRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<ScheduledTaskController>> _mockLogger;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IUnitOfWork> _mockLUOW;
        private Mock<IUrlHelper> _urlHelper;
        private ScheduledTaskController _sut;

        #region GET Method Tests
        [Fact]
        public async Task Get_ValidPrinciple_CallsRepoWithCorrectArgs()
        {
            // Arrange
            SetUp();

            // Act
            var result = await _sut.Get();

            // Assert
            _mockRepo.Verify(m => m.GetScheduledTasksForUser(_userId));
        }

        [Fact]
        public async Task Get_ValidPrinciple_ReturnsOkWithTaskDTOs()
        {
            // Arrange
            SetUp();

            // Act
            var result = await _sut.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<ScheduledTaskDTO>>(okResult.Value);
        }

        [Fact]
        public async Task Get_ValidId_CallsRepoWithCorrectArgs()
        {
            // Arrange
            SetUp();
            Guid id = Guid.NewGuid();

            // Act
            var result = await _sut.Get(id);

            // Assert
            _mockRepo.Verify(m => m.GetByIdAsync(id));
        }

        [Fact]
        public async Task Get_ValidId_ReturnsOk()
        {
            // Arrange
            SetUp();
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new ScheduledTask());
            Guid id = Guid.NewGuid();

            // Act
            var result = await _sut.Get(id);

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public async Task Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            SetUp();
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>())).ReturnsAsync((ScheduledTask) null);
            Guid id = default;

            // Act
            var result = await _sut.Get(id);

            // Assert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        #endregion

        #region POST Method Tests
        [Fact]
        public async Task Post_ValidObject_CallsMapperAndRepoWithCorrectArgsAndReturnsOk()
        {
            // Arrange
            SetUp();

            var taskDto = new PostScheduledTaskDTO()
            {
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow
            };

            var task = new ScheduledTask()
            {
                //Id = 1,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow
            };

            _mockMapper.Setup(m => m.Map<ScheduledTask>(It.IsAny<PostScheduledTaskDTO>())).Returns(task);

            // Act
            var result = await _sut.Post(taskDto);

            // Assert
            _mockMapper.Verify(m => m.Map<ScheduledTask>(taskDto));
            _mockRepo.Verify(r => r.AddAsync(task));
            _mockLUOW.Verify(u => u.CompleteAsync());

            Assert.IsAssignableFrom<CreatedResult>(result);
        }
        #endregion

        #region PUT Method Tests
        [Fact]
        public async Task Put_ValidObject_CallsMapperAndRepoWithCorrectArgsAndReturnsOk()
        {
            // Arrange
            SetUp();

            Guid id = Guid.NewGuid();

            var taskDto = new PutScheduledTaskDTO()
            {
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow
            };

            var task = new ScheduledTask()
            {
                //Id = 1,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow
            };

            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>())).ReturnsAsync(task);

            // Act
            var result = await _sut.Put(id, taskDto);

            // Assert
            _mockMapper.Verify(m => m.Map(taskDto, task));
            _mockRepo.Verify(r => r.FindAsync(id));
            _mockLUOW.Verify(u => u.CompleteAsync());

            Assert.IsAssignableFrom<OkObjectResult>(result);
        }
        #endregion

        #region PUT Method Tests
        [Fact]
        public async Task Delete_ValidObject_CallsRepoWithCorrectArgsAndReturnsNoContent()
        {
            // Arrange
            SetUp();

            Guid id = Guid.NewGuid();

            var taskDto = new PutScheduledTaskDTO()
            {

                Start = DateTime.UtcNow,
                End = DateTime.UtcNow
            };

            var task = new ScheduledTask()
            {
                Id = Guid.NewGuid(),
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow
            };

            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>())).ReturnsAsync(task);

            // Act
            var result = await _sut.Delete(id);

            // Assert
            _mockRepo.Verify(r => r.FindAsync(id));
            _mockRepo.Verify(r => r.Delete(task));
            _mockLUOW.Verify(u => u.CompleteAsync());

            Assert.IsAssignableFrom<NoContentResult>(result);
        }
        #endregion

        #region Private Methods
        private void SetUp()
        {
            _userId = "abcadasda";

            _mockRepo = new Mock<IScheduledTaskRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ScheduledTaskController>>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockLUOW = new Mock<IUnitOfWork>();

            var claimsPrinc = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.NameIdentifier, _userId) }));

            _urlHelper = new Mock<IUrlHelper>();
            _urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost/api/ScheduledTask/1");

            _sut = new ScheduledTaskController(_mockRepo.Object
                , _mockLUOW.Object
                , _mockMapper.Object
                , _mockNotificationService.Object
                , _mockLogger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrinc
                    }
                },
                Url = _urlHelper.Object
            };
        }
        #endregion
    }
}
