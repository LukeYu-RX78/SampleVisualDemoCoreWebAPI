using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleVisualDemoCoreWebAPI.Controllers;
using SampleVisualDemoCoreWebAPI.Models.Entities;
using Xunit;

namespace SampleVisualDemoCoreWebAPI.Tests.Tests
{
    public class TitelineDDRRejectLogControllerTests
    {
        private TitelineDDRRejectLogController GetControllerWithInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<DorsDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new DorsDbContext(options);
            return new TitelineDDRRejectLogController(context);
        }

        private DDRRejectLog CreateSampleLog(int lid = 1)
        {
            return new DDRRejectLog
            {
                Lid = lid,
                Pid = 100,
                RejectedBy = 200,
                RollBackTo = 300,
                CreationDateTime = "2024-05-01T10:00:00Z",
                Message = "Test log entry"
            };
        }

        [Fact]
        public async Task AddLog_ShouldCreateNewLog()
        {
            var controller = GetControllerWithInMemoryDb(nameof(AddLog_ShouldCreateNewLog));
            var log = CreateSampleLog();

            var result = await controller.AddLog(log);
            var okResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdLog = Assert.IsType<DDRRejectLog>(okResult.Value);
            Assert.Equal("Test log entry", createdLog.Message);
        }

        [Fact]
        public async Task GetAllLogs_ShouldReturnLogs()
        {
            var controller = GetControllerWithInMemoryDb(nameof(GetAllLogs_ShouldReturnLogs));
            await controller.AddLog(CreateSampleLog());

            var result = await controller.GetAllLogs();
            Assert.Single(result.Value!);
        }

        [Fact]
        public async Task GetLogByLid_ShouldReturnLog_WhenFound()
        {
            var controller = GetControllerWithInMemoryDb(nameof(GetLogByLid_ShouldReturnLog_WhenFound));
            var log = CreateSampleLog();
            await controller.AddLog(log);

            var result = await controller.GetLogByLid(log.Lid);
            Assert.NotNull(result.Value);
            Assert.Equal(log.Lid, result.Value!.Lid);
        }

        [Fact]
        public async Task GetLogByLid_ShouldReturnNotFound_WhenMissing()
        {
            var controller = GetControllerWithInMemoryDb(nameof(GetLogByLid_ShouldReturnNotFound_WhenMissing));
            var result = await controller.GetLogByLid(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateLogByLid_ShouldUpdate_WhenValid()
        {
            var controller = GetControllerWithInMemoryDb(nameof(UpdateLogByLid_ShouldUpdate_WhenValid));
            var log = CreateSampleLog();
            await controller.AddLog(log);

            log.Message = "Updated";
            var result = await controller.UpdateLogByLid(log.Lid, log);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteLogByLid_ShouldDelete_WhenFound()
        {
            var controller = GetControllerWithInMemoryDb(nameof(DeleteLogByLid_ShouldDelete_WhenFound));
            var log = CreateSampleLog();
            await controller.AddLog(log);

            var result = await controller.DeleteLogByLid(log.Lid);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddLogs_ShouldAddMultiple()
        {
            var controller = GetControllerWithInMemoryDb(nameof(AddLogs_ShouldAddMultiple));
            var logs = new List<DDRRejectLog>
            {
                CreateSampleLog(1),
                CreateSampleLog(2)
            };

            var result = await controller.AddLogs(logs);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetLogsByPidAndAid_ShouldReturnLatestLog_WhenMultipleMatch()
        {
            var controller = GetControllerWithInMemoryDb(nameof(GetLogsByPidAndAid_ShouldReturnLatestLog_WhenMultipleMatch));

            // Older matching log
            await controller.AddLog(new DDRRejectLog
            {
                Lid = 1,
                Pid = 222,
                RollBackTo = 999,
                RejectedBy = 10,
                CreationDateTime = "2025-05-18T10:00:00Z",
                Message = "Older match"
            });

            // Newer matching log (should be returned)
            await controller.AddLog(new DDRRejectLog
            {
                Lid = 2,
                Pid = 222,
                RollBackTo = 999,
                RejectedBy = 11,
                CreationDateTime = "2025-05-19T10:00:00Z",
                Message = "Latest match"
            });

            // Mismatching Aid
            await controller.AddLog(new DDRRejectLog
            {
                Lid = 3,
                Pid = 222,
                RollBackTo = 888,
                RejectedBy = 12,
                CreationDateTime = "2025-05-19T10:00:00Z",
                Message = "Wrong Aid"
            });

            var result = await controller.GetLogsByPidAndAid(222, 999);

            var log = Assert.IsType<DDRRejectLog>(result.Value);
            Assert.Equal(2, log.Lid);
            Assert.Equal("Latest match", log.Message);
        }
    }
}