using NEURAL.Models.Entities;

namespace NEURAL.Repositories.Interfaces
{
    public interface IJobsiteRepository
    {
        IQueryable<JOBSITE_T> Query(bool includeDeleted = false);
    }
}
