using System.Data.Common;                  
using Microsoft.EntityFrameworkCore;      
using Microsoft.EntityFrameworkCore.Storage;
using NEURAL.Repositories.Context;
using NEURAL.Repositories.Interfaces;


namespace NEURAL.Repositories.Implementations
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public UnitOfWork(AppDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
            => _db.Database.BeginTransactionAsync(ct);

        public DbConnection GetDbConnection()
            => _db.Database.GetDbConnection();

        public DbTransaction? GetDbTransaction()
            => _db.Database.CurrentTransaction?.GetDbTransaction();

        public ValueTask DisposeAsync() => _db.DisposeAsync();
    }
}
