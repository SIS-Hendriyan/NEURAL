using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using NEURAL.Config;
using NEURAL.Models.Entities;

namespace NEURAL.Controllers
{
    public class DivisionDisplayController : Controller
    {
        private readonly MenuSkoteRecursive _MenuRecursive;

        public IConfiguration Configuration { get; private set; }

        public DivisionDisplayController()
        {
            _MenuRecursive = new MenuSkoteRecursive();
        }

        public async Task<String> RawRoleMenu(int groupId)
        {
            try
            {
                List<MENU_T> _MENU_T = new List<MENU_T>();
                var menu = getDataMenu(groupId);

                _MenuRecursive.InitMenu(_MENU_T, menu);
                var RawMenu = _MenuRecursive.GenerateMenuUi();

                return await Task.FromResult(RawMenu);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MENU_T> getDataMenu(int GROUP_ID)
        {
            try
            {
                string AppName = Startup.serverConfig;
                List<MENU_T> Data = new List<MENU_T>();

                SqlConnection con = new SqlConnection(Startup.connectionstring);
                con.Open();

                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    string app = "";
                    if (AppName != "")
                    {
                        app = AppName + "/";
                    }
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandTimeout = 3600;
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "[dbo].[SP_GET_MENU]";
                    cmd.Parameters.Add(new SqlParameter("@GROUP_ID", SqlDbType.Int)).Value = GROUP_ID;

                    sda.SelectCommand = cmd;

                    using (DataTable dt = new DataTable())
                    {
                        dt.TableName = "MENU_T";
                        sda.Fill(dt);

                        foreach (DataRow dr in dt.Rows)
                        {
                            string urlx = Convert.ToString(dr[4]);
                            string appx = app;
                            if (urlx.Count() > 3)
                            {
                                if (urlx.Substring(0, 4).Equals("http"))
                                {
                                    appx = "";
                                }
                            }

                            var item = new MENU_T()
                            {
                                MENU_ID = Convert.ToString(dr[0]),
                                MENU = Convert.ToString(dr[1]),
                                PARENT = Convert.ToString(dr[2]),
                                PARENT_ID = Convert.ToString(dr[3]),
                                URL = appx + Convert.ToString(dr[4]),
                                RESTRICTION_TYPE = Convert.ToString(dr[5])
                            };

                            Data.Add(item);
                        }
                    }
                    con.Close();
                }
                return Data;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}