using Microsoft.Extensions.Logging;
using Moq;
using Planner.Api.Services;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using System;
using Xunit;

namespace Planner.Api.Tests.Services
{
    public class SyncronizationServiceTests
    {
        private Mock<IScheduledTaskRepository> _mockTaskRepo;
        private Mock<ISyncronizationLockRepository> _mockLockRepo;
        private Mock<ILogger<SyncronizationService>> _mockLogger;
        private Mock<IUnitOfWork> _mockLUOW;
        private SyncronizationService _sut;

        private readonly string _userId = "USER1";
        private Guid _lockId = Guid.NewGuid();

        private void SetUp()
        {
            _mockTaskRepo = new Mock<IScheduledTaskRepository>();
            _mockLockRepo = new Mock<ISyncronizationLockRepository>();
            _mockLogger = new Mock<ILogger<SyncronizationService>>();
            _mockLUOW = new Mock<IUnitOfWork>();

            _sut = new SyncronizationService(_mockLockRepo.Object
                , _mockTaskRepo.Object
                , _mockLUOW.Object
                , _mockLogger.Object);
        }

        [Fact]
        private async void TakeLockAsync_ValidUserId_CallsLockRepoWithCorrectArgs()
        {
            // Arrange
            SetUp();

            // Act
            await _sut.TakeLockAsync(_userId);

            // Assert
            _mockLockRepo.Verify(r => r.GetSyncronizationLockByUserId(_userId), Times.Once);
        }

        [Fact]
        private async void TakeLockAsync_ValidUserIdAndLockNotTaken_ReturnsLock()
        {
            // Arrange
            SetUp();

            // Act
            var lk = await _sut.TakeLockAsync(_userId);

            // Assert
            Assert.True(lk.IsSucceeded);
            Assert.NotNull(lk.Lock);
        }

        [Fact]
        private async void TakeLockAsync_ValidUserIdAndLockTaken_ReturnsNull()
        {
            // Arrange
            SetUp();
            _mockLockRepo.Setup(r => r.GetSyncronizationLockByUserId(_userId)).ReturnsAsync(new SyncronizationLock());

            // Act
            var lk = await _sut.TakeLockAsync(_userId);

            // Assert
            Assert.False(lk.IsSucceeded);
            Assert.Null(lk.Lock);
        }

        [Fact]
        private async void GetLockAsync_WhenCalled_CallRepoWithCorrectArgs()
        {
            // Arrange
            SetUp();

            // Act
            var lk = await _sut.GetLockAsync(_lockId);

            // Assert
            _mockLockRepo.Verify(r => r.FindAsync(_lockId), Times.Once);
        }
    }
}
