using Assessment.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assessment.Controllers
{
    public class SchoolController : Controller
    {
        //
        // GET: /School/

        public ActionResult Index()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataSchool.txt");
            string data = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<SchoolModel>>(data);
            ViewData["GetDataSchool"] = result;
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(string id)
        {
            return View();
        }

        public ActionResult Type()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataSchoolType.txt");
            string data = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<SchoolTypeModel>>(data);
            ViewData["DataSchoolType"] = result;
            return View();
        }

        public ActionResult TypeCreate()
        {
            return View();
        }

        public ActionResult TypeEdit(string id)
        {
            return View();
        }

        public ActionResult Group()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataSchoolGroup.txt");
            string data = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<SchoolGroupModel>>(data);
            ViewData["DataSchoolGroup"] = result;
            return View();
        }

        public ActionResult GroupCreate()
        {
            return View();
        }

        public ActionResult GroupEdit(string id)
        {
            return View();
        }
    }
}
