using issues.server.Infrastructure.Models.Main;

namespace issues.server.Infrastructure.Data.Interface.Main
{
    public interface IUsers
    {
        Task<bool> CheckEmail(string Email, int? UserID);
        Task<Users>? Login(Users entity);
        Task<Users>? Register(Users entity);
        Task<Users>? Get(int? ID, string? Username);
        Task<bool>? ChangePassword(int UserID, string currentPassword, string newPassword);
        Task<string>? UpdateEmail(int ID, string Email);
    }
}
