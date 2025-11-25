using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Core.DataAccess
{
    public interface IToDoRepository
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken _token = default);
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken _token = default);
        Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken _token = default);
        Task<ToDoItem?> Get(Guid id, CancellationToken _token = default);
        Task Add(ToDoItem item, CancellationToken _token = default);
        Task Update(ToDoItem item, CancellationToken _token = default);
        Task Delete(Guid id, CancellationToken _token = default);
        Task<bool> ExistsByName(Guid userId, string name, CancellationToken _token = default);
        Task<int> CountActive(Guid userId, CancellationToken _token = default);
    }
}
