using issues.server.Infrasructure.Models.Helpers;

namespace issues.server.Infrastructure.Data.Interface
{
    public interface IBase<T> where T : class
    {
        Task<FilteredList<T>?> FilteredList(Filter filter);
        Task<T?> Get(int ID);
        Task<T?> Manage(T entity);
        Task<bool> Archive(T entity);
    }
}
