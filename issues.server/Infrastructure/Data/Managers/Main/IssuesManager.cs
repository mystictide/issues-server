using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Main;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Data.Managers.Main
{
    public class IssuesManager : AppSettings, IIssue
    {
        private readonly IIssue _repo;
        public IssuesManager()
        {
            _repo = new IssuesRepository();
        }

        public async Task<bool> Archive(Issues entity)
        {
            return await _repo.Archive(entity);
        }

        public async Task<FilteredList<Issues>?> FilteredList(Filter filter)
        {
            return await _repo.FilteredList(filter);
        }

        public async Task<Issues?> Get(int ID)
        {
            return await _repo.Get(ID);
        }

        public async Task<Issues?> Manage(Issues entity)
        {
            return await _repo.Manage(entity);
        }
    }
}
