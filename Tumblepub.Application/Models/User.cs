using StronglyTypedIds;
using Tumblepub.Application.Events;

namespace Tumblepub.Application.Models;

[StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.NewtonsoftJson)]
public partial struct UserId { }

public class User : Aggregate<UserId>
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public static User Create(string email, string passwordHash)
    {
        return new User(email, passwordHash);
    }

    private User(string email, string passwordHash)
    {
        var postCreated = new UserCreated(UserId.New(), email, passwordHash, DateTimeOffset.UtcNow);
        
        Enqueue(postCreated);
        Apply(postCreated);
    }

    internal User(UserCreated e)
    {
        Apply(e);
    }
    
    internal void Apply(UserCreated e)
    {
        Id = e.UserId;
        Email = e.Email;
        PasswordHash = e.PasswordHash;
        UpdatedAt = CreatedAt = e.At;
    }
}