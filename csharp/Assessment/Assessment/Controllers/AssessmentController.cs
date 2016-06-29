using Assessment.Class;
using Assessment.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assessment.Controllers
{
    public class AssessmentController : Controller
    {
        //
        // GET: /Assessment/

        public ActionResult Index()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataAssessment.txt");
            string data = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<AssessmentModel>>(data);
            ViewData["GetDataAssessment"] = result;
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult PrintAssemssment()
        {
            try
            {
                var cm = new Common();
                var A = cm.GetAssessment();
                var Ad = cm.GetAssessDetail();
                var At = cm.GetAssessType();
                var S = cm.GetStudent();

                var query = from _A in A
                            from _Ad in Ad
                            from _At in At
                            where _A.ass_id == _Ad.ass_id && _A.ass_type_id == _At.asstype_id.ToString() && _Ad.ass_no == _At.ass_no
                            select new { _At.question, _Ad.answer };
                DataTable dt = new DataTable();
                DataRow dr;
                dt.Columns.Add("question");
                dt.Columns.Add("answer");
                foreach (var aa in query)
                {
                    dr = dt.NewRow();
                    dr["question"] = aa.question;
                    dr["answer"] = aa.answer;
                    dt.Rows.Add(dr);
                }
                if (dt.Rows.Count > 0)
                {
                    using (CrystalDecisions.CrystalReports.Engine.ReportClass rptH = new CrystalDecisions.CrystalReports.Engine.ReportClass())
                    {
                        string path = HttpContext.Server.MapPath("~/App_Data/reportPdf.rpt");
                        rptH.FileName = path;
                        rptH.Load();
                        rptH.SetDataSource(dt);
                        //   rptH.SetParameterValue("isAdmin", (user.Role.RoleName == RoleName.Admintstrator ? "T" : "F"));
                        System.IO.Stream stream = rptH.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        rptH.Close();
                        return File(stream, "application/pdf");
                    }
                }
                return new EmptyResult();

            }
            catch (Exception ex)
            {

                return new EmptyResult();
            }
        }

 
    }

}


