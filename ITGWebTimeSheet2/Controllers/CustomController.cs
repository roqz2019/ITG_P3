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
    public class CustomController : Controller
    {
        public CustomController()
        {

            //  List<CustomersModule> customerlist = new List<CustomersModule>();

            IList<CustomersModule> customerlist = new List<CustomersModule>();
            IList<Staff> stafflist = new List<Staff>();
            IList<ProjectModule> projectlist = new List<ProjectModule>();
            IList<TaskManModule> taskmode = new List<TaskManModule>();

            IList<TaskManModule> tasklist = new List<TaskManModule>();


            string conString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conString))
            {

                con.Open();

                string query = "Select * from [timesheet].[dbo].[customers]";
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

                query = "Select * from [timesheet].[dbo].[staff]";
                SqlCommand cmd2 = new SqlCommand(query, con);
                SqlDataReader dataReader2 = cmd2.ExecuteReader();


                while (dataReader2.Read())
                {

                    Staff sitems = new Staff();
                    sitems.id = Convert.ToString(dataReader2["id"]);
                    sitems.fullname = Convert.ToString(dataReader2["fullname"]) + " " + Convert.ToString(dataReader2["lastname"]);
                    sitems.alias = Convert.ToString(dataReader2["alias"]);
                    stafflist.Add(sitems);
                }


                query = "Select * from [timesheet].[dbo].[project]";
                SqlCommand cmd3 = new SqlCommand(query, con);
                SqlDataReader dataReader3 = cmd3.ExecuteReader();


                while (dataReader3.Read())
                {

                    ProjectModule sitems = new ProjectModule();
                    sitems.id = Convert.ToString(dataReader3["id"]);
                    sitems.name = Convert.ToString(dataReader3["name"]);
                    sitems.code = Convert.ToString(dataReader3["code"]);
                    sitems.custid = Convert.ToString(dataReader3["custid"]);
                    projectlist.Add(sitems);
                }



                query = "Select id, description from [timesheet].[dbo].[taskman]";
                SqlCommand cmd5 = new SqlCommand(query, con);
                SqlDataReader dataReader5 = cmd5.ExecuteReader();

                while (dataReader5.Read())
                {
                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16(dataReader5["id"]);
                    sitems.description = Convert.ToString(dataReader5["description"]);
                    tasklist.Add(sitems);
                }


                //--------------------------------------------------------------------------------
                query = " SELECT T.id as id, C.code as cust,P.code as proj, T.description as descr, T.dev as dev, T.note as note, (Select fullname from[timesheet].[dbo].[staff] where id = T.resource) as resc, T.pr as pr,T.start as start,T.finish as finish, T.stat as stat, T.esthours as est,T.ddate as ddate from[timesheet].[dbo].[taskman] as T,[timesheet].[dbo].[customers] as C,[timesheet].[dbo].[project] as P,[timesheet].[dbo].[staff] as S WHERE C.id=T.customerid AND P.id=T.projectid AND T.resource=S.id ORDER BY T.id DESC";


                SqlCommand cmd4 = new SqlCommand(query, con);
                SqlDataReader dataReader4 = cmd4.ExecuteReader();

                if (dataReader4.HasRows)
                {
                    while (dataReader4.Read())
                    {
                        TaskManModule sitems = new TaskManModule();
                        sitems.id = Convert.ToInt16(dataReader4["id"]);
                        sitems.cust = Convert.ToString(dataReader4["cust"]);
                        sitems.proj = Convert.ToString(dataReader4["proj"]);
                        sitems.description = Convert.ToString(dataReader4["descr"]);
                        taskmode.Add(sitems);

                    }

                }
                else
                {
                    TaskManModule sitems = new TaskManModule();
                    sitems.id = Convert.ToInt16("0");
                    sitems.cust = Convert.ToString("no");
                    sitems.proj = Convert.ToString("no");
                    sitems.description = Convert.ToString("no");
                    taskmode.Add(sitems);

                }

              

                con.Close();
            }

                // Now store it in your model
                ViewData["CustomerList"] = customerlist;
                ViewData["StaffList"] = stafflist;
                ViewData["ProjectList"] = projectlist;
                ViewData["Taskman"] = taskmode;

            ViewData["TaskList"] = tasklist;

            ViewData["SystemDate"] = DateTime.Now.ToString("MM/dd/yyyy");


            // date range -------------- here 

            DayOfWeek firstWeekDay = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            DateTime startDateOfWeek = DateTime.Now;
            while (startDateOfWeek.DayOfWeek != firstWeekDay)
            { startDateOfWeek = startDateOfWeek.AddDays(-1d); }
          
            DateTime endDateOfWeek = startDateOfWeek.AddDays(6d);

            ViewData["begin"]= startDateOfWeek.ToString("MM/dd/yyyy");
            ViewData["end"] = endDateOfWeek.ToString("MM/dd/yyyy");




            // end of date range



        }
    }
}
