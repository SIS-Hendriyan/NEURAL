using Microsoft.AspNetCore.Mvc;
using NEURAL.Models.DPR;
using NEURAL.Services.DPR;

namespace NEURAL.Controllers
{
    public class DPRController : Controller
    {
        private readonly IDPRService _dprService;

        public DPRController(IDPRService dprService)
        {
            _dprService = dprService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel
            {
                DPRData = await _dprService.GetAllDPRDataAsync(),
                JobsiteOptions = await _dprService.GetJobsiteOptionsAsync(),
                MonthOptions = await _dprService.GetMonthOptionsAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FilterData(FilterOptions filters)
        {
            var filteredData = await _dprService.FilterDPRDataAsync(filters);
            return Json(filteredData);
        }

        [HttpGet]
        public async Task<IActionResult> GetDPRData()
        {
            var data = await _dprService.GetAllDPRDataAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetDPRById(string id)
        {
            var data = await _dprService.GetDPRDataByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DPRFormData formData)
        {
            if (ModelState.IsValid)
            {
                var newDPR = await _dprService.CreateDPRDataAsync(formData);
                return Json(new { success = true, data = newDPR, message = $"DPR created for {formData.Jobsite} on {formData.Date:yyyy-MM-dd}" });
            }
            return Json(new { success = false, message = "Invalid data provided" });
        }

        [HttpPost]
        public async Task<IActionResult> Update(DPRData dprData)
        {
            if (ModelState.IsValid)
            {
                var success = await _dprService.UpdateDPRDataAsync(dprData);
                if (success)
                {
                    return Json(new { success = true, message = "DPR updated successfully" });
                }
            }
            return Json(new { success = false, message = "Failed to update DPR" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _dprService.DeleteDPRDataAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "DPR deleted successfully" });
            }
            return Json(new { success = false, message = "Failed to delete DPR" });
        }

        [HttpGet]
        public async Task<IActionResult> GetEmailSettings()
        {
            var emailSettings = await _dprService.GetAllEmailSettingsAsync();
            return Json(emailSettings);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEmailSetting(EmailSetting emailSetting)
        {
            if (ModelState.IsValid)
            {
                EmailSetting result;
                if (string.IsNullOrEmpty(emailSetting.Id))
                {
                    result = await _dprService.CreateEmailSettingAsync(emailSetting);
                }
                else
                {
                    await _dprService.UpdateEmailSettingAsync(emailSetting);
                    result = emailSetting;
                }
                return Json(new { success = true, data = result, message = "Email setting saved successfully" });
            }
            return Json(new { success = false, message = "Invalid email setting data" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmailSetting(string id)
        {
            var success = await _dprService.DeleteEmailSettingAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Email setting deleted successfully" });
            }
            return Json(new { success = false, message = "Failed to delete email setting" });
        }

        [HttpGet]
        public async Task<IActionResult> GetMasterData(string type)
        {
            switch (type.ToLower())
            {
                case "production":
                    var productionData = await _dprService.GetMasterProductionDataAsync();
                    return Json(productionData);
                case "mining":
                    var miningData = await _dprService.GetMasterMiningDataAsync();
                    return Json(miningData);
                case "transport":
                    var transportData = await _dprService.GetMasterTransportDataAsync();
                    return Json(transportData);
                case "stockpile":
                    var stockpileData = await _dprService.GetMasterStockpileDataAsync();
                    return Json(stockpileData);
                case "globalweather":
                    var weatherData = await _dprService.GetMasterGlobalWeatherDataAsync();
                    return Json(weatherData);
                default:
                    return BadRequest("Invalid master data type");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReportData(string jobsite, DateTime date)
        {
            var reportData = await _dprService.GetReportDataByJobsiteAndDateAsync(jobsite, date);
            if (reportData == null)
            {
                return NotFound();
            }
            return Json(reportData);
        }

        [HttpGet]
        public async Task<IActionResult> GetDropdownOptions(string type)
        {
            switch (type.ToLower())
            {
                case "jobsite":
                    return Json(await _dprService.GetJobsiteOptionsAsync());
                case "month":
                    return Json(await _dprService.GetMonthOptionsAsync());
                case "area":
                    return Json(await _dprService.GetAreaOptionsAsync());
                case "pit":
                    return Json(await _dprService.GetPitOptionsAsync());
                case "process":
                    return Json(await _dprService.GetProcessOptionsAsync());
                case "stockpile":
                    return Json(await _dprService.GetStockpileOptionsAsync());
                case "owner":
                    return Json(await _dprService.GetOwnerOptionsAsync());
                case "location":
                    return Json(await _dprService.GetLocationOptionsAsync());
                case "weatherstation":
                    return Json(await _dprService.GetWeatherStationOptionsAsync());
                case "mbrversion":
                    return Json(await _dprService.GetMBRVersionOptionsAsync());
                default:
                    return BadRequest("Invalid dropdown type");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PivotDPR()
        {
            // Placeholder for pivot functionality
            return Json(new { success = true, message = "Opening DPR Pivot view..." });
        }

        [HttpPost]
        public async Task<IActionResult> DownloadErrorLog(string id)
        {
            var dprData = await _dprService.GetDPRDataByIdAsync(id);
            if (dprData != null)
            {
                return Json(new { success = true, message = $"Downloading error log for: {dprData.Jobsite} - {dprData.Date:yyyy-MM-dd}" });
            }
            return Json(new { success = false, message = "DPR data not found" });
        }
    }

    public class IndexViewModel
    {
        public List<DPRData> DPRData { get; set; } = new List<DPRData>();
        public List<string> JobsiteOptions { get; set; } = new List<string>();
        public List<string> MonthOptions { get; set; } = new List<string>();
    }
}
