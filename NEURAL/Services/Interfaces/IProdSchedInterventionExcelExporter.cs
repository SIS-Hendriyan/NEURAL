using NEURAL.Models.ViewModel;

namespace NEURAL.Services.Interfaces
{
    public interface IProdSchedInterventionExcelExporter
    {
        Task<byte[]> BuildFromData(
           List<ProdSchedInterventionDownloadViewModel> rows,
           List<ProdSchedInterventionMonthStatViewModel> monthStats,
           int month,
           string mbrVersion,
           CancellationToken ct);
    }
}
