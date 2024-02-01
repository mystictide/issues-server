using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Data.Repo.Main;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Managers.Main
{
    public class UserManager : AppSettings, IUsers
    {
        private readonly IUsers _repo;
        public UserManager()
        {
            _repo = new UserRepository();
        }

        private string generateToken(Users user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(GetSecret());
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.ID.ToString()),
                    new Claim("role", user.Role.ToString())
                }),
                    Expires = DateTime.UtcNow.AddDays(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<Users>? Register(Users entity)
        {
            if (entity.FirstName == null || entity.LastName == null || entity.Email == null || entity.Password == null)
            {
                throw new Exception("User information missing");
            }

            bool userExists = await CheckEmail(entity.Email, null);
            if (userExists)
            {
                throw new Exception("Email address already registered");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password, salt);
            entity.Password = hashedPassword;

            var result = await _repo.Register(entity);
            if (result != null)
            {
                result.Token = generateToken(result);
                return result;
            }
            throw new Exception("Server error.");
        }

        public async Task<Users>? Login(Users entity)
        {
            if (entity.Email == null || entity.Password == null)
            {
                throw new Exception("User information missing");
            }

            var result = await _repo.Login(entity);

            if (result != null && BCrypt.Net.BCrypt.Verify(entity.Password, result.Password))
            {
                result.Token = generateToken(result);
                return result;
            }

            throw new Exception("Invalid credentials");
        }

        public async Task<bool> CheckEmail(string Email, int? UserID)
        {
            return await _repo.CheckEmail(Email, UserID);
        }

        public async Task<Users>? Get(int? ID, string? Username)
        {
            return await _repo.Get(ID, Username);
        }

        public async Task<string>? UpdateEmail(int ID, string Email)
        {
            return await _repo.UpdateEmail(ID, Email);
        }

        public async Task<bool>? ChangePassword(int UserID, string currentPassword, string newPassword)
        {
            var result = await _repo.Get(UserID, null);
            if (result != null && BCrypt.Net.BCrypt.Verify(currentPassword, result.Password))
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword, salt);
                return await _repo.ChangePassword(UserID, currentPassword, newPassword);
            }
            return false;
        }
    }
}
