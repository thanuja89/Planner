using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Planner.Api.Controllers;
using Planner.Domain.Repositories.Interfaces;
using Planner.Dto;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Planner.Api.Tests
{
    public class ScheduledTaskControllerTests
    {
        private string _userId;
        private Mock<IScheduledTaskRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private ScheduledTaskController _sut;

        [Fact]
        public async void Get_ValidPrinciple_CallsRepoWithCorrectArgs()
        {
            // Arrange
            SetUp();

            // Act
            var result = await _sut.Get();

            // Assert
            _mockRepo.Verify(m => m.GetScheduledTasksForUser(_userId));
        }

        [Fact]
        public async void Get_ValidPrinciple_ReturnsOkWithTaskDTOs()
        {
            // Arrange
            SetUp();

            // Act
            var result = await _sut.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<ScheduledTaskDTO>>(okResult.Value);
        }

        private void SetUp()
        {
            _userId = "abcadasda";

            _mockRepo = new Mock<IScheduledTaskRepository>();
            _mockMapper = new Mock<IMapper>();

            var claimsPrinc = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.NameIdentifier, _userId) }));

            _sut = new ScheduledTaskController(_mockRepo.Object, _mockMapper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrinc
                    }
                }
            };
        }
    }
}