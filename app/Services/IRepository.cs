using Models;

namespace Services;

public interface IRepository
{
    public Task<Guid> CreateAsync(NewUserDto user);
    public Task<UserDto?> GetAsync(Guid id);
    public Task<AccountDto?> GetAdminUnitAsync(string name);
}
