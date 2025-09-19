using System.Data;

namespace NEURAL.Repositories.Interfaces
{
    public interface IProdSchedStagingInterventionRepository
    {
        Task<int> DeleteByHeaderId(long headerId, CancellationToken ct);
        Task BulkWriteAsync(DataTable data,string newMbrVersion, CancellationToken ct);
    }
}
