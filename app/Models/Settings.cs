namespace Models;

public class Settings
{
    public DbSettings DbSettings { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public AuthOptions AuthOptions { get; set; } = null!;

}
public class AuthOptions
{
    public string ISSUER { get; set; } = null!;
    public string AUDIENCE { get; set; } = null!;
    public string Key { get; set; } = null!;
}