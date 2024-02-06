using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;

namespace issues.server.Infrastructure.Data.Interface.Main
{
    public interface IUsers
    {
        Task<Users?> Manage(Users entity);
        Task<Users?> Get(int ID);
        Task<IEnumerable<Users>?> GetCompanyUsers(int ID);
        Task<FilteredList<Users>?> FilteredList(Filter filter);
        Task<bool?> ChangePassword(int UserID, string currentPassword, string newPassword);
        Task<string?> UpdateEmail(int ID, string Email);
    }
}
