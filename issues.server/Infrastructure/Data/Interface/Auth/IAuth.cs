using issues.server.Infrastructure.Models.Main;

namespace issues.server.Infrastructure.Data.Interface.Auth
{
    public interface IAuth
    {
        Task<bool> CheckEmail(bool company, string Email, int? UserID);
        Task<Companies>? CompanyLogin(Companies entity);
        Task<Users>? Login(Users entity);
        Task<Companies>? Register(Companies entity);
    }
}
