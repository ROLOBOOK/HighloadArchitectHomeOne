using Bogus;
using Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement;

internal static class DmlDbProvader
{
    private const string _insert = """ INSERT INTO public."User" ("Login","Password","FirstName","LastName","BirthDay","Sex","Interests","City") VALUES """;
    private const string _values = "(@login{0},@password{0},@firsName{0},@lastName{0},@birthDay{0},@sex{0},@interests{0},@city{0}),";

    public static async Task SetFakeData(DbSettings bdSettings, int countFakeData)
    {
        try
        {
            using var con = new NpgsqlConnection(bdSettings.ToString());
            await con.OpenAsync();
            using DbCommand com = new NpgsqlCommand();
            com.Connection = con;

            Faker<User> testUsers = GenerateFakeUser();
            StringBuilder sb = new StringBuilder();
            if(countFakeData != 0)
            {
                int oneStep = countFakeData > 100 ? 100 : countFakeData;
                int steps = countFakeData > oneStep ? countFakeData : 1;

                for (int i = 0; i < steps; i += oneStep)
                {
                    SetFakeUser(com, testUsers, sb, oneStep);
                    await com.ExecuteNonQueryAsync();
                }
                Console.WriteLine($"Set test {countFakeData} row");
            }
            await SetOneExampleUserAsync(com, testUsers);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }

    }

    private static void SetFakeUser(DbCommand com, Faker<User> testUsers, StringBuilder sb, int oneStep)
    {
        sb.Clear();
        com.Parameters.Clear();

        sb.AppendLine(_insert);
        for (int step = 0; step < oneStep; step++)
        {
            var user = testUsers.Generate();
            sb.Append(string.Format(_values, step));
            com.Parameters.AddRange(new NpgsqlParameter[]
            {
                new NpgsqlParameter($"@login{step}", user.Login),
                new NpgsqlParameter($"@password{step}", user.Password),
                new NpgsqlParameter($"@firsName{step}", user.FirstName),
                new NpgsqlParameter($"@lastName{step}", user.LastName),
                new NpgsqlParameter($"@birthDay{step}", user.BirthDay),
                new NpgsqlParameter($"@sex{step}", (int)user.Sex),
                new NpgsqlParameter($"@interests{step}", user.Interests),
                new NpgsqlParameter($"@city{step}", user.City),
             });
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append(" on conflict (\"Login\") do nothing");
        com.CommandText = sb.ToString();
    }
    
    /// <summary>
    /// Тестовый юзер example/Qwerty123
    /// </summary>
    private static async Task SetOneExampleUserAsync(DbCommand com, Faker<User> testUsers)
    {
        string _insert = """
            INSERT INTO public."User" ("Id","Login","Password","FirstName","LastName","BirthDay","Sex","Interests","City") VALUES
            (@id,@login,@password,@firsName,@lastName,@birthDay,@sex,@interests,@city)  on conflict ("Login") do nothing
            """;

        var user = testUsers.Generate();

        user.Id = Guid.Parse("e96afde8-5223-482a-8fa7-734266f3c992");
        user.Login = "example";
        user.Password = "55B947429E299D8627BE8E825965B68664AFF56638D7E7BE92B6C688572F1D1CF1B9464780BB6B07C316F9CC25111AAD167E51B5E9F39FC3021A71B0E6C6C732";

        com.Parameters.AddRange(new NpgsqlParameter[]
        {
                new NpgsqlParameter($"@id", user.Id),
                new NpgsqlParameter($"@login", user.Login),
                new NpgsqlParameter($"@password", user.Password),
                new NpgsqlParameter($"@firsName", user.FirstName),
                new NpgsqlParameter($"@lastName", user.LastName),
                new NpgsqlParameter($"@birthDay", user.BirthDay),
                new NpgsqlParameter($"@sex", (int)user.Sex),
                new NpgsqlParameter($"@interests", user.Interests),
                new NpgsqlParameter($"@city", user.City),
         });
        com.CommandText = _insert;
        await com.ExecuteNonQueryAsync();
    }

    private static Faker<User> GenerateFakeUser()
    {
        return new Faker<User>()
                        .RuleFor(u => u.Login, f => f.Person.UserName)
                        .RuleFor(u => u.Password, f => f.Random.AlphaNumeric(15))
                        .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                        .RuleFor(u => u.LastName, f => f.Name.LastName())
                        .RuleFor(u => u.BirthDay, f => f.Date.Past(15))
                        .RuleFor(u => u.Sex, f => f.Random.Enum<Sex>())
                        .RuleFor(u => u.City, f => f.Address.City())
                        .RuleFor(u => u.Interests, f => f.Lorem.Letter(2));
    }

}
