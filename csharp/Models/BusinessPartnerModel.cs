using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebExtension.Models
{
    public class BusinessPartnerModel
    {
        public string CustomerType { get; set; }
        public string TitleName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CardNo { get; set; }
        public string Cellular { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string Amphur { get; set; }
        public string Zipcode { get; set; }
        public string ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string FrozenFor { get; set; }
    }
}