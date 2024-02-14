using static Dapper.SqlMapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Models.Stats;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Main;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Data.Managers.Main
{
    public class ProjectsManager : AppSettings, IProject
    {
        private readonly IProject _repo;
        public ProjectsManager()
        {
            _repo = new ProjectsRepository();
        }
        public async Task<bool> Archive(Projects entity)
        {
            return await _repo.Archive(entity);
        }

        public async Task<FilteredList<Projects>?> FilteredList(Filter filter)
        {
            return await _repo.FilteredList(filter);
        }

        public async Task<Projects?> Get(int ID)
        {
            return await _repo.Get(ID);
        }

        public async Task<IEnumerable<Projects>?> GetCompanyProjects(int ID, int? limit)
        {
            return await _repo.GetCompanyProjects(ID, limit);
        }

        public async Task<IEnumerable<ProjectStats>?> GetStatistics(int ID)
        {
            return await _repo.GetStatistics(ID);
        }

        public async Task<Projects?> Manage(Projects entity)
        {
            return await _repo.Manage(entity);
        }
    }
}
