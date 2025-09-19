using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEURAL.Config;
using NEURAL.Repositories.Interfaces;
using NEURAL.Utils;

namespace NEURAL.Controllers
{
    public class ProductionSchedulePivotController : Controller
    {
        int groupId = 0;
        private readonly IProdSchedHeaderRepository _prodSchedHeaderRepository;
        private readonly IProdSchedPivotRepository _prodSchedPivotRepository;

        public ProductionSchedulePivotController(IProdSchedHeaderRepository prodSchedHeaderRepository, IProdSchedPivotRepository prodSchedPivotRepository)
        {
            _prodSchedHeaderRepository = prodSchedHeaderRepository;
            _prodSchedPivotRepository = prodSchedPivotRepository;
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
        public async Task<IActionResult> GetMbrVersion([FromQuery] int? year)
        {
            try
            {
                var y = year ?? DateTime.UtcNow.Year; 
                var data = await _prodSchedHeaderRepository.GetMBRVersion(y);
                return Json(new { data = data, total = data.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetYearList()
        {
            try
            {
                var data = await _prodSchedHeaderRepository.GetYearList();
                return Json(new { data = data, total = data.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPivotData( [FromQuery] int year,
                                                        [FromQuery] string tipeData,
                                                        [FromQuery] string periode,
                                                        [FromQuery] string mbrVersion)
        {
            try
            {

                var tipe = (tipeData ?? "").Trim().ToLowerInvariant();
                var per = (periode ?? "").Trim().ToLowerInvariant();
                var mbr = mbrVersion;

                var data  = await _prodSchedPivotRepository.GetPivot(per, tipe, mbr, year);
                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
