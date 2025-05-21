using Microsoft.AspNetCore.Mvc;
using SampleVisualDemoCoreWebAPI.Interfaces;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineDDRRejectLogController : ControllerBase
    {
        private readonly IDDRRejectLogService _service;

        public TitelineDDRRejectLogController(IDDRRejectLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DDRRejectLog>>> GetAllLogs()
        {
            var logs = await _service.GetAllLogsAsync();
            return Ok(logs);
        }

        [HttpGet("by-lid/{lid}")]
        public async Task<ActionResult<DDRRejectLog>> GetLogByLid(int lid)
        {
            var log = await _service.GetLogByLidAsync(lid);
            return log == null ? NotFound() : Ok(log);
        }

        [HttpGet("by-pid-and-aid/{pid}/{aid}")]
        public async Task<ActionResult<DDRRejectLog>> GetLogsByPidAndAid(int pid, int aid)
        {
            var log = await _service.GetLogByPidAndAidAsync(pid, aid);
            return log == null ? NotFound() : Ok(log);
        }

        [HttpPost]
        public async Task<ActionResult<DDRRejectLog>> AddLog([FromBody] DDRRejectLog log)
        {
            var addedLog = await _service.AddLogAsync(log);
            return CreatedAtAction(nameof(GetLogByLid), new { lid = addedLog.Lid }, addedLog);
        }

        [HttpPost("batch")]
        public async Task<IActionResult> AddLogs([FromBody] List<DDRRejectLog> logs)
        {
            if (logs == null || logs.Count == 0)
                return BadRequest("Log list is empty.");

            await _service.AddLogsAsync(logs);
            return Ok("Logs added successfully.");
        }

        [HttpPut("update-lid/{lid}")]
        public async Task<IActionResult> UpdateLogByLid(int lid, DDRRejectLog updatedLog)
        {
            var result = await _service.UpdateLogByLidAsync(lid, updatedLog);
            return result ? Ok("Log updated successfully.") : NotFound();
        }

        [HttpDelete("delete-lid/{lid}")]
        public async Task<IActionResult> DeleteLogByLid(int lid)
        {
            var result = await _service.DeleteLogByLidAsync(lid);
            return result ? Ok("Log deleted successfully.") : NotFound();
        }
    }
}
