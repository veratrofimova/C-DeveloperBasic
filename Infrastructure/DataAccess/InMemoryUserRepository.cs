using DZ_Lessons.Core.DataAccess;
using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Infrastructure.DataAccess
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<ToDoUser> _users = new();

        public void Add(ToDoUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = GetUserByTelegramUserId(user.TelegramUserId);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Пользователь с Telegram ID {user.TelegramUserId} уже существует");
            }

            _users.Add(user);
        }

        public ToDoUser? GetUser(Guid userId)
        {
            return _users.FirstOrDefault(t => t.UserId == userId);
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            return _users.FirstOrDefault(t => t.TelegramUserId == telegramUserId);
        }
    }
}
