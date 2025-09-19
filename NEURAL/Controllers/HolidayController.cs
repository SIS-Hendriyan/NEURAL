using Microsoft.AspNetCore.Mvc;
using NEURAL.Config;
using NEURAL.Repositories.Interfaces;
using System.Net;

namespace NEURAL.Controllers
{
    public class HolidayController : Controller
    {
        int groupId = 0;
        private readonly IHolidayRepository _holidayRepository;
        public HolidayController(IHolidayRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }
        public async Task<IActionResult> Index()
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
        public async Task<IActionResult> GetSummary()
        {
            var data = await _holidayRepository.GetSummary();
            return Json(new { data = data, total = data.Count });
        }


        public async Task<IActionResult> DetailDialog(string jobsite,long jobsiteId, int holidayYear, string mbrVersion)
        {
            var data = await _holidayRepository.GetDetailByFilter(holidayYear, jobsiteId, mbrVersion);
            ViewBag.JobSite = jobsite;
            ViewBag.MbrVersion = mbrVersion;
            ViewBag.HolidayYear = holidayYear;
            ViewBag.JobsiteId = jobsiteId;

            return PartialView("_HolidayDetailDialog",data);
        }

        [HttpGet]
        public async Task<IActionResult> SyncHoliday()
        {
            HttpStatusCode status = await _holidayRepository.CallHolidayApi();

            return status switch
            {
                HttpStatusCode.OK => Ok(),
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.BadRequest => BadRequest(),
                _ => StatusCode((int)status)
            };
        }

    }
}
