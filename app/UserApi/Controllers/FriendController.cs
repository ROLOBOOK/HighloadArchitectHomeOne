using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace UserApi.Controllers;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Produces("application/json")]
public class FriendController : Controller
{
    private readonly IFriendService _firendService;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public FriendController(IFriendService firendService, IHttpContextAccessor httpContextAccessor)
    {
        _firendService = firendService;
        _httpContextAccessor = httpContextAccessor;
    }

    [Authorize]
    [HttpPost("friend/add/{id}")]
    public async Task<IActionResult> Add(Guid id)
    {
        string responseMessage = "fail add friend";
        try
        {
            if (await _firendService.AddAsync(_httpContextAccessor.HttpContext?.User.Identities.First()?.Name, id))
            {
                return Ok("Friend add");
            }
        }
        catch (Exception ex)
        {
            responseMessage = ex.Message;
        }
        return BadRequest(responseMessage);
    }

    [Authorize]
    [HttpPost("friend/delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        string responseMessage = "fail delete friend";
        try
        {
            if (await _firendService.DeleteAsync(_httpContextAccessor.HttpContext?.User.Identities.First()?.Name, id))
            {
                return Ok("Friend delete");
            }
        }
        catch (Exception ex)
        {
            responseMessage = ex.Message;
        }
        return BadRequest(responseMessage);
    }
}