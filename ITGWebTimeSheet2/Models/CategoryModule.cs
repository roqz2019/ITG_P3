using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITGWebTimeSheet2.Models
{
    public class CategoryModule
    {
        public int id { get; set; }
        public int projectid { get; set; }
        public string status { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime datecreated { get; set; }
    }
}