using issues.server.Infrastructure.Models.Main;

namespace issues.server.Infrastructure.Data.Interface.Main
{
    public interface IRole : IBase<Roles>
    {
        Task<IEnumerable<Roles>?> GetCompanyRoles(int ID);
    }
}
