using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assessment.Models
{
    public class TeacherModel
    {
        public string tc_id { get; set; }
        public string tc_title { get; set; }
        public string tc_firstname { get; set; }
        public string tc_lastname { get; set; }
        public string tc_grade { get; set; }
        public string tc_tel { get; set; }
        public string tc_certificate { get; set; }
        public string sc_id { get; set; }
    }
}