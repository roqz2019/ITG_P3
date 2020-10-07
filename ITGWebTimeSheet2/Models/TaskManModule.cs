using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITGWebTimeSheet2.Models
{
    public class TaskManModule

    { public string project_status { get; set; }
        public string ddate { get; set; }
        public  int id { get; set; }

        public string cust { get; set; }

        public string proj { get; set; }

        public string category { get; set; }
        public string staff { get; set; }

        public string description { get; set; }

        public string status { get; set; }

        public string pr { get; set; }
        public string resource { get; set; }

        public decimal esthours { get; set; }

        public decimal acthours { get; set; }

        public string start { get; set; }

        public string finish { get; set; }

        public string note { get; set; }

        public string dev { get; set; }

        public int taskid { get; set; }

        public string notes { get; set; }  //timesheet notes

        public string stime { get; set; }

        public string ftime { get; set; }

        public string taskstatus { get; set; }

        public string act_new { get; set; }

        public int tnum { get; set; }

        


    }
}
