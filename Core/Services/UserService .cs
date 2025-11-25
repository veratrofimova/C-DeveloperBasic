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

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            var existingUser = _userRepository.GetUserByTelegramUserId(telegramUserId);
            if (existingUser != null)
            {
                return existingUser;
            }

            var user = new ToDoUser(telegramUserId, telegramUserName);
            _userRepository.Add(user);
            return user;
        }

        public ToDoUser? GetUser(long telegramUserId)
        {
            return _userRepository.GetUserByTelegramUserId(telegramUserId);
        }
    }
}
