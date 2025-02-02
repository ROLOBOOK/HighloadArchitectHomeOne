using Models;

namespace Services;

public interface IRepository
{
    public Task<Guid> CreateAsync(NewUserDto user);
    public Task<UserDto?> GetAsync(Guid id);
    public Task<AccountDto?> GetAdminUnitAsync(string name);
    public Task<List<UserDto>?> SearchUsersAsync(string firstName, string lastName);
}
