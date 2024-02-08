using issues.server.Infrastructure.Models.Main;

namespace issues.server.Infrastructure.Data.Interface.Main
{
    public interface IIssue : IBase<Issues>
    {
        Task<int?> ManageType(int type);
        Task<int?> ManageStatus(int status);
        Task<int?> ManagePriority(int priority);
    }
}
