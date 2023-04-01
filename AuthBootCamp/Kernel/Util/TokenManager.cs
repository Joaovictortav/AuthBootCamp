using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Kernel.Util
{
    public abstract class TokenManager
    {
        private static readonly string Secret = Convert.ToBase64String(Encoding.ASCII.GetBytes("UmaStringCommaisde128bits"));  // Modificar !!!  SecretManager.GetSecret()
        private static byte[] ByteSecret => Convert.FromBase64String(Secret);
        
        public static string GenerateJwt(string subjectGuid, string? actorEmail, string? actorName, DateTime? expireDate)
        {
            var securityKey = new SymmetricSecurityKey(ByteSecret);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim("subject_type", "user"),
                    new Claim(ClaimTypes.PrimarySid, subjectGuid),
                    new Claim(ClaimTypes.Email, actorEmail),
                    new Claim(ClaimTypes.GivenName, actorName),
                }),
                Expires = expireDate ?? DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
     
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = (JwtSecurityToken?)tokenHandler.ReadToken(token);
                
                if (jwtToken is null)
                    return null!;
                
                var key = Convert.FromBase64String(Secret);
                var parameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                
                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null!;
            }
        }

        private static bool TryValidateToken(string token, out ClaimsIdentity? actor)
        {
            var principal = GetPrincipalFromToken(token);

            try
            {
                actor = ((ClaimsIdentity)principal.Identity!)!;
            }
            catch (NullReferenceException)
            {
                actor = null;
                return false;
            }
            return true;
        }

        public static VerifyResponse GetSubjectFromToken(string token)
        {
            if (!TryValidateToken(token, out var session))
                throw new Exception("Invalid or expired token");
				
            var subjectId = Guid.Parse(session!.FindFirst(ClaimTypes.PrimarySid)!.Value);

            var iat = long.Parse(session.FindFirst("iat")!.Value);
            var exp = long.Parse(session.FindFirst("exp")!.Value);

            return new VerifyResponse
            {
                SubjectType = session.FindFirst("subject_type")!.Value,
                SubjectGuid = subjectId,
                SubjectEmail = session.FindFirst(ClaimTypes.Email)!.Value,
                SubjectName = session.FindFirst(ClaimTypes.GivenName)!.Value,
                SubjectPhone = session.FindFirst("subject_phone")!.Value,
                IssuedAt = iat,
                ExpiresAt = exp,
            };
        }
    }

    public class VerifyResponse
    {
        public string? SubjectType { get; set; }
        public Guid SubjectGuid { get; set; }
        public string? SubjectEmail { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectPhone { get; set; }
        public long ExpiresAt { get; set; }
        public long IssuedAt { get; set; }
    }
}