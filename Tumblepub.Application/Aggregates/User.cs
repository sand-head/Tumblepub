using Isopoh.Cryptography.Argon2;
using Tumblepub.Application.Events;

namespace Tumblepub.Application.Aggregates;

public class User : Aggregate<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public User() { }

    public User(string email, string password)
    {
        var passwordHash = Argon2.Hash(password).TrimEnd('\0');
        var postCreated = new UserCreated(Guid.NewGuid(), email, passwordHash, DateTimeOffset.UtcNow);
        Enqueue(postCreated);
        
        Id = postCreated.UserId;
        Email = postCreated.Email;
        PasswordHash = postCreated.PasswordHash;
        UpdatedAt = CreatedAt = postCreated.At;
    }

    public User(UserCreated e)
    {
        Id = e.UserId;
        Email = e.Email;
        PasswordHash = e.PasswordHash;
        UpdatedAt = CreatedAt = e.At;
    }
    
    internal bool ShouldDelete(UserDeleted e) => true;
}