using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NEURAL.Models;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Security.Principal;
using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authentication;
using System.Data;
using SSO;
using System.Security;
using Novell.Directory.Ldap;
using System.Data.SqlClient;
using Aspose.Slides.Theme;
using NEURAL.Config;
using Microsoft.PowerBI.Api.Models;
using System.Text.RegularExpressions;

namespace NEURAL.Controllers
{
    public class HomeController : Controller
    {
        int groupId = 0;

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
    }
}