using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assessment.Models
{
    public class AssessmentModel
    {
        public String ass_id { get; set; }
        public String ass_year { get; set; }
        public String ass_date { get; set; }
        public String ass_age { get; set; }
        public String ass_score { get; set; }
        public String ass_result { get; set; }
        public String ass_certdisable { get; set; }
        public String ass_type_id { get; set; }
        public String tc_id { get; set; }
        public String std_id { get; set; }
    }

    public class AssessTypeModel
    {
        public int asstype_id { get; set; }
        public string asstype_name { get; set; }
        public int ass_no { get; set; }
        public string question { get; set; }
    }

    public class AssessDetailModel
    {
        public String ass_id { get; set; }
        public int ass_no { get; set; }
        public String answer { get; set; }
    }
}