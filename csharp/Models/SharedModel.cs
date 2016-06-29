using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebExtension.Models
{
    public class JsonDataTable
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object data { get; set; }
    }

    public class JsonResponse
    {
        public bool status { get; set; }
        public object data { get; set; }
        public int total { get; set; }
        public string message { get; set; }
        public List<object> list { get; set; }
    }

    public class DropdownEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }

    public class FilterModel
    {
        public string Query { get; set; }
        public string Disable { get; set; }
        public List<string> ProvinceCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public FilterModel()
        {
            ProvinceCode = new List<string>();
        }
    }

    public class DatableOption : FilterModel
    {
        public int start { get; set; }
        public int length { get; set; }
        public int draw { get; set; }
        public List<Dictionary<string, string>> order { get; set; }
    }

    public class FilterCustomer : FilterModel
    {
        public List<string> Status { get; set; }

        public FilterCustomer()
        {
            ProvinceCode = new List<string>();
            Status = new List<string>();
        }
    }
}
