using authentication_server.Configurations;
using authentication_server.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;

namespace authentication_server.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfig jwtConfig;
        private readonly JwtSecurityTokenHandler jwtHandler;
        private readonly byte[] signKey;

        public JwtService(
            IOptions<JwtConfig> jwtConfig, 
            JwtSecurityTokenHandler jwtHandler
        )
        {
            this.jwtConfig = jwtConfig.Value;
            this.jwtHandler = jwtHandler;
            signKey = Encoding.UTF8.GetBytes(this.jwtConfig.TokenKey);
        }

        public string GetToken(IEnumerable<Claim> claims, DateTime expirationTime)
        {
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(signKey), SecurityAlgorithms.HmacSha256
                )
            };

            SecurityToken token =  jwtHandler.CreateToken(tokenDescriptor);
            return jwtHandler.WriteToken(token);
        }

        public bool IsTokenValid(string token)
        {
            try
            {
                jwtHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(signKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);

                // If validation passes, return true
                return true;
            }
            catch
            {
                // Token is invalid or expired
                return false;
            }
        }

        public string? GetUserEmailFromToken(string token)
        {
            JwtSecurityToken? jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;
            
            if (jwtToken is null)
            {
                return null;
            }

            Claim? emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
            return emailClaim?.Value;
        }

    }
}
