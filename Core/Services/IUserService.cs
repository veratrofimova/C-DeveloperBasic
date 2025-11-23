using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Core.Services
{
    public interface IUserService
    {
        Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken _token = default);
        Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken _token = default);
    }
}
