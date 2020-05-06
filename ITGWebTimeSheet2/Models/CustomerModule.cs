using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITGWebTimeSheet2.Models
{
    public class CustomersModule
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please Enter Customers name")]
        public string name { get; set; }

        public string id { get; set; }

        public string code { get; set; }


    }
}
