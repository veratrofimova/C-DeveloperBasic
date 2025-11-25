using DZ_Lessons.Core.DataAccess;
using DZ_Lessons.Core.Entities;
using DZ_Lessons.Core.Exceptions;
using DZ_Lessons.DAL;

namespace DZ_Lessons.Core.Services
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;

        public ToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository ?? throw new ArgumentNullException(nameof(toDoRepository));
        }

        public int MaxCountTasks { get; set; }
        public int MaxLengthTasks { get; set; }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken token)
        {
            return await _toDoRepository.GetAllByUserId(userId);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken token)
        {
            return await _toDoRepository.GetActiveByUserId(userId);
        }

        public async Task<ToDoItem> Add(ToDoUser user, string name, CancellationToken token)
        {
            int activeTasksCount = await _toDoRepository.CountActive(user.UserId);

            if (activeTasksCount >= MaxCountTasks)
                throw new TaskCountLimitException(MaxCountTasks);

            new ParseAndValidate().ValidateString(name);
            if (name.Length > MaxLengthTasks)
                throw new TaskLengthLimitException(name.Length, MaxLengthTasks);

            bool hasDuplicate = await _toDoRepository.ExistsByName(user.UserId, name);

            if (hasDuplicate)
                throw new DuplicateTaskException(name);

            ToDoItem newTask = new ToDoItem(user, name);
            await _toDoRepository.Add(newTask);

            return newTask;
        }

        public async Task MarkCompleted(Guid id, CancellationToken token)
        {
            var task = await _toDoRepository.Get(id);
            if (task != null)
            {
                task.MarkAsCompleted();
                await _toDoRepository.Update(task, token);
            }
        }

        public async Task Delete(Guid id, CancellationToken token)
        {
            await _toDoRepository.Delete(id);
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken token)
        {
            return await _toDoRepository.Find(user.UserId, task => task.Name.StartsWith(namePrefix));
        }
    }
}