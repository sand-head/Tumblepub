using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;
using Tumblepub.Application.Events;

namespace Tumblepub.Application.Aggregates;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Blog : Aggregate<Guid>
{
    public Guid? UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public JsonDocument? Metadata { get; set; }
    public string PublicKey { get; set; } = default!;
    public string? PrivateKey { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public Blog() { }

    public Blog(Guid userId, string name)
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
        
        var blogCreated = new BlogCreated(Guid.NewGuid(), userId, name, publicKey, privateKey, DateTimeOffset.UtcNow);
        Enqueue(blogCreated);
        
        Id = blogCreated.BlogId;
        UserId = blogCreated.UserId;
        Name = blogCreated.BlogName;
        PublicKey = blogCreated.PublicKey;
        PrivateKey = blogCreated.PrivateKey;
        UpdatedAt = CreatedAt = blogCreated.At;
    }

    public Blog(BlogCreated e)
    {
        Id = e.BlogId;
        UserId = e.UserId;
        Name = e.BlogName;
        PublicKey = e.PublicKey;
        PrivateKey = e.PrivateKey;
        UpdatedAt = CreatedAt = e.At;
    }
    public Blog(BlogDiscovered e)
    {
        Id = e.BlogId;
        Name = e.BlogName;
        PublicKey = e.PublicKey;
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