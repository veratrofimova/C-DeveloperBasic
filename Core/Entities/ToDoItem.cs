using static DZ_Lessons.Core.Enum.ToDoItemStateEnum;

namespace DZ_Lessons.Core.Entities
{
    public class ToDoItem
    {
        public Guid Id { get; }
        public ToDoUser User { get; }
        public string Name { get; }
        public DateTime CreatedAt { get; }
        public ToDoItemState State { get; set; }
        public DateTime? StateChangedAt { get; set; }

        public ToDoItem(ToDoUser user, string name)
        {
            Id = Guid.NewGuid();
            User = user ?? throw new ArgumentNullException(nameof(user));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CreatedAt = DateTime.UtcNow;
            State = ToDoItemState.Active;
            StateChangedAt = null;
        }

        public void MarkAsCompleted()
        {
            State = ToDoItemState.Completed;
            StateChangedAt = DateTime.UtcNow;
        }
    }
}
