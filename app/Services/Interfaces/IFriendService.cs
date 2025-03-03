using Models;

namespace Services.Interfaces;

public interface IFriendService
{
    Task<bool> AddAsync(string? login, Guid id);
    Task<bool> DeleteAsync(string? login, Guid id);
}
