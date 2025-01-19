namespace Models;

public class DbSettings
{

    public int Port { get; set; } = 5432;
    public string User { get; set; } = "postgres";
    public string Password { get; set; } = "postgres";
    public string BdName { get; set; } = "postgres";
    public string Server { get; set; } = "localhost";

    public override string ToString()
    {
        return $"Server={Server};Port={Port};Database={BdName.ToLower()};User ID={User};password={Password}";
    }
}
