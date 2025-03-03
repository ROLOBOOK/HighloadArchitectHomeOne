using Microsoft.Extensions.Caching.Distributed;
using Models;
using Services.Interfaces;
using System.Text.Json;

namespace Services;

public class PostService : IPostService
{
    private readonly IRepository _repository;
    private readonly IDistributedCache _cache;

    public PostService(IRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;

    }

    public async Task<Guid> AddAsync(string? name, PostDto post)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(post.Text))
        {
            return Guid.Empty;
        }
        return await _repository.AddPostAsync(name, post);
    }

    public async Task<bool> DeleteAsync(string? name, Guid id)
    {
        if (string.IsNullOrWhiteSpace(name) || id == Guid.Empty)
        {
            return false;
        }
        return await _repository.DeletePostAsync(name, id);
    }

    public async Task<List<PostDto>?> FeedAsync(int offset, int limit)
    {
        if(offset <= 0 || limit <= 0)
        {
            return null; 
        }
        offset--;
        string cacheKey = "FeedPostDto";

        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (cachedData != null)
        {
            return JsonSerializer
                            .Deserialize<List<PostDto>>(cachedData)
                            ?.Skip(offset * limit).Take(limit).ToList();
        }

        var list = await _repository.FeedPostAsync();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(list), options);

        return list?.Skip(offset * limit).Take(limit).ToList();
    }

    public async Task<PostDto?> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return null;
        }
        return await _repository.GetPostAsync(id);
    }

    public async Task<bool> UpdateAsync(string? name, PostDto post)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(post.Text) || post.Id == Guid.Empty)
        {
            return false;
        }
        return await _repository.UpdatePost(name, post);
    }
}
