using NEURAL.Models;

namespace NEURAL.Services
{
    public interface ISurveyService
    {
        Task<IEnumerable<SurveyModel>> GetSurveyDataAsync(SurveyFilterModel filter);
        Task<bool> DeleteSurveyAsync(int id);
        Task<IEnumerable<EmailSettingModel>> GetEmailSettingsAsync();
        Task<bool> SaveEmailSettingAsync(EmailSettingModel emailSetting);
        Task<bool> UpdateEmailSettingAsync(EmailSettingModel emailSetting);
        Task<bool> DeleteEmailSettingAsync(int id);
        Task<ReportDataModel> GetReportDataAsync(string year);
        Task<bool> SendEmailReportAsync(string year);
        Task<byte[]> GenerateTemplateAsync();
        Task<bool> UploadDataAsync(Stream fileStream, string fileName);
    }

    public class SurveyFilterModel
    {
        public string? Year { get; set; }
        public string? Jobsite { get; set; }
        public int Take { get; set; } = 10;
        public int Skip { get; set; } = 0;
        public string? SortField { get; set; }
        public string? SortDir { get; set; }
    }

    public class EmailSettingModel
    {
        public int Id { get; set; }
        public string Jobsite { get; set; } = string.Empty;
        public string EmailTo { get; set; } = string.Empty;
        public string EmailCC { get; set; } = string.Empty;
    }

    public class ReportDataModel
    {
        public string Year { get; set; } = string.Empty;
        public List<ReportRowModel> Data { get; set; } = new();
    }

    public class ReportRowModel
    {
        public string Site { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pit { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public decimal[] Months { get; set; } = new decimal[12];
        public decimal Total { get; set; }
        public string ClassName { get; set; } = string.Empty;
    }
}