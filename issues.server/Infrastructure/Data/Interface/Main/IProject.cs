using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Models.Stats;

namespace issues.server.Infrastructure.Data.Interface.Main
{
    public interface IProject : IBase<Projects>
    {
        Task<IEnumerable<Projects>?> GetCompanyProjects(int ID, int? limit);
        Task<IEnumerable<ProjectStats>?> GetStatistics(int ID);
    }
}
