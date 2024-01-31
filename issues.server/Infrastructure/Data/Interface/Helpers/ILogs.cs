using issues.server.Infrasructure.Models.Helpers;

namespace issues.server.Infrastructure.Data.Interface.Helpers
{
    public interface ILogs
    {
        Task<int> Add(Logs entity);
    }
}
