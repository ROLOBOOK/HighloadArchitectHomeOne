using Services.Interfaces;

namespace Services;

public class FriendService : IFriendService
{
    private readonly IRepository _repository;

    public FriendService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> AddAsync(string? name, Guid id)
    {
        if (string.IsNullOrWhiteSpace(name) || id == Guid.Empty)
        {
            return false;
        }
        return await _repository.AddFriend(name, id);
    }

    public async Task<bool> DeleteAsync(string? name, Guid id)
    {
        if (string.IsNullOrWhiteSpace(name) || id == Guid.Empty)
        {
            return false;
        }
        return await _repository.DeleteFriend(name, id);

    }
}
