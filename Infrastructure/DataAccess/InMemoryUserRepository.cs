using DZ_Lessons.Core.DataAccess;
using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Infrastructure.DataAccess
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<ToDoUser> _users = new();

        public async Task Add(ToDoUser user, CancellationToken token)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            token.ThrowIfCancellationRequested();

            var existingUser = await GetUserByTelegramUserId(user.TelegramUserId, token);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Пользователь с Telegram ID {user.TelegramUserId} уже существует");
            }

            _users.Add(user);
        }

        public async Task<ToDoUser?> GetUser(Guid userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return _users.FirstOrDefault(t => t.UserId == userId);
        }

        public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return _users.FirstOrDefault(t => t.TelegramUserId == telegramUserId);
        }
    }
}
