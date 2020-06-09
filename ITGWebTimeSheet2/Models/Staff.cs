using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ITGWebTimeSheet2.Models
{
    public class Staff
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required(ErrorMessage = "email is needed")]
        public string email { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Alias is needed")]
        public string alias { get; set; }


        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please Enter fullname")]
        public string fullname { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }


        public string id { get; set; }

        public string[] File_file { get; set; }
    }
}