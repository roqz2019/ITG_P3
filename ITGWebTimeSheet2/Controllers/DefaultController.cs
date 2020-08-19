using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using ITGWebTimeSheet2.Models;

namespace ITGWebTimeSheet2.Controllers
{
    public class DefaultController : CustomController
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users users)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "Select * from  [dbo].[staff] where username=@Username and pass=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", users.Username);
                cmd.Parameters.AddWithValue("@Password", users.Password);
                SqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.Read())
                {
                    //  Session["Username"] = users.Username;
                    //  Session["Fullname"] = users.fullname;
                    Session["alias"] = Convert.ToString(dataReader["fullname"]);
                    Session["res_id"] = Convert.ToString(dataReader["id"]);
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Message = "Your Login details failed!";
                }
                con.Close();
            }
            return View("Index");
        }


        public ActionResult Login()
        {
            if (Session["alias"] == null)
            {
                Session.Abandon();
                return View("Index");
            }


            DayOfWeek firstWeekDay = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            DateTime startDateOfWeek = DateTime.Now;
            while (startDateOfWeek.DayOfWeek != firstWeekDay)
            { startDateOfWeek = startDateOfWeek.AddDays(-1d); }

            DateTime endDateOfWeek = startDateOfWeek.AddDays(6d);

            DateTime nextweek = startDateOfWeek.AddDays(7d);
            DateTime prevweek = startDateOfWeek.AddDays(-2d);
            //return checkDate >= startDateOfWeek && checkDate <= endDateOfWeek;

            string sweek = startDateOfWeek.ToString("yyyy-MM-dd");
            string fweek = endDateOfWeek.ToString("yyyy-MM-dd");


            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                //    string query = " SELECT TL.id as id, TL.taskid as taskid, C.code as cust,P.code as proj, T.description as descr,  T.note as note, TL.edate as edate, TL.acthours as acthours " +
                //                   " from  [dbo].[taskman] as T, [dbo].[customers] as C, [dbo].[project] as P, [dbo].[tasklist] as TL" +
                //                   " WHERE C.id=T.customerid AND P.id=T.projectid AND TL.taskid=T.id ORDER BY TL.id DESC ";

                //AND TL.taskid=T.id
                string query = "Select TL.id as id,TL.taskid as taskid,TL.custid as custid,TL.projid as projid, TL.edate as edate, TL.notes as notes, TL.acthours as acthours,TL.stime as stime,TL.ftime as ftime, TK.resource as res_id, TL.status as status,TK.status as status1 From  [dbo].[tasklist] as TL, [dbo].[taskman] as TK WHERE edate BETWEEN '" +
                    sweek + "' AND '" + fweek + "' AND TL.taskid=TK.id AND TK.resource=" + Session["res_id"] + " ORDER BY TL.id DESC";

                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.taskid = Convert.ToInt16(dataReader["taskid"]);
                    sitems.cust = Convert.ToString(dataReader["custid"]);
                    sitems.proj = Convert.ToString(dataReader["projid"]);
                    //  sitems.description = Convert.ToString(dataReader["descr"]);
                    sitems.acthours = Convert.ToDecimal(dataReader["acthours"]);
                    string edate = Convert.ToString(dataReader["edate"]);
                    sitems.ddate = String.Format("{0:MM/dd/yyyy}", edate);
                    //   sitems.note = Convert.ToString(dataReader["note"]);

                    sitems.notes = Convert.ToString(dataReader["notes"]);
                    sitems.taskstatus = Convert.ToString(dataReader["status"]);
                    sitems.status = Convert.ToString(dataReader["status1"]);

                    string q3 = "Select note From  [dbo].[taskman] WHERE id=" + Convert.ToString(dataReader["taskid"]);
                    //  string not = "";
                    SqlCommand cmd2 = new SqlCommand(q3, con);
                    SqlDataReader dataReader2 = cmd2.ExecuteReader();
                    while (dataReader2.Read())
                    {
                        sitems.note = Convert.ToString(dataReader2["note"]);
                    }


                    string st = "";
                    if (dataReader["stime"] == DBNull.Value)
                    {
                        st = "Set time";
                    }
                    else
                    {
                        st = Convert.ToString(dataReader["stime"]);
                        var timeFromInput = DateTime.ParseExact(st, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None);
                        string timeOutput = timeFromInput.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                        st = timeOutput;

                    }
                    string ft = "";

                    if (dataReader["ftime"] == DBNull.Value)
                    {
                        ft = " ";
                    }
                    else
                    {
                        ft = Convert.ToString(dataReader["ftime"]);
                        var timeFromInput = DateTime.ParseExact(ft, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None);
                        string timeOutput = timeFromInput.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                        ft = timeOutput;
                    }


                    sitems.stime = st;
                    sitems.ftime = ft;


                    taskmanlist.Add(sitems);

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();
            }
            ViewData["begin1"] = startDateOfWeek.ToString("MM/dd/yyyy");
            ViewData["end1"] = endDateOfWeek.ToString("MM/dd/yyyy");

            ViewData["next1"] = nextweek.ToString("yyyy-MM-dd");
            ViewData["prev1"] = prevweek.ToString("yyyy-MM-dd");
            return View(taskmanlist);

        }




        public ActionResult UpdateProject(string id, string name)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "UPDATE  [dbo].[project]  SET name=@name WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return RedirectToAction("Project");
        }

        //---------------- Task Manager Updates sectio here  ---------------------
        public ContentResult ShowTaskList(string custid, string projid)
        {

            string tempannounce = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                DateTime myDateTime = DateTime.Now;
                string sqld = myDateTime.ToString("yyyy-MM-dd");

                string query = "Select T.id as taskid, T.description as taskdesc " +
                                "FROM [dbo].[taskman] as T,  [dbo].[customers] as C,  [dbo].[project] as P " +
                                "Where T.projectid > 0  AND C.id = T.customerid AND P.id= T.projectid AND T.description NOT LIKE 'Add comment' AND  P.id=" + projid + " AND T.resource=" + Session["res_id"];

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    string taskid = Convert.ToString(dataReader["taskid"]);
                    string taskname = Convert.ToString(dataReader["taskdesc"]);

                    tempannounce = tempannounce + taskid + ":::" + taskname;
                    tempannounce = tempannounce + "^";
                }
                con.Close();
            }

            return Content(tempannounce);

        }

        public ContentResult ShowProject(string id)
        {
            string tempannounce = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                DateTime myDateTime = DateTime.Now;
                string sqld = myDateTime.ToString("yyyy-MM-dd");

                string query = "Select (T.customerid) as custid,C.code as custcode, T.projectid as projid, P.code as pcode " +
                                " FROM  [dbo].[taskman] as T,  [dbo].[customers] as C,  [dbo].[project] as P" +
                                " Where T.projectid > 0  AND C.id = T.customerid AND P.id= T.projectid AND T.description NOT LIKE 'Add comment' AND T.customerid=" + id;


                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    string custid = Convert.ToString(dataReader["projid"]);
                    string custcode = Convert.ToString(dataReader["pcode"]);

                    tempannounce = tempannounce + custid + ":" + custcode;
                    tempannounce = tempannounce + "^";



                }
                con.Close();
            }

            return Content(tempannounce);
        }

        public ContentResult ShowCustWithProj()
        {
            string tempannounce = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                DateTime myDateTime = DateTime.Now;
                string sqld = myDateTime.ToString("yyyy-MM-dd");

                string query = "Select DISTINCT(T.customerid) as custid,C.code as custcode " +
                                "FROM [dbo].[taskman] as T,  [dbo].[customers] as C " +
                                " Where T.projectid > 0  AND C.id = T.customerid AND T.description NOT LIKE 'Add comment' AND T.resource=" + Session["res_id"];

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    string custid = Convert.ToString(dataReader["custid"]);
                    string custcode = Convert.ToString(dataReader["custcode"]);

                    tempannounce = tempannounce + custid + ":" + custcode;
                    tempannounce = tempannounce + "^";



                }
                con.Close();
            }

            return Content(tempannounce);
        }

        public ActionResult UpdateTimeCust(string id, string custid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[tasklist]  SET custid=@custid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@custid", custid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }

        [HttpGet]
        public ActionResult UpdateTaskCust(string id, string custid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET customerid=@custid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@custid", custid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }

        public ActionResult UpdateTimeTask(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[tasklist]  SET taskid=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();

        }
        public ActionResult UpdateTimeProj(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[tasklist]  SET projid=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }

        public ActionResult UpdateTaskProj(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET projectid=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }
        public ActionResult UpdateTaskStaff(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET staffid=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }

        public ActionResult UpdateTaskDesc(string id, string descr)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET description=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", descr);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }

        public ContentResult UpdateStime(string id, string pid, string ft)
        {

            //   TimeSpan time = TimeSpan.Parse(pid);

            string ftime2 = "";
            DateTime stime = DateTime.ParseExact(pid, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            double ft1 = Convert.ToDouble(ft);
            DateTime ftime = stime.AddHours(ft1);

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[tasklist]  SET stime=@pid,ftime=@ftime WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", stime);
                cmd.Parameters.AddWithValue("@ftime", ftime);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            //---------------------------------------
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "Select ftime From  [dbo].[tasklist] WHERE id=" + id + " ORDER BY id DESC";

                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    ft = Convert.ToString(dataReader["ftime"]);
                }
                con.Close();
            }

            var timeFromInput = DateTime.ParseExact(ft, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None);
            string timeOutput = timeFromInput.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            ft = timeOutput;

            ftime2 = Convert.ToString(ft);

            return Content(ftime2);

        }

        public ActionResult UpdateTimesheetNote(string id, string name)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "UPDATE  [dbo].[tasklist]  SET notes=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", name);
                cmd.ExecuteNonQuery();
                con.Close();

            }
            return View();
        }

        public ActionResult TimeSheet(string ed)
        {
            if (Session["alias"] == null)
            {
                Session.Abandon();
                return View("Index");
            }



            DateTime result = DateTime.MinValue;
            if (ed.Contains("-") == true)
            {
                result = DateTime.ParseExact(ed, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (ed.Contains("/") == true)
            {
                result = DateTime.ParseExact(ed, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            // reference date is ed
            DayOfWeek firstWeekDay = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            DateTime startDateOfWeek = result;//DateTime.Now;
            while (startDateOfWeek.DayOfWeek != firstWeekDay)
            { startDateOfWeek = startDateOfWeek.AddDays(-1d); }

            DateTime endDateOfWeek = startDateOfWeek.AddDays(6d);

            DateTime nextweek = startDateOfWeek.AddDays(7d);
            DateTime prevweek = startDateOfWeek.AddDays(-2d);

            //return checkDate >= startDateOfWeek && checkDate <= endDateOfWeek;

            string sweek = startDateOfWeek.ToString("yyyy-MM-dd");
            string fweek = endDateOfWeek.ToString("yyyy-MM-dd");


            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                //    string query = " SELECT TL.id as id, TL.taskid as taskid, C.code as cust,P.code as proj, T.description as descr,  T.note as note, TL.edate as edate, TL.acthours as acthours " +
                //                   " from  [dbo].[taskman] as T, [dbo].[customers] as C, [dbo].[project] as P, [dbo].[tasklist] as TL" +
                //                   " WHERE C.id=T.customerid AND P.id=T.projectid AND TL.taskid=T.id ORDER BY TL.id DESC ";

                //AND TL.taskid=T.id
                string query = "";
                if (ed.Contains("-") == true)
                {
                    //     query = "Select * From  [dbo].[tasklist] WHERE edate BETWEEN '" +
                    //     sweek + "' AND '" + fweek + "' ORDER BY id DESC";
                }
                if (ed.Contains("/") == true)
                {
                    string seldate = result.ToString("yyyy-MM-dd");
                    //      query = "Select * From  [dbo].[tasklist] WHERE edate BETWEEN '" +
                    //       sweek + "' AND '" + fweek + "' ORDER BY id DESC";
                }

                query = "Select TK.status as status1, TL.id as id,TL.taskid as taskid,TL.custid as custid,TL.projid as projid, TL.edate as edate, TL.notes as notes, TL.acthours as acthours,TL.stime as stime,TL.ftime as ftime, TK.resource as res_id, TL.status as status From  [dbo].[tasklist] as TL, [dbo].[taskman] as TK WHERE edate BETWEEN '" +
                  sweek + "' AND '" + fweek + "' AND TL.taskid=TK.id AND TK.resource=" + Session["res_id"] + " ORDER BY TL.id DESC";


                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.taskid = Convert.ToInt16(dataReader["taskid"]);
                    sitems.cust = Convert.ToString(dataReader["custid"]);
                    sitems.proj = Convert.ToString(dataReader["projid"]);
                    //  sitems.description = Convert.ToString(dataReader["descr"]);
                    sitems.acthours = Convert.ToDecimal(dataReader["acthours"]);
                    string edate = Convert.ToString(dataReader["edate"]);
                    sitems.ddate = String.Format("{0:MM/dd/yyyy}", edate);
                    //   sitems.note = Convert.ToString(dataReader["note"]);

                    sitems.notes = Convert.ToString(dataReader["notes"]);
                    sitems.taskstatus = Convert.ToString(dataReader["status"]);
                    sitems.status = Convert.ToString(dataReader["status1"]);

                    string q3 = "Select note From  [dbo].[taskman] WHERE id=" + Convert.ToString(dataReader["taskid"]);
                    //  string not = "";
                    SqlCommand cmd2 = new SqlCommand(q3, con);
                    SqlDataReader dataReader2 = cmd2.ExecuteReader(); 
                    while (dataReader2.Read())
                    {
                        sitems.note = Convert.ToString(dataReader2["note"]);
                    }

                    string st = "";
                    if (dataReader["stime"] == DBNull.Value)
                    {
                        st = "Set time";
                    }
                    else
                    {
                        st = Convert.ToString(dataReader["stime"]);
                        var timeFromInput = DateTime.ParseExact(st, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None);
                        string timeOutput = timeFromInput.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                        st = timeOutput;

                    }
                    string ft = "";

                    if (dataReader["ftime"] == DBNull.Value)
                    {
                        ft = " ";
                    }
                    else
                    {
                        ft = Convert.ToString(dataReader["ftime"]);
                        var timeFromInput = DateTime.ParseExact(ft, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None);
                        string timeOutput = timeFromInput.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                        ft = timeOutput;
                    }


                    sitems.stime = st;
                    sitems.ftime = ft;


                    taskmanlist.Add(sitems);

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();


            }
            ViewData["begin1"] = startDateOfWeek.ToString("MM/dd/yyyy");
            ViewData["end1"] = endDateOfWeek.ToString("MM/dd/yyyy");

            ViewData["next1"] = nextweek.ToString("yyyy-MM-dd");
            ViewData["prev1"] = prevweek.ToString("yyyy-MM-dd");
            return View(taskmanlist);

        }

        public ActionResult UpdateTaskAct(string id, string pid)

        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[tasklist]  SET  acthours=@pid WHERE id=@id";
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return View();
        }



        [HttpGet]
        public ContentResult UpdateTimesheetEdate(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[tasklist]  SET edate=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return Content("true");

        }


        public ContentResult CheckDate(string refdate, string based)
        {
            string data = "";
            DateTime refDay = Convert.ToDateTime(refdate);
            DateTime baseday = Convert.ToDateTime(based);
            DayOfWeek firstWeekDay = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            //   DateTime startDateOfWeek = DateTime.Now;

            DateTime startDateOfWeek = baseday;
            // DateTime startDateOfWeek = refDay;
            while (startDateOfWeek.DayOfWeek != firstWeekDay)
            { startDateOfWeek = startDateOfWeek.AddDays(-1); }  //startDateOfWeek.AddDays(-1d)

            DateTime endDateOfWeek = startDateOfWeek.AddDays(6d);

            if (refDay >= startDateOfWeek.AddDays(-1) && refDay <= endDateOfWeek)
            {
                data = "true";

            }
            else { data = "false"; }

            return Content(data);
        }



        // JSON vaues
        public ContentResult GetTimesheetScope(string id)
        {
            string id_id = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                DateTime myDateTime = DateTime.Now;
                string sqld = myDateTime.ToString("yyyy-MM-dd");

                string query = "Insert into  [dbo].[tasklist](taskid,edate,acthours,custid,projid)" +
                   " OUTPUT INSERTED.ID values(@a,@b,@d,@e,@f)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@a", 0);
                cmd.Parameters.AddWithValue("@b", sqld);
                //  cmd.Parameters.AddWithValue("@c", Convert.ToInt16(staff));
                cmd.Parameters.AddWithValue("@d", 0);
                cmd.Parameters.AddWithValue("@e", Convert.ToInt16(id));
                cmd.Parameters.AddWithValue("@f", 0);
                // cmd.ExecuteNonQuery();
                int insertedID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                id_id = Convert.ToString(insertedID);
            }


            return Content(id_id);
        }


        public ContentResult GetLastScope(string id)
        {

            string id_id = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                //     string query = "Insert into  [dbo].[taskman](customerid) OUTPUT INSERTED.ID values(@a)";
                // SqlCommand cmd = new SqlCommand(query, con);
                // cmd.Parameters.AddWithValue("@a", Convert.ToInt16(id));
                string query = "Insert into  [dbo].[taskman](customerid,projectid, description,stat,esthours,ddate,resource,pr,dev,note)" +
                   " OUTPUT INSERTED.ID values(@a,@b,@d,@e,@f,@h,@i,@j,@k,@l)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@a", Convert.ToInt16(id));
                cmd.Parameters.AddWithValue("@b", 0);
                //  cmd.Parameters.AddWithValue("@c", Convert.ToInt16(staff));
                cmd.Parameters.AddWithValue("@d", "Add comment");
                cmd.Parameters.AddWithValue("@e", "Select Stat");
                cmd.Parameters.AddWithValue("@f", 0);
                // cmd.Parameters.AddWithValue("@g", staff);
                cmd.Parameters.AddWithValue("@h", "Select Date");
                cmd.Parameters.AddWithValue("@i", 0);
                cmd.Parameters.AddWithValue("@j", 0);
                cmd.Parameters.AddWithValue("@k", "Dev");
                cmd.Parameters.AddWithValue("@l", "Add note");

                // cmd.ExecuteNonQuery();
                int insertedID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                id_id = Convert.ToString(insertedID);
            }


            return Content(id_id);

        }



        public ActionResult ReturnOnlyStaffTask(string based)
        {

            DateTime baseday = Convert.ToDateTime(based);
            DayOfWeek firstWeekDay = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            //DateTime startDateOfWeek = DateTime.Now;
            DateTime startDateOfWeek = baseday;
            while (startDateOfWeek.DayOfWeek != firstWeekDay)
            { startDateOfWeek = startDateOfWeek.AddDays(-1d); }

            DateTime endDateOfWeek = startDateOfWeek.AddDays(6d);

            DateTime nextweek = startDateOfWeek.AddDays(7d);
            DateTime prevweek = startDateOfWeek.AddDays(-2d);
            //return checkDate >= startDateOfWeek && checkDate <= endDateOfWeek;

            string sweek = startDateOfWeek.ToString("yyyy-MM-dd");
            string fweek = endDateOfWeek.ToString("yyyy-MM-dd");


            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "";

                query = "Select TL.id as id,TL.taskid as taskid,TL.custid as custid,TL.projid as projid, TL.edate as edate, TL.notes as notes, TL.acthours as acthours,TL.stime as stime,TL.ftime as ftime, TK.resource as res_id, TL.status as status From  [dbo].[tasklist] as TL, [dbo].[taskman] as TK WHERE edate BETWEEN '" +
                    sweek + "' AND '" + fweek + "' AND TL.taskid=TK.id AND TK.resource=" + Session["res_id"] + " ORDER BY TL.id DESC";


                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.taskid = Convert.ToInt16(dataReader["taskid"]);
                    sitems.cust = Convert.ToString(dataReader["custid"]);
                    sitems.proj = Convert.ToString(dataReader["projid"]);
                    //  sitems.description = Convert.ToString(dataReader["descr"]);
                    sitems.acthours = Convert.ToDecimal(dataReader["acthours"]);
                    string edate = Convert.ToString(dataReader["edate"]);
                    sitems.ddate = String.Format("{0:MM/dd/yyyy}", edate);
                    //   sitems.note = Convert.ToString(dataReader["note"]);

                    sitems.notes = Convert.ToString(dataReader["notes"]);
                    sitems.taskstatus = Convert.ToString(dataReader["status"]);

                    string q3 = "Select note From  [dbo].[taskman] WHERE id=" + Convert.ToString(dataReader["taskid"]);
                    //  string not = "";
                    SqlCommand cmd2 = new SqlCommand(q3, con);
                    SqlDataReader dataReader2 = cmd2.ExecuteReader();
                    while (dataReader2.Read())
                    {
                        sitems.note = Convert.ToString(dataReader2["note"]);
                    }


                    string st = "";
                    if (dataReader["stime"] == DBNull.Value)
                    {
                        st = "Set time";
                    }
                    else
                    {
                        st = Convert.ToString(dataReader["stime"]);
                        var timeFromInput = DateTime.ParseExact(st, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None);
                        string timeOutput = timeFromInput.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                        st = timeOutput;

                    }
                    string ft = "";

                    if (dataReader["ftime"] == DBNull.Value)
                    {
                        ft = " ";
                    }
                    else
                    {
                        ft = Convert.ToString(dataReader["ftime"]);
                        var timeFromInput = DateTime.ParseExact(ft, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None);
                        string timeOutput = timeFromInput.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                        ft = timeOutput;
                    }
                    sitems.stime = st;
                    sitems.ftime = ft;
                    taskmanlist.Add(sitems);
                }

                con.Close();
            }
            ViewData["begin1"] = startDateOfWeek.ToString("MM/dd/yyyy");
            ViewData["end1"] = endDateOfWeek.ToString("MM/dd/yyyy");

            ViewData["next1"] = nextweek.ToString("yyyy-MM-dd");
            ViewData["prev1"] = prevweek.ToString("yyyy-MM-dd");


            return PartialView("LoginPartial", taskmanlist);
        }


        public ActionResult UpdateTimeStatus(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[tasklist]  SET status=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }

        public ContentResult DeleteTaskList(string id)
        {
            string app = "true";
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string query = "DELETE FROM  [dbo].[tasklist]  WHERE id=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }
            return Content(app);
        }


        [HttpGet]
        public ContentResult GetProjects()
        {
            string app = "";

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                con.Open();
                string querydayd = "Select description from [dbo].[taskman] WHERE resource=" + Session["res_id"];

                SqlCommand cmddd = new SqlCommand(querydayd, con);
                SqlDataReader dataReaderdd = cmddd.ExecuteReader();
                while (dataReaderdd.Read())
                {

                    app = app + "^" + Convert.ToString(dataReaderdd["description"]);

                }
                con.Close();
            }
            return Content(app);
        }

        public ActionResult TaskPartial()
        {
            IList<TaskManModule> taskplanner = new List<TaskManModule>();

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                con.Open();
                //  string querydayd = "Select TM.id as id,TM.customerid as customerid,TM.projectid as projectid,TM.pr as pr, TM.description as description from [dbo].[taskman] as TM WHERE resource=" + Session["res_id"] + " AND stat='In Progress' OR stat='Closed, Timesheet Incomplete'  OR TM.pr<>0  ORDER BY pr ASC";
                string querydayd = "Select TM.status as status, TM.id as id,TM.customerid as customerid,TM.projectid as projectid,TM.pr as pr, TM.description as description from [dbo].[taskman] as TM WHERE resource=" + Session["res_id"] +
                                    " AND TM.pr < 22 AND TM.stat = 'In Progress'  order by TM.pr";

                string q2= "Select TM.status as status, TM.id as id,TM.customerid as customerid,TM.projectid as projectid,TM.pr as pr, TM.description as description from [dbo].[taskman] as TM WHERE resource=" + Session["res_id"] +
                         " AND TM.pr < 22 AND TM.stat = 'Closed, Timesheet Incomplete'  order by TM.pr";


                SqlCommand cmddd = new SqlCommand(querydayd, con);
                SqlDataReader dataReaderdd = cmddd.ExecuteReader();
                while (dataReaderdd.Read())
                {
                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReaderdd["id"]);
                    sitems.cust = Convert.ToString(dataReaderdd["customerid"]);
                    sitems.proj = Convert.ToString(dataReaderdd["projectid"]);
                    sitems.description = Convert.ToString(dataReaderdd["description"]);
                    //  sitems.category = Convert.ToString(dataReaderdd["cat"]);
                    //  sitems.taskid = Convert.ToInt16(dataReaderdd["taskid"]);
                    sitems.pr = Convert.ToString(dataReaderdd["pr"]);
                    sitems.status = Convert.ToString(dataReaderdd["status"]);
                    taskplanner.Add(sitems);
                }

                SqlCommand c2 = new SqlCommand(q2, con);
                SqlDataReader dataReaderdd2 = c2.ExecuteReader();
                while (dataReaderdd2.Read())
                {
                    TaskManModule sitems2 = new TaskManModule();
                    sitems2.id = Convert.ToInt16(dataReaderdd2["id"]);
                    sitems2.cust = Convert.ToString(dataReaderdd2["customerid"]);
                    sitems2.proj = Convert.ToString(dataReaderdd2["projectid"]);
                    sitems2.description = Convert.ToString(dataReaderdd2["description"]);
                    //  sitems.category = Convert.ToString(dataReaderdd["cat"]);
                    //  sitems.taskid = Convert.ToInt16(dataReaderdd["taskid"]);
                    sitems2.pr = Convert.ToString(dataReaderdd2["pr"]);
                    sitems2.status = Convert.ToString(dataReaderdd2["status"]);
                    taskplanner.Add(sitems2);
                }




                con.Close();
            }

            return PartialView("TaskPartial", taskplanner);
        }



        public ContentResult GetTimeSheetScope2(string custid, string projectid, string taskid)
        {

            string id_id = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                DateTime myDateTime = DateTime.Now;
                string sqld = myDateTime.ToString("yyyy-MM-dd");

                string query = "Insert into  [dbo].[tasklist](taskid,edate,acthours,custid,projid)" +
                   " OUTPUT INSERTED.ID values(@a,@b,@d,@e,@f)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@a", Convert.ToInt16(taskid));
                cmd.Parameters.AddWithValue("@b", sqld);
                //  cmd.Parameters.AddWithValue("@c", Convert.ToInt16(staff));
                cmd.Parameters.AddWithValue("@d", 0);
                cmd.Parameters.AddWithValue("@e", Convert.ToInt16(custid));
                cmd.Parameters.AddWithValue("@f", Convert.ToInt16(projectid));
                // cmd.ExecuteNonQuery();
                int insertedID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                id_id = Convert.ToString(insertedID);


            }


            return Content(id_id);

        }


        public ActionResult Logout()
        {
            Session.Abandon();
            return View("Index");

        }

        //------- sub task v2 code area

        public ActionResult AddSubTaskItem(string plannerId, string item)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "INSERT INTO [dbo].[subtask] (planner_id,info) VALUES (@pid,@item);";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", plannerId);
                cmd.Parameters.AddWithValue("@item", item);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }

        [HttpGet]
        public ActionResult DisplaySubTask(string planner_id)
        {
            List<SubTask> subtasklist = new List<SubTask>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "Select info from [dbo].[subtask] WHERE planner_id=" + planner_id + " Order by id ASC OFFSET 0 ROWS  FETCH FIRST 4 ROWS ONLY";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    SubTask sitems = new SubTask();
                    //  sitems.id = (int)dataReader["id"];
                    sitems.info = dataReader["info"].ToString();
                    subtasklist.Add(sitems);
                }

                con.Close();
            }
            return PartialView("DisplaySubTask", subtasklist);
        }


        public ActionResult DisplayAllSubTask(string planner_id)
        {
            List<SubTask> subtasklist = new List<SubTask>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "Select * from [dbo].[subtask] WHERE planner_id=" + planner_id + " Order by id ASC";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    SubTask sitems = new SubTask();
                    sitems.id = (int)dataReader["id"];
                    sitems.info = dataReader["info"].ToString();
                    subtasklist.Add(sitems);
                }

                con.Close();
            }

            return PartialView("DisplayAllSubTask", subtasklist);
        }


        public ContentResult GetStatus(string taskid)
        {
            string cont = "";

            List<TaskManModule> subtasklist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "Select status from [dbo].[taskman] WHERE id=" + taskid;
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    TaskManModule sitems = new TaskManModule();
                    // sitems.id = (int)dataReader["id"];
                   cont = dataReader["status"].ToString();
                 
                }

                con.Close();
            }

            return Content(cont);
        }

        public ActionResult DisplayReq(string planner_id)
        {
            List<TaskManModule> subtasklist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "Select status from [dbo].[taskman] WHERE id='" + planner_id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    TaskManModule sitems = new TaskManModule();
                    sitems.status = dataReader["status"].ToString();
                    subtasklist.Add(sitems);
                }

                con.Close();
            }

            return PartialView("DisplayReq", subtasklist);
        }
    }
}