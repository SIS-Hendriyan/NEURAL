using NEURAL.Models.ProductionActual;

namespace NEURAL.Services.ProductionActual
{
    public interface IProductionActualService
    {
        Task<List<ProductionActualModel>> GetAllProductionActualAsync();
        Task<ProductionActualModel?> GetProductionActualByIdAsync(string id);
        Task<bool> CreateProductionActualAsync(ProductionActualModel model);
        Task<bool> UpdateProductionActualAsync(ProductionActualModel model);
        Task<bool> DeleteProductionActualAsync(string id);
        Task<bool> RetryProductionActualAsync(string id);
        Task<string> DownloadErrorLogAsync(string id);
        Task<bool> InterventionProductionActualAsync(string id);
        Task<List<string>> GetYearOptionsAsync();
        Task<List<string>> GetJobsiteOptionsAsync();
        Task<List<ProductionActualModel>> FilterProductionActualAsync(FilterOptions filters);
    }
}
