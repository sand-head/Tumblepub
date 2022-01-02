using System.Security.Cryptography;

namespace Tumblepub.Infrastructure;

public static class CryptoUtils
{
    /// <summary>
    /// Generates a PEM public & private key pair from a 2048-bit RSA key.
    /// </summary>
    public static (string PublicKey, string PrivateKey) CreateKeyPair()
    {
        var rsa = RSA.Create(2048);

        // get the appropriately formatted public and private key bits
        // these are chunked into 64 character lines to conform with RFC 1421
        var publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()).Chunk(64);
        var privateKey = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey()).Chunk(64);

        return (
            $"-----BEGIN RSA PUBLIC KEY-----\n{string.Join('\n', publicKey)}\n-----END RSA PUBLIC KEY-----\n",
            $"-----BEGIN RSA PRIVATE KEY-----\n{string.Join('\n', privateKey)}\n-----END RSA PRIVATE KEY-----\n");
    }
}
