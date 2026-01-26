using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankMore.Accounts.Api.Infra.Security
{
    public sealed class JwtOptions
    {
        public string Issuer { get; init; } = "BankMore";
        public string Audience { get; init; } = "BankMore";
        public string Key { get; init; } = string.Empty; // segredo
        public int ExpMinutes { get; init; } = 60;
    }

    public interface IJwtTokenService
    {
        string GenerateToken(Guid contaId, int numeroConta);
    }

    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(Guid contaId, int numeroConta)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, contaId.ToString()),
            new("accountId", contaId.ToString()),
            new("accountNumber", numeroConta.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
