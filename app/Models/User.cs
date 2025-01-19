namespace Models;

public enum Sex { Man=0, Woman=1 }

public class User
{
    public Guid Id{ get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDay{ get; set; }
    public Sex Sex { get; set; }
    public string? Interests { get; set; }
    public string? City { get; set; }
}

public class AccountDto
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class NewUserDto
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDay { get; set; }
    public Sex Sex { get; set; }
    public string? Interests { get; set; }
    public string? City { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDay { get; set; }
    public Sex Sex { get; set; }
    public string? Interests { get; set; }
    public string? City { get; set; }
}