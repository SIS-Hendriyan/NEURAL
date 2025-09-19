using NEURAL.Models.DPR;

namespace NEURAL.Services.DPR
{
    public interface IDPRService
    {
        // DPR Data Management
        Task<List<DPRData>> GetAllDPRDataAsync();
        Task<DPRData?> GetDPRDataByIdAsync(string id);
        Task<DPRData> CreateDPRDataAsync(DPRFormData formData);
        Task<bool> UpdateDPRDataAsync(DPRData dprData);
        Task<bool> DeleteDPRDataAsync(string id);
        Task<List<DPRData>> FilterDPRDataAsync(FilterOptions filters);

        // Email Settings Management
        Task<List<EmailSetting>> GetAllEmailSettingsAsync();
        Task<EmailSetting?> GetEmailSettingByIdAsync(string id);
        Task<EmailSetting> CreateEmailSettingAsync(EmailSetting emailSetting);
        Task<bool> UpdateEmailSettingAsync(EmailSetting emailSetting);
        Task<bool> DeleteEmailSettingAsync(string id);

        // Master Data Management
        Task<List<MasterProductionData>> GetMasterProductionDataAsync();
        Task<List<MasterMiningData>> GetMasterMiningDataAsync();
        Task<List<MasterTransportData>> GetMasterTransportDataAsync();
        Task<List<MasterStockpileData>> GetMasterStockpileDataAsync();
        Task<List<MasterGlobalWeatherData>> GetMasterGlobalWeatherDataAsync();

        // Report Data Management
        Task<List<ReportData>> GetAllReportDataAsync();
        Task<ReportData?> GetReportDataByJobsiteAndDateAsync(string jobsite, DateTime date);

        // Dropdown Options
        Task<List<string>> GetJobsiteOptionsAsync();
        Task<List<string>> GetMonthOptionsAsync();
        Task<List<string>> GetAreaOptionsAsync();
        Task<List<string>> GetPitOptionsAsync();
        Task<List<string>> GetProcessOptionsAsync();
        Task<List<string>> GetStockpileOptionsAsync();
        Task<List<string>> GetOwnerOptionsAsync();
        Task<List<string>> GetLocationOptionsAsync();
        Task<List<string>> GetWeatherStationOptionsAsync();
        Task<List<string>> GetMBRVersionOptionsAsync();
    }
}
