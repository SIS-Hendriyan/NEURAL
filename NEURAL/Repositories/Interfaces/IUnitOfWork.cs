using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace NEURAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
        DbConnection GetDbConnection();
        DbTransaction? GetDbTransaction();
    }
}
