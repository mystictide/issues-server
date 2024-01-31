﻿using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using issues.server.Infrastructure.Data.Repo.User;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Interface.User;

namespace issues.server.Infrastructure.Managers.Users
{
    public class UserManager : AppSettings, IUsers
    {
        private readonly IUsers _repo;
        public UserManager()
        {
            _repo = new UserRepository();
        }

        private string generateToken(Infrasructure.Models.Users.Users user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(GetSecret());
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.ID.ToString()),
                    new Claim("authType", user.AuthType.ToString())
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

        public async Task<Infrasructure.Models.Users.Users>? Register(Infrasructure.Models.Users.Users entity)
        {
            if (entity.Username == null || entity.Email == null || entity.Password == null)
            {
                throw new Exception("User information missing");
            }

            bool userExists = await CheckEmail(entity.Email, null);
            if (userExists)
            {
                throw new Exception("Email address already registered");
            }

            bool usernameExists = await CheckUsername(entity.Username, null);
            if (usernameExists)
            {
                throw new Exception("Username already exists");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password, salt);
            entity.Password = hashedPassword;
            entity.AuthType = 1;

            var result = await _repo.Register(entity);
            if (result != null)
            {
                result.AuthType = entity.AuthType;
                var user = new Infrasructure.Models.Users.Users();
                user.Username = entity.Username;
                user.Email = entity.Email;
                user.Token = generateToken(result);
                return user;
            }
            throw new Exception("Server error.");
        }

        public async Task<Infrasructure.Models.Users.Users>? Login(Infrasructure.Models.Users.Users entity)
        {
            if (entity.Email == null || entity.Password == null)
            {
                throw new Exception("User information missing");
            }

            var result = await _repo.Login(entity);

            if (result != null && BCrypt.Net.BCrypt.Verify(entity.Password, result.Password))
            {
                var user = new Infrasructure.Models.Users.Users();
                user.ID = result.ID;
                user.Username = result.Username;
                user.Email = result.Email;
                user.Token = generateToken(result);
                return user;
            }

            throw new Exception("Invalid credentials");
        }

        public async Task<bool> CheckEmail(string Email, int? UserID)
        {
            return await _repo.CheckEmail(Email, UserID);
        }

        public async Task<bool> CheckUsername(string Username, int? UserID)
        {
            return await _repo.CheckUsername(Username, UserID);
        }

        public async Task<Infrasructure.Models.Users.Users>? Get(int? ID, string? Username)
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

        public async Task<string>? UpdateBio(int ID, string Bio)
        {
            return await _repo.UpdateBio(ID, Bio);
        }

        public async Task<bool?> ManageFollow(int TargetID, int UserID)
        {
            return await _repo.ManageFollow(TargetID, UserID);
        }

        public async Task<bool?> ManageBlock(int TargetID, int UserID)
        {
            return await _repo.ManageBlock(TargetID, UserID);
        }

        public async Task<int?> GetUserFunctionID(int TargetID, int UserID, bool function)
        {
            return await _repo.GetUserFunctionID(TargetID, UserID, function);
        }
    }
}
