using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Core.DataAccess
{
    public interface IToDoReportService
    {
        (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId);
    }
}
