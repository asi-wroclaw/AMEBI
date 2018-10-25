using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AMEBI.Domain.Configs;
using AMEBI.Domain.Extensions;
using AMEBI.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AMEBI.Domain.Services
{
    public class JwtService : IJwtService
    {
        private readonly AppConfig _config;
        public JwtService(IOptions<AppConfig> config)
        {
            _config = config.Value;
        }

        public JwtToken CreateToken(Guid userId, string role)
        {
            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToTimestamp().ToString(), ClaimValueTypes.Integer64)
            };

            var expires = now.AddMinutes(30);
            var signiagCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.IssuerSigningKey)), SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken
            (
                issuer: _config.ValidIssuer,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: signiagCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JwtToken
            {
                Token = token,
                Expires = expires.ToTimestamp()
            };
        }
    }
}