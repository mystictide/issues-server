using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Models.Stats;
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

        public async Task<IEnumerable<Issues>?> GetCompanyIssues(int ID, int? limit)
        {
            return await _repo.GetCompanyIssues(ID, limit);
        }

        public async Task<Issues?> Manage(Issues entity)
        {
            return await _repo.Manage(entity);
        }

        public async Task<Issues?> ManageAssignedUsers(Issues entity)
        {
            return await _repo.ManageAssignedUsers(entity);
        }

        public async Task<int?> ManagePriority(int ID, int priority)
        {
            return await _repo.ManagePriority(ID, priority);
        }

        public async Task<int?> ManageStatus(int ID, int status)
        {
            return await _repo.ManageStatus(ID, status);
        }

        public async Task<int?> ManageType(int ID, int type)
        {
            return await _repo.ManageType(ID, type);
        }

        public async Task<Comments?> GetComment(int ID)
        {
            return await _repo.GetComment(ID);
        }

        public async Task<Comments?> ManageComment(Comments entity)
        {
            return await _repo.ManageComment(entity);
        }

        public async Task<bool?> DeleteComment(Comments entity)
        {
            return await _repo.DeleteComment(entity);
        }

        public async Task<FilteredList<Comments>?> FilteredComments(Filter filter)
        {
            return await _repo.FilteredComments(filter);
        }

        public async Task<IssueStats?> GetStatistics(int ID)
        {
            return await _repo.GetStatistics(ID);
        }

        public async Task<IEnumerable<Comments>?> GetCompanyComments(int ID, int? limit)
        {
            return await _repo.GetCompanyComments(ID, limit);
        }
    }
}
