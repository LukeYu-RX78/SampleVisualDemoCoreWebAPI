using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitelineDDRRejectLogController : ControllerBase
    {
        private readonly DorsDbContext _context;

        public TitelineDDRRejectLogController(DorsDbContext context)
        {
            _context = context;
        }

        // GET: api/TitelineDDRRejectLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DDRRejectLog>>> GetAllLogs()
        {
            return await _context.DDRRejectLogs.ToListAsync();
        }

        // GET: api/TitelineDDRRejectLog/by-lid/5
        [HttpGet("by-lid/{lid}")]
        public async Task<ActionResult<DDRRejectLog>> GetLogByLid(int lid)
        {
            var log = await _context.DDRRejectLogs.FindAsync(lid);
            if (log == null)
                return NotFound();
            return log;
        }

        // GET: api/TitelineDDRRejectLog/by-pid/5
        [HttpGet("by-pid/{pid}")]
        public async Task<ActionResult<IEnumerable<DDRRejectLog>>> GetLogsByPid(int pid)
        {
            var logs = await _context.DDRRejectLogs
                .Where(l => l.Pid == pid)
                .ToListAsync();

            return logs;
        }

        // POST: api/TitelineDDRRejectLog
        [HttpPost]
        public async Task<ActionResult<DDRRejectLog>> AddLog([FromBody] DDRRejectLog log)
        {
            _context.DDRRejectLogs.Add(log);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLogByLid), new { lid = log.Lid }, log);
        }

        // POST: api/TitelineDDRRejectLog/batch
        [HttpPost("batch")]
        public async Task<IActionResult> AddLogs([FromBody] List<DDRRejectLog> logs)
        {
            if (logs == null || logs.Count == 0)
                return BadRequest("Log list is empty.");

            _context.DDRRejectLogs.AddRange(logs);
            await _context.SaveChangesAsync();

            return Ok("Logs added successfully.");
        }

        // PUT: api/TitelineDDRRejectLog/update-lid/5
        [HttpPut("update-lid/{lid}")]
        public async Task<IActionResult> UpdateLogByLid(int lid, DDRRejectLog updatedLog)
        {
            if (lid != updatedLog.Lid)
                return BadRequest("Lid mismatch.");

            _context.Entry(updatedLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Log updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DDRRejectLogs.Any(e => e.Lid == lid))
                    return NotFound();
                throw;
            }
        }

        // DELETE: api/TitelineDDRRejectLog/delete-lid/5
        [HttpDelete("delete-lid/{lid}")]
        public async Task<IActionResult> DeleteLogByLid(int lid)
        {
            var log = await _context.DDRRejectLogs.FindAsync(lid);
            if (log == null)
                return NotFound();

            _context.DDRRejectLogs.Remove(log);
            await _context.SaveChangesAsync();

            return Ok("Log deleted successfully.");
        }
    }
}
