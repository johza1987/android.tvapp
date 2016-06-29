using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Assessment.Models;
using Newtonsoft.Json;
namespace Assessment.Controllers
{
    public class TeacherController : Controller
    {
        //
        // GET: /Teacher/

        public ActionResult Index()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataTeacher.txt");
            string data = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<TeacherModel>>(data);
            ViewData["GetDataTeacher"] = result;
            return View();
        }

        //
        // GET: /Teacher/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Teacher/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Teacher/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Teacher/Edit/5

        public ActionResult Edit(string id)
        {
            ViewData["dddd"] = "test";
            return View();
        }

        //
        // POST: /Teacher/Edit/5

        [HttpPost]
        public ActionResult Edit(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Teacher/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Teacher/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
