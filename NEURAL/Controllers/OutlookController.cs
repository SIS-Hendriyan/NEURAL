using Microsoft.AspNetCore.Mvc;
using NEURAL.Models.Outlook;
using NEURAL.Services.Outlook;

namespace NEURAL.Controllers
{
    public class OutlookController : Controller
    {
        private readonly IOutlookService _outlookService;

        public OutlookController(IOutlookService outlookService)
        {
            _outlookService = outlookService;
        }

        // GET: Outlook Landing Page
        public IActionResult Index()
        {
            var filterOptions = new
            {
                ProdschedVersions = _outlookService.GetProdschedVersionOptions(),
                ActualVersions = _outlookService.GetActualVersionOptions(),
                Jobsites = new[] { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" }
            };

            ViewBag.FilterOptions = filterOptions;
            return View();
        }

        // POST: Get data for Kendo Grid (Server-side processing)
        [HttpPost]
        public IActionResult GetOutlookData([FromBody] KendoDataSourceRequestModel request)
        {
            try
            {
                var result = _outlookService.GetFilteredOutlookData(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // GET: Create New Outlook
        public IActionResult Create()
        {
            var model = new OutlookModel
            {
                Status = OutlookStatus.Draft,
                CreatedDate = DateTime.Now,
                Author = User.Identity?.Name ?? "Current User"
            };

            ViewBag.ProdschedVersions = _outlookService.GetProdschedVersionOptions();
            ViewBag.ActualVersions = _outlookService.GetActualVersionOptions();
            ViewBag.Jobsites = new[] { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" };

            return View(model);
        }

        // POST: Create New Outlook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OutlookModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedDate = DateTime.Now;
                    model.Author = User.Identity?.Name ?? "Current User";

                    var createdOutlook = _outlookService.CreateOutlook(model);

                    TempData["SuccessMessage"] = "Outlook created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating outlook: {ex.Message}");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.ProdschedVersions = _outlookService.GetProdschedVersionOptions();
            ViewBag.ActualVersions = _outlookService.GetActualVersionOptions();
            ViewBag.Jobsites = new[] { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" };

            return View(model);
        }

        // GET: Create From Existing
        public IActionResult CreateFromExisting(int id)
        {
            try
            {
                var existingOutlook = _outlookService.GetOutlookById(id);
                if (existingOutlook == null)
                {
                    TempData["ErrorMessage"] = "Outlook not found!";
                    return RedirectToAction(nameof(Index));
                }

                var model = new OutlookModel
                {
                    ProdschedVersion = existingOutlook.ProdschedVersion,
                    ActualVersion = existingOutlook.ActualVersion,
                    Status = OutlookStatus.Draft,
                    CreatedDate = DateTime.Now,
                    Author = User.Identity?.Name ?? "Current User",
                    Jobsite = existingOutlook.Jobsite
                };

                ViewBag.ProdschedVersions = _outlookService.GetProdschedVersionOptions();
                ViewBag.ActualVersions = _outlookService.GetActualVersionOptions();
                ViewBag.Jobsites = new[] { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" };
                ViewBag.SourceOutlook = existingOutlook;

                return View("Create", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error copying outlook: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Edit Outlook
        public IActionResult Edit(int id)
        {
            try
            {
                var model = _outlookService.GetOutlookById(id);
                if (model == null)
                {
                    TempData["ErrorMessage"] = "Outlook not found!";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ProdschedVersions = _outlookService.GetProdschedVersionOptions();
                ViewBag.ActualVersions = _outlookService.GetActualVersionOptions();
                ViewBag.Jobsites = new[] { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading outlook: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Edit Outlook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OutlookModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _outlookService.UpdateOutlook(model);
                    TempData["SuccessMessage"] = "Outlook updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating outlook: {ex.Message}");
                }
            }

            ViewBag.ProdschedVersions = _outlookService.GetProdschedVersionOptions();
            ViewBag.ActualVersions = _outlookService.GetActualVersionOptions();
            ViewBag.Jobsites = new[] { "ADARO INDONESIA", "BALANGAN COAL", "MARUWAI COAL" };

            return View(model);
        }

        // POST: Delete Outlook(s)
        [HttpPost]
        public IActionResult Delete([FromBody] int[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return Json(new { success = false, message = "No outlook IDs provided" });
                }

                var deletedCount = _outlookService.DeleteOutlooks(ids);
                return Json(new { success = true, message = $"Successfully deleted {deletedCount} outlook(s)" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting outlooks: {ex.Message}" });
            }
        }

        // POST: Update Status (for workflow transitions)
        [HttpPost]
        public IActionResult UpdateStatus(int id, OutlookStatus newStatus)
        {
            try
            {
                _outlookService.UpdateOutlookStatus(id, newStatus, User.Identity?.Name ?? "Current User");
                return Json(new { success = true, message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating status: {ex.Message}" });
            }
        }
    }
}
