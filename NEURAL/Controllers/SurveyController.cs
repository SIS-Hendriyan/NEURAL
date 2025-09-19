using Microsoft.AspNetCore.Mvc;
using NEURAL.Models;
using NEURAL.Services;

namespace NEURAL.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: Survey Landing Page
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // POST: Get data for Kendo Grid (Server-side processing)
        [HttpPost]
        public async Task<IActionResult> GetSurveyData([FromBody] SurveyDataRequest request)
        {
            try
            {
                var filter = new Services.SurveyFilterModel
                {
                    Year = request.Filter?.Year ?? "all",
                    Jobsite = request.Filter?.Jobsite ?? "all",
                    Take = request.Take,
                    Skip = request.Skip,
                    SortField = request.SortField,
                    SortDir = request.SortDir
                };

                var data = await _surveyService.GetSurveyDataAsync(filter);
                var total = data.Count(); // In real implementation, get total count separately

                return Json(new SurveyDataResponse
                {
                    Data = data.ToList(),
                    Total = total
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // POST: Delete Survey
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return Json(new { success = false, message = "No survey IDs provided" });
                }

                int deletedCount = 0;
                foreach (var id in ids)
                {
                    var success = await _surveyService.DeleteSurveyAsync(id);
                    if (success) deletedCount++;
                }

                return Json(new { success = true, message = $"Successfully deleted {deletedCount} survey(s)" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting surveys: {ex.Message}" });
            }
        }

        // GET: View Report
        public async Task<IActionResult> ViewReport(string year = "2024")
        {
            try
            {
                var reportData = await _surveyService.GetReportDataAsync(year);
                return View(reportData);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading report: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Download Template
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                var templateData = await _surveyService.GenerateTemplateAsync();
                return File(templateData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Survey_Template.xlsx");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error generating template: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Upload File
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "Please select a file to upload" });
                }

                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                {
                    return Json(new { success = false, message = "Please upload an Excel file (.xlsx or .xls)" });
                }

                using (var stream = file.OpenReadStream())
                {
                    var success = await _surveyService.UploadDataAsync(stream, file.FileName);
                    if (success)
                    {
                        return Json(new { success = true, message = "File uploaded successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to process uploaded file" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error uploading file: {ex.Message}" });
            }
        }

        // Email Settings CRUD Operations

        // GET: Get Email Settings
        [HttpGet]
        public async Task<IActionResult> GetEmailSettings()
        {
            try
            {
                var settings = await _surveyService.GetEmailSettingsAsync();
                return Json(new { success = true, data = settings });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Create Email Setting
        [HttpPost]
        public async Task<IActionResult> CreateEmailSetting([FromBody] EmailSettingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid data provided" });
                }

                var success = await _surveyService.SaveEmailSettingAsync(model);
                if (success)
                {
                    return Json(new { success = true, message = "Email setting created successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to create email setting" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Update Email Setting
        [HttpPost]
        public async Task<IActionResult> UpdateEmailSetting([FromBody] EmailSettingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid data provided" });
                }

                var success = await _surveyService.UpdateEmailSettingAsync(model);
                if (success)
                {
                    return Json(new { success = true, message = "Email setting updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Email setting not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Delete Email Setting
        [HttpPost]
        public async Task<IActionResult> DeleteEmailSetting(int id)
        {
            try
            {
                var success = await _surveyService.DeleteEmailSettingAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Email setting deleted successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Email setting not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Send Email Report
        [HttpPost]
        public async Task<IActionResult> SendEmailReport([FromBody] string year)
        {
            try
            {
                var success = await _surveyService.SendEmailReportAsync(year ?? "2024");
                if (success)
                {
                    return Json(new { success = true, message = "Email report sent successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to send email report" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}