using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assessment.Models
{
    public class SchoolModel
    {
        public string sc_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string sc_name { get; set; }
        public string sc_address { get; set; }
        public string sc_tel { get; set; }
        public int sctype_id { get; set; }
        public int scgroup_id { get; set; }
    }

    public class SchoolTypeModel
    {
        public int sctype_id { get; set; }
        public String sctype_name { get; set; }
    }

    public class SchoolGroupModel
    {
        public int scgroup_id { get; set; }
        public String scgroup_name { get; set; }
    }
}