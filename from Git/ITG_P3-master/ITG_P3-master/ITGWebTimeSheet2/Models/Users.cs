using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITGWebTimeSheet2.Models
{
    public class Users
    {

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please Enter Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }


        [DataType(DataType.Text)]
        public string fullname { get; set; }

        public string id { get; set; }

        public string[] File_file { get; set; }
    }
}