using NEURAL.Models.DPR;

namespace NEURAL.Services.DPR
{
    public class DPRService : IDPRService
    {
        // Mock data storage - in production, this would be replaced with database access
        private static List<DPRData> _dprData = new List<DPRData>();
        private static List<EmailSetting> _emailSettings = new List<EmailSetting>();
        private static List<ReportData> _reportData = new List<ReportData>();

        static DPRService()
        {
            InitializeMockData();
        }

        #region DPR Data Management

        public async Task<List<DPRData>> GetAllDPRDataAsync()
        {
            return await Task.FromResult(_dprData.OrderByDescending(x => x.Date).ToList());
        }

        public async Task<DPRData?> GetDPRDataByIdAsync(string id)
        {
            return await Task.FromResult(_dprData.FirstOrDefault(x => x.Id == id));
        }

        public async Task<DPRData> CreateDPRDataAsync(DPRFormData formData)
        {
            var newDPR = new DPRData
            {
                Id = Guid.NewGuid().ToString(),
                Jobsite = formData.Jobsite,
                Date = formData.Date,
                Status = formData.Status,
                Progress = new Progress
                {
                    GenerateDataDP = "pending",
                    GenerateDataDF = "pending",
                    GenerateReport = "pending",
                    Success = "pending",
                    SendEmail = "pending",
                    CurrentStage = "generate_data"
                }
            };

            _dprData.Add(newDPR);
            return await Task.FromResult(newDPR);
        }

        public async Task<bool> UpdateDPRDataAsync(DPRData dprData)
        {
            var existingDPR = _dprData.FirstOrDefault(x => x.Id == dprData.Id);
            if (existingDPR != null)
            {
                existingDPR.Jobsite = dprData.Jobsite;
                existingDPR.Date = dprData.Date;
                existingDPR.Status = dprData.Status;
                existingDPR.Progress = dprData.Progress;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteDPRDataAsync(string id)
        {
            var dpr = _dprData.FirstOrDefault(x => x.Id == id);
            if (dpr != null)
            {
                _dprData.Remove(dpr);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<List<DPRData>> FilterDPRDataAsync(FilterOptions filters)
        {
            var query = _dprData.AsQueryable();

            if (filters.Jobsite != "all")
            {
                query = query.Where(x => x.Jobsite == filters.Jobsite);
            }

            if (filters.Month != "all")
            {
                var monthYear = filters.Month.Split(' ');
                if (monthYear.Length == 2)
                {
                    var month = monthYear[0];
                    var year = int.Parse(monthYear[1]);
                    query = query.Where(x => x.Date.ToString("MMMM") == month && x.Date.Year == year);
                }
            }

            return await Task.FromResult(query.OrderByDescending(x => x.Date).ToList());
        }

        #endregion

        #region Email Settings Management

        public async Task<List<EmailSetting>> GetAllEmailSettingsAsync()
        {
            return await Task.FromResult(_emailSettings.ToList());
        }

        public async Task<EmailSetting?> GetEmailSettingByIdAsync(string id)
        {
            return await Task.FromResult(_emailSettings.FirstOrDefault(x => x.Id == id));
        }

        public async Task<EmailSetting> CreateEmailSettingAsync(EmailSetting emailSetting)
        {
            emailSetting.Id = Guid.NewGuid().ToString();
            _emailSettings.Add(emailSetting);
            return await Task.FromResult(emailSetting);
        }

        public async Task<bool> UpdateEmailSettingAsync(EmailSetting emailSetting)
        {
            var existing = _emailSettings.FirstOrDefault(x => x.Id == emailSetting.Id);
            if (existing != null)
            {
                existing.Jobsite = emailSetting.Jobsite;
                existing.EmailTo = emailSetting.EmailTo;
                existing.EmailCC = emailSetting.EmailCC;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteEmailSettingAsync(string id)
        {
            var emailSetting = _emailSettings.FirstOrDefault(x => x.Id == id);
            if (emailSetting != null)
            {
                _emailSettings.Remove(emailSetting);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        #endregion

        #region Master Data Management

        public async Task<List<MasterProductionData>> GetMasterProductionDataAsync()
        {
            return await Task.FromResult(new List<MasterProductionData>
            {
                new MasterProductionData { Id = "1", Area = "Area 1", Pit = "Pit A", Process = "Mining" },
                new MasterProductionData { Id = "2", Area = "Area 2", Pit = "Pit B", Process = "Hauling" },
                new MasterProductionData { Id = "3", Area = "Area 3", Pit = "Pit C", Process = "Loading" }
            });
        }

        public async Task<List<MasterMiningData>> GetMasterMiningDataAsync()
        {
            return await Task.FromResult(new List<MasterMiningData>
            {
                new MasterMiningData { Id = "1", Area = "Area 1", Pit = "Pit A", Process = "Drilling" },
                new MasterMiningData { Id = "2", Area = "Area 2", Pit = "Pit B", Process = "Blasting" },
                new MasterMiningData { Id = "3", Area = "Area 3", Pit = "Pit C", Process = "Excavation" }
            });
        }

        public async Task<List<MasterTransportData>> GetMasterTransportDataAsync()
        {
            return await Task.FromResult(new List<MasterTransportData>
            {
                new MasterTransportData { Id = "1", Area = "Area 1", Pit = "Pit A", Process = "Hauling" },
                new MasterTransportData { Id = "2", Area = "Area 2", Pit = "Pit B", Process = "Transportation" },
                new MasterTransportData { Id = "3", Area = "Area 3", Pit = "Pit C", Process = "Loading" }
            });
        }

        public async Task<List<MasterStockpileData>> GetMasterStockpileDataAsync()
        {
            return await Task.FromResult(new List<MasterStockpileData>
            {
                new MasterStockpileData { Id = "1", Area = "Area 1", Pit = "Pit A", Process = "Stockpiling", Owner = "ADARO INDONESIA", Location = "West Block", StockpileName = "Stockpile Alpha" },
                new MasterStockpileData { Id = "2", Area = "Area 2", Pit = "Pit B", Process = "Storage", Owner = "BALANGAN COAL", Location = "East Block", StockpileName = "Stockpile Beta" },
                new MasterStockpileData { Id = "3", Area = "Area 3", Pit = "Pit C", Process = "Inventory", Owner = "MARUWAI COAL", Location = "Central Block", StockpileName = "Stockpile Gamma" }
            });
        }

        public async Task<List<MasterGlobalWeatherData>> GetMasterGlobalWeatherDataAsync()
        {
            return await Task.FromResult(new List<MasterGlobalWeatherData>
            {
                new MasterGlobalWeatherData { Id = "1", Location = "Main Site", WeatherStation = "WS-001", Description = "Primary weather monitoring station" },
                new MasterGlobalWeatherData { Id = "2", Location = "North Area", WeatherStation = "WS-002", Description = "Northern sector weather station" },
                new MasterGlobalWeatherData { Id = "3", Location = "South Area", WeatherStation = "WS-003", Description = "Southern sector weather station" }
            });
        }

        #endregion

        #region Report Data Management

        public async Task<List<ReportData>> GetAllReportDataAsync()
        {
            return await Task.FromResult(_reportData.ToList());
        }

        public async Task<ReportData?> GetReportDataByJobsiteAndDateAsync(string jobsite, DateTime date)
        {
            return await Task.FromResult(_reportData.FirstOrDefault(x => x.Jobsite == jobsite && x.Date.Date == date.Date));
        }

        #endregion

        #region Dropdown Options

        public async Task<List<string>> GetJobsiteOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" });
        }

        public async Task<List<string>> GetMonthOptionsAsync()
        {
            return await Task.FromResult(new List<string>
            {
                "January 2024", "February 2024", "March 2024", "April 2024", "May 2024", "June 2024",
                "July 2024", "August 2024", "September 2024", "October 2024", "November 2024", "December 2024"
            });
        }

        public async Task<List<string>> GetAreaOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "Area 1", "Area 2", "Area 3", "Area 4", "Area 5" });
        }

        public async Task<List<string>> GetPitOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "Pit A", "Pit B", "Pit C", "Pit D", "Pit E" });
        }

        public async Task<List<string>> GetProcessOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "Mining", "Hauling", "Loading", "Processing", "Transportation" });
        }

        public async Task<List<string>> GetStockpileOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "Stockpile 1", "Stockpile 2", "Stockpile 3", "Stockpile 4", "Stockpile 5" });
        }

        public async Task<List<string>> GetOwnerOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" });
        }

        public async Task<List<string>> GetLocationOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "West Block", "East Block", "Central Block", "North Block", "South Block" });
        }

        public async Task<List<string>> GetWeatherStationOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "WS-001", "WS-002", "WS-003", "WS-004", "WS-005" });
        }

        public async Task<List<string>> GetMBRVersionOptionsAsync()
        {
            return await Task.FromResult(new List<string> { "MBR0", "MBR1", "MBR2", "MBR3" });
        }

        #endregion

        #region Mock Data Initialization

        private static void InitializeMockData()
        {
            // Initialize DPR Data
            _dprData = new List<DPRData>
            {
                new DPRData
                {
                    Id = "1",
                    Jobsite = "ADARO INDONESIA",
                    Date = new DateTime(2024, 9, 15),
                    Status = "Quick",
                    Progress = new Progress
                    {
                        GenerateDataDP = "completed",
                        GenerateDataDF = "completed",
                        GenerateReport = "completed",
                        Success = "completed",
                        SendEmail = "completed",
                        CurrentStage = "send_email"
                    }
                },
                new DPRData
                {
                    Id = "2",
                    Jobsite = "BALANGAN COAL",
                    Date = new DateTime(2024, 9, 15),
                    Status = "Fix",
                    Progress = new Progress
                    {
                        GenerateDataDP = "completed",
                        GenerateDataDF = "completed",
                        GenerateReport = "error",
                        Success = "pending",
                        SendEmail = "pending",
                        CurrentStage = "generate_report",
                        ErrorLogs = new ErrorLogs
                        {
                            GenerateReport = "Database connection timeout during report generation"
                        }
                    }
                },
                new DPRData
                {
                    Id = "3",
                    Jobsite = "MARUWAI COAL",
                    Date = new DateTime(2024, 9, 15),
                    Status = "Quick",
                    Progress = new Progress
                    {
                        GenerateDataDP = "completed",
                        GenerateDataDF = "completed",
                        GenerateReport = "running",
                        Success = "pending",
                        SendEmail = "pending",
                        CurrentStage = "generate_report"
                    }
                },
                new DPRData
                {
                    Id = "4",
                    Jobsite = "ADARO INDONESIA",
                    Date = new DateTime(2024, 9, 14),
                    Status = "Fix",
                    Progress = new Progress
                    {
                        GenerateDataDP = "error",
                        GenerateDataDF = "pending",
                        GenerateReport = "pending",
                        Success = "pending",
                        SendEmail = "pending",
                        CurrentStage = "generate_data",
                        ErrorLogs = new ErrorLogs
                        {
                            GenerateDataDP = "Invalid data format in source files"
                        }
                    }
                },
                new DPRData
                {
                    Id = "5",
                    Jobsite = "BALANGAN COAL",
                    Date = new DateTime(2024, 9, 14),
                    Status = "Quick",
                    Progress = new Progress
                    {
                        GenerateDataDP = "completed",
                        GenerateDataDF = "completed",
                        GenerateReport = "completed",
                        Success = "completed",
                        SendEmail = "completed",
                        CurrentStage = "send_email"
                    }
                },
                new DPRData
                {
                    Id = "6",
                    Jobsite = "MARUWAI COAL",
                    Date = new DateTime(2024, 9, 14),
                    Status = "Fix",
                    Progress = new Progress
                    {
                        GenerateDataDP = "running",
                        GenerateDataDF = "pending",
                        GenerateReport = "pending",
                        Success = "pending",
                        SendEmail = "pending",
                        CurrentStage = "generate_data"
                    }
                },
                new DPRData
                {
                    Id = "7",
                    Jobsite = "ADARO INDONESIA",
                    Date = new DateTime(2024, 9, 13),
                    Status = "Quick",
                    Progress = new Progress
                    {
                        GenerateDataDP = "completed",
                        GenerateDataDF = "running",
                        GenerateReport = "pending",
                        Success = "pending",
                        SendEmail = "pending",
                        CurrentStage = "generate_data"
                    }
                },
                new DPRData
                {
                    Id = "8",
                    Jobsite = "BALANGAN COAL",
                    Date = new DateTime(2024, 9, 13),
                    Status = "Fix",
                    Progress = new Progress
                    {
                        GenerateDataDP = "completed",
                        GenerateDataDF = "error",
                        GenerateReport = "pending",
                        Success = "pending",
                        SendEmail = "pending",
                        CurrentStage = "generate_data",
                        ErrorLogs = new ErrorLogs
                        {
                            GenerateDataDF = "Failed to process data format conversion"
                        }
                    }
                },
                new DPRData
                {
                    Id = "9",
                    Jobsite = "MARUWAI COAL",
                    Date = new DateTime(2024, 8, 30),
                    Status = "Quick",
                    Progress = new Progress
                    {
                        GenerateDataDP = "completed",
                        GenerateDataDF = "completed",
                        GenerateReport = "completed",
                        Success = "completed",
                        SendEmail = "completed",
                        CurrentStage = "send_email"
                    }
                },
                new DPRData
                {
                    Id = "10",
                    Jobsite = "ADARO INDONESIA",
                    Date = new DateTime(2024, 8, 29),
                    Status = "Fix",
                    Progress = new Progress
                    {
                        GenerateDataDP = "completed",
                        GenerateDataDF = "completed",
                        GenerateReport = "running",
                        Success = "pending",
                        SendEmail = "pending",
                        CurrentStage = "generate_report"
                    }
                }
            };

            // Initialize Email Settings
            _emailSettings = new List<EmailSetting>
            {
                new EmailSetting { Id = "1", Jobsite = "ADARO INDONESIA", EmailTo = "adaro-reports@example.com", EmailCC = "manager@adaro.com;supervisor@adaro.com" },
                new EmailSetting { Id = "2", Jobsite = "BALANGAN COAL", EmailTo = "balangan-reports@example.com", EmailCC = "ops@balangan.com" },
                new EmailSetting { Id = "3", Jobsite = "MARUWAI COAL", EmailTo = "maruwai-reports@example.com", EmailCC = "admin@maruwai.com;director@maruwai.com" }
            };

            // Initialize Report Data
            var mockReportActivities = new List<ReportActivity>
            {
                new ReportActivity
                {
                    Id = "1",
                    Activity = "Overburden (Bcm)",
                    Category = "mining",
                    Daily = new ReportDataPoint { Actual = 354132, Plan = 429627, Achievement = 82, Status = "critical" },
                    MTD = new ReportDataPoint { Actual = 1761722, Plan = 1690747, Achievement = 104, Status = "good" },
                    YTD = new ReportDataPointWithDeviation { Actual = 73474040, Plan = 74250747, Achievement = 99, Status = "warning", Deviation = -776707 }
                },
                new ReportActivity
                {
                    Id = "2",
                    Activity = "Coal PTR (Ton)",
                    Category = "mining",
                    Daily = new ReportDataPoint { Actual = 66562, Plan = 87669, Achievement = 76, Status = "critical" },
                    MTD = new ReportDataPoint { Actual = 314060, Plan = 344862, Achievement = 91, Status = "warning" },
                    YTD = new ReportDataPointWithDeviation { Actual = 20186556, Plan = 19310862, Achievement = 105, Status = "good", Deviation = 875695 }
                }
            };

            _reportData = new List<ReportData>
            {
                new ReportData
                {
                    Id = "1",
                    Jobsite = "ADARO INDONESIA",
                    Date = new DateTime(2024, 9, 15),
                    MBRVersion = "MBR0",
                    EstimatedTruckCount = "2024-09-14",
                    ElapsedDays = 258,
                    RemainingDays = 107,
                    Activities = mockReportActivities
                }
            };
        }

        #endregion
    }
}
