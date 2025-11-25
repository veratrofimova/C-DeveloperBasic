using DZ_Lessons.Core.DataAccess;
using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken token)
        {
            var existingUser = await _userRepository.GetUserByTelegramUserId(telegramUserId);
            if (existingUser != null)
            {
                return existingUser;
            }

            token.ThrowIfCancellationRequested();

            var user = new ToDoUser(telegramUserId, telegramUserName);
            await _userRepository.Add(user);

            return user;
        }

        public async Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return await _userRepository.GetUserByTelegramUserId(telegramUserId);
        }
    }
}
