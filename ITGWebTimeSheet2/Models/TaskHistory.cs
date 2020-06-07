using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITGWebTimeSheet2.Models
{
    public class TaskHistory
    {
        public int id { get; set; }
        public int planner_id { get; set; }

        public string history { get; set; }
        public DateTime datecreated { get; set; }
    }
}