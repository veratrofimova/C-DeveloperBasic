using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Core.Services
{
    public interface IToDoService
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken _token = default);
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken _token = default);
        Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken _token = default);
        Task<ToDoItem> Add(ToDoUser user, string name, CancellationToken _token = default);
        Task MarkCompleted(Guid id, CancellationToken _token = default);
        Task Delete(Guid id, CancellationToken _token = default);
    }
}
