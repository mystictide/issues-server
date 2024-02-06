using issues.server.Infrastructure.Models.Main;

namespace issues.server.Infrastructure.Data.Interface.Main
{
    public interface IProject : IBase<Projects>
    {
        Task<IEnumerable<Projects>?> GetCompanyProjects(int ID);
    }
}
