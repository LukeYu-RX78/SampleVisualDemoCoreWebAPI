using Microsoft.EntityFrameworkCore;
using SampleVisualDemoCoreWebAPI.Events;
using SampleVisualDemoCoreWebAPI.Interfaces;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Services
{
    public class DDRRejectLogService : IDDRRejectLogService
    {
        private readonly DorsDbContext _context;
        private readonly IEventBus _eventBus;

        public DDRRejectLogService(DorsDbContext context, IEventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus;
        }

        public async Task<List<DDRRejectLog>> GetAllLogsAsync()
        {
            return await _context.DDRRejectLogs.ToListAsync();
        }

        public async Task<DDRRejectLog?> GetLogByLidAsync(int lid)
        {
            return await _context.DDRRejectLogs.FindAsync(lid);
        }

        public async Task<DDRRejectLog?> GetLogByPidAndAidAsync(int pid, int aid)
        {
            return await _context.DDRRejectLogs
                .Where(l => l.Pid == pid && l.RollBackTo == aid)
                .OrderByDescending(l => l.Lid)
                .FirstOrDefaultAsync();
        }

        public async Task<DDRRejectLog> AddLogAsync(DDRRejectLog log)
        {
            // Auto-fill RollBackTo from Plod.SendTo
            var plod = await _context.Plods.FindAsync(log.Pid);
            if (plod != null && int.TryParse(plod.SendTo, out int sendToAid))
            {
                log.RollBackTo = sendToAid;
            }

            // Save the rejection log
            _context.DDRRejectLogs.Add(log);
            await _context.SaveChangesAsync();

            // Fetch reviewer and rejectedBy user
            var reviewer = await _context.AppAccounts.FindAsync(log.RejectedBy); // the reviewer
            var rejectedUser = await _context.AppAccounts.FindAsync(log.RollBackTo); // the one receiving the rejection

            if (plod != null && reviewer != null && rejectedUser != null)
            {
                var emailEvent = new DDRRejectLogCreatedEvent(
                    pid: log.Pid ?? -1,
                    message: log.Message ?? "Reviewer left no feedback :(",
                    reviewerName: reviewer.FirstName ?? "Reviewer",
                    reviewerEmail: reviewer.EmailAddress ?? "unknown@example.com",
                    userFirstName: rejectedUser.FirstName ?? "User",
                    userEmail: rejectedUser.EmailAddress ?? "unknown@example.com",
                    contractNo: plod.ContractNo ?? "N/A",
                    rigNo: plod.RigNo ?? "N/A",
                    plodDate: plod.PlodDate ?? "N/A",
                    plodShift: plod.PlodShift ?? "N/A",
                    rejectionDateTime: log.CreationDateTime ?? DateTime.UtcNow.ToString("s")
                );
                await _eventBus.PublishAsync(emailEvent);
            }
            return log;
        }

        public async Task AddLogsAsync(List<DDRRejectLog> logs)
        {
            _context.DDRRejectLogs.AddRange(logs);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateLogByLidAsync(int lid, DDRRejectLog updatedLog)
        {
            var existing = await _context.DDRRejectLogs.FindAsync(lid);
            if (existing == null) return false;

            if (updatedLog.Pid != null)
                existing.Pid = updatedLog.Pid;

            if (updatedLog.RejectedBy != null)
                existing.RejectedBy = updatedLog.RejectedBy;

            if (updatedLog.RollBackTo != null)
                existing.RollBackTo = updatedLog.RollBackTo;

            if (!string.IsNullOrWhiteSpace(updatedLog.CreationDateTime))
                existing.CreationDateTime = updatedLog.CreationDateTime;

            if (!string.IsNullOrWhiteSpace(updatedLog.Message))
                existing.Message = updatedLog.Message;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return await _context.DDRRejectLogs.AnyAsync(e => e.Lid == lid);
            }
        }


        public async Task<bool> DeleteLogByLidAsync(int lid)
        {
            var log = await _context.DDRRejectLogs.FindAsync(lid);
            if (log == null) return false;

            _context.DDRRejectLogs.Remove(log);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
