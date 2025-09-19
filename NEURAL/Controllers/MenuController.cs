using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
using NEURAL.Models.Entities;

namespace NEURAL.Controllers
{
    public class MenuController : Controller
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

        [AcceptVerbs("Get", "Post")]
        public async Task<ActionResult> CRUD(string data, string ket)
        {
            try
            {
                List<MENU_T> MENU_T;

                List<MENU_T> dataList = new List<MENU_T>();
                List<RETURN_T> Return = new List<RETURN_T>();

                SqlConnection con = new SqlConnection(Startup.connectionstring);
                con.Open();

                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandTimeout = 3600;
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "[dbo].[MENU_SP]";

                    if (!ket.Equals("READ"))
                    {
                        MENU_T = JsonConvert.DeserializeObject<List<MENU_T>>(data);

                        cmd.Parameters.Add(new SqlParameter("@MENU_ID", SqlDbType.VarChar)).Value = MENU_T[0].MENU_ID;
                        cmd.Parameters.Add(new SqlParameter("@MENU", SqlDbType.VarChar)).Value = MENU_T[0].MENU;
                        cmd.Parameters.Add(new SqlParameter("@URL", SqlDbType.VarChar)).Value = MENU_T[0].URL;
                        cmd.Parameters.Add(new SqlParameter("@PARENT_ID", SqlDbType.VarChar)).Value = MENU_T[0].PARENT_ID;
                    }

                    cmd.Parameters.Add(new SqlParameter("@USER_NRP", SqlDbType.VarChar)).Value = HttpContext.Session.GetString("NRP");
                    cmd.Parameters.Add(new SqlParameter("@KET", SqlDbType.VarChar)).Value = ket;

                    sda.SelectCommand = cmd;

                    using (DataTable dt = new DataTable())
                    {
                        dt.TableName = "MENU_T";
                        sda.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            if (ket.Equals("READ"))
                            {
                                var item = new MENU_T()
                                {
                                    MENU_ID = Convert.ToString(dr[0]),
                                    MENU = Convert.ToString(dr[1]),
                                    URL = Convert.ToString(dr[2]),
                                    PARENT_ID = Convert.ToString(dr[3]),
                                    PARENT = Convert.ToString(dr[4])
                                };

                                dataList.Add(item);
                            }
                            else
                            {
                                var item = new RETURN_T()
                                {
                                    VALUE = Convert.ToString(dr[0]),
                                    STATUS = Convert.ToString(dr[1])
                                };

                                Return.Add(item);
                            }
                        }
                    }
                    con.Close();
                }

                if (ket.Equals("READ"))
                {
                    return Json(dataList);
                }
                else
                {
                    return Json(new { status = Return[0].STATUS, title = Return[0].VALUE, text = Return[0].VALUE });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "failed", title = ex.Message.ToString(), text = ex.ToString() });
            }
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<ActionResult> getMenuAccess(string menuName, string restrictionType)
        {
            try
            {
                List<MENU_T> MenuList = new List<MENU_T>();

                SqlConnection con = new SqlConnection(Startup.connectionstring);
                con.Open();

                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandTimeout = 3600;
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "[dbo].[MENU_RESTRICTION_SP]";
                    cmd.Parameters.Add(new SqlParameter("@MENU", SqlDbType.VarChar)).Value = menuName;
                    cmd.Parameters.Add(new SqlParameter("@GROUP_ID", SqlDbType.VarChar)).Value = HttpContext.Session.GetString("GroupId");
                    cmd.Parameters.Add(new SqlParameter("@USER_NRP", SqlDbType.VarChar)).Value = HttpContext.Session.GetString("NRP");
                    cmd.Parameters.Add(new SqlParameter("@KET", SqlDbType.VarChar)).Value = "Get User Menu Restriction";
                    cmd.Parameters.Add(new SqlParameter("@RESTRICTION_TYPE", SqlDbType.VarChar)).Value = restrictionType;

                    sda.SelectCommand = cmd;

                    using (DataTable dt = new DataTable())
                    {
                        dt.TableName = "MENU_T";
                        sda.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            var item = new MENU_T()
                            {
                                MENU_ID = Convert.ToString(dr[0]),
                                MENU = Convert.ToString(dr[1]),
                                PARENT = Convert.ToString(dr[2]),
                                PARENT_ID = Convert.ToString(dr[3]),
                                URL = Convert.ToString(dr[4]),
                                TYPE = Convert.ToString(dr[5])
                            };

                            MenuList.Add(item);
                        }
                    }
                    con.Close();
                }
                bool readOnly = MenuList.Count > 0 ? true : false;
                return Json(readOnly);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}