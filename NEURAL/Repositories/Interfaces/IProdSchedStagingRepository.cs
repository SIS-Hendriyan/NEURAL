using System.Data;

namespace NEURAL.Repositories.Interfaces
{
    public interface IProdSchedStagingRepository
    {
        Task<int> DeleteByMbrAsync(string mbrVersion, CancellationToken ct);
        Task BulkWriteAsync(DataTable data, CancellationToken ct);
    }
}
