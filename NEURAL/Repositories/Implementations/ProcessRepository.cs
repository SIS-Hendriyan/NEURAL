using Microsoft.EntityFrameworkCore;
using NEURAL.Models.Entities;
using NEURAL.Repositories.Context;
using NEURAL.Repositories.Interfaces;

namespace NEURAL.Repositories.Implementations
{
    public sealed class ProcessRepository : IProcessRepository
    {
        public readonly AppDbContext _db;

        public ProcessRepository(AppDbContext db) => _db = db;
        public IQueryable<PROCESS_T> Query(bool includeDeleted = false)
        {
            var q = _db.Processs.AsNoTracking();
            return includeDeleted ? q : q.Where(x => x.DeletedAt == null);
        }
    }
}
