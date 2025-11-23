using DZ_Lessons.Core.DataAccess;
using DZ_Lessons.Core.Entities;
using static DZ_Lessons.Core.Enum.ToDoItemStateEnum;

namespace DZ_Lessons.Infrastructure.DataAccess
{
    public class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _tasks = new();

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _tasks
                .Where(t => t.User.UserId == userId)
                .ToList();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _tasks
                .Where(t =>
                    t.User.UserId == userId &&
                    t.State == ToDoItemState.Active)
                .ToList();
        }

        public ToDoItem? Get(Guid id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public void Add(ToDoItem item)
        {
            _tasks.Add(item);
        }

        public void Update(ToDoItem item)
        {
            var existingItem = Get(item.Id);
            if (existingItem != null)
            {
                _tasks.Remove(existingItem);
                _tasks.Add(item);
            }
        }

        public void Delete(Guid id)
        {
            var item = Get(id);
            if (item != null)
            {
                _tasks.Remove(item);
            }
        }

        public bool ExistsByName(Guid userId, string name)
        {
            return _tasks.Any(t =>
                t.User.UserId == userId &&
                t.Name == name &&
                t.State == ToDoItemState.Active);
        }

        public int CountActive(Guid userId)
        {
            return _tasks.Count(t =>
                t.User.UserId == userId &&
                t.State == ToDoItemState.Active);
        }

        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            return _tasks
                .Where(t => t.User.UserId == userId && predicate(t))
                .ToList()
                .AsReadOnly();
        }
    }
}
