using Assessment.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assessment.Controllers
{
    public class SettingController : Controller
    {
        //
        // GET: /Setting/

        public ActionResult SystemConfig()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataSystemConfig.txt");
            string data = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<SystemConfigModel>>(data);
            ViewData["DataSystemConfig"] = result;
            return View();
        }

        public ActionResult SystemConfigCreate()
        {
            return View();
        }

        public ActionResult SystemConfigEdit(string id)
        {
            return View();
        }
        
    }
}
