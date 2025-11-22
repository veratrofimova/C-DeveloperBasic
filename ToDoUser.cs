namespace DZ_Lesson_5
{
    public class ToDoUser
    {
        public Guid UserId { get; }
        public string TelegramUserName { get; }
        public DateTime RegisteredAt { get; }

        public ToDoUser(string telegramUserName)
        {
            UserId = Guid.NewGuid();
            TelegramUserName = telegramUserName ?? throw new ArgumentNullException(nameof(telegramUserName));
            RegisteredAt = DateTime.UtcNow;
        }
    }
}
