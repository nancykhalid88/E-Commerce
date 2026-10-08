using E_Commerce.Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class TokenService(IOptions<JWTSettings>jwtOptions) : ITokenService
    {
        private readonly JWTSettings _settings = jwtOptions.Value;
        public string CreateToken(string userId, string email, string userName, IEnumerable<string> roles)
        {
            var claims = new List <Claim>
            {
                new (ClaimTypes.NameIdentifier,userId),
                new (ClaimTypes.Email,email),
                new(ClaimTypes.Name,userName)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            if (string.IsNullOrEmpty(_settings.SecretKey))
                throw new InvalidOperationException("JWT Key is missing");
            if (_settings.SecretKey.Length < 32)
                throw new InvalidOperationException("JWT Secret key is too short");
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
            var Token = new JwtSecurityToken
            (
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinuites),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(Token);

        }
    }
    public class JWTSettings {
        public string SecretKey { get; init; } = default!;
        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public int ExpirationMinuites { get; init; } = 60;
    }

}
