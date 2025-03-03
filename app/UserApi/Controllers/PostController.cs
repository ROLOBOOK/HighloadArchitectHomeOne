using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Services.Interfaces;

namespace UserApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Produces("application/json")]
public class PostController : Controller
{
    private readonly IPostService _service;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostController(IPostService service, IHttpContextAccessor httpContextAccessor)
    {
        _service = service;
        _httpContextAccessor = httpContextAccessor;
    }

    [Authorize]
    [HttpPost("post/create")]
    public async Task<IActionResult> Create([FromBody] PostDto post)
    {
        string responseMessage = "fail add Post";
        try
        {
            var id = await _service.AddAsync(_httpContextAccessor.HttpContext?.User.Identities.First()?.Name, post);
            if (id != Guid.Empty)
            {
                return Ok(id);
            }
        }
        catch (Exception ex)
        {
            responseMessage = ex.Message;
        }
        return BadRequest(responseMessage);
    }

    [Authorize]
    [HttpPost("post/update")]
    public async Task<IActionResult> Update([FromBody] PostDto post)
    {
        string responseMessage = "fail add Post";
        try
        {
            if (await _service.UpdateAsync(_httpContextAccessor.HttpContext?.User.Identities.First()?.Name, post))
            {
                return Ok("Post update");
            }
        }
        catch (Exception ex)
        {
            responseMessage = ex.Message;
        }
        return BadRequest(responseMessage);
    }

    [Authorize]
    [HttpPost("post/delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        string responseMessage = "fail delete Post";
        try
        {
            if (await _service.DeleteAsync(_httpContextAccessor.HttpContext?.User.Identities.First()?.Name, id))
            {
                return Ok("Post delete");
            }
        }
        catch (Exception ex)
        {
            responseMessage = ex.Message;
        }
        return BadRequest(responseMessage);
    }

    [Authorize]
    [HttpGet("post/get/{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        string responseMessage = "fail get Post";
        try
        {
            var post = await _service.GetAsync(id);
            if (post is not null)
            {
                return Ok(post);
            }
        }
        catch (Exception ex)
        {
            responseMessage = ex.Message;
        }
        return BadRequest(responseMessage);
    }

    [Authorize]
    [HttpGet("post/feed")]
    public async Task<IActionResult> Feed([FromQuery] int offset, [FromQuery] int limit)
    {
        string responseMessage = "fail Feed post";
        try
        {
            var list = await _service.FeedAsync(offset, limit);
            if (list != null)
            {
                return Ok(list);
            }
        }
        catch (Exception ex)
        {
            responseMessage = ex.Message;
        }
        return BadRequest(responseMessage);
    }
}
