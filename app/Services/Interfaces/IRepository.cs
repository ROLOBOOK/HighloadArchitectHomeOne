using Models;

namespace Services.Interfaces;

public interface IRepository
{
    public Task<Guid> CreateUserAsync(NewUserDto user);
    public Task<UserDto?> GetUserAsync(Guid id);
    public Task<AccountDto?> GetAdminUnitAsync(string name);
    public Task<List<UserDto>?> SearchUsersAsync(string firstName, string lastName);
    Task<bool> AddFriend(string name, Guid id);
    Task<bool> DeleteFriend(string name, Guid id);
    Task<Guid> AddPostAsync(string name, PostDto post);
    Task<bool> DeletePostAsync(string name, Guid id);
    Task<PostDto?> GetPostAsync(Guid id);
    Task<bool> UpdatePost(string name, PostDto post);
    Task<List<PostDto>> FeedPostAsync();
}
