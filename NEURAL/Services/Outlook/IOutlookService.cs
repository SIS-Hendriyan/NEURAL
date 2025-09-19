using NEURAL.Models.Outlook;

namespace NEURAL.Services.Outlook
{
    public interface IOutlookService
    {
        // CRUD Operations
        OutlookModel CreateOutlook(OutlookModel outlook);
        OutlookModel? GetOutlookById(int id);
        OutlookModel UpdateOutlook(OutlookModel outlook);
        int DeleteOutlooks(int[] ids);

        // Data for Kendo Grid
        KendoDataSourceResultModel GetFilteredOutlookData(KendoDataSourceRequestModel request);

        // Filter Options
        List<string> GetProdschedVersionOptions();
        List<string> GetActualVersionOptions();

        // Status Management
        void UpdateOutlookStatus(int id, OutlookStatus newStatus, string currentUser);

        // Version Generation
        string GenerateOutlookVersion(string prodschedVersion, string identity);
    }
}
