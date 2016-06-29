using Assessment.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assessment.Controllers
{
    public class StudentController : Controller
    {
        //
        // GET: /Student/

        public ActionResult Index()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataStudent.txt");
            string data = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<StudentModel>>(data);
            ViewData["GetDataStudent"] = result;
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
    }
}
