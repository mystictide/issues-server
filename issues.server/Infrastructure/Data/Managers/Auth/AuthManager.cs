using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Data.Repo.Auth;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Interface.Auth;

namespace issues.server.Infrastructure.Data.Managers.Auth
{
    public class AuthManager : AppSettings, IAuth
    {
        private readonly IAuth _repo;
        public AuthManager()
        {
            _repo = new AuthRepository();
        }

        private string generateToken(int ID, List<int> Role)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(GetSecret());
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                    new Claim("id", ID.ToString()),
                    new Claim("role", JsonSerializer.Serialize(Role))
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

        public async Task<Companies>? Register(Companies entity)
        {
            if (entity.Name == null || entity.Email == null || entity.Password == null)
            {
                throw new Exception("Company information missing");
            }

            bool userExists = await CheckEmail(true, entity.Email, null);
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
                var admin = new Companies();
                admin.Name = entity.Name;
                admin.Email = entity.Email;
                admin.Token = generateToken(result.ID, [1]);
                return admin;
            }
            throw new Exception("Server error.");
        }

        public async Task<Companies>? CompanyLogin(Companies entity)
        {
            if (entity.Email == null || entity.Password == null)
            {
                throw new Exception("Company information missing");
            }

            var result = await _repo.CompanyLogin(entity);

            if (result != null && BCrypt.Net.BCrypt.Verify(entity.Password, result.Password))
            {
                var admin = new Companies();
                admin.ID = result.ID;
                admin.Name = result.Name;
                admin.Email = result.Email;
                admin.Token = generateToken(result.ID, [1]);
                return admin;
            }

            throw new Exception("Invalid credentials");
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
                var user = new Users();
                user.ID = result.ID;
                user.FirstName = result.FirstName;
                user.LastName = result.LastName;
                user.Email = result.Email;
                user.Token = generateToken(result.ID, result.Role.Attributes);
                return user;
            }

            throw new Exception("Invalid credentials");
        }

        public async Task<bool> CheckEmail(bool company, string Email, int? UserID)
        {
            return await _repo.CheckEmail(company, Email, UserID);
        }
    }
}
