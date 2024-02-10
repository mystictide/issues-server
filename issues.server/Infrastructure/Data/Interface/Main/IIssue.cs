using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;

namespace issues.server.Infrastructure.Data.Interface.Main
{
    public interface IIssue : IBase<Issues>
    {
        Task<int?> ManageType(int ID, int type);
        Task<int?> ManageStatus(int ID, int status);
        Task<int?> ManagePriority(int ID, int priority);
        Task<Comments?> GetComment(int ID);
        Task<Comments?> ManageComment(Comments entity);
        Task<bool?> DeleteComment(Comments entity);
        Task<FilteredList<Comments>?> FilteredComments(Filter filter);
    }
}
