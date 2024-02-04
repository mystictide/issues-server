using System.Text;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using issues.server.Infrastructure.Models.Helpers;

namespace issues.server.Infrastructure.Helpers
{
    public class AuthHelpers : AppSettings
    {
        public static bool Authorize(HttpContext context, int[] AuthorizedRoles)
        {
            return ValidateToken(ReadBearerToken(context), AuthorizedRoles);
        }
        public static int CurrentUserID(HttpContext context)
        {
            return ValidateUser(ReadBearerToken(context));
        }

        public static string? ReadBearerToken(HttpContext context)
        {
            try
            {
                string header = (string)context.Request.Headers["Authorization"];
                if (header != null)
                {
                    return header.Substring(7);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool ValidateToken(string? encodedToken, int[] AuthorizedRoles)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(GetSecret());
                tokenHandler.ValidateToken(encodedToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                int[]? roleAttributes = JsonConvert.DeserializeObject<int[]>(jwtToken.Claims.First(x => x.Type == "role").Value);
                if (AuthorizedRoles.Any(l2 => roleAttributes.Contains(l2)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int ValidateUser(string? encodedToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(GetSecret());
                tokenHandler.ValidateToken(encodedToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}