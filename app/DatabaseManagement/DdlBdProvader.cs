using Models;
using Npgsql;
using System.Data.Common;

namespace DatabaseManagement;

internal static class DdlBdProvader
{
    public static async Task CreateIfNotExistsAsync(DbSettings bdSettings)
    {
        try
        {
            var dbSettingsDefault = new DbSettings
            {
                Server = bdSettings.Server,
                Port = bdSettings.Port,
            };
            using var con = new NpgsqlConnection(dbSettingsDefault.ToString());
            await con.OpenAsync();
            using DbCommand com = new NpgsqlCommand();
            com.Connection = con;

            if (!await BdExistsAsync(com, bdSettings.BdName))
            {
                await CreateDbAsync(com, bdSettings);
                await Console.Out.WriteLineAsync($"Create bd {bdSettings.BdName}");
            }
            await CreateTablesAsync(bdSettings);

        }
        catch
        {

        }
    }

    private static async Task<bool> BdExistsAsync(DbCommand com, string bdName)
    {
        com.CommandText = "SELECT datname FROM pg_database where datname=@datname";
        com.Parameters.Add(new NpgsqlParameter("@datname", bdName));
        var datname = await com.ExecuteScalarAsync();

        return !string.IsNullOrWhiteSpace(datname?.ToString());
    }

    private static async Task CreateDbAsync(DbCommand com, DbSettings bdSettings)
    {
        com.CommandText = $"CREATE DATABASE {bdSettings.BdName}";
        await com.ExecuteScalarAsync();

        com.CommandText = $"CREATE USER {bdSettings.User} WITH PASSWORD '{bdSettings.Password}'";
        await com.ExecuteScalarAsync();

        com.CommandText = $"ALTER USER {bdSettings.User} WITH SUPERUSER";
        await com.ExecuteScalarAsync();

        com.CommandText = $"GRANT ALL PRIVILEGES ON DATABASE {bdSettings.BdName} to {bdSettings.User}";
        await com.ExecuteScalarAsync();
    }

    private static async Task CreateTablesAsync(DbSettings bdSettings)
    {
        var dic = new Dictionary<string, string>
        {
            ["User"] = """
            CREATE table IF NOT EXISTS  public."User" (
            	"Id" uuid DEFAULT uuid_generate_v4() NOT NULL,
                "Login" varchar(250) DEFAULT ''::character varying NOT NULL,
                "Password" varchar(250) DEFAULT ''::character varying NOT NULL,
            	"FirstName" varchar(250) DEFAULT ''::character varying NOT NULL,
            	"LastName" varchar(250) DEFAULT ''::character varying NOT NULL,
            	"BirthDay" timestamp DEFAULT timezone('utc'::text, CURRENT_TIMESTAMP) NULL,
            	"Sex" int4 DEFAULT 0 NOT NULL,
            	"Interests" text DEFAULT ''::character varying NOT NULL,
            	"City" varchar(250) DEFAULT ''::character varying NOT NULL,

            	CONSTRAINT "PK4Ko3lzDeEQLWtgJH5fdZO3BQIxc" PRIMARY KEY ("Id"),
                CONSTRAINT adminunit_unique UNIQUE ("Login")
            );
            """,
            ["Friend"] = """
            CREATE TABLE IF NOT EXISTS public."Friend" (
            	"Id" uuid DEFAULT uuid_generate_v4() NOT NULL,
            	"UserId" uuid NOT NULL,
            	"FriendId" uuid NOT NULL,
            	CONSTRAINT "PK4Ko3lzDeEQLWtgJH5fdZO3BQIxx" PRIMARY KEY ("Id"),
            	CONSTRAINT friend_unique UNIQUE ("UserId", "FriendId"),
            	CONSTRAINT "FK4vuAoQFpJNm3KVrfzlDHOztOhk" FOREIGN KEY ("UserId") REFERENCES public."User"("Id"),
            	CONSTRAINT "FK7FiIMVIMNS5l5GKCLP3pqpjE8yY" FOREIGN KEY ("FriendId") REFERENCES public."User"("Id")
            );
            """,
            ["Post"] = """
            CREATE table IF NOT EXISTS public."Post" (
                "Id" uuid DEFAULT uuid_generate_v4() NOT NULL,
                "CreatedOn" timestamp DEFAULT timezone('utc'::text, CURRENT_TIMESTAMP) NULL,
                "ModifiedOn" timestamp DEFAULT timezone('utc'::text, CURRENT_TIMESTAMP) NULL,
                "UserId" uuid NOT NULL,
                "Text" text DEFAULT ''::text NOT NULL,
                CONSTRAINT "PK4Ko3lzDeEQLWtgJH6fdZO3BQIxx" PRIMARY KEY ("Id"),
                CONSTRAINT "FK5vuAoQFpJNm3KVrfzlDHOztOhk" FOREIGN KEY ("UserId") REFERENCES public."User"("Id")
            );
            """,
        };

        using var con = new NpgsqlConnection(bdSettings.ToString());
        await con.OpenAsync();
        using DbCommand com = new NpgsqlCommand();
        com.Connection = con;

        com.CommandText = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"";
        await com.ExecuteScalarAsync();
        await Console.Out.WriteLineAsync("Will create tables: " + string.Join(",", dic.Keys));
        foreach (var item in dic)
        {
            com.CommandText = item.Value;
            await com.ExecuteScalarAsync();
            await Console.Out.WriteLineAsync($"Create table {item.Key}");
        }
    }
}
