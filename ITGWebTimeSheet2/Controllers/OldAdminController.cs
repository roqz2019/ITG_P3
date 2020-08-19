using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using ITGWebTimeSheet2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.Text;




namespace ITGWebTimeSheet2.Controllers
{
    public class AdminController : CustomController
    {
        // GET: Admin
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
                string query = "Select * from  [dbo].[user] where uname=@Username and upass=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", users.Username);
                cmd.Parameters.AddWithValue("@Password", users.Password);
                SqlDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.Read())
                {
                    Session["Username"] = users.Username;
                    Session["Fullname"] = users.fullname;
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





        public ActionResult TimeSheet(string ed)
        {
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
                    query = "Select * From  [dbo].[tasklist] WHERE edate BETWEEN '" +
                    sweek + "' AND '" + fweek + "' ORDER BY id DESC";
                }
                if (ed.Contains("/") == true)
                {
                    string seldate = result.ToString("yyyy-MM-dd");
                    query = "Select * From  [dbo].[tasklist] WHERE edate BETWEEN '" +
                     sweek + "' AND '" + fweek + "' ORDER BY id DESC";
                }


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


        public ActionResult Login()
        {

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
                string query = "Select * From  [dbo].[tasklist] WHERE edate BETWEEN '" +
                    sweek + "' AND '" + fweek + "' ORDER BY id DESC";

                //  string q2=

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

                    string q3 = "Select note From  [dbo].[taskman] WHERE id=" + Convert.ToString(dataReader["taskid"]);
                    // string not = "";
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


        public ContentResult TaskContent(string id)
        {

            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = " SELECT T.id as id, C.code as cust,P.code as proj, T.description as descr,  T.dev as dev, T.note as note," +
                               " (Select fullname from [dbo].[staff] where id = T.resource) as resc," +
                               " T.pr as pr,T.start as start,T.finish as finish, T.stat as stat, T.esthours as est,T.ddate as ddate" +
                               " from [dbo].[taskman] as T, [dbo].[customers] as C, [dbo].[project] as P, [dbo].[staff] as S" +
                               " WHERE C.id=T.customerid AND P.id=T.projectid AND T.resource=S.id AND T.id=" + id + " ORDER BY T.id DESC ";


                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.cust = Convert.ToString(dataReader["cust"]);
                    sitems.proj = Convert.ToString(dataReader["proj"]);

                    sitems.description = Convert.ToString(dataReader["descr"]);
                    sitems.resource = Convert.ToString(dataReader["resc"]);
                    //  sitems.staff = Convert.ToString(dataReader["staff"]);
                    sitems.pr = Convert.ToString(dataReader["pr"]);
                    sitems.status = Convert.ToString(dataReader["stat"]);
                    sitems.esthours = Convert.ToDecimal(dataReader["est"]);
                    // sitems.acthours = Convert.ToDecimal(dataReader["act"]);
                    sitems.ddate = Convert.ToString(dataReader["ddate"]);
                    sitems.start = Convert.ToString(dataReader["start"]);
                    sitems.finish = Convert.ToString(dataReader["finish"]);
                    sitems.dev = Convert.ToString(dataReader["dev"]);
                    sitems.note = Convert.ToString(dataReader["note"]);
                    taskmanlist.Add(sitems);

                    tempAnnounce = "";

                    tempAnnounce = String.Concat(sitems.cust, "-", sitems.proj, '-', sitems.description, '-', sitems.note);



                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();

                return Content(tempAnnounce);
            }
        }


        public ActionResult Planner(string filter, string value, string page)
        {
            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();



                int pageRows = 15;
                int pageOffset = 0;
                int pageCount = 0;

                if (!String.IsNullOrEmpty(page))
                {
                    pageOffset = ((int.Parse(page) - 1) * pageRows);
                }



                string query1 = "SELECT COUNT(id) as pages FROM  [dbo].[taskman]";
                SqlCommand cmd1 = new SqlCommand(query1, con);
                SqlDataReader dataReader1 = cmd1.ExecuteReader();
                while (dataReader1.Read())
                {
                    pageCount = Convert.ToInt16(dataReader1["pages"]);
                }

                //pageCount = pageCount / pageRows;
                pageCount = (int)Math.Ceiling((double)pageCount / (double)pageRows);

                string query2 = "Select *, (select status from project where id = projectid) as project_status from  [dbo].[taskman] ORDER BY  ddate,id DESC OFFSET  " + pageOffset + " ROWS FETCH NEXT " + pageRows + " ROWS ONLY ";



                if (!String.IsNullOrEmpty(filter) && !String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(page))
                {
                    //pageOffset = pageRows * int.Parse(page);




                    string query11 = "Select count(id) as pages from  [dbo].[taskman] where " + filter + " = '" + value + "'";
                    SqlCommand cmd11 = new SqlCommand(query11, con);
                    SqlDataReader dataReader11 = cmd11.ExecuteReader();
                    while (dataReader11.Read())
                    {
                        pageCount = Convert.ToInt16(dataReader11["pages"]);
                    }

                    //pageCount = pageCount / pageRows;
                    pageCount = (int)Math.Ceiling((double)pageCount / (double)pageRows);
                    query2 = "Select *, (select status from project where id = projectid) as project_status from  [dbo].[taskman] where " + filter + " = '" + value + "' ORDER BY  " + filter + " DESC ";
                    if (pageCount > 0)
                    {
                        query2 = "Select *, (select status from project where id = projectid) as project_status from  [dbo].[taskman] where " + filter + " = '" + value + "' ORDER BY  " + filter + " DESC OFFSET " + pageOffset + " ROWS FETCH NEXT " + pageRows + " ROWS ONLY";
                    }
                }



                if (!String.IsNullOrEmpty(filter) && !String.IsNullOrEmpty(value) && filter == "stat" && value == "All")
                {
                    query2 = "Select *, (select status from project where id = projectid) as project_status from  [dbo].[taskman] where stat != 'Closed' ORDER BY  " + filter + " DESC OFFSET  " + pageOffset + " ROWS FETCH NEXT " + pageRows + " ROWS ONLY  ";

                    string query22 = "Select count(id) as pages from  [dbo].[taskman] where stat != 'Closed' ";
                    SqlCommand cmd22 = new SqlCommand(query22, con);
                    SqlDataReader dataReader22 = cmd22.ExecuteReader();
                    while (dataReader22.Read())
                    {
                        pageCount = Convert.ToInt16(dataReader22["pages"]);
                    }

                    //pageCount = pageCount / pageRows;
                    pageCount = (int)Math.Ceiling((double)pageCount / (double)pageRows);
                    query2 = "Select *, (select status from project where id = projectid) as project_status from  [dbo].[taskman] where stat != 'Closed' ORDER BY  " + filter + " DESC ";
                    if (pageCount > 0)
                    {
                        query2 = "Select *, (select status from project where id = projectid) as project_status from  [dbo].[taskman] where stat != 'Closed' ORDER BY  " + filter + " DESC OFFSET " + pageOffset + " ROWS FETCH NEXT " + pageRows + " ROWS ONLY  ";
                    }
                }


                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query2, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.cust = Convert.ToString(dataReader["customerid"]);

                    sitems.proj = Convert.ToString(dataReader["projectid"]);

                    sitems.description = Convert.ToString(dataReader["description"]);


                    sitems.resource = Convert.ToString(dataReader["resource"]);

                    sitems.pr = Convert.ToString(dataReader["pr"]);



                    sitems.status = Convert.ToString(dataReader["stat"]);






                    sitems.esthours = Convert.ToDecimal(dataReader["esthours"]);

                    sitems.ddate = Convert.ToString(dataReader["ddate"]);
                    sitems.start = Convert.ToString(dataReader["start"]);
                    sitems.finish = Convert.ToString(dataReader["finish"]);
                    sitems.dev = Convert.ToString(dataReader["dev"]);
                    sitems.note = Convert.ToString(dataReader["note"]);
                    sitems.project_status = Convert.ToString(dataReader["project_status"]);
                    taskmanlist.Add(sitems);

                }

                con.Close();
                ViewBag.PageCount = pageCount;
                return View(taskmanlist);

            }

        }
        public ActionResult TaskManager()
        {
            List<TaskManModule> taskmanlist = new List<TaskManModule>();


            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {

                con.Open();

                string query = " SELECT T.id as id, C.name as cust,P.name as proj, T.description as descr, T.dev as dev, T.note as note," +
                               " (Select fullname from [dbo].[staff] where id = T.resource) as resc," +
                               " T.pr as pr,T.start as start,T.finish as finish, T.stat as stat, T.esthours as est,T.ddate as ddate" +
                               " from [dbo].[taskman] as T, [dbo].[customers] as C, [dbo].[project] as P, [dbo].[staff] as S" +
                               " WHERE C.id=T.customerid AND P.id=T.projectid AND T.resource=S.id ORDER BY T.id DESC ";


                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.cust = Convert.ToString(dataReader["cust"]);
                    sitems.proj = Convert.ToString(dataReader["proj"]);

                    sitems.description = Convert.ToString(dataReader["descr"]);
                    sitems.resource = Convert.ToString(dataReader["resc"]);
                    //  sitems.staff = Convert.ToString(dataReader["staff"]);
                    sitems.pr = Convert.ToString(dataReader["pr"]);
                    sitems.status = Convert.ToString(dataReader["stat"]);
                    sitems.esthours = Convert.ToDecimal(dataReader["est"]);
                    // sitems.acthours = Convert.ToDecimal(dataReader["act"]);
                    sitems.ddate = Convert.ToString(dataReader["ddate"]);
                    sitems.start = Convert.ToString(dataReader["ddate"]);
                    sitems.finish = Convert.ToString(dataReader["finish"]);
                    sitems.dev = Convert.ToString(dataReader["dev"]);
                    sitems.note = Convert.ToString(dataReader["note"]);
                    taskmanlist.Add(sitems);

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();
                return View(taskmanlist);

            }
        }

        [HttpPost]
        public ContentResult AddTask(string cust, string proj, string description, string resc, string pr, string stat, string esthours, string dev, string startdate, string note)

        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            int taskId = 0;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                if (esthours.Trim() == "")
                { esthours = "0"; }

                string ddate = startdate; // DateTime.Now.Date.ToString("MM/dd/yyy");
                                          // staff Insert into  [dbo].[taskman](customerid,projectid,staffid,description,status,esthours,acthours,ddate,resource,pr)" +
                                          // "values(@a,@b,@c,@d,@e,@f,@g,@h,@i,@j)"


                string query = "Insert into  [dbo].[taskman](customerid,projectid, description,stat,esthours,ddate,resource,pr,dev,note)" +
                   "values(@a,@b,@d,@e,@f,@h,@i,@j,@k,@l); SELECT SCOPE_IDENTITY()";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@a", Convert.ToInt16(cust));
                cmd.Parameters.AddWithValue("@b", Convert.ToInt16(proj));
                //  cmd.Parameters.AddWithValue("@c", Convert.ToInt16(staff));
                cmd.Parameters.AddWithValue("@d", description);
                cmd.Parameters.AddWithValue("@e", stat);
                cmd.Parameters.AddWithValue("@f", esthours);
                // cmd.Parameters.AddWithValue("@g", staff);
                cmd.Parameters.AddWithValue("@h", ddate);
                cmd.Parameters.AddWithValue("@i", resc);
                cmd.Parameters.AddWithValue("@j", Convert.ToInt16(pr));
                cmd.Parameters.AddWithValue("@k", dev);
                cmd.Parameters.AddWithValue("@l", note);

                taskId = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

            }


            //JObject jsonResult = JObject.Parse("{Result : { TaskID : " + taskId.ToString() + " }}");

            //return Content("{Result : { TaskID : " + taskId.ToString() + " }}", "application/json");
            return Content(taskId.ToString());


        }


        public ActionResult Reporting()
        {
            return View();
        }



        public ActionResult Staff()
        {

            List<Staff> stafflist = new List<Staff>();


            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {

                List<string> termsList = new List<string>();

                termsList.Clear();

                con.Open();

                string query = "Select * from  [dbo].[staff] ORDER By fullname ASC";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read())
                {

                    Staff sitems = new Staff();
                    sitems.id = Convert.ToString(dataReader["id"]);
                    sitems.fullname = Convert.ToString(dataReader["fullname"]);
                    sitems.lastname = Convert.ToString(dataReader["lastname"]);
                    sitems.alias = Convert.ToString(dataReader["alias"]);
                    sitems.email = Convert.ToString(dataReader["email"]);
                    sitems.username = Convert.ToString(dataReader["username"]);
                    sitems.password = Convert.ToString(dataReader["pass"]);
                    sitems.accesstype = Convert.ToString(dataReader["accesstype"]);
                    sitems.type = Convert.ToString(dataReader["type"]);




                    stafflist.Add(sitems);

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();
                return View(stafflist);

            }


        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult AddStaff(string fullname, string alias, string email)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "Insert into  [dbo].[staff](fullname,alias,email,pass)" +
                   "values(@fullname,@alias,@email,@pass)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@fullname", fullname);
                cmd.Parameters.AddWithValue("@alias", alias);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pass", "12345");
                cmd.ExecuteNonQuery();
                con.Close();

            }

            //  return View();
            return RedirectToAction("Staff");
        }

        public ActionResult Project()
        {
            List<ProjectModule> projectlist = new List<ProjectModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                List<string> termsList = new List<string>();
                termsList.Clear();
                con.Open();

                string query = "Select * from  [dbo].[project]";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read())
                {

                    ProjectModule sitems = new ProjectModule();
                    sitems.id = Convert.ToString(dataReader["id"]);
                    sitems.name = Convert.ToString(dataReader["name"]);
                    sitems.code = Convert.ToString(dataReader["code"]);
                    sitems.custid = Convert.ToString(dataReader["custid"]);
                    projectlist.Add(sitems);

                }

                con.Close();
                return View(projectlist);

            }
        }



        public ActionResult AddProject(string name, string code, string custid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "Insert into  [dbo].[project](name,code,custid)" +
                   "values(@a,@b,@c)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@a", name);
                cmd.Parameters.AddWithValue("@b", code);
                cmd.Parameters.AddWithValue("@c", custid);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            return RedirectToAction("Project");
        }


        public ActionResult Customers()
        {
            List<CustomersModule> customerlist = new List<CustomersModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {

                List<string> termsList = new List<string>();

                termsList.Clear();

                con.Open();
                string query = "";

                query = "Select * from  [dbo].[customers]";


                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read())
                {

                    CustomersModule sitems = new CustomersModule();
                    sitems.id = Convert.ToString(dataReader["id"]);
                    sitems.name = Convert.ToString(dataReader["name"]);
                    sitems.code = Convert.ToString(dataReader["code"]);
                    customerlist.Add(sitems);

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();
                return View(customerlist);

            }
        }

        public ActionResult AddCustomers(string name, string code)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "Insert into  [dbo].[customers](name,code)" +
                 "values(@name,@c)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@c", code);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            return RedirectToAction("Customers");
        }


        public ActionResult DeleteStaff(string id)
        {

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {

                string query = "DELETE FROM  [dbo].[staff]  WHERE id='" + id + "'";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return View();
        }

        public ActionResult DeleteProject(string id)
        {

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {

                string query = "DELETE FROM  [dbo].[project]  WHERE id='" + id + "'";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return View();

        }

        public ActionResult DeleteCustomers(string id)
        {

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string query = "DELETE FROM  [dbo].[customers]  WHERE id='" + id + "'";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return View();
        }

        public ActionResult DeleteTaskList(string id)
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string query = "DELETE FROM  [dbo].[tasklist]  WHERE id=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return View();
        }
        public ActionResult DeleteTask(string id)
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString))
            {
                string query = "DELETE FROM  [dbo].[taskman]  WHERE id=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return View();
        }

        //----------------  Edit / Update sections -----------------------------------------------


        public ActionResult EditStaff(string id)
        {
            List<Staff> stafflist = new List<Staff>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "Select * from  [dbo].[staff] WHERE id='" + id + "'";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Staff sitems = new Staff();
                    sitems.id = Convert.ToString(dataReader["id"]);
                    sitems.fullname = Convert.ToString(dataReader["fullname"]);
                    sitems.alias = Convert.ToString(dataReader["alias"]);
                    sitems.email = Convert.ToString(dataReader["email"]);
                    stafflist.Add(sitems);
                }
                con.Close();
            }
            return View(stafflist);
        }



        public ActionResult UpdateStaff(string id, string fullname, string alias, string email)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[staff]  SET fullname=@fullname,alias=@alias," +
                   "email=@email WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@fullname", fullname);
                cmd.Parameters.AddWithValue("@alias", alias);
                cmd.Parameters.AddWithValue("@email", email);
                // cmd.Parameters.AddWithValue("@pass", "12345");
                cmd.ExecuteNonQuery();
                con.Close();

            }
            return RedirectToAction("Staff");
        }



        // update customers

        public ActionResult UpdateCustomers(string id, string name)
        {

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "UPDATE  [dbo].[customers]  SET name=@name WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return RedirectToAction("Customers");
        }

        public ActionResult UpdateCustCode(string id, string name)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "UPDATE  [dbo].[customers]  SET code=@name WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return RedirectToAction("Customers");

        }


        public ActionResult EditCustomers(int id)
        {
            List<CustomersModule> customerlist = new List<CustomersModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {

                List<string> termsList = new List<string>();

                termsList.Clear();

                con.Open();

                string query = "Select * from  [dbo].[customers] WHERE id='" + id + "'";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read())
                {
                    CustomersModule sitems = new CustomersModule();
                    sitems.id = Convert.ToString(dataReader["id"]);
                    sitems.name = Convert.ToString(dataReader["name"]);
                    customerlist.Add(sitems);

                }
                con.Close();
            }
            return View(customerlist);
        } //


        // Edit Project name ------

        public ActionResult EditProject(int id)
        {
            List<ProjectModule> projectlist = new List<ProjectModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                List<string> termsList = new List<string>();
                termsList.Clear();
                con.Open();

                string query = "Select * from  [dbo].[project] WHERE id='" + id + "'";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read())
                {

                    ProjectModule sitems = new ProjectModule();
                    sitems.id = Convert.ToString(dataReader["id"]);
                    sitems.name = Convert.ToString(dataReader["name"]);
                    projectlist.Add(sitems);

                }

                con.Close();
            }
            return View(projectlist);
        }

        // update project

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
                                "Where T.projectid > 0  AND C.id = T.customerid AND P.id= T.projectid AND T.description NOT LIKE 'Add comment' AND T.customerid=" + custid + " AND P.id=" + projid;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    string taskid = Convert.ToString(dataReader["taskid"]);
                    string taskname = Convert.ToString(dataReader["taskdesc"]);
                    //  string tasknote = Convert.ToString(dataReader["taskdnote"]);

                    tempannounce = tempannounce + taskid + ":" + taskname;
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
                                " Where T.projectid > 0  AND C.id = T.customerid AND P.id= T.projectid AND T.description NOT LIKE 'No Data' AND T.customerid=" + id;






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
                                " Where T.projectid > 0  AND C.id = T.customerid AND T.description NOT LIKE 'Add comment'";

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


        [HttpPost]
        public JsonResult FetchTaskImages(string id)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "SELECT * FROM  [dbo].[planner_images] WHERE planner_id = '" + id + "'";

                List<TaskImages> listTaskImages = new List<TaskImages>();

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    TaskImages taskimages = new TaskImages();
                    taskimages.id = Convert.ToInt16(dataReader["id"]);
                    taskimages.planner_id = Convert.ToInt16(dataReader["planner_id"]);
                    taskimages.imagebase64 = dataReader["imagebase64"].ToString();
                    taskimages.imageraw = dataReader["imageraw"].ToString();
                    taskimages.datecreated = Convert.ToDateTime(dataReader["datecreated"]);
                    listTaskImages.Add(taskimages);
                }

                con.Close();

                //Encoding.UTF8,    JsonRequestBehavior.AllowGet
                // new { field: "value" }

                return Json(listTaskImages, JsonRequestBehavior.AllowGet);

                //return Content("{Result : { Message : 'Success' }}", "application/json");
            }
            //return JsonResult("{Result : { Message : 'Failed' }}", "application/json");
        }


        [HttpPost]
        public ContentResult AddTaskImage(string id, string image)
        {
            string mega = this.Request["image"];
            int imageId = 1;

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "INSERT INTO  [dbo].[planner_images] (planner_id, imagebase64, imageraw)  VALUES ('" + id + "', '" + mega + "', '" + image + "');  SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, con);
                imageId = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }

            //JObject jsonResult = JObject.Parse("{Result : { ImageId : " + imageId + " }}");

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            return Content(imageId.ToString());
        }

        [HttpPost]
        public ContentResult RemoveTaskImage(string id)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "DELETE FROM  [dbo].[planner_images] where id = '" + id + "' ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return Content("success");
        }


        [HttpGet]
        public ContentResult FetchTaskProjectStatus(string id)
        {

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string taskProjectStatus = "";
                string query = "SELECT * FROM  [dbo].[project] WHERE id = '" + id + "'";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    taskProjectStatus = dataReader["status"].ToString();
                }

                con.Close();
                return Content(taskProjectStatus);
            }
        }




        [HttpPost]
        public ContentResult UpdateTaskProjectStatus(string id, string name)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "UPDATE  [dbo].[project]  SET status= '" + name + "' WHERE id= (select projectid from taskman where id = '" + id + "')";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return Content("{Result : { Message : 'Success' }}", "application/json");
        }



        [HttpPost]
        public ContentResult UpdateTaskCust(string id, string custid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "UPDATE  [dbo].[taskman]  SET customerid= '" + custid + "' WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.ExecuteNonQuery();
                con.Close();

            }
            return Content("{Result : { Message : 'Success' }}", "application/json");

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

        [HttpPost]
        public ContentResult UpdateTaskProj(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET projectid='" + pid + "' WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.ExecuteNonQuery();
                con.Close();

            }

            return Content("{Result : { Message : 'Success' }}", "application/json");
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

        [HttpPost]
        public ContentResult UpdateTaskDesc(string id, string descr)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET description='" + descr + "' WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.ExecuteNonQuery();
                con.Close();

            }
            return Content("{Result : { Message : 'Success' }}", "application/json");

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

        public ActionResult UpdateTimesheetEdate(string id, string pid)
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
            return View();

        }

        [HttpPost]
        public ContentResult UpdateTaskDate(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET ddate='" + pid + "' WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            return Content("{Result : { Message : 'Success' }}", "application/json");

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

        [HttpPost]
        public ContentResult UpdateTimeNote(string id, string name)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET note='" + name + "' WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();

            }
            return Content("{Result : { Message : 'Success' }}", "application/json");

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

        public ActionResult UpdateTaskNote(string id, string name)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "UPDATE  [dbo].[taskman]  SET note=@pid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", name);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();
        }




        [HttpPost]
        public ActionResult UpdateTaskDev(string id, string name)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET dev='" + name + "' WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.ExecuteNonQuery();
                con.Close();

            }
            return Content("{Result : { Message : 'Success' }}", "application/json");

        }

        [HttpPost]
        public ContentResult UpdateTaskStat(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[taskman]  SET  stat='" + pid + "' WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.ExecuteNonQuery();
                con.Close();

            }

            return Content("{Result : { Message : 'Success' }}", "application/json");
        }

        public ActionResult UpdateTaskAct(string id, string pid)

        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[tasklist]  SET  acthours='" + pid + "' WHERE id= '" + pid + "' ";

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return View();
        }

        [HttpPost]
        public ContentResult UpdateTaskEst(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[taskman]  SET  esthours='" + pid + "' WHERE id='" + id + "' ";



                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return Content("{Result : { Message : 'Success' }}", "application/json");
        }

        [HttpPost]
        public ContentResult UpdateTaskResc(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "UPDATE  [dbo].[taskman]  SET resource='" + pid + "' WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.ExecuteNonQuery();
                con.Close();

            }
            return Content("{Result : { Message : 'Success' }}", "application/json");

        }


        [HttpPost]
        public ContentResult UpdatePr(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[taskman]  SET pr='" + pid + "' WHERE id=" + id;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }
            return Content("{Result : { Message : 'Success' }}", "application/json");
        }


        public ActionResult UpdateStart(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[taskman]  SET start=@pid WHERE id=@id";
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return View();
        }

        public ActionResult UpdateFinish(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[taskman]  SET finish=@pid WHERE id=@id";
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return View();
        }






        //------------------ Filter setion ----------------------------------------------------
        [HttpPost]
        public ActionResult TaskFilterCust(string custid)
        {
            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {

                con.Open();
                string query = "SELECT T.id as id, C.name as cust,P.name as proj, T.description as descr, T.stat as stat, T.esthours as est,T.ddate as ddate from " +
                    " [dbo].[taskman] as T, [dbo].[customers] as C, [dbo].[project] as P" +
                    " WHERE C.id=T.customerid AND P.id=T.projectid AND C.id = '" + custid + "' ORDER BY T.id DESC";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();


                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.cust = Convert.ToString(dataReader["cust"]);
                    sitems.proj = Convert.ToString(dataReader["proj"]);
                    // sitems.staff = Convert.ToString(dataReader["staff"]);
                    sitems.description = Convert.ToString(dataReader["descr"]);
                    sitems.status = Convert.ToString(dataReader["stat"]);
                    sitems.esthours = Convert.ToDecimal(dataReader["est"]);
                    // sitems.acthours = Convert.ToDecimal(dataReader["act"]);
                    sitems.ddate = Convert.ToString(dataReader["ddate"]);
                    taskmanlist.Add(sitems);

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();
                return View(taskmanlist);
            }
        }

        public ActionResult TaskFilterProj(string projid)
        {
            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {

                con.Open();
                string query = "SELECT T.id as id, C.name as cust,P.name as proj, T.description as descr, T.stat as stat, T.esthours as est,T.ddate as ddate from " +
                    " [dbo].[taskman] as T, [dbo].[customers] as C, [dbo].[project] as P" +
                    " WHERE C.id=T.customerid AND P.id=T.projectid AND P.id = '" + projid + "' ORDER BY T.id DESC";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.cust = Convert.ToString(dataReader["cust"]);
                    sitems.proj = Convert.ToString(dataReader["proj"]);
                    // sitems.staff = Convert.ToString(dataReader["staff"]);
                    sitems.description = Convert.ToString(dataReader["descr"]);
                    sitems.status = Convert.ToString(dataReader["stat"]);
                    sitems.esthours = Convert.ToDecimal(dataReader["est"]);
                    // sitems.acthours = Convert.ToDecimal(dataReader["act"]);
                    sitems.ddate = Convert.ToString(dataReader["ddate"]);
                    taskmanlist.Add(sitems);

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();
                return View(taskmanlist);
            }
        }

        public ActionResult TaskFilterStat(string stat)
        {
            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {

                con.Open();
                string query = "SELECT T.id as id, C.name as cust,P.name as proj, T.description as descr, T.stat as stat, T.esthours as est,T.ddate as ddate from " +
                    " [dbo].[taskman] as T, [dbo].[customers] as C, [dbo].[project] as P" +
                    " WHERE C.id=T.customerid AND P.id=T.projectid AND T.status = '" + stat + "' ORDER BY T.id DESC";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.cust = Convert.ToString(dataReader["cust"]);
                    sitems.proj = Convert.ToString(dataReader["proj"]);
                    // sitems.staff = Convert.ToString(dataReader["staff"]);
                    sitems.description = Convert.ToString(dataReader["descr"]);
                    sitems.status = Convert.ToString(dataReader["stat"]);
                    sitems.esthours = Convert.ToDecimal(dataReader["est"]);
                    // sitems.acthours = Convert.ToDecimal(dataReader["act"]);
                    sitems.ddate = Convert.ToString(dataReader["ddate"]);
                    taskmanlist.Add(sitems);

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();
                return View(taskmanlist);
            }

        }


        //------------------ End of  Filter setion ----------------------------------------------------



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




        [HttpGet]
        public JsonResult GetPlannerDataBy(string id, string custid)
        {
            List<TaskManModule> taskmanlist = new List<TaskManModule>();
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query2 = "Select * from  [dbo].[taskman] ORDER BY  id DESC ";

                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query2, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader["id"]);
                    sitems.cust = Convert.ToString(dataReader["customerid"]);
                    sitems.proj = Convert.ToString(dataReader["projectid"]);
                    sitems.description = Convert.ToString(dataReader["description"]);
                    sitems.resource = Convert.ToString(dataReader["resource"]);
                    sitems.pr = Convert.ToString(dataReader["pr"]);
                    sitems.status = Convert.ToString(dataReader["stat"]);

                    sitems.esthours = Convert.ToDecimal(dataReader["esthours"]);
                    sitems.ddate = Convert.ToString(dataReader["ddate"]);
                    sitems.start = Convert.ToString(dataReader["start"]);
                    sitems.finish = Convert.ToString(dataReader["finish"]);
                    sitems.dev = Convert.ToString(dataReader["dev"]);
                    sitems.note = Convert.ToString(dataReader["note"]);
                    taskmanlist.Add(sitems);

                }

                con.Close();

                return Json(taskmanlist, JsonRequestBehavior.AllowGet);

            }
        }

        public ContentResult GetEdates()
        {
            string alledates = "";


            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "Select edate From  [dbo].[tasklist]";
                string tempAnnounce = string.Empty;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    string edate = Convert.ToString(dataReader["edate"]);
                    string ddate = String.Format("{0:MM/dd/yyyy}", edate);

                    alledates = alledates + ddate;

                }
                //   sitems.File_file = termsList.ToArray();

                con.Close();
            }

            return Content(alledates);
        }


        //-------------------------  Staff Section editing and adding -------------------------------------

        public ActionResult stEditUsername(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[staff]  SET username='" + pid + "'  WHERE id=@id";
                //  cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return View();

        }


        public ActionResult stEditPassword(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[staff]  SET pass='" + pid + "'  WHERE id=@id";
                //  cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return View();

        }

        public ActionResult stEditFullname(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[staff]  SET fullname=@pid WHERE id=@id";
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return View();


        }

        public ActionResult stEditLastname(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[staff]  SET lastname=@pid WHERE id=@id";
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return View();
        }

        public ActionResult stEditCode(string id, string pid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE  [dbo].[staff]  SET alias=@pid WHERE id=@id";
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return View();
        }

        public ContentResult GetStaffScope(string id)
        {
            string id_id = "";
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();

                string query = "Insert into  [dbo].[staff](fullname)" +
                   " OUTPUT INSERTED.ID values(@a)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@a", id);

                // cmd.ExecuteNonQuery();
                int insertedID = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                id_id = Convert.ToString(insertedID);
            }


            return Content(id_id);
        }



        public ContentResult GetSType()
        {
            string tempannounce = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                string query = "Select * FROM [dbo].[type]";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    string custid = Convert.ToString(dataReader["id"]);
                    string i = Convert.ToString(dataReader["type"]);
                    tempannounce = tempannounce + custid + ":" + i;
                    tempannounce = tempannounce + "^";
                }
                con.Close();
            }

            return Content(tempannounce);
        }

        public ContentResult GetSPriv()
        {
            string tempannounce = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                string query = "Select * FROM [dbo].[priv]";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    string custid = Convert.ToString(dataReader["id"]);
                    string i = Convert.ToString(dataReader["type"]);
                    tempannounce = tempannounce + custid + ":" + i;
                    tempannounce = tempannounce + "^";
                }
                con.Close();
            }

            return Content(tempannounce);
        }


        public ActionResult UpdateStaffType(string id, string custid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[staff]  SET type=@custid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@custid", custid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();

        }

        public ActionResult UpdateAdminPriv(string id, string custid)
        {
            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                string query = "UPDATE  [dbo].[staff]  SET accesstype=@custid WHERE id='" + id + "'";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@custid", custid);
                cmd.ExecuteNonQuery();
                con.Close();

            }

            return View();


        }

        public ContentResult GetSubtask(string id)
        {
            string tempannounce = "";

            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))

            {
                con.Open();
                string query = "Select note FROM [dbo].[taskman] where id=" + id;

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {

                    string custid = Convert.ToString(dataReader["note"]);

                    tempannounce = custid;
                }
                con.Close();
            }

            return Content(tempannounce);

        }








    }
}