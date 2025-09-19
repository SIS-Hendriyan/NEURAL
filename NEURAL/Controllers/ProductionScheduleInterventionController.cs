
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEURAL.Config;
using NEURAL.Models.Entities;
using NEURAL.Models.Request;
using NEURAL.Models.ViewModel;
using NEURAL.Repositories.Interfaces;
using NEURAL.Services.Implementations;
using NEURAL.Services.Interfaces;
using System.Data;
using System.Threading.Channels;

namespace NEURAL.Controllers
{
    public class ProductionScheduleInterventionController : Controller
    {
        int groupId = 0;
        private readonly IProdSchedInterventionRepository _repository;
        private readonly IProdSchedHeaderRepository _headerRepository;
        private readonly IProdSchedStagingInterventionRepository _stagingInterventionRepository;
        private readonly IProdSchedInterventionExcelExporter _excelExporter;
        private readonly IJobsiteRepository _jobsiteRepository;
        private readonly IProcessRepository _processRepository;
        private readonly Channel<ProdSchedInterventionUploadRequest> _channel;
        private long? jobsiteId = null;
        private string? jobsite = null;
        public ProductionScheduleInterventionController(
            IProdSchedInterventionRepository repository,
            IJobsiteRepository jobsiteRepository,
            IProcessRepository processRepository,
            IProdSchedHeaderRepository headerRepository,
            IProdSchedStagingInterventionRepository stagingInterventionRepository,
            IProdSchedInterventionExcelExporter excelExporter, 
            Channel<ProdSchedInterventionUploadRequest> channel)
        {
            _repository = repository;
            _excelExporter = excelExporter;
            _channel = channel;
            _jobsiteRepository = jobsiteRepository;
            _processRepository = processRepository;
            _headerRepository = headerRepository;
            _stagingInterventionRepository = stagingInterventionRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(long? headerId, string? mbrVersion, string? year)
        {
            var cekCookies = new CekCookies();
            bool statusLogin = false;
            statusLogin = cekCookies.cekCookies(Request, HttpContext, Response);
            
            if (statusLogin)
            {
                ViewBag.NRP = HttpContext.Session.GetString("NRP") ?? string.Empty;
                ViewBag.Email = HttpContext.Session.GetString("EMAIL") ?? string.Empty;
                ViewBag.Name = HttpContext.Session.GetString("Name") ?? string.Empty;
                ViewBag.AppName = HttpContext.Session.GetString("AppName") ?? string.Empty;

                ViewBag.displayBtnLogin = "none";
                ViewBag.displayNRP = "block";
                ViewBag.displayBtnSignOut = "none !important";

                var groupIdStr = HttpContext.Session.GetString("GroupId");
                if (int.TryParse(groupIdStr, out int groupId))
                {
                    try
                    {
                        var divisionCtrl = new DivisionDisplayController();
                        ViewBag.RawMenu = await divisionCtrl.RawRoleMenu(groupId);
                    }
                    catch (Exception ex)
                    {

                        ViewBag.RawMenu = Enumerable.Empty<object>();
                    }
                }
                else
                {
                    ViewBag.RawMenu = Enumerable.Empty<object>();
                }
            }
            else
            {
                ViewBag.displayBtnLogin = "block";
                ViewBag.displayNRP = "none";
                ViewBag.displayBtnSignOut = "none";
                ViewBag.RawMenu = Enumerable.Empty<object>();
            }

            if (headerId is null || headerId <= 0)
            {
                TempData["ToastError"] = "Select a header first.";
                return RedirectToAction("Index", "ProductionSchedule", new { msg = "Select a header first." });
            }

            ViewBag.HeaderId = headerId.Value;
            ViewBag.MbrVersion = mbrVersion ?? string.Empty;
            ViewBag.Year = string.IsNullOrWhiteSpace(year) ? DateTime.UtcNow.Year.ToString() : year;
            ViewBag.JobsiteId = HttpContext.Session.GetInt32("JobsiteId");
            ViewBag.Jobsite = HttpContext.Session.GetString("Jobsite") ?? "";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProcess([FromQuery] long headerId)
        {
            try
            {
                var data = await _processRepository.Query()
                          .Select(s => new ProcessViewModel{
                             Process = (s.NameSicoppPlus ?? s.Name) ?? string.Empty,
                             ProcessId = s.Id
                          }
                          ).ToListAsync();
                var defaultProcess = data.FirstOrDefault(x => string.Equals(x.Process, "OB", StringComparison.OrdinalIgnoreCase));
                var defaultId = defaultProcess?.ProcessId ?? 0;
                return Json(new { data = data, total = data.Count,defaultId});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJobsites([FromQuery] long headerId, CancellationToken ct)
        {
            try
            {
                var q = _jobsiteRepository.Query();
                if(jobsite != null && jobsite.ToUpper() !!= "JAHO")
                {
                    q = q.Where(f => f.Name.ToUpper() == jobsite.ToUpper());
                }

                var data = await q
                    .Select(s => new JobsiteViewModel
                    {
                        JobsiteId = s.Id,
                        Jobsite = (s.NameSicoppPlus ?? s.Name) ?? string.Empty
                    })
                    .ToListAsync(ct);


                var defaultJobsite = data.FirstOrDefault(x =>string.Equals(x.Jobsite, "ADARO INDONESIA", StringComparison.OrdinalIgnoreCase));
                var defaultId = defaultJobsite?.JobsiteId ?? 0;

                return Json(new { data = data, total = data.Count,defaultId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInterventions([FromQuery] long headerId, long jobsiteId, long processId)
        {
            try
            {
                var data = await _repository.GetInterventions(headerId, jobsiteId, processId);
                return Json(new { data = data, total = data.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }

        public async Task<IActionResult> DownloadDialog(long headerId, string mbrVersion)
        {
            ViewBag.HeaderId = headerId;
            ViewBag.MbrVersion = mbrVersion;
            return PartialView("_ProdSchedDownloadDialog");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadIntervention(
                long headerId,            
                int month,
                string mbrVersion,
                CancellationToken ct)
        {
     
            var rows = await _repository.GetInterventionsForDownload(headerId, month, ct);
            var monthStat = await _repository.GetMonthStats(headerId, month);

            if (rows.Count == 0) return NotFound("No data to export.");

            var bytes = await _excelExporter.BuildFromData(rows, monthStat, month, mbrVersion, ct);
            var file = $"InterventionPivot_{headerId}_{month}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file);

        }

        public async Task<IActionResult> UploadDialog()
        {
            return PartialView("_ProdSchedUploadDialog");
        }

        [HttpPost]
        public async Task<IActionResult> UploadIntervention(IFormFile file, CancellationToken ct)
        {
            try
            {
                if (file is null || file.Length == 0)
                    return BadRequest("No file.");

                //1 EXTRACT XL
                var ms = new MemoryStream((int)Math.Min(file.Length, int.MaxValue));
                await file.CopyToAsync(ms, ct);
                ms.Position = 0; // rewind


                var createdBy = User?.Identity?.Name ?? "system";
                var data = ProdSchedPivotExcelInterventionReader.Read(ms, createdBy: createdBy);
                long headerId = data.Select(s => s.ProdSchedHeaderId).Distinct().FirstOrDefault();
                int year = data.Select(s => s.DailyDate.Year).Distinct().FirstOrDefault();
                int month = data.Select(s => s.DailyDate.Month).Distinct().FirstOrDefault();
                string mbrVersion = data.Select(s => s.MbrVersion).Distinct().FirstOrDefault();

                //2 GET next MBR VERSION
                var newMbrVersion = await _headerRepository.GetNextMbr(headerId);

                //3 INSERT HEADER
                var fileBytes = ms.ToArray();
                var header = new PRODSCHED_HEADER_T
                {
                    MBR_VERSION = newMbrVersion,
                    YEAR = year.ToString(),
                    DOCUMENT_NAME = file.FileName,
                    FILE_DATA = fileBytes,
                    CREATED_AT = DateTime.Now,
                    CREATED_BY = User?.Identity?.Name ?? "System"
                };
                await _headerRepository.InsertProdSchedHeader(header);

                //4 DELETE STAGING BY HEADER ID
                await _stagingInterventionRepository.DeleteByHeaderId(headerId, CancellationToken.None);

                //5 BULK INSERT STAGING
                await _stagingInterventionRepository.BulkWriteAsync(ToDataTable(data, newMbrVersion), newMbrVersion, CancellationToken.None);

                //6 ENQUEUE BACKGROUND SERVICE 
                await _channel.Writer.WriteAsync(new ProdSchedInterventionUploadRequest(headerId, mbrVersion, newMbrVersion, year, month), CancellationToken.None);
                return Ok(new { message = "Upload Processed"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }

        }

        private static DataTable ToDataTable(
        IEnumerable<ProdSchedStagingInterventionViewModel> data,
        string newMbrVersion)
        {
            var table = new DataTable();

            table.Columns.Add("PRODSCHED_HEADER_ID", typeof(long));
            table.Columns.Add("MBR_VERSION", typeof(string));
            table.Columns.Add("MBR_VERSION_NEW", typeof(string));
            table.Columns.Add("JOBSITE_ID", typeof(long));
            table.Columns.Add("PROCESS_ID", typeof(long));
            table.Columns.Add("DAILY_DATE", typeof(DateTime));
            table.Columns.Add("PARAMETER", typeof(string));
            table.Columns.Add("VALUE", typeof(string));
            table.Columns.Add("CREATED_AT", typeof(DateTime));
            table.Columns.Add("CREATED_BY", typeof(string));

            foreach (var r in data)
            {
                table.Rows.Add(
                    r.ProdSchedHeaderId,
                    r.MbrVersion,
                    newMbrVersion,
                    r.JobsiteId,
                    r.ProcessId,
                    r.DailyDate,
                    r.Parameter,
                    (object?)r.Value ?? DBNull.Value,
                    r.CreatedAt,
                    r.CreatedBy
                );
            }

            return table;
        }
    }
}
