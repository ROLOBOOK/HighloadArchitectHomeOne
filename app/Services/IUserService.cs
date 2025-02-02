using Models;

namespace Services;

public interface IUserService
{
    public Task<Response?> CreateAsync(NewUserDto user);
    public Task<UserDto?> GetAsync(Guid id);
    public Task<List<UserDto>?> SearchUsersAsync(string firstName, string lastName);
}
