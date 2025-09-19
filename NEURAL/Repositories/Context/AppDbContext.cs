using Microsoft.EntityFrameworkCore;
using NEURAL.Models.Entities;

namespace NEURAL.Repositories.Context
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }
        public DbSet<JOBSITE_T> Jobsites => Set<JOBSITE_T>();
        public DbSet<PROCESS_T> Processs => Set<PROCESS_T>();
    }
}
