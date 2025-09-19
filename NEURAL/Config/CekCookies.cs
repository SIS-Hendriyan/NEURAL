using System.Data.SqlClient;
using System.Data;
using NEURAL.Controllers;
using NEURAL.Models.Entities;


namespace NEURAL.Config
{
    public class CekCookies
    {
        public bool cekCookies(HttpRequest Request, HttpContext HttpContext, HttpResponse Response)
        {
            bool status = false;
            CommonFunction _CommonFunction = new CommonFunction();
            string cookiesName = Startup.cookiesName;
            //string NRP = _CommonFunction.Decrypt(Request.Cookies[cookiesName]);
            string NRP = "00118354";
            var Notif = "";

            var HostNameExternal = Request.Cookies["SuperAppsHostNameExternal"];
            var HostNameInternal = Request.Cookies["SuperAppsHostNameInternal"];
            var hostName = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            if (!hostName.Equals(HostNameExternal))
            {
                hostName = HostNameInternal;
            }

            if (NRP == null)
            {
                HttpContext.Session.Clear();
                var SuperAppsLogin = Request.Cookies["SuperAppsLogin"];

                Response.Redirect(hostName + SuperAppsLogin);
            }
            else
            {
                string AppName = Startup.serverConfig;
                HttpContext.Session.SetString("AppName", AppName);
                HttpContext.Session.SetString("NRP", NRP);
                
                try
                {
                    List<USER_T> UserList = new List<USER_T>();
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
                        cmd.CommandText = "[dbo].[SP_LOGIN]";
                        cmd.Parameters.Add(new SqlParameter("@NRP", SqlDbType.VarChar)).Value = NRP;

                        sda.SelectCommand = cmd;

                        using (DataTable dt = new DataTable())
                        {
                            dt.TableName = "USER_T";
                            sda.Fill(dt);

                            foreach (DataRow dr in dt.Rows)
                            {
                                var item = new USER_T()
                                {
                                    USER_ID = Convert.ToString(dr[0]),
                                    NRP = Convert.ToString(dr[1]),
                                    GROUP_ID = int.Parse(Convert.ToString(dr[2])),
                                    GROUP = Convert.ToString(dr[3]),
                                    JOBSITE_ID = int.Parse(Convert.ToString(dr[4])),
                                    JOBSITE = Convert.ToString(dr[5]),
                                    NAME = Convert.ToString(dr[6]),
                                    EMAIL = Convert.ToString(dr[7])
                                };

                                UserList.Add(item);
                            }
                        }
                        con.Close();
                    }

                    if (!UserList[0].USER_ID.Equals("0"))
                    {
                        HttpContext.Session.SetString("USER_ID", UserList[0].USER_ID);
                        HttpContext.Session.SetString("EMAIL", UserList[0].EMAIL);
                        HttpContext.Session.SetString("GroupId", UserList[0].GROUP_ID.ToString());
                        HttpContext.Session.SetString("Group", UserList[0].GROUP);
                        HttpContext.Session.SetInt32("JobsiteId", UserList[0].JOBSITE_ID);
                        HttpContext.Session.SetString("Jobsite", UserList[0].JOBSITE);
                        HttpContext.Session.SetString("Name", UserList[0].NAME);

                        if (HttpContext.Session.GetString("Jobsite") == "JAHO")
                        {
                            HttpContext.Session.SetString("Access", "ALL");
                        }
                        else
                        {
                            HttpContext.Session.SetString("Access", HttpContext.Session.GetString("Jobsite"));
                        }

                        HttpContext.Session.SetString("Notif", Notif);
                    }
                    else
                    {
                        HttpContext.Session.SetString("NRP", NRP);
                        Notif = "User with NRP " + NRP + " is not registered in System, Please contact your Admin to register your NRP";

                        HttpContext.Session.SetString("Notif", Notif);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                status = true;
            }

            return status;
        }

    }
}
