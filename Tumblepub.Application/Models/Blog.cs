using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using StronglyTypedIds;
using Tumblepub.Application.Events;

namespace Tumblepub.Application.Models;

[StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.NewtonsoftJson)]
public partial struct BlogId { }

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Blog : Aggregate<BlogId>
{
    public UserId? UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public JsonDocument? Metadata { get; set; }
    public string PublicKey { get; set; } = default!;
    public string? PrivateKey { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public static Blog Create(UserId userId, string name)
    {
        // generate public/private keys and create initial BlogCreated event
        var rsa = RSA.Create(2048);

        // get the appropriately formatted public and private key bits
        // these are chunked into 64 character lines to conform with RFC 1421
        var publicKeyChunked = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()).Chunk(64).Select(c => new string(c));
        var privateKeyChunked = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey()).Chunk(64).Select(c => new string(c));

        var publicKey =
            $"-----BEGIN RSA PUBLIC KEY-----\n{string.Join('\n', publicKeyChunked)}\n-----END RSA PUBLIC KEY-----\n";
        var privateKey =
            $"-----BEGIN RSA PRIVATE KEY-----\n{string.Join('\n', privateKeyChunked)}\n-----END RSA PRIVATE KEY-----\n";
        
        return new Blog(userId, name, publicKey, privateKey);
    }

    private Blog(UserId userId, string name, string publicKey, string privateKey)
    {
        var blogCreated = new BlogCreated(BlogId.New(), userId, name, publicKey, privateKey, DateTimeOffset.UtcNow);
        
        Enqueue(blogCreated);
        Apply(blogCreated);
    }

    internal Blog(BlogCreated e)
    {
        Apply(e);
    }
    internal Blog(BlogDiscovered e)
    {
        Id = e.BlogId;
        Name = e.BlogName;
        PublicKey = e.PublicKey;
        UpdatedAt = CreatedAt = e.At;
    }


    internal void Apply(BlogCreated e)
    {
        Id = e.BlogId;
        UserId = e.UserId;
        Name = e.BlogName;
        PublicKey = e.PublicKey;
        PrivateKey = e.PrivateKey;
        UpdatedAt = CreatedAt = e.At;
    }
    internal void Apply(BlogMetadataUpdated e)
    {
        Title = e.Title ?? Title;
        Description = e.Description ?? Description;
        Metadata = e.Metadata ?? Metadata;
        Version++;
    }
}