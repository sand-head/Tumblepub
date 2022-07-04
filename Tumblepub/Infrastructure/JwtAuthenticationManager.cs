using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tumblepub.Application.Aggregates;

namespace Tumblepub.Infrastructure;

public record JwtTokenConfig(string Issuer, string Secret, int AccessTokenExpiration, int RefreshTokenExpiration);
public record JwtAuthResult(string AccessToken, RefreshToken RefreshToken);
public record RefreshToken(string Username, string Token, DateTime ExpireAt);

public class JwtAuthenticationManager
{
    public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => _usersRefreshTokens.ToImmutableDictionary();
    // todo: store in database
    private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens;
    private readonly JwtTokenConfig _jwtTokenConfig;
    private readonly byte[] _secret;

    public JwtAuthenticationManager(JwtTokenConfig jwtTokenConfig)
    {
        _jwtTokenConfig = jwtTokenConfig;
        _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
        _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
    }

    public JwtAuthResult GenerateTokens(User user)
    {
        var now = DateTime.UtcNow;
        var jwtToken = new JwtSecurityToken(
            issuer: _jwtTokenConfig.Issuer,
            claims: new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email)
            },
            expires: now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        var refreshToken = new RefreshToken(user.Email, GenerateRefreshTokenString(), now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration));
        _usersRefreshTokens.AddOrUpdate(refreshToken.Token, refreshToken, (s, t) => refreshToken);

        return new JwtAuthResult(accessToken, refreshToken);
    }

    private static string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
