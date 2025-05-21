using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Interfaces
{
    public interface IDDRRejectLogService
    {
        Task<List<DDRRejectLog>> GetAllLogsAsync();
        Task<DDRRejectLog?> GetLogByLidAsync(int lid);
        Task<DDRRejectLog?> GetLogByPidAndAidAsync(int pid, int aid);
        Task<DDRRejectLog> AddLogAsync(DDRRejectLog log);
        Task AddLogsAsync(List<DDRRejectLog> logs);
        Task<bool> UpdateLogByLidAsync(int lid, DDRRejectLog updatedLog);
        Task<bool> DeleteLogByLidAsync(int lid);
    }
}
