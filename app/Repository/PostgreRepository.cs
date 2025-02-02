using Models;
using Services;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data.Common;

namespace Repository;

public class PostgreRepository : IRepository
{
    private readonly IOptions<Settings> _settings;

    public PostgreRepository(IOptions<Settings> settings)
    {
        _settings = settings;
    }

    public async Task<Guid> CreateAsync(NewUserDto user)
    {
        var id = Guid.NewGuid();
        string queue =
           "INSERT INTO public.\"User\"\n(\"Id\",\"Login\",\"Password\",\"FirstName\",\"LastName\",\"BirthDay\",\"Sex\",\"Interests\",\"City\")\n" +
           "VALUES(@id,@login,@password,@firsName,@lastName,@birthDay,@sex,@interests,@city);";
        
        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.AddRange( new NpgsqlParameter[] 
        {
            new NpgsqlParameter("@id", id),
            new NpgsqlParameter("@login", user.Login),
            new NpgsqlParameter("@password", user.Password),
            new NpgsqlParameter("@firsName", user.FirstName),
            new NpgsqlParameter("@lastName", user.LastName),
            new NpgsqlParameter("@birthDay", user.BirthDay),
            new NpgsqlParameter("@sex", (int)user.Sex),
            new NpgsqlParameter("@interests", user.Interests),
            new NpgsqlParameter("@city", user.City),
        });
        
        return await com.ExecuteNonQueryAsync() == 1 ? id : Guid.Empty;
    }

    public async Task<AccountDto?> GetAdminUnitAsync(string name)
    {
        string queue = "Select \"Id\",\"Login\",\"Password\" from \"User\" where \"Login\"=@name";

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.Add(new NpgsqlParameter("@name", name));

        var reader = await com.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }
        return new AccountDto
        {
            Id = reader.GetGuid(0),
            Login = reader.GetString(1),
            Password = reader.GetString(2),
        };
    }

    public async Task<UserDto?> GetAsync(Guid id)
    {
        string queue = "Select \"Id\",\"FirstName\",\"LastName\",\"BirthDay\",\"Sex\",\"Interests\",\"City\" from \"User\" where \"Id\"=@id";

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.Add(new NpgsqlParameter("@id", id));

        var reader = await com.ExecuteReaderAsync();
        if(!await reader.ReadAsync())
        {
            return null;
        }

        return new UserDto
        {
            Id = reader.GetGuid(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            BirthDay = reader.GetDateTime(3),
            Sex =(Sex)reader.GetInt32(4),
            Interests=reader.GetString(5),
            City=reader.GetString(6),
        };
    }

    public async Task<List<UserDto>?> SearchUsersAsync(string firstName, string lastName)
    {
        string queue = """
            Select "Id","FirstName","LastName","BirthDay","Sex","Interests","City" from "User"
            where "FirstName" like @firstName and "LastName" like @lastName
            order by "Id"
            """;

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.Add(new NpgsqlParameter("@firstName", firstName + "%"));
        com.Parameters.Add(new NpgsqlParameter("@lastName", lastName + "%"));

        var reader = await com.ExecuteReaderAsync();
        var list = new List<UserDto>();
        while(await reader.ReadAsync())
        {
            list.Add(
                new UserDto
                {
                    Id = reader.GetGuid(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    BirthDay = reader.GetDateTime(3),
                    Sex = (Sex)reader.GetInt32(4),
                    Interests = reader.GetString(5),
                    City = reader.GetString(6),
                }
                );
        }
        return list;
    }
}
