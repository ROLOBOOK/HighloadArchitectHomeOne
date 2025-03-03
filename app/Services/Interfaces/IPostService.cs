using Models;

namespace Services.Interfaces;

public interface IPostService
{
    Task<Guid> AddAsync(string? name, PostDto post);
    Task<bool> DeleteAsync(string? name, Guid id);
    Task<List<PostDto>?> FeedAsync(int offset, int limit);
    Task<PostDto?> GetAsync(Guid id);
    Task<bool> UpdateAsync(string? name, PostDto post);
}
