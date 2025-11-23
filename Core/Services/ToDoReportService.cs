using DZ_Lessons.Core.DataAccess;

namespace DZ_Lessons.Core.Services
{
    public class ToDoReportService : IToDoReportService
    {
        private readonly IToDoService _toDoService;

        public ToDoReportService(IToDoService toDoService)
        {
            _toDoService = toDoService ?? throw new ArgumentNullException(nameof(toDoService));
        }

        public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var tasks = await _toDoService.GetAllByUserId(userId);

            var total = tasks.Count();
            var completed = tasks.Count(t => t.State == Enum.ToDoItemStateEnum.ToDoItemState.Completed);
            var active = tasks.Count(t => t.State == Enum.ToDoItemStateEnum.ToDoItemState.Active);

            return (total, completed, active, DateTime.UtcNow);
        }
    }
}
