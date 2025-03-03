using Models;

namespace Services.Interfaces;

public interface IUserService
{
    public Task<Response?> CreateAsync(NewUserDto user);
    public Task<UserDto?> GetAsync(Guid id);
    public Task<List<UserDto>?> SearchUsersAsync(string firstName, string lastName);
}
