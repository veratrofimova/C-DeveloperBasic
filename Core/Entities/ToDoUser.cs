namespace DZ_Lessons.Core.Entities
{
    public class ToDoUser
    {
        public Guid UserId { get; }
        public long TelegramUserId { get; }
        public string TelegramUserName { get; }
        public DateTime RegisteredAt { get; }

        public ToDoUser(long telegramUserId, string telegramUserName)
        {
            UserId = Guid.NewGuid();
            TelegramUserId = telegramUserId;
            TelegramUserName = telegramUserName ?? throw new ArgumentNullException(nameof(telegramUserName));
            RegisteredAt = DateTime.UtcNow;
        }
    }
}
