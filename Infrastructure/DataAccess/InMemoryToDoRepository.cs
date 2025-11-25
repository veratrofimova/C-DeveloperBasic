using DZ_Lessons.Core.DataAccess;
using DZ_Lessons.Core.Entities;
using static DZ_Lessons.Core.Enum.ToDoItemStateEnum;

namespace DZ_Lessons.Infrastructure.DataAccess
{
    public class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _tasks = new();

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return _tasks
                .Where(t => t.User.UserId == userId)
                .ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return _tasks
                .Where(t =>
                    t.User.UserId == userId &&
                    t.State == ToDoItemState.Active)
                .ToList();
        }

        public async Task<ToDoItem?> Get(Guid id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public async Task Add(ToDoItem item, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _tasks.Add(item);
        }

        public async Task Update(ToDoItem item, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var existingItem = await Get(item.Id, token);
            if (existingItem != null)
            {
                _tasks.Remove(existingItem);
                _tasks.Add(item);
            }
        }

        public async Task Delete(Guid id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var item = await Get(id, token);
            if (item != null)
            {
                _tasks.Remove(item);
            }
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return _tasks.Any(t =>
                t.User.UserId == userId &&
                t.Name == name &&
                t.State == ToDoItemState.Active);
        }

        public async Task<int> CountActive(Guid userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return _tasks.Count(t =>
                t.User.UserId == userId &&
                t.State == ToDoItemState.Active);
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return _tasks
                .Where(t => t.User.UserId == userId && predicate(t))
                .ToList()
                .AsReadOnly();
        }
    }
}
