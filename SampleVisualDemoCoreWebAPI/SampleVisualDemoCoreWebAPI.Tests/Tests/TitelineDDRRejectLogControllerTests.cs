using Microsoft.EntityFrameworkCore;
using Moq;
using SampleVisualDemoCoreWebAPI.Events;
using SampleVisualDemoCoreWebAPI.Infrastructure;
using SampleVisualDemoCoreWebAPI.Models.Entities;
using SampleVisualDemoCoreWebAPI.Services;
using Xunit;

namespace SampleVisualDemoCoreWebAPI.Tests.Tests
{
    public class DDRRejectLogServiceTests
    {
        private DorsDbContext GetInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<DorsDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new DorsDbContext(options);
        }

        private DDRRejectLogService GetService(string dbName, out Mock<IEventBus> mockEventBus)
        {
            var db = GetInMemoryDb(dbName);
            mockEventBus = new Mock<IEventBus>();
            return new DDRRejectLogService(db, mockEventBus.Object);
        }

        private DDRRejectLog CreateSampleLog(int lid = 1)
        {
            return new DDRRejectLog
            {
                Lid = lid,
                Pid = 101,
                Message = "Test log",
                RejectedBy = 201,
                RollBackTo = 301,
                CreationDateTime = DateTime.UtcNow.ToString("s")
            };
        }

        [Fact]
        public async Task GetLogByLidAsync_ShouldReturnLog_WhenExists()
        {
            var db = GetInMemoryDb(nameof(GetLogByLidAsync_ShouldReturnLog_WhenExists));
            var log = CreateSampleLog(1);
            db.DDRRejectLogs.Add(log);
            await db.SaveChangesAsync();

            var service = new DDRRejectLogService(db, Mock.Of<IEventBus>());
            var result = await service.GetLogByLidAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Lid);
        }

        [Fact]
        public async Task DeleteLogByLidAsync_ShouldRemoveLog()
        {
            var db = GetInMemoryDb(nameof(DeleteLogByLidAsync_ShouldRemoveLog));
            var log = CreateSampleLog(1);
            db.DDRRejectLogs.Add(log);
            await db.SaveChangesAsync();

            var service = new DDRRejectLogService(db, Mock.Of<IEventBus>());
            var success = await service.DeleteLogByLidAsync(1);

            Assert.True(success);
            Assert.Empty(db.DDRRejectLogs);
        }

        [Fact]
        public async Task UpdateLogByLidAsync_ShouldUpdate_WhenValid()
        {
            var db = GetInMemoryDb(nameof(UpdateLogByLidAsync_ShouldUpdate_WhenValid));
            var log = CreateSampleLog(1);
            db.DDRRejectLogs.Add(log);
            await db.SaveChangesAsync();

            log.Message = "Updated!";
            var service = new DDRRejectLogService(db, Mock.Of<IEventBus>());
            var result = await service.UpdateLogByLidAsync(1, log);

            Assert.True(result);
            var updated = await db.DDRRejectLogs.FindAsync(1);
            Assert.Equal("Updated!", updated!.Message);
        }
    }
}