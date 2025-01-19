
using Models;

namespace DatabaseManagement;

internal static class Program
{
    static async Task Main(string[] args)
    {
        await Task.Delay(TimeSpan.FromSeconds(5)); 
        
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

            int.TryParse(Environment.GetEnvironmentVariable("countFakeData"), out int count);
            await DmlDbProvader.SetFakeData(settings, count);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Error. {settings}\n{ex.Message}\n{ex.StackTrace}");
        }
        
    }

    static string GetEnv(string name)
            => Environment.GetEnvironmentVariable(name) ?? throw new Exception($"Not set EnvironmentVariable {name}");
}
