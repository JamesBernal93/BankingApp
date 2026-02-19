namespace BankingApp.Domain.Entities;

public class User
{
    public string Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public User(string username, string email, string passwordHash)
    {
        Id = Guid.NewGuid().ToString();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }
}
