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

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _toDoRepository.GetAllByUserId(userId);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _toDoRepository.GetActiveByUserId(userId);
        }

        public ToDoItem Add(ToDoUser user, string name)
        {
            int activeTasksCount = _toDoRepository.CountActive(user.UserId);

            if (activeTasksCount >= MaxCountTasks)
                throw new TaskCountLimitException(MaxCountTasks);

            new ParseAndValidate().ValidateString(name);
            if (name.Length > MaxLengthTasks)
                throw new TaskLengthLimitException(name.Length, MaxLengthTasks);

            bool hasDuplicate = _toDoRepository.ExistsByName(user.UserId, name);

            if (hasDuplicate)
                throw new DuplicateTaskException(name);

            ToDoItem newTask = new ToDoItem(user, name);
            _toDoRepository.Add(newTask);

            return newTask;
        }

        public void MarkCompleted(Guid id)
        {
            var task = _toDoRepository.Get(id);
            if (task != null)
            {
                task.MarkAsCompleted();
                _toDoRepository.Update(task);
            }
        }

        public void Delete(Guid id)
        {
            _toDoRepository.Delete(id);
        }

        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            return _toDoRepository.Find(user.UserId, task => task.Name.StartsWith(namePrefix));
        }
    }
}