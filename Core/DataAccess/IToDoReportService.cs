using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Core.DataAccess
{
    public interface IToDoReportService
    {
        Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken _token = default);
    }
}
