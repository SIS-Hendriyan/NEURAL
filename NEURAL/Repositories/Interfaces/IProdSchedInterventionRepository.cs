using NEURAL.Models.ViewModel;

namespace NEURAL.Repositories.Interfaces
{
    public interface IProdSchedInterventionRepository
    {
        Task<List<ProdSchedInterventionViewModel>> GetInterventions(long headerId, long jobsiteId, long processId);
        Task<List<ProdSchedInterventionDownloadViewModel>> GetInterventionsForDownload(long headerId, int month, CancellationToken ct);
        Task<List<ProdSchedInterventionMonthStatViewModel>> GetMonthStats(long headerId, int month);
    }
}
