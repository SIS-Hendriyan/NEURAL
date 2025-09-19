using NEURAL.Models.ViewModel;

namespace NEURAL.Repositories.Interfaces
{
    public interface IProdSchedPivotRepository
    {
        Task<List<ProdSchedPivotViewModel>> GetPivot(string periode, string tipeData, string mbrVersion, int year);
    }
}
