using DZ_Lessons.Core.Entities;

namespace DZ_Lessons.Core.DataAccess
{
    public interface IUserRepository
    {
        Task<ToDoUser?> GetUser(Guid userId, CancellationToken _token = default);
        Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken _token = default);
        Task Add(ToDoUser user, CancellationToken _token = default);
    }
}
