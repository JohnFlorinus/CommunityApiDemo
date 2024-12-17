using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CommunityApiDemo.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace CommunityApiDemo.Services
{
    public class JWTService
    {
        private readonly string? _key;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly IHttpContextAccessor _contextAccessor;

        public JWTService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _key = configuration.GetValue<string>("JWT:Key");
            _issuer = configuration.GetValue<string>("JWT:Issuer");
            _audience = configuration.GetValue<string>("JWT:Audience");
            _contextAccessor = httpContextAccessor;
        }

        public async Task<string> CreateJwtToken(int accountID, string accountName)
        {
            var claims = new List<Claim> {
        new Claim("accountid", accountID.ToString()),
        new Claim("accountname", accountName),
    };

            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                audience: _audience,
                issuer: _issuer,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(_key)
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public string GetJwtClaim(string claimName)
        {
            try
            {
                string token = GetJwtToken();
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
                return claimValue;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string? GetJwtToken()
        {
            // Retrieve the Authorization header
            var authHeader = _contextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            // Check if the Authorization header contains a Bearer token
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }
    }
}
