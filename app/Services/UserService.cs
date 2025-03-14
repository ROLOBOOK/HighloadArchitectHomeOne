﻿using Models;
using Services.Interfaces;
using System.Security.Claims;

namespace Services;

public class UserService: IUserService
{
    private readonly IRepository _repository;
    private readonly ISecuriteService _passwordService;

    public UserService(IRepository repository, ISecuriteService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<Response?> CreateAsync(NewUserDto user)
    {
        user.Password = _passwordService.HashPasword(user.Password);
        var id = await _repository.CreateUserAsync(user);
        if (id != Guid.Empty)
        {
            return new Response
            {
                Id = id,
                Login = user.Login,
                Token = _passwordService.GetJwtSecurityToken(new Claim(ClaimTypes.Name, user.Login, user.Password)),
            };
        }
        return null;
    }

    public async Task<UserDto?> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return null;
        }
        return await _repository.GetUserAsync(id);
    }

    public async Task<List<UserDto>?> SearchUsersAsync(string firstName, string lastName)
    {
        if(string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
        {
            return null;
        }

        return await _repository.SearchUsersAsync(firstName, lastName);
    }
}
