using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternetAccountModel.EDM;

namespace WebExtension.Models
{
    public class UserLogon : Users
    {
        public List<string> AccessAreaCode { get; set; } 
        public List<string> PermissionCode { get; set; }
        public UserLogon()
        {
            AccessAreaCode = new List<string>();
            PermissionCode = new List<string>();
        }
    }
}
