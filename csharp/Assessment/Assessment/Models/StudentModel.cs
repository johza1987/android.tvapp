using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assessment.Models
{
    public class StudentModel
    {
        public string std_id { get; set; }
        public string std_title { get; set; }
        public string std_firstname { get; set; }
        public string std_lastname { get; set; }
        public string std_birthday { get; set; }
        public string std_idcard { get; set; }
        public string std_grade { get; set; }
        public string std_disabled { get; set; }
        public string sc_id { get; set; }
    }

    public class StudentDetailModel
    {
        public String std_id { get; set; }
        public int dsbtype_id { get; set; }
        public String year { get; set; }
    }
}