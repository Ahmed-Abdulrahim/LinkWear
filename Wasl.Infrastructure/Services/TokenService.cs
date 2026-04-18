namespace Wasl.Infrastructure.Services
{
    public class TokenService(IOptions<JwtSettings> _jwtSettings) : ITokenService
    {
        private readonly JwtSettings jwtSettings = _jwtSettings.Value;
        public Task<string> GenerateAccesToken(ApplicationUser applicationUser, IList<string> roles)
        {
            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Email , applicationUser.Email ?? string.Empty),
              new Claim(JwtRegisteredClaimNames.Jti ,  Guid.NewGuid().ToString()),
              new Claim(ClaimTypes.NameIdentifier  ,applicationUser.Id.ToString()),
              new Claim(ClaimTypes.Email  ,applicationUser.Email!),
              new Claim("IsDeleted", applicationUser.IsDeleted.ToString().ToLower())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey") ?? jwtSettings.SecretKey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
           issuer: jwtSettings.Issuer,
           audience: jwtSettings.Audience,
           claims: claims,
           expires: GetAccessTokenExpiration(),
           signingCredentials: credential
       );
            var writeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(writeToken);
        }

        public string GenerateRefreshtoken()
        {
            var randomNum = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNum);
            return Convert.ToBase64String(randomNum);
        }

        public DateTime GetAccessTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey") ?? jwtSettings.SecretKey))
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public DateTime GetRefreshTokenExpiration()
        {
            return DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays);
        }
    }
}
