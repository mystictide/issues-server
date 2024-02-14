using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Main;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Managers.Auth;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Data.Managers.Main
{
    public class UserManager : AppSettings, IUsers
    {
        private readonly IUsers _repo;
        public UserManager()
        {
            _repo = new UserRepository();
        }

        public async Task<Users?> Manage(Users entity)
        {
            if (entity.FirstName == null || entity.LastName == null || entity.Email == null)
            {
                throw new Exception("User information missing");
            }

            bool userExists = await new AuthManager().CheckEmail(false, entity.Email, entity.ID);
            if (userExists)
            {
                throw new Exception("Email address already registered");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            if (entity.Password != null)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password, salt);
                entity.Password = hashedPassword;
            }
            return await _repo.Manage(entity);
        }

        public async Task<Users?> Get(int ID)
        {
            return await _repo.Get(ID);
        }

        public async Task<string?> UpdateEmail(int ID, string Email)
        {
            return await _repo.UpdateEmail(ID, Email);
        }

        public async Task<bool?> ChangePassword(int UserID, string currentPassword, string newPassword)
        {
            var result = await _repo.Get(UserID);
            if (result != null && BCrypt.Net.BCrypt.Verify(currentPassword, result.Password))
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword, salt);
                return await _repo.ChangePassword(UserID, currentPassword, newPassword);
            }
            return false;
        }

        public async Task<FilteredList<Users>?> FilteredList(Filter filter)
        {
            return await _repo.FilteredList(filter);
        }

        public async Task<IEnumerable<Users>?> GetCompanyUsers(int ID)
        {
            return await _repo.GetCompanyUsers(ID);
        }
    }
}
