
using Models;

namespace DatabaseManagement;

internal static class Program
{
    static async Task Main(string[] args)
    {
        await Task.Delay(TimeSpan.FromSeconds(10)); 
        
        var settings = new DbSettings
        {
            Server = GetEnv("DbSettings__Server"),
            BdName = GetEnv("DbSettings__BdName"),
            Port = int.TryParse(GetEnv("DbSettings__Port"), out var port) ? port : 0,
            User = GetEnv("DbSettings__User"),
            Password = GetEnv("DbSettings__Password"),
        };

        try
        {
            await DdlBdProvader.CreateIfNotExistsAsync(settings);

            long.TryParse(Environment.GetEnvironmentVariable("countFakeUser"), out long countUser);
            long.TryParse(Environment.GetEnvironmentVariable("countFakePost"), out long countPost);
            await DmlDbProvader.SetFakeData(settings, countUser, countPost);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Error. {settings}\n{ex.Message}\n{ex.StackTrace}");
        }
        
    }

    static string GetEnv(string name)
            => Environment.GetEnvironmentVariable(name) ?? throw new Exception($"Not set EnvironmentVariable {name}");
}
