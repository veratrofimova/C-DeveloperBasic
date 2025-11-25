using DZ_Lessons.DAL;
using DZ_Lessons.DAL.Exceptions;
using DZ_Lessons.Infrastracture.Interface;
using static DZ_Lessons.DAL.Enum.ToDoItemStateEnum;

namespace DZ_Lessons.Infrastracture
{
    public class ToDoService : IToDoService
    {
        private readonly List<ToDoItem> _tasks = new();
        public int MaxCountTasks { get; set; }
        public int MaxLengthTasks { get; set; }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _tasks
                .Where(t => t.User.UserId == userId)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _tasks
                .Where(t =>
                    t.User.UserId == userId &&
                    t.State == ToDoItemState.Active)
                .ToList()
                .AsReadOnly();
        }

        public ToDoItem Add(ToDoUser user, string name)
        {
            int activeTasksCount = _tasks
                .Count(t =>
                    t.State == ToDoItemState.Active &&
                    t.User.UserId == user.UserId);

            if (activeTasksCount >= MaxCountTasks)
                throw new TaskCountLimitException(MaxCountTasks);

            (new ParseAndValidate()).ValidateString(name);
            if (name.Length > MaxLengthTasks)
                throw new TaskLengthLimitException(name.Length, MaxLengthTasks);

            bool hasDuplicate = _tasks
                .Any(t =>
                    t.Name == name &&
                    t.State == ToDoItemState.Active &&
                    t.User.UserId == user.UserId);

            if (hasDuplicate)
                throw new DuplicateTaskException(name);

            ToDoItem newTask = new ToDoItem(user, name);
            _tasks.Add(newTask);

            return newTask;
        }

        public void MarkCompleted(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.MarkAsCompleted();
            }
        }

        public void Delete(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                _tasks.Remove(task);
            }
        }
    }
}