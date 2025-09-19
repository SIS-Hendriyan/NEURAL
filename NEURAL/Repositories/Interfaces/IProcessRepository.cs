using NEURAL.Models.Entities;

namespace NEURAL.Repositories.Interfaces
{
    public interface IProcessRepository
    {
        IQueryable<PROCESS_T> Query(bool includeDeleted = false);
    }
}
