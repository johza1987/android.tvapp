using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinetWifi.Models
{
    [Table("batch_history")]
    public class BatchUser
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string batch_name { get; set; }
        public string batch_status { get; set; }
        public int hotspot_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string id_card { get; set; }
        public string passport_id { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string mobilephone { get; set; }
        public string email { get; set; }
        public int packageId { get; set; }
        public int quantity { get; set; }
        public DateTime creationdate { get; set; }
        public string creationby { get; set; }
    }

    [Table("userinfo")]
    public class Userinfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string username { get; set; }
        [Required]
        public string firstname { get; set; }
        [Required]
        public string lastname { get; set; }
        public string id_card { get; set; }
        public string passport_id { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string mobilephone { get; set; }
        public string email { get; set; }
        public DateTime creationdate { get; set; }
        public string creationby { get; set; }

        public DateTime? firstaccttime { get; set; }
        public DateTime? expiredate { get; set; }
    }

    [Table("userbillinfo")]
    public class UserBillinfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string username { get; set; }
        public string planName { get; set; }
        public int hotspot_id { get; set; }
        public int batch_id { get; set; }
        public DateTime creationdate { get; set; }
        public string creationby { get; set; }
    }

    public class UserinfoList
    {
        public Nullable<int> id { get; set; }
        public int bill_id { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string id_card { get; set; }
        public string passport_id { get; set; }
        public string mobilephone { get; set; }
        public string email { get; set; }
        public string packagename { get; set; }
        public string groupname { get; set; }
        public string packageprice { get; set; }
        public Nullable<int> hotspot_id { get; set; }
        public string hotspot_name { get; set; }
        public DateTime creationdate { get; set; }
        public string creationby { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public DateTime? expiredate { get; set; }
    }

    public class CouponList
    {
        public string username { get; set; }
        public string password { get; set; }
        public string package { get; set; }
        public string groupname { get; set; }
        public DateTime creationdate { get; set; }
        public DateTime? firstaccttime { get; set; }
        public DateTime? expiredate { get; set; }
    }
}