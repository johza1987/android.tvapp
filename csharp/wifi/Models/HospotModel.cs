using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinetWifi.Models
{
    [Table("hotspots")] 
    public class Hotspot
    {
        [Key]
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public virtual ICollection<HotspotUser> HotspotUser { get; set; }
    }

    [Table("hotspots_users")]
    public class HotspotUser
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("Hotspot")]
        public int hotspot_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string fullname { get; set; }
        public bool status { get; set; }
        public virtual Hotspot Hotspot { get; set; }
    }

    [Table("hotspots_plans")]
    public class HotspotPlan
    {
        [Key]
        public int id { get; set; }
        public int hotspot_id { get; set; }
        public int plan_id { get; set; }
    }

    [Table("billing_plans")]
    public class Plan
    {
        [Key]
        public int id { get; set; }
        public string planName { get; set; }
        public string planCost { get; set; }
    }

    [Table("billing_plans_profiles")]
    public class Profile
    {
        [Key]
        public int id { get; set; }
        public string plan_name { get; set; }
        public string profile_name { get; set; }
    }

    public class PlanList
    {
        public int id { get; set; }
        public string planName { get; set; }
    }
}