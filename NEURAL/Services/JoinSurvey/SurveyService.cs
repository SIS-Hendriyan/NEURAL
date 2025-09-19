using NEURAL.Models;

namespace NEURAL.Services
{
    public class SurveyService : ISurveyService
    {
        // Mock data untuk demonstrasi
        private static readonly List<SurveyModel> _mockSurveyData = new()
        {
            new SurveyModel { Id = 1, Month = "January", Area = "CENTRAL", Pit = "PIT-A", Process = "OB Removal", Survey = "JS", Volume = 5665333, Horizontal = 2.5m, Vertical = 1.8m, Weighted = 2.1m, StatusEmail = true, Jobsite = "ADMO" },
            new SurveyModel { Id = 2, Month = "February", Area = "CENTRAL", Pit = "PIT-A", Process = "Coal Mining", Survey = "CLAIM", Volume = 1630481, Horizontal = 1.9m, Vertical = 2.1m, Weighted = 2.0m, StatusEmail = false, Jobsite = "ADMO" },
            new SurveyModel { Id = 3, Month = "March", Area = "TUTUPAN", Pit = "PIT-B", Process = "Coal Transport", Survey = "JS", Volume = 1694162, Horizontal = 12.5m, Vertical = 3.2m, Weighted = 7.8m, StatusEmail = true, Jobsite = "SERA" },
            new SurveyModel { Id = 4, Month = "April", Area = "NORTH", Pit = "PIT-C", Process = "Outpit", Survey = "CLAIM", Volume = 441659, Horizontal = 0.8m, Vertical = 0.6m, Weighted = 0.7m, StatusEmail = false, Jobsite = "MACO" },
            new SurveyModel { Id = 5, Month = "May", Area = "CENTRAL", Pit = "PIT-A", Process = "OB Removal", Survey = "JS", Volume = 6251000, Horizontal = 2.8m, Vertical = 1.9m, Weighted = 2.3m, StatusEmail = true, Jobsite = "ADMO" },
            new SurveyModel { Id = 6, Month = "June", Area = "TUTUPAN", Pit = "PIT-B", Process = "Coal Mining", Survey = "CLAIM", Volume = 1542503, Horizontal = 2.1m, Vertical = 2.3m, Weighted = 2.2m, StatusEmail = false, Jobsite = "SERA" }
        };

        private static readonly List<EmailSettingModel> _mockEmailSettings = new()
        {
            new EmailSettingModel { Id = 1, Jobsite = "ADMO", EmailTo = "admin@admo.com", EmailCC = "supervisor@admo.com" },
            new EmailSettingModel { Id = 2, Jobsite = "SERA", EmailTo = "admin@sera.com;manager@sera.com", EmailCC = "director@sera.com" },
            new EmailSettingModel { Id = 3, Jobsite = "MACO", EmailTo = "admin@maco.com", EmailCC = "" }
        };

        public async Task<IEnumerable<SurveyModel>> GetSurveyDataAsync(SurveyFilterModel filter)
        {
            await Task.Delay(100); // Simulate async operation

            var query = _mockSurveyData.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Jobsite) && filter.Jobsite != "all")
            {
                query = query.Where(x => x.Jobsite == filter.Jobsite);
            }

            if (!string.IsNullOrEmpty(filter.Year) && filter.Year != "all")
            {
                // For demo purposes, assume all data is from 2024
                // In real implementation, you would filter by actual date
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                if (filter.SortDir == "desc")
                {
                    query = filter.SortField.ToLower() switch
                    {
                        "month" => query.OrderByDescending(x => x.Month),
                        "area" => query.OrderByDescending(x => x.Area),
                        "pit" => query.OrderByDescending(x => x.Pit),
                        "process" => query.OrderByDescending(x => x.Process),
                        "volume" => query.OrderByDescending(x => x.Volume),
                        _ => query.OrderByDescending(x => x.Id)
                    };
                }
                else
                {
                    query = filter.SortField.ToLower() switch
                    {
                        "month" => query.OrderBy(x => x.Month),
                        "area" => query.OrderBy(x => x.Area),
                        "pit" => query.OrderBy(x => x.Pit),
                        "process" => query.OrderBy(x => x.Process),
                        "volume" => query.OrderBy(x => x.Volume),
                        _ => query.OrderBy(x => x.Id)
                    };
                }
            }

            return query.Skip(filter.Skip).Take(filter.Take).ToList();
        }

        public async Task<bool> DeleteSurveyAsync(int id)
        {
            await Task.Delay(100); // Simulate async operation
            var item = _mockSurveyData.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                _mockSurveyData.Remove(item);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<EmailSettingModel>> GetEmailSettingsAsync()
        {
            await Task.Delay(100); // Simulate async operation
            return _mockEmailSettings;
        }

        public async Task<bool> SaveEmailSettingAsync(EmailSettingModel emailSetting)
        {
            await Task.Delay(100); // Simulate async operation
            emailSetting.Id = _mockEmailSettings.Max(x => x.Id) + 1;
            _mockEmailSettings.Add(emailSetting);
            return true;
        }

        public async Task<bool> UpdateEmailSettingAsync(EmailSettingModel emailSetting)
        {
            await Task.Delay(100); // Simulate async operation
            var existing = _mockEmailSettings.FirstOrDefault(x => x.Id == emailSetting.Id);
            if (existing != null)
            {
                existing.Jobsite = emailSetting.Jobsite;
                existing.EmailTo = emailSetting.EmailTo;
                existing.EmailCC = emailSetting.EmailCC;
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteEmailSettingAsync(int id)
        {
            await Task.Delay(100); // Simulate async operation
            var item = _mockEmailSettings.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                _mockEmailSettings.Remove(item);
                return true;
            }
            return false;
        }

        public async Task<ReportDataModel> GetReportDataAsync(string year)
        {
            await Task.Delay(100); // Simulate async operation

            return new ReportDataModel
            {
                Year = year,
                Data = new List<ReportRowModel>
                {
                    new ReportRowModel
                    {
                        Site = "ADMO/TUTUPAN/CENTRAL",
                        Area = "CENTRAL",
                        Pit = "PIT-A",
                        Unit = "Production",
                        Process = "OB Removal JS",
                        Months = new decimal[] { 5665333, 6251000, 6037339, 7256091, 8544045, 8797395, 10249477, 3197827, 0, 0, 0, 0 },
                        Total = 55998507,
                        ClassName = "bg-blue-50"
                    },
                    new ReportRowModel
                    {
                        Site = "ADMO/TUTUPAN/CENTRAL",
                        Area = "CENTRAL",
                        Pit = "PIT-A",
                        Unit = "Production",
                        Process = "Coal Mining JS",
                        Months = new decimal[] { 1630481, 1542503, 1694162, 1719323, 2108220, 1894308, 2147185, 1563051, 0, 0, 0, 0 },
                        Total = 14299233,
                        ClassName = "bg-green-100"
                    },
                    new ReportRowModel
                    {
                        Site = "ADMO/TUTUPAN/CENTRAL",
                        Area = "CENTRAL",
                        Pit = "PIT-B",
                        Unit = "Production",
                        Process = "Coal Transport JS",
                        Months = new decimal[] { 1630481, 1542503, 1694162, 1719323, 2108220, 1894308, 2147185, 1563051, 0, 0, 0, 0 },
                        Total = 14299233,
                        ClassName = "bg-orange-100"
                    },
                    new ReportRowModel
                    {
                        Site = "ADMO/TUTUPAN/CENTRAL",
                        Area = "CENTRAL",
                        Pit = "PIT-B",
                        Unit = "Distance",
                        Process = "OB Removal (m)",
                        Months = new decimal[] { 2.5m, 2.8m, 2.3m, 2.6m, 2.9m, 3.1m, 3.4m, 2.7m, 0, 0, 0, 0 },
                        Total = 2.8m,
                        ClassName = "bg-teal-100"
                    }
                }
            };
        }

        public async Task<bool> SendEmailReportAsync(string year)
        {
            await Task.Delay(1000); // Simulate email sending
            return true;
        }

        public async Task<byte[]> GenerateTemplateAsync()
        {
            await Task.Delay(500); // Simulate file generation
            // In real implementation, generate Excel template
            return System.Text.Encoding.UTF8.GetBytes("Template content");
        }

        public async Task<bool> UploadDataAsync(Stream fileStream, string fileName)
        {
            await Task.Delay(1000); // Simulate file processing
            // In real implementation, process uploaded file
            return true;
        }
    }
}