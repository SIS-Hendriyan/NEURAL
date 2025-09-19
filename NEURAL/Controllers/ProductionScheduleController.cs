
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEURAL.Config;
using NEURAL.Models.Entities;
using NEURAL.Models.Request;
using NEURAL.Repositories.Interfaces;
using NEURAL.Services.Interfaces;
using System.Threading.Channels;

namespace NEURAL.Controllers
{
    public class ProductionScheduleController : Controller
    {
        int groupId = 0;
        private readonly IProdSchedHeaderRepository _prodSchedHeaderRepository;
        private readonly IProdSchedExcelReader _excelReader;  
        private readonly IProdSchedStagingRepository _prodSchedStagingRepository;       
        private readonly Channel<ProdSchedUploadRequest> _spChannel;

        public ProductionScheduleController(IProdSchedHeaderRepository prodSchedHeaderRepository, IProdSchedExcelReader excelReader, IProdSchedStagingRepository prodSchedStagingRepository, Channel<ProdSchedUploadRequest> spChannel)
        {
            _prodSchedHeaderRepository = prodSchedHeaderRepository;
            _excelReader = excelReader;
            _prodSchedStagingRepository = prodSchedStagingRepository;
            _spChannel = spChannel;
        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync()
        {
            CekCookies _CekCookies = new CekCookies();

            bool statusLogin = _CekCookies.cekCookies(Request, HttpContext, Response);

            if (statusLogin)
            {
                ViewBag.NRP = HttpContext.Session.GetString("NRP");
                ViewBag.Email = HttpContext.Session.GetString("EMAIL");
                ViewBag.Name = HttpContext.Session.GetString("Name");
                ViewBag.AppName = HttpContext.Session.GetString("AppName");
                ViewBag.displayBtnLogin = "none";
                ViewBag.displayNRP = "block";//"none !important";
                ViewBag.displayBtnSignOut = "none !important";
                string GroupId = HttpContext.Session.GetString("GroupId");

                DivisionDisplayController _DivisionDisplayController = new DivisionDisplayController();
                ViewBag.RawMenu = await _DivisionDisplayController.RawRoleMenu(groupId);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetHeaders()
        {
            try
            {
                var data = await _prodSchedHeaderRepository.GetAllHeaders();
                return Json(new { data = data, total = data.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> UploadDialog()
        {
            return PartialView("_ProdSchedUploadDialog");
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            try
            {
                // 1) EXTRACT EXCEL 
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms, CancellationToken.None);
                ms.Position = 0;

                var parsed = _excelReader.Read(ms);   
                if (string.IsNullOrWhiteSpace(parsed.ExtractedMbrVersion))
                    return BadRequest("MBR_VERSION not found in Excel.");

                var mbr = parsed.ExtractedMbrVersion!;
                var year = parsed.ExtractedYear;
                var fileBytes = ms.ToArray();              

                // 2) INSERT HEADER
                var header = new PRODSCHED_HEADER_T
                {
                    MBR_VERSION = mbr,
                    YEAR = year,
                    DOCUMENT_NAME = file.FileName,
                    FILE_DATA = fileBytes,         
                    CREATED_AT = DateTime.Now,
                    CREATED_BY = User?.Identity?.Name ?? "System"
                };
                await _prodSchedHeaderRepository.InsertProdSchedHeader(header);

                // 3) DELETE STAGING BY MBR
                await _prodSchedStagingRepository.DeleteByMbrAsync(mbr, CancellationToken.None);

                // 4) BULK INSERT STAGING
                await _prodSchedStagingRepository.BulkWriteAsync(parsed.Data, CancellationToken.None);

                // 5) ENQUEUE BACKGROUND SERVICE 
                await _spChannel.Writer.WriteAsync(new ProdSchedUploadRequest(mbr), CancellationToken.None);

             
                return Ok(new
                {
                    message = $"Upload Processed",
                    headerId = header.ID,
                    versionName = header.VERSION_NAME,
                    mbrVersion = mbr
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> Download(long id, CancellationToken ct)
        {
            var (found, fileName, contentType, data) = await _prodSchedHeaderRepository.DownloadFile(id, ct);
            if (!found) return NotFound();

            return File(data, contentType, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadError(long id, CancellationToken ct)
        {
            try
            {
                var errorContent = await _prodSchedHeaderRepository.DownloadError(id, ct);
                if (string.IsNullOrEmpty(errorContent))
                    return NotFound("No error content found for the specified ID.");
                var fileName = $"ProdSchedErrors_{id}_{DateTime.UtcNow:yyyyMMddHHmmss}.txt";
                var contentType = "text/plain";
                var data = System.Text.Encoding.UTF8.GetBytes(errorContent);
                return File(data, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _prodSchedHeaderRepository.DeleteProdschedHeader(id);
                return Json(new { success = true,message ="Delete Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
