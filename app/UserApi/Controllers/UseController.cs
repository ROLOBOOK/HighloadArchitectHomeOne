using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace UserApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Produces("application/json")]
public class UseController : Controller
{
    private readonly IUserService _userService;

    public UseController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("user/register")]
    public async Task<IActionResult> Register(NewUserDto user)
    {
        string responseMessage = "fail register user";
        try
        {
            var response = await _userService.CreateAsync(user);
            if(response !=null)
            {
                return Ok(response);
            }
        }
        catch (Exception ex)
        {
            responseMessage = ex.Message;
        }

        return BadRequest(responseMessage);
    }


    [Authorize]
    [HttpGet("user/get/{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        try
        {
            var user = await _userService.GetAsync(id);
            if(user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
