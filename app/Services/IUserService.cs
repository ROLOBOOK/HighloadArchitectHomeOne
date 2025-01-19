using Models;

namespace Services;

public interface IUserService
{
    public Task<Response?> CreateAsync(NewUserDto user);
    public Task<UserDto?> GetAsync(Guid id);
}
