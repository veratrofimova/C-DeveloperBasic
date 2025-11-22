using DZ_Lessons.Infrastracture.Interface;

namespace DZ_Lessons.Infrastracture
{
    public class UserService : IUserService
    {
        private readonly Dictionary<long, ToDoUser> _users = new();

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            if (_users.ContainsKey(telegramUserId))
            {
                return _users[telegramUserId];
            }

            var user = new ToDoUser(telegramUserId, telegramUserName);
            _users[telegramUserId] = user;
            return user;
        }

        public ToDoUser? GetUser(long telegramUserId)
        {
            return _users.ContainsKey(telegramUserId) ? _users[telegramUserId] : null;
        }
    }
}
