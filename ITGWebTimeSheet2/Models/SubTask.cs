using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITGWebTimeSheet2.Models
{
    public class SubTask
    {

        public int id { get; set; }

        public string planner_id { get; set; }

        public string info { get; set; }

        public string finish { get; set; }
        public string datefinish { get; set; }

    }
}