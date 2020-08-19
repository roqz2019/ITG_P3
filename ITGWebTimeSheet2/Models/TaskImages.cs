using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITGWebTimeSheet2.Models
{
    public class TaskImages
    {
       public string imagebase64 { get; set; }
       public string imageraw { get; set; }
       public int id { get; set; }
       public int planner_id { get; set; }
       public DateTime datecreated { get; set; }
    }
}