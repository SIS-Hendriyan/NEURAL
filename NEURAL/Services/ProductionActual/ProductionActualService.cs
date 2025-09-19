using NEURAL.Models.ProductionActual;

namespace NEURAL.Services.ProductionActual
{
    public class ProductionActualService : IProductionActualService
    {
        private readonly List<ProductionActualModel> _mockData;

        public ProductionActualService()
        {
            _mockData = GenerateMockData();
        }

        public async Task<List<ProductionActualModel>> GetAllProductionActualAsync()
        {
            await Task.Delay(10); // Simulate async operation
            return _mockData;
        }

        public async Task<ProductionActualModel?> GetProductionActualByIdAsync(string id)
        {
            await Task.Delay(10);
            return _mockData.FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> CreateProductionActualAsync(ProductionActualModel model)
        {
            await Task.Delay(10);
            model.Id = Guid.NewGuid().ToString();
            _mockData.Add(model);
            return true;
        }

        public async Task<bool> UpdateProductionActualAsync(ProductionActualModel model)
        {
            await Task.Delay(10);
            var existing = _mockData.FirstOrDefault(x => x.Id == model.Id);
            if (existing != null)
            {
                var index = _mockData.IndexOf(existing);
                _mockData[index] = model;
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteProductionActualAsync(string id)
        {
            await Task.Delay(10);
            var item = _mockData.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                _mockData.Remove(item);
                return true;
            }
            return false;
        }

        public async Task<bool> RetryProductionActualAsync(string id)
        {
            await Task.Delay(10);
            // Simulate retry logic
            return true;
        }

        public async Task<string> DownloaderrorLogAsync(string id)
        {
            await Task.Delay(10);
            // Simulate downloading error log
            return $"error log for {id}";
        }

        public async Task<bool> InterventionProductionActualAsync(string id)
        {
            await Task.Delay(10);
            // Simulate intervention logic
            return true;
        }

        public async Task<List<string>> GetYearOptionsAsync()
        {
            await Task.Delay(10);
            return new List<string> { "2024", "2023", "2022", "2021" };
        }

        public async Task<List<string>> GetJobsiteOptionsAsync()
        {
            await Task.Delay(10);
            return new List<string> { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" };
        }

        public async Task<List<ProductionActualModel>> FilterProductionActualAsync(FilterOptions filters)
        {
            await Task.Delay(10);
            var filtered = _mockData.AsQueryable();

            if (filters.Year != "all")
            {
                filtered = filtered.Where(x => x.Year == filters.Year);
            }

            if (filters.Jobsite != "all")
            {
                filtered = filtered.Where(x => x.Jobsite == filters.Jobsite);
            }

            return filtered.ToList();
        }

        private List<ProductionActualModel> GenerateMockData()
        {
            return new List<ProductionActualModel>
            {
                new ProductionActualModel
                {
                    Id = "1",
                    Jobsite = "ADARO INDONESIA",
                    Year = "2024",
                    VersionName = GenerateVersionName(new DateTime(2024, 9, 15, 14, 30, 0)),
                    Progress = new Progress
                    {
                        DailyFleet = ProgressStatus.completed,
                        MonthlyFleet = ProgressStatus.completed,
                        DailyProcess = ProgressStatus.completed,
                        MonthlyProcess = ProgressStatus.completed,
                        YearlyProcess = ProgressStatus.completed,
                        YearlyFleet = ProgressStatus.completed,
                        Success = ProgressStatus.completed,
                        CurrentStage = ProgressStage.Success
                    }
                },
                new ProductionActualModel
                {
                    Id = "2",
                    Jobsite = "BALANGAN COAL",
                    Year = "2024",
                    VersionName = GenerateVersionName(new DateTime(2024, 9, 15, 10, 15, 0)),
                    Progress = new Progress
                    {
                        DailyFleet = ProgressStatus.completed,
                        MonthlyFleet = ProgressStatus.completed,
                        DailyProcess = ProgressStatus.error,
                        MonthlyProcess = ProgressStatus.pending,
                        YearlyProcess = ProgressStatus.pending,
                        YearlyFleet = ProgressStatus.pending,
                        Success = ProgressStatus.pending,
                        CurrentStage = ProgressStage.DailyProcess,
                        ErrorLogs = new Dictionary<string, string>
                        {
                            ["DailyProcess"] = "Database connection timeout during process execution"
                        }
                    }
                },
                new ProductionActualModel
                {
                    Id = "3",
                    Jobsite = "MARUWAI COAL",
                    Year = "2024",
                    VersionName = GenerateVersionName(new DateTime(2024, 9, 15, 16, 45, 0)),
                    Progress = new Progress
                    {
                        DailyFleet = ProgressStatus.completed,
                        MonthlyFleet = ProgressStatus.running,
                        DailyProcess = ProgressStatus.pending,
                        MonthlyProcess = ProgressStatus.pending,
                        YearlyProcess = ProgressStatus.pending,
                        YearlyFleet = ProgressStatus.pending,
                        Success = ProgressStatus.pending,
                        CurrentStage = ProgressStage.MonthlyFleet
                    }
                },
                new ProductionActualModel
                {
                    Id = "4",
                    Jobsite = "ADARO INDONESIA",
                    Year = "2023",
                    VersionName = GenerateVersionName(new DateTime(2023, 12, 14, 9, 20, 0)),
                    Progress = new Progress
                    {
                        DailyFleet = ProgressStatus.error,
                        MonthlyFleet = ProgressStatus.pending,
                        DailyProcess = ProgressStatus.pending,
                        MonthlyProcess = ProgressStatus.pending,
                        YearlyProcess = ProgressStatus.pending,
                        YearlyFleet = ProgressStatus.pending,
                        Success = ProgressStatus.pending,
                        CurrentStage = ProgressStage.DailyFleet,
                        ErrorLogs = new Dictionary<string, string>
                        {
                            ["DailyFleet"] = "Invalid data format in source files"
                        }
                    }
                },
                new ProductionActualModel
                {
                    Id = "5",
                    Jobsite = "BALANGAN COAL",
                    Year = "2023",
                    VersionName = GenerateVersionName(new DateTime(2023, 12, 20, 13, 55, 0)),
                    Progress = new Progress
                    {
                        DailyFleet = ProgressStatus.completed,
                        MonthlyFleet = ProgressStatus.completed,
                        DailyProcess = ProgressStatus.completed,
                        MonthlyProcess = ProgressStatus.completed,
                        YearlyProcess = ProgressStatus.completed,
                        YearlyFleet = ProgressStatus.completed,
                        Success = ProgressStatus.completed,
                        CurrentStage = ProgressStage.Success
                    }
                },
                new ProductionActualModel
                {
                    Id = "6",
                    Jobsite = "MARUWAI COAL",
                    Year = "2023",
                    VersionName = GenerateVersionName(new DateTime(2023, 11, 28, 11, 10, 0)),
                    Progress = new Progress
                    {
                        DailyFleet = ProgressStatus.running,
                        MonthlyFleet = ProgressStatus.pending,
                        DailyProcess = ProgressStatus.pending,
                        MonthlyProcess = ProgressStatus.pending,
                        YearlyProcess = ProgressStatus.pending,
                        YearlyFleet = ProgressStatus.pending,
                        Success = ProgressStatus.pending,
                        CurrentStage = ProgressStage.DailyFleet
                    }
                }
            };
        }

        private string GenerateVersionName(DateTime baseDate)
        {
            return baseDate.ToString("dd/MM/yy HH:mm");
        }

        public Task<string> DownloadErrorLogAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
