using Models;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data.Common;
using Services.Interfaces;

namespace Repository;

public class PostgreRepository : IRepository
{
    private readonly IOptions<Settings> _settings;

    public PostgreRepository(IOptions<Settings> settings)
    {
        _settings = settings;
    }

    public async Task<bool> AddFriend(string name, Guid id)
    {
        string queue =
           """
            INSERT INTO public."Friend" ( "UserId", "FriendId")
           VALUES((select "Id" from "User" where "Login"=@login) , @id);
           """;

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.AddRange(new NpgsqlParameter[]
        {
            new NpgsqlParameter("@login", name),
            new NpgsqlParameter("@id", id),
        });
        return await com.ExecuteNonQueryAsync() != 0;
    }

    public async Task<bool> DeleteFriend(string name, Guid id)
    {
        string queue =
           """
            Delete from  public."Friend"
           where "UserId"=(select "Id" from "User" where "Login"=@login)  and "FriendId" = @id ;
           """;

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.AddRange(new NpgsqlParameter[]
        {
            new NpgsqlParameter("@login", name),
            new NpgsqlParameter("@id", id),
        });
        return await com.ExecuteNonQueryAsync() != 0;
    }


    public async Task<Guid> CreateUserAsync(NewUserDto user)
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

    public async Task<UserDto?> GetUserAsync(Guid id)
    {
        string queue = "Select \"Id\",\"FirstName\",\"LastName\",\"BirthDay\",\"Sex\",\"Interests\",\"City\" from \"User\" where \"Id\"=@id";

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.Add(new NpgsqlParameter("@id", id));

        var reader = await com.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new UserDto
        {
            Id = reader.GetGuid(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            BirthDay = reader.GetDateTime(3),
            Sex = (Sex)reader.GetInt32(4),
            Interests = reader.GetString(5),
            City = reader.GetString(6),
        };
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

    public async Task<Guid> AddPostAsync(string name, PostDto post)
    {
        string queue =
           """
            INSERT INTO public."Post"
           ("Id","UserId", "Text")
           VALUES(@id, (select "Id" from "User" where "Login"=@login), @text)
           RETURNING "Id";
           """;

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.AddRange(new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", post.Id == Guid.Empty ? Guid.NewGuid() : post.Id),
            new NpgsqlParameter("@login", name),
            new NpgsqlParameter("@text", post.Text),
        });
        if(await com.ExecuteScalarAsync() is Guid id)
        {
            return id;
        }
        return Guid.Empty;
    }
    
    public async Task<bool> DeletePostAsync(string name, Guid id)
    {
        string queue =
           """
           delete from public."Post"  where "Id"=@id and "UserId"=(select "Id" from "User" where "Login"=@login)
           """;

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.AddRange(new NpgsqlParameter[]
        {
            new NpgsqlParameter("@login", name),
            new NpgsqlParameter("@id", id),
        });

        return await com.ExecuteNonQueryAsync() != 0;
    }

    public async Task<bool> UpdatePost(string name, PostDto post)
    {
        string queue =
           """
           update  public."Post"set "Text"=@text,"ModifiedOn"=@modifiedOn
           where "Id"=@id and "UserId"=(select "Id" from "User" where "Login"=@login)
           """;

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.AddRange(new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", post.Id),
            new NpgsqlParameter("@login", name),
            new NpgsqlParameter("@text", post.Text),
            new NpgsqlParameter("@modifiedOn", DateTime.UtcNow),
        });

        return await com.ExecuteNonQueryAsync() != 0;
    }

    public async Task<PostDto?> GetPostAsync(Guid id)
    {
        string queue = """
            Select "Id", "Text" from "Post" where "Id"=@id
            """;
        
        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);
        com.Parameters.Add(new NpgsqlParameter("@id", id));

        var reader = await com.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new PostDto
        {
            Id = reader.GetGuid(0),
            Text = reader.GetString(1),
        };
    }
    
    public async Task<List<PostDto>> FeedPostAsync()
    {
        string queue = """
            Select "Id", "Text"  from "Post" order by "ModifiedOn" desc limit 1000
            """;

        using var con = new NpgsqlConnection(_settings.Value.DbSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand(queue, con);

        var reader = await com.ExecuteReaderAsync();
        var list = new List<PostDto>();
        while(await reader.ReadAsync())
        {
            list.Add(new PostDto
            {
                Id = reader.GetGuid(0),
                Text = reader.GetString(1),
            });
        }
        return list;
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
