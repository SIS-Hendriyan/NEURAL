using Microsoft.EntityFrameworkCore;
using NEURAL.Models.Entities;
using NEURAL.Repositories.Context;
using NEURAL.Repositories.Interfaces;

namespace NEURAL.Repositories.Implementations
{
    public sealed class JobsiteRepository : IJobsiteRepository
    {
        private readonly AppDbContext _db;
        public JobsiteRepository(AppDbContext db) => _db = db;

        public IQueryable<JOBSITE_T> Query(bool includeDeleted = false)
        {
            var q = _db.Jobsites.AsNoTracking();
            return includeDeleted ? q : q.Where(x => x.DeletedAt == null);
        }
    }
}
