using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Core.DataAccess
{
    public interface IToDoRepository
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate);
        ToDoItem? Get(Guid id);
        void Add(ToDoItem item);
        void Update(ToDoItem item);
        void Delete(Guid id);
        bool ExistsByName(Guid userId, string name);
        int CountActive(Guid userId);
    }
}
