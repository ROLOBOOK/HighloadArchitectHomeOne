using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using Services;
using System.Security.Claims;

namespace UserApi.Controllers;

public class LoginController : Controller
{
    private readonly IRepository _repository;
    private readonly ISecuriteService _passwordService;

    public LoginController(IRepository repository, ISecuriteService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(string login, string password)
    {
        var user = await _repository.GetAdminUnitAsync(login);
        
        if(user is not null && _passwordService.VerifyPassword(password, user.Password))
        {
            return Ok(new Response
            {
                Token = _passwordService.GetJwtSecurityToken(new Claim(ClaimTypes.Name, user.Login, user.Password)),
                Id = user.Id,
                Login = user.Login
            });
        }
        return BadRequest(login);
    }
}