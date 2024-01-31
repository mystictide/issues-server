using issues.server.Infrasructure.Models.Users;

namespace issues.server.Infrastructure.Data.Interface.User
{
    public interface IUsers
    {
        Task<bool> CheckEmail(string Email, int? UserID);
        Task<bool> CheckUsername(string Username, int? UserID);
        Task<Users>? Login(Users entity);
        Task<Users>? Register(Users entity);
        Task<Users>? Get(int? ID, string? Username);
        Task<bool>? ChangePassword(int UserID, string currentPassword, string newPassword);
        Task<string>? UpdateEmail(int ID, string Email);
        Task<string>? UpdateBio(int ID, string Bio);
        Task<bool?> ManageFollow(int TargetID, int UserID);
        Task<bool?> ManageBlock(int TargetID, int UserID);
        Task<int?> GetUserFunctionID(int TargetID, int UserID, bool function);
    }
}
