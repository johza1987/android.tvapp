using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinetWifi.Models
{
    [Table("radcheck")]
    public class RadCheck
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string username { get; set; }
        public string attribute { get; set; }
        public string value { get; set; }
        public string op { get; set; }
    }

    [Table("radusergroup")]
    public class RadUsergroup
    {
        [Key]
        public string username { get; set; }
        public string groupname { get; set; }
        public int priority { get; set; }
    }

    [Table("radgroupcheck")]
    public class RadGroupCheck
    {
        [Key]
        public int id { get; set; }
        public string groupname { get; set; }
    }

    [Table("radacct")]
    public class RadAcct
    {
        [Key]
        public int radacctid { get; set; }
        public string username { get; set; }
        public DateTime? acctstarttime { get; set; }
        public DateTime? acctstoptime { get; set; }
    }
}