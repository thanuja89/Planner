using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Planner.Api.Controllers;
using Planner.Domain.Entities;
using Planner.Domain.Repositories;
using System;
using System.Security.Claims;
using Xunit;

namespace Planner.Api.Tests
{
    public class ScheduledTaskControllerTests
    {
        private readonly string _userId;
        private Mock<ScheduledTaskRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private ScheduledTaskController _sut;

        public ScheduledTaskControllerTests()
        {
            _userId = "abcadasda";

            var user = new ClaimsPrincipal();

            var claimsPrinc = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.NameIdentifier, _userId) }));

            _sut.HttpContext.User = user;
        }

        [Fact]
        public void Get_ValidPrinciple_CallsRepository()
        {

        }
    }
}
