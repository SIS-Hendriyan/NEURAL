using Microsoft.AspNetCore.Mvc;
using NEURAL.Models.ProductionActual;
using NEURAL.Services.ProductionActual;

namespace NEURAL.Controllers
{
    public class ProductionActualController : Controller
    {
        private readonly IProductionActualService _productionActualService;

        public ProductionActualController(IProductionActualService productionActualService)
        {
            _productionActualService = productionActualService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _productionActualService.GetAllProductionActualAsync();
            var jobsiteOptions = await _productionActualService.GetJobsiteOptionsAsync();

            ViewBag.JobsiteOptions = jobsiteOptions;

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> GetFilteredData([FromBody] FilterOptions filters)
        {
            var filteredData = await _productionActualService.FilterProductionActualAsync(filters);
            return Json(filteredData);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _productionActualService.DeleteProductionActualAsync(id);
            if (result)
            {
                return Json(new { success = true, message = "Production Actual deleted successfully" });
            }
            return Json(new { success = false, message = "Failed to delete Production Actual" });
        }

        [HttpPost]
        public async Task<IActionResult> Retry(string id)
        {
            var result = await _productionActualService.RetryProductionActualAsync(id);
            if (result)
            {
                return Json(new { success = true, message = "Retry initiated successfully" });
            }
            return Json(new { success = false, message = "Failed to initiate retry" });
        }

        [HttpPost]
        public async Task<IActionResult> DownloadErrorLog(string id)
        {
            var log = await _productionActualService.DownloadErrorLogAsync(id);
            return Json(new { success = true, message = "Error log downloaded", data = log });
        }

        [HttpPost]
        public async Task<IActionResult> Intervention(string id)
        {
            var result = await _productionActualService.InterventionProductionActualAsync(id);
            if (result)
            {
                return Json(new { success = true, message = "Intervention initiated successfully" });
            }
            return Json(new { success = false, message = "Failed to initiate intervention" });
        }

        [HttpPost]
        public IActionResult PivotActual()
        {
            // Simulate opening pivot view
            return Json(new { success = true, message = "Opening Production Actual Pivot view..." });
        }
    }
}
