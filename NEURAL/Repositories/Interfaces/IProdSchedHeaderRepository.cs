using NEURAL.Models.Entities;
using NEURAL.Models.ViewModel;
using System.Numerics;

namespace NEURAL.Repositories.Interfaces
{
    public interface IProdSchedHeaderRepository
    {
        Task<List<ProdSchedHeaderViewModel>> GetAllHeaders();
        Task InsertProdSchedHeader(PRODSCHED_HEADER_T header);
        Task<(bool Found, string FileName, string ContentType, byte[] Data)> DownloadFile(long id, CancellationToken ct);
        Task DeleteProdschedHeader(long id);
        Task<List<string>> GetMBRVersion(int year); 
        Task<List<int>> GetYearList();
        Task<string> DownloadError(long id, CancellationToken ct);
        Task<string?> GetNextMbr(long headerId);
    }
}
