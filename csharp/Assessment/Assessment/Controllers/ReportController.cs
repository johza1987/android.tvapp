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
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            //var cm = new Common();
            //var sch = cm.GetSchool();
            //var schG = cm.GetSchoolGroup();
            //var S = cm.GetStudent();
            //var sD = cm.GetStudentDetail();
            //var schT = cm.GetSchoolType();
            //////var dT = GetDisabledType();
            //var amt = cm.GetAssessment();
            //var query = from _sch in sch
            //            from _schG in schG
            //            from _schT in schT
            //            from _S in S
            //            from _sD in sD
            //            from _amt in amt
            //            where _schG.scgroup_id == _sch.scgroup_id && _schT.sctype_id == _sch.sctype_id && _S.sc_id == _S.sc_id && _sD.std_id == _S.std_id && _amt.std_id == _S.std_id && _sch.sc_name == "บ้านโคกกระเพอ"
            //            select new  { _sch.sc_name, _schG.scgroup_name, _S.std_title, _S.std_firstname, _S.std_lastname, _S.std_idcard, _S.std_birthday, _S.std_grade, _sD.dsbtype_id, _schT.sctype_name, _amt.ass_certdisable };
            //var lst = new List<BySchoolModel>();


            //foreach (var obj in query)
            //{
            //    var model = new BySchoolModel();
            //    model.sc_name = obj.sc_name;
            //}
            //var result = JsonConvert.DeserializeObject<List<SchoolModel>>(query);
            //ViewData["GetDataSchool"] = result;
            return View();
        }

        public ActionResult TestExportToExcel(String type)
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataStudent.txt");
            string data = System.IO.File.ReadAllText(path);
            String namefile = "";
            List<StudentModel> List = JsonConvert.DeserializeObject<List<StudentModel>>(data);

            if (List.Count > 0)
            {
                try
                {
                    System.IO.MemoryStream streamExcel = new System.IO.MemoryStream();
                    var cm = new Common();
                    using (var excel = new ExcelPackage(streamExcel))
                    {
                        System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                        int[] sum = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                        var Worksheets = excel.Workbook.Worksheets.Add("Sheet1");
                        int rows = 1;
                        int column = 1;
                        if (type == "รายงานรายโรงเรียน")
                        {
                            #region "รายงานรายโรงเรียน"
                            namefile = "รายงานรายโรงเรียน";
                            GenHeadTableTabBySchool(ref Worksheets, ref rows);
                            var sch = cm.GetSchool();
                            var schG = cm.GetSchoolGroup();
                            var S = cm.GetStudent();
                            var sD = cm.GetStudentDetail();
                            var schT = cm.GetSchoolType();
                            ////var dT = GetDisabledType();
                            var amt = cm.GetAssessment();
                            var query = from _sch in sch
                                        from _schG in schG
                                        from _schT in schT
                                        from _S in S
                                        from _sD in sD
                                        from _amt in amt
                                        where _schG.scgroup_id == _sch.scgroup_id && _schT.sctype_id == _sch.sctype_id && _S.sc_id == _S.sc_id && _sD.std_id == _S.std_id && _amt.std_id == _S.std_id && _sch.sc_name == "บ้านโคกกระเพอ"
                                        select new { _sch.sc_name, _schG.scgroup_name, _S.std_title, _S.std_firstname, _S.std_lastname, _S.std_idcard, _S.std_birthday, _S.std_grade, _sD.dsbtype_id, _schT.sctype_name, _amt.ass_certdisable };
                            rows += 2;
                            int[] sumSub = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                            foreach (var aa in query.OrderBy(x => x.scgroup_name))
                            {
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = (rows - 4).ToString();
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = aa.sc_name;
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = aa.scgroup_name;
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = aa.std_title + aa.std_firstname + " " + aa.std_lastname;
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = aa.std_idcard;
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = (2558 - Convert.ToInt16(aa.std_birthday.Split('-')[0]));
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = aa.std_grade;
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column + aa.dsbtype_id - 1].Value = "/";
                                Worksheets.Cells[rows, column + aa.dsbtype_id - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                BorderCells(ref Worksheets, rows, column);

                                int i = Convert.ToInt16(aa.dsbtype_id);
                                sumSub[i - 1]++;
                                column += 9;
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = aa.sctype_name;
                                BorderCells(ref Worksheets, rows, column);
                                Worksheets.Cells[rows, column++].Value = aa.ass_certdisable;
                                rows++; //dsbtype_id
                                column = 1;
                            }
                            rows++;
                            column = 8;

                            WriteExcele(ref Worksheets, "B", rows, "G", rows, "รวม");
                            int flagRow = rows - 2;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                            column++;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                            column++;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                            column++;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                            column++;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                            column++;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                            column++;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                            column++;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                            column++;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column].Value = sumSub[column - 8];

                            rows += 3;
                            WriteExcele(ref Worksheets, "A", rows, "R", rows, "ประเภทความพิการ 1=ด้านการมองเห็น 2=ด้านการได้ยิน 3=ด้านสติปัญญา 4=ด้านร่างกาย/สุขภาพ 5=ด้านการเรียนรู้");
                            rows++;
                            WriteExcele(ref Worksheets, "A", rows, "R", rows, "6=ด้านการพูด/ภาษา 7=ด้านพฤติกรรมอารมร์ 8=ออทิสติค 9=พิการซ้อน");

                            for (int i = 5; i <= flagRow; i++)
                            {
                                for (int j = 8; j <= 16; j++)
                                {
                                    BorderCells(ref Worksheets, i, j);
                                }
                            }
                            for (int j = 1; j <= 18; j++)
                            {
                                BorderCells(ref Worksheets, rows - 5, j);
                            }
                            BorderCells(ref Worksheets, rows - 4, 1);
                            BorderCells(ref Worksheets, rows - 4, 17);
                            BorderCells(ref Worksheets, rows - 4, 18);
                            #endregion
                        }
                        else if (type == "")
                        {
                            #region "Tab 1"
                            namefile = "ร.ร.ทั้งหมด";
                            GenHeadTableTabl(ref Worksheets, ref rows);

                            var sch = cm.GetSchool();
                            var schG = cm.GetSchoolGroup();
                            var S = cm.GetStudent();
                            var dT = cm.GetDisabledType();
                            var sD = cm.GetStudentDetail();
                            var schT = cm.GetSchoolType();
                            var amt = cm.GetAssessment();
                            var query = from _sch in sch
                                        from _schG in schG
                                        from _S in S
                                        from _schT in schT
                                        from _amt in amt
                                        where _S.sc_id == _sch.sc_id && _schG.scgroup_id == _sch.scgroup_id && _schT.sctype_id == _sch.sctype_id && _amt.std_id == _S.std_id //&& _S.std_grade == "ป.1"
                                        select new { _S.std_id, _sch.sc_id, _sch.sc_name, _schG.scgroup_name, _S.std_title, _S.std_firstname, _S.std_lastname, _S.std_idcard, _S.std_birthday, _S.std_grade, _schT.sctype_name, _amt.ass_certdisable };
                            rows += 2;

                            String group = "";
                            foreach (var aa in query.OrderBy(x => x.scgroup_name))
                            {
                                if (group == "" || group != aa.scgroup_name)
                                {
                                    group = aa.scgroup_name;
                                    WriteExcele(ref Worksheets, "B", rows, "C", rows, group);
                                    rows++;
                                }


                                Worksheets.Cells[rows, column++].Value = aa.sc_id;
                                Worksheets.Cells[rows, column++].Value = aa.sc_name;
                                Worksheets.Cells[rows, column++].Value = aa.scgroup_name;
                                Worksheets.Cells[rows, column++].Value = aa.std_title + aa.std_firstname + " " + aa.std_lastname;
                                Worksheets.Cells[rows, column++].Value = aa.std_idcard;
                                Worksheets.Cells[rows, column++].Value = (2558 - Convert.ToInt16(aa.std_birthday.Split('-')[0]));
                                Worksheets.Cells[rows, column++].Value = aa.std_grade;
                                var _query = from _S in S
                                             from _sD in sD
                                             where _S.std_id == _sD.std_id && _S.std_id == aa.std_id
                                             select new { _sD.dsbtype_id };
                                foreach (var _aa in _query)
                                {
                                    int i = Convert.ToInt16(_aa.dsbtype_id);
                                    Worksheets.Cells[rows, column + i - 1].Value = "1";
                                    Worksheets.Cells[rows, column + i - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    sum[i - 1]++;
                                }
                                Worksheets.Cells[rows, column += 9].Value = aa.sctype_name;
                                Worksheets.Cells[rows, ++column].Value = aa.ass_certdisable;
                                rows++;
                                column = 1;
                            }
                            for (int i = 8; i < 8 + sum.Length; i++)
                            {
                                Worksheets.Cells[rows, i].Value = sum[i - 8];
                                Worksheets.Cells[rows, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            rows += 3;
                            WriteExcele(ref Worksheets, "A", rows, "R", rows, "ประเภทความพิการ 1=ด้านการมองเห็น 2=ด้านการได้ยิน 3=ด้านสติปัญญา 4=ด้านร่างกาย/สุขภาพ 5=ด้านการเรียนรู้");
                            rows++;
                            WriteExcele(ref Worksheets, "A", rows, "R", rows, "6=ด้านการพูด/ภาษา 7=ด้านพฤติกรรมอารมร์ 8=ออทิสติค 9=พิการซ้อน");


                            for (int i = 5; i <= rows - 4; i++)
                            {
                                for (int j = 1; j <= 18; j++)
                                {
                                    BorderCells(ref Worksheets, i, j);
                                }
                            }


                            #endregion
                        }
                        else if (type == "")
                        {
                            #region "Tab 2"
                            namefile = "ร.ร.ที่คัดกรอง";
                            GenHeadTableTab2(ref Worksheets, ref rows);
                            var sch = cm.GetSchool();
                            var schG = cm.GetSchoolGroup();
                            var S = cm.GetStudent();
                            var sD = cm.GetStudentDetail();
                            var schT = cm.GetSchoolType();

                            var query = from _sch in sch
                                        from _schG in schG
                                        from _schT in schT
                                        where _schG.scgroup_id == _sch.scgroup_id && _schT.sctype_id == _sch.sctype_id
                                        select new { _sch.sc_id, _sch.sc_name, _schG.scgroup_name, _schT.sctype_name };
                            rows += 2;

                            String group = "";
                            foreach (var aa in query.OrderBy(x => x.scgroup_name))
                            {
                                if (group == "" || group != aa.scgroup_name)
                                {
                                    group = aa.scgroup_name;
                                    WriteExcele(ref Worksheets, "B", rows, "C", rows, group);
                                    rows++;
                                }
                                int[] sumSub = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                Worksheets.Cells[rows, column++].Value = aa.sc_id;
                                Worksheets.Cells[rows, column++].Value = aa.sc_name;
                                Worksheets.Cells[rows, column++].Value = aa.scgroup_name;

                                var _query = from _S in S
                                             from _sD in sD
                                             where _S.std_id == _sD.std_id && _S.sc_id == aa.sc_id
                                             select new { _sD.dsbtype_id };
                                foreach (var _aa in _query)
                                {
                                    int i = Convert.ToInt16(_aa.dsbtype_id);
                                    sumSub[i - 1]++;
                                    sum[i - 1]++;
                                    Worksheets.Cells[rows, column + i - 1].Value = sumSub[i - 1].ToString() == "0" ? "" : sumSub[i - 1].ToString();
                                    Worksheets.Cells[rows, column + i - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }

                                Worksheets.Cells[rows, column += 9].Value = sumSub.Sum().ToString();
                                Worksheets.Cells[rows, ++column].Value = aa.sctype_name;
                                rows++;
                                column = 1;
                            }
                            int nextCells = 4;
                            int ii;
                            for (ii = nextCells; ii < nextCells + sum.Length; ii++)
                            {
                                Worksheets.Cells[rows, ii].Value = sum[ii - nextCells];
                                Worksheets.Cells[rows, ii].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            Worksheets.Cells[rows, ii].Value = sum.Sum().ToString();

                            rows += 3;
                            WriteExcele(ref Worksheets, "A", rows, "N", rows, "ประเภทความพิการ 1=ด้านการมองเห็น 2=ด้านการได้ยิน 3=ด้านสติปัญญา 4=ด้านร่างกาย/สุขภาพ 5=ด้านการเรียนรู้");
                            rows++;
                            WriteExcele(ref Worksheets, "A", rows, "N", rows, "6=ด้านการพูด/ภาษา 7=ด้านพฤติกรรมอารมร์ 8=ออทิสติค 9=พิการซ้อน");
                            for (int i = 5; i <= rows - 4; i++)
                            {
                                for (int j = 1; j <= 14; j++)
                                {
                                    BorderCells(ref Worksheets, i, j);
                                }
                            }


                            #endregion
                        }
                        else if (type == "")
                        {
                            #region "Tab 3"
                            namefile = "ข้อมูล ระดับชั้น";
                            GenHeadTableTab3(ref Worksheets, ref rows);

                            var S = cm.GetStudent();
                            var sD = cm.GetStudentDetail();

                            var query = from _S in S
                                        from _sD in sD
                                        where _sD.std_id == _S.std_id
                                        select new { _S.std_grade, _sD.dsbtype_id };
                            rows += 1;
                            String std_grade = "";
                            int[] sumSub = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                            int[] reset = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                            foreach (var aa in query.OrderByDescending(x => x.std_grade).OrderBy(y => y.dsbtype_id))
                            {
                                if (std_grade == "" || std_grade == aa.std_grade)
                                {
                                    int i = Convert.ToInt16(aa.dsbtype_id);
                                    sumSub[i - 1]++;
                                    sum[i - 1]++;
                                    std_grade = aa.std_grade;
                                }
                                else if (std_grade != aa.std_grade)
                                {
                                    column = 1;
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = (rows - 4).ToString();
                                    Worksheets.Cells[rows, column++].Value = std_grade;
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                    Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    Worksheets.Cells[rows, column++].Value = sumSub.Sum().ToString();

                                    for (int ii = 0; ii < sumSub.Length; ii++)
                                        sumSub[ii] = 0;

                                    int i = Convert.ToInt16(aa.dsbtype_id);
                                    sumSub[i - 1]++;
                                    sum[i - 1]++;
                                    std_grade = aa.std_grade;
                                    rows++;
                                }
                            }
                            column = 1;
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = (rows - 4).ToString();
                            Worksheets.Cells[rows, column++].Value = std_grade;
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sumSub.Sum().ToString();
                            rows++;

                            column = 2;
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = "รวม";
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum[column - 4];
                            Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            Worksheets.Cells[rows, column++].Value = sum.Sum().ToString();

                            for (int i = 5; i <= rows; i++)
                            {
                                for (int j = 1; j <= 12; j++)
                                {
                                    BorderCells(ref Worksheets, i, j);
                                }
                            }

                            #endregion
                        }
                      
                        Worksheets.Cells.AutoFitColumns();
                        excel.Save();
                    }

                    string attachment = "attachment; filename=" + namefile + ".xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.Charset = "";
                    streamExcel.WriteTo(Response.OutputStream);

                    Response.End();
                }
                catch 
                {

                }
            }

            return new EmptyResult();
        }

        private void GenHeadTableTabl(ref ExcelWorksheet ec, ref int rows)
        {
            WriteExcele(ref ec, "A", rows, "R", rows, "ข้อมูลคัดกรองพื้นฐานนักเรียนพิการเรียนร่วม ปีการศึกษา 2557");
            rows++;
            WriteExcele(ref ec, "A", rows, "R", rows, "สำนักงานเขตพื้นที่การศึกษาประถมศึกษา สุรินทร์ เขต 1 (แสดงรายชื่อโรงเรียนทุกโรงและรายชื่อโรงเรียนที่มี น.ร.พิการ)");
            rows++;
            WriteExcele(ref ec, "A", rows, "A", rows + 1, "รหัสโรงเรียน");
            WriteExcele(ref ec, "B", rows, "B", rows + 1, "โรงเรียน");
            WriteExcele(ref ec, "C", rows, "C", rows + 1, "อำเภอ");
            WriteExcele(ref ec, "D", rows, "D", rows + 1, "ชื่อ-สกุล");
            WriteExcele(ref ec, "E", rows, "E", rows + 1, "เลขประจำตัว  13  หลัก");
            WriteExcele(ref ec, "F", rows, "F", rows + 1, "อายุ");
            WriteExcele(ref ec, "G", rows, "G", rows, "ระดับชั้นปี");
            WriteExcele(ref ec, "G", rows + 1, "G", rows + 1, "(ป.1- ม.3)");
            WriteExcele(ref ec, "H", rows, "P", rows, "ประเภทความพิการ");
            WriteExcele(ref ec, "H", rows + 1, "H", rows + 1, "1");
            WriteExcele(ref ec, "I", rows + 1, "I", rows + 1, "2");
            WriteExcele(ref ec, "J", rows + 1, "J", rows + 1, "3");
            WriteExcele(ref ec, "K", rows + 1, "K", rows + 1, "4");
            WriteExcele(ref ec, "L", rows + 1, "L", rows + 1, "5");
            WriteExcele(ref ec, "M", rows + 1, "M", rows + 1, "6");
            WriteExcele(ref ec, "N", rows + 1, "N", rows + 1, "7");
            WriteExcele(ref ec, "O", rows + 1, "O", rows + 1, "8");
            WriteExcele(ref ec, "P", rows + 1, "P", rows + 1, "9");
            WriteExcele(ref ec, "Q", rows, "Q", rows, "ประเภทโรงเรียน");
            WriteExcele(ref ec, "Q", rows + 1, "Q", rows + 1, "ประถมฯ/ขยายโอกาส");
            WriteExcele(ref ec, "R", rows, "R", rows + 1, " ใบรับรอง");
        }
        private void GenHeadTableTab2(ref ExcelWorksheet ec, ref int rows)
        {
            WriteExcele(ref ec, "A", rows, "N", rows, "ข้อมูลคัดกรองพื้นฐานนักเรียนพิการเรียนร่วม ปีการศึกษา 2557");
            rows++;
            WriteExcele(ref ec, "A", rows, "N", rows, "สำนักงานเขตพื้นที่การศึกษาประถมศึกษา สุรินทร์ เขต 1 (แสดงรายชื่อโรงเรียนทุกโรงและรายชื่อโรงเรียนที่มี น.ร.พิการ)");
            rows++;
            WriteExcele(ref ec, "A", rows, "A", rows + 1, "รหัสโรงเรียน");
            WriteExcele(ref ec, "B", rows, "B", rows + 1, "โรงเรียน");
            WriteExcele(ref ec, "C", rows, "C", rows + 1, "อำเภอ");
            WriteExcele(ref ec, "D", rows, "L", rows, "ประเภทความพิการ");
            WriteExcele(ref ec, "D", rows + 1, "D", rows + 1, "1");
            WriteExcele(ref ec, "E", rows + 1, "E", rows + 1, "2");
            WriteExcele(ref ec, "F", rows + 1, "F", rows + 1, "3");
            WriteExcele(ref ec, "G", rows + 1, "G", rows + 1, "4");
            WriteExcele(ref ec, "H", rows + 1, "H", rows + 1, "5");
            WriteExcele(ref ec, "I", rows + 1, "I", rows + 1, "6");
            WriteExcele(ref ec, "J", rows + 1, "J", rows + 1, "7");
            WriteExcele(ref ec, "K", rows + 1, "K", rows + 1, "8");
            WriteExcele(ref ec, "L", rows + 1, "L", rows + 1, "9");
            WriteExcele(ref ec, "M", rows, "M", rows + 1, "รวมทั้งสิ้น");
            WriteExcele(ref ec, "N", rows, "N", rows, "ประเภทโรงเรียน");
            WriteExcele(ref ec, "N", rows + 1, "N", rows + 1, "ประถมฯ/ขยายโอกาส");
        }
        private void GenHeadTableTab3(ref ExcelWorksheet ec, ref int rows)
        {
            WriteExcele(ref ec, "A", rows, "A", rows + 3, "ที่");
            WriteExcele(ref ec, "B", rows, "L", rows, "ข้อมูลคัดกรองพื้นฐานนักเรียนพิการเรียนร่วม ปีการศึกษา 2557");
            rows++;
            WriteExcele(ref ec, "B", rows, "L", rows, "สำนักงานเขตพื้นที่การศึกษาประถมศึกษา สุรินทร์ เขต 1 (จำแนกจำนวน น.ร.พิการเป็นรายชั้น)");
            rows++;
            WriteExcele(ref ec, "B", rows, "B", rows + 1, "ระดับชั้นเรียน");
            WriteExcele(ref ec, "C", rows, "K", rows, "ประเภทความพิการ");
            WriteExcele(ref ec, "L", rows, "L", rows + 1, "รวมทั้งสิ้น");
            rows++;
            WriteExcele(ref ec, "C", rows, "C", rows, "1");
            WriteExcele(ref ec, "D", rows, "D", rows, "2");
            WriteExcele(ref ec, "E", rows, "E", rows, "3");
            WriteExcele(ref ec, "F", rows, "F", rows, "4");
            WriteExcele(ref ec, "G", rows, "G", rows, "5");
            WriteExcele(ref ec, "H", rows, "H", rows, "6");
            WriteExcele(ref ec, "I", rows, "I", rows, "7");
            WriteExcele(ref ec, "J", rows, "J", rows, "8");
            WriteExcele(ref ec, "K", rows, "K", rows, "9");
        }
        private void GenHeadTableTabBySchool(ref ExcelWorksheet ec, ref int rows)
        {
           
            WriteExcele(ref ec, "A", rows, "R", rows, "ข้อมูลพื้นฐานนักเรียนที่มีความต้องการพิเศษเรียนร่วม ปีการศึกษา 2557");
            rows++;
            WriteExcele(ref ec, "A", rows, "R", rows, "สำนักงานเขตพื้นที่การศึกษาสุรินทร์ เขต 1");
            rows++;
            WriteExcele(ref ec, "A", rows, "A", rows + 1, "ที่");
            WriteExcele(ref ec, "B", rows, "B", rows + 1, "โรงเรียน");
            WriteExcele(ref ec, "C", rows, "C", rows + 1, "อำเภอ");
            WriteExcele(ref ec, "D", rows, "D", rows + 1, "ชื่อ-สกุล");
            WriteExcele(ref ec, "E", rows, "E", rows + 1, "เลขประจำตัว  13  หลัก");
            WriteExcele(ref ec, "F", rows, "F", rows + 1, "อายุ");
            WriteExcele(ref ec, "G", rows, "G", rows, "ระดับชั้นปี");
            WriteExcele(ref ec, "G", rows + 1, "G", rows + 1, "(ป.1- ม.3)");
            WriteExcele(ref ec, "H", rows, "P", rows, "ประเภทความพิการ");
            WriteExcele(ref ec, "H", rows + 1, "H", rows + 1, "1");
            WriteExcele(ref ec, "I", rows + 1, "I", rows + 1, "2");
            WriteExcele(ref ec, "J", rows + 1, "J", rows + 1, "3");
            WriteExcele(ref ec, "K", rows + 1, "K", rows + 1, "4");
            WriteExcele(ref ec, "L", rows + 1, "L", rows + 1, "5");
            WriteExcele(ref ec, "M", rows + 1, "M", rows + 1, "6");
            WriteExcele(ref ec, "N", rows + 1, "N", rows + 1, "7");
            WriteExcele(ref ec, "O", rows + 1, "O", rows + 1, "8");
            WriteExcele(ref ec, "P", rows + 1, "P", rows + 1, "9");
            WriteExcele(ref ec, "Q", rows, "Q", rows + 1, "ประเภทโรงเรียนประถมศึกษา ขยายโอกาส สามัญเดิม");
            WriteExcele(ref ec, "R", rows, "R", rows + 1, "เอกสารรับรอง");
        }
        private void WriteExcele(ref ExcelWorksheet ec, String colX, int x, String colY, int y, String val)
        {
            String cells = colX + x.ToString() + ":" + colY + y.ToString();
            ec.Cells[cells].Merge = true;
            ec.Cells[cells].Style.Font.Bold = true;
            ec.Cells[cells].Value = val;
            ec.Cells[cells].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ec.Cells[cells].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ec.Cells[cells].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ec.Cells[cells].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ec.Cells[cells].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        private void BorderCells(ref ExcelWorksheet ec, int row, int column)
        {
            ec.Cells[row, column].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ec.Cells[row, column].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ec.Cells[row, column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ec.Cells[row, column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        public ActionResult School()
        {
            return View();
        }

        public ActionResult ExportSchoolToExcel()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataStudent.txt");
            string data = System.IO.File.ReadAllText(path);
            String namefile = "รายงานรายโรงเรียน";
            List<StudentModel> List = JsonConvert.DeserializeObject<List<StudentModel>>(data);

            if (List.Count > 0)
            {
                try
                {
                    System.IO.MemoryStream streamExcel = new System.IO.MemoryStream();
                    var cm = new Common();
                    using (var excel = new ExcelPackage(streamExcel))
                    {
                        System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                        int[] sum = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                        var Worksheets = excel.Workbook.Worksheets.Add("Sheet1");
                        int rows = 1;
                        int column = 1;

                        #region "รายงานรายโรงเรียน"
                        GenHeadTableTabBySchool(ref Worksheets, ref rows);
                        var sch = cm.GetSchool();
                        var schG = cm.GetSchoolGroup();
                        var S = cm.GetStudent();
                        var sD = cm.GetStudentDetail();
                        var schT = cm.GetSchoolType();

                        var amt = cm.GetAssessment();
                        var query = from _sch in sch
                                    from _schG in schG
                                    from _schT in schT
                                    from _S in S
                                    from _sD in sD
                                    from _amt in amt
                                    where _schG.scgroup_id == _sch.scgroup_id && _schT.sctype_id == _sch.sctype_id && _S.sc_id == _S.sc_id && _sD.std_id == _S.std_id && _amt.std_id == _S.std_id && _sch.sc_name == "บ้านโคกกระเพอ"
                                    select new { _sch.sc_name, _schG.scgroup_name, _S.std_title, _S.std_firstname, _S.std_lastname, _S.std_idcard, _S.std_birthday, _S.std_grade, _sD.dsbtype_id, _schT.sctype_name, _amt.ass_certdisable };
                        rows += 2;
                        int[] sumSub = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        foreach (var aa in query.OrderBy(x => x.scgroup_name))
                        {
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = (rows - 4).ToString();
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = aa.sc_name;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = aa.scgroup_name;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = aa.std_title + aa.std_firstname + " " + aa.std_lastname;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = aa.std_idcard;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = (2558 - Convert.ToInt16(aa.std_birthday.Split('-')[0]));
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = aa.std_grade;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column + aa.dsbtype_id - 1].Value = "/";
                            Worksheets.Cells[rows, column + aa.dsbtype_id - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            BorderCells(ref Worksheets, rows, column);

                            int i = Convert.ToInt16(aa.dsbtype_id);
                            sumSub[i - 1]++;
                            column += 9;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = aa.sctype_name;
                            BorderCells(ref Worksheets, rows, column);
                            Worksheets.Cells[rows, column++].Value = aa.ass_certdisable;
                            rows++; //dsbtype_id
                            column = 1;
                        }
                        rows++;
                        column = 8;

                        WriteExcele(ref Worksheets, "B", rows, "G", rows, "รวม");
                        int flagRow = rows - 2;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                        column++;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                        column++;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                        column++;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                        column++;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                        column++;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                        column++;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                        column++;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];
                        column++;
                        BorderCells(ref Worksheets, rows, column);
                        Worksheets.Cells[rows, column].Value = sumSub[column - 8];

                        rows += 3;
                        WriteExcele(ref Worksheets, "A", rows, "R", rows, "ประเภทความพิการ 1=ด้านการมองเห็น 2=ด้านการได้ยิน 3=ด้านสติปัญญา 4=ด้านร่างกาย/สุขภาพ 5=ด้านการเรียนรู้");
                        rows++;
                        WriteExcele(ref Worksheets, "A", rows, "R", rows, "6=ด้านการพูด/ภาษา 7=ด้านพฤติกรรมอารมร์ 8=ออทิสติค 9=พิการซ้อน");

                        for (int i = 5; i <= flagRow; i++)
                        {
                            for (int j = 8; j <= 16; j++)
                            {
                                BorderCells(ref Worksheets, i, j);
                            }
                        }
                        for (int j = 1; j <= 18; j++)
                        {
                            BorderCells(ref Worksheets, rows - 5, j);
                        }
                        BorderCells(ref Worksheets, rows - 4, 1);
                        BorderCells(ref Worksheets, rows - 4, 17);
                        BorderCells(ref Worksheets, rows - 4, 18);
                        #endregion

                        Worksheets.Cells.AutoFitColumns();
                        excel.Save();
                    }

                    string attachment = "attachment; filename=" + namefile + ".xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.Charset = "";
                    streamExcel.WriteTo(Response.OutputStream);

                    Response.End();
                }
                catch { }
            }

            return new EmptyResult();
        }

        public ActionResult AllSchool()
        {
            return View();
        }

        public ActionResult ExportAllSchoolToExcel()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataStudent.txt");
            string data = System.IO.File.ReadAllText(path);
            String namefile = "ร.ร.ทั้งหมด";
            List<StudentModel> List = JsonConvert.DeserializeObject<List<StudentModel>>(data);

            if (List.Count > 0)
            {
                try
                {
                    System.IO.MemoryStream streamExcel = new System.IO.MemoryStream();
                    var cm = new Common();
                    using (var excel = new ExcelPackage(streamExcel))
                    {
                        System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                        int[] sum = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                        var Worksheets = excel.Workbook.Worksheets.Add("Sheet1");
                        int rows = 1;
                        int column = 1;

                        #region "ร.ร.ทั้งหมด"
                        GenHeadTableTabl(ref Worksheets, ref rows);

                        var sch = cm.GetSchool();
                        var schG = cm.GetSchoolGroup();
                        var S = cm.GetStudent();
                        var dT = cm.GetDisabledType();
                        var sD = cm.GetStudentDetail();
                        var schT = cm.GetSchoolType();
                        var amt = cm.GetAssessment();
                        var query = from _sch in sch
                                    from _schG in schG
                                    from _S in S
                                    from _schT in schT
                                    from _amt in amt
                                    where _S.sc_id == _sch.sc_id && _schG.scgroup_id == _sch.scgroup_id && _schT.sctype_id == _sch.sctype_id && _amt.std_id == _S.std_id //&& _S.std_grade == "ป.1"
                                    select new { _S.std_id, _sch.sc_id, _sch.sc_name, _schG.scgroup_name, _S.std_title, _S.std_firstname, _S.std_lastname, _S.std_idcard, _S.std_birthday, _S.std_grade, _schT.sctype_name, _amt.ass_certdisable };
                        rows += 2;

                        String group = "";
                        foreach (var aa in query.OrderBy(x => x.scgroup_name))
                        {
                            if (group == "" || group != aa.scgroup_name)
                            {
                                group = aa.scgroup_name;
                                WriteExcele(ref Worksheets, "B", rows, "C", rows, group);
                                rows++;
                            }


                            Worksheets.Cells[rows, column++].Value = aa.sc_id;
                            Worksheets.Cells[rows, column++].Value = aa.sc_name;
                            Worksheets.Cells[rows, column++].Value = aa.scgroup_name;
                            Worksheets.Cells[rows, column++].Value = aa.std_title + aa.std_firstname + " " + aa.std_lastname;
                            Worksheets.Cells[rows, column++].Value = aa.std_idcard;
                            Worksheets.Cells[rows, column++].Value = (2558 - Convert.ToInt16(aa.std_birthday.Split('-')[0]));
                            Worksheets.Cells[rows, column++].Value = aa.std_grade;
                            var _query = from _S in S
                                            from _sD in sD
                                            where _S.std_id == _sD.std_id && _S.std_id == aa.std_id
                                            select new { _sD.dsbtype_id };
                            foreach (var _aa in _query)
                            {
                                int i = Convert.ToInt16(_aa.dsbtype_id);
                                Worksheets.Cells[rows, column + i - 1].Value = "1";
                                Worksheets.Cells[rows, column + i - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                sum[i - 1]++;
                            }
                            Worksheets.Cells[rows, column += 9].Value = aa.sctype_name;
                            Worksheets.Cells[rows, ++column].Value = aa.ass_certdisable;
                            rows++;
                            column = 1;
                        }
                        for (int i = 8; i < 8 + sum.Length; i++)
                        {
                            Worksheets.Cells[rows, i].Value = sum[i - 8];
                            Worksheets.Cells[rows, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }

                        rows += 3;
                        WriteExcele(ref Worksheets, "A", rows, "R", rows, "ประเภทความพิการ 1=ด้านการมองเห็น 2=ด้านการได้ยิน 3=ด้านสติปัญญา 4=ด้านร่างกาย/สุขภาพ 5=ด้านการเรียนรู้");
                        rows++;
                        WriteExcele(ref Worksheets, "A", rows, "R", rows, "6=ด้านการพูด/ภาษา 7=ด้านพฤติกรรมอารมร์ 8=ออทิสติค 9=พิการซ้อน");


                        for (int i = 5; i <= rows - 4; i++)
                        {
                            for (int j = 1; j <= 18; j++)
                            {
                                BorderCells(ref Worksheets, i, j);
                            }
                        }

                        #endregion

                        Worksheets.Cells.AutoFitColumns();
                        excel.Save();
                    }

                    string attachment = "attachment; filename=" + namefile + ".xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.Charset = "";
                    streamExcel.WriteTo(Response.OutputStream);

                    Response.End();
                }
                catch { }
            }

            return new EmptyResult();
        }

        public ActionResult Zone()
        {
            return View();
        }

        public ActionResult ExportZoneToExcel()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataStudent.txt");
            string data = System.IO.File.ReadAllText(path);
            String namefile = "ร.ร.ที่คัดกรอง";
            List<StudentModel> List = JsonConvert.DeserializeObject<List<StudentModel>>(data);

            if (List.Count > 0)
            {
                try
                {
                    System.IO.MemoryStream streamExcel = new System.IO.MemoryStream();
                    var cm = new Common();
                    using (var excel = new ExcelPackage(streamExcel))
                    {
                        System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                        int[] sum = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                        var Worksheets = excel.Workbook.Worksheets.Add("Sheet1");
                        int rows = 1;
                        int column = 1;
  
                        #region "ร.ร.ที่คัดกรอง"
                        GenHeadTableTab2(ref Worksheets, ref rows);
                        var sch = cm.GetSchool();
                        var schG = cm.GetSchoolGroup();
                        var S = cm.GetStudent();
                        var sD = cm.GetStudentDetail();
                        var schT = cm.GetSchoolType();

                        var query = from _sch in sch
                                    from _schG in schG
                                    from _schT in schT
                                    where _schG.scgroup_id == _sch.scgroup_id && _schT.sctype_id == _sch.sctype_id
                                    select new { _sch.sc_id, _sch.sc_name, _schG.scgroup_name, _schT.sctype_name };
                        rows += 2;

                        String group = "";
                        foreach (var aa in query.OrderBy(x => x.scgroup_name))
                        {
                            if (group == "" || group != aa.scgroup_name)
                            {
                                group = aa.scgroup_name;
                                WriteExcele(ref Worksheets, "B", rows, "C", rows, group);
                                rows++;
                            }
                            int[] sumSub = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                            Worksheets.Cells[rows, column++].Value = aa.sc_id;
                            Worksheets.Cells[rows, column++].Value = aa.sc_name;
                            Worksheets.Cells[rows, column++].Value = aa.scgroup_name;

                            var _query = from _S in S
                                            from _sD in sD
                                            where _S.std_id == _sD.std_id && _S.sc_id == aa.sc_id
                                            select new { _sD.dsbtype_id };
                            foreach (var _aa in _query)
                            {
                                int i = Convert.ToInt16(_aa.dsbtype_id);
                                sumSub[i - 1]++;
                                sum[i - 1]++;
                                Worksheets.Cells[rows, column + i - 1].Value = sumSub[i - 1].ToString() == "0" ? "" : sumSub[i - 1].ToString();
                                Worksheets.Cells[rows, column + i - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }

                            Worksheets.Cells[rows, column += 9].Value = sumSub.Sum().ToString();
                            Worksheets.Cells[rows, ++column].Value = aa.sctype_name;
                            rows++;
                            column = 1;
                        }
                        int nextCells = 4;
                        int ii;
                        for (ii = nextCells; ii < nextCells + sum.Length; ii++)
                        {
                            Worksheets.Cells[rows, ii].Value = sum[ii - nextCells];
                            Worksheets.Cells[rows, ii].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }

                        Worksheets.Cells[rows, ii].Value = sum.Sum().ToString();

                        rows += 3;
                        WriteExcele(ref Worksheets, "A", rows, "N", rows, "ประเภทความพิการ 1=ด้านการมองเห็น 2=ด้านการได้ยิน 3=ด้านสติปัญญา 4=ด้านร่างกาย/สุขภาพ 5=ด้านการเรียนรู้");
                        rows++;
                        WriteExcele(ref Worksheets, "A", rows, "N", rows, "6=ด้านการพูด/ภาษา 7=ด้านพฤติกรรมอารมร์ 8=ออทิสติค 9=พิการซ้อน");
                        for (int i = 5; i <= rows - 4; i++)
                        {
                            for (int j = 1; j <= 14; j++)
                            {
                                BorderCells(ref Worksheets, i, j);
                            }
                        }

                        #endregion

                        Worksheets.Cells.AutoFitColumns();
                        excel.Save();
                    }

                    string attachment = "attachment; filename=" + namefile + ".xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.Charset = "";
                    streamExcel.WriteTo(Response.OutputStream);

                    Response.End();
                }
                catch { }
            }

            return new EmptyResult();
        }

        public ActionResult Level()
        {
            return View();
        }

        public ActionResult ExportLevelToExcel()
        {
            string path = HttpContext.Server.MapPath("~/App_Data/DataStudent.txt");
            string data = System.IO.File.ReadAllText(path);
            String namefile = "ข้อมูลระดับชั้น";
            List<StudentModel> List = JsonConvert.DeserializeObject<List<StudentModel>>(data);

            if (List.Count > 0)
            {
                try
                {
                    System.IO.MemoryStream streamExcel = new System.IO.MemoryStream();
                    var cm = new Common();
                    using (var excel = new ExcelPackage(streamExcel))
                    {
                        System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                        int[] sum = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                        var Worksheets = excel.Workbook.Worksheets.Add("Sheet1");
                        int rows = 1;
                        int column = 1;

                        #region "ข้อมูลระดับชั้น"
                        namefile = "ข้อมูลระดับชั้น";
                        GenHeadTableTab3(ref Worksheets, ref rows);

                        var S = cm.GetStudent();
                        var sD = cm.GetStudentDetail();

                        var query = from _S in S
                                    from _sD in sD
                                    where _sD.std_id == _S.std_id
                                    select new { _S.std_grade, _sD.dsbtype_id };
                        rows += 1;
                        String std_grade = "";
                        int[] sumSub = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        int[] reset = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        foreach (var aa in query.OrderByDescending(x => x.std_grade).OrderBy(y => y.dsbtype_id))
                        {
                            if (std_grade == "" || std_grade == aa.std_grade)
                            {
                                int i = Convert.ToInt16(aa.dsbtype_id);
                                sumSub[i - 1]++;
                                sum[i - 1]++;
                                std_grade = aa.std_grade;
                            }
                            else if (std_grade != aa.std_grade)
                            {
                                column = 1;
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = (rows - 4).ToString();
                                Worksheets.Cells[rows, column++].Value = std_grade;
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                                Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                Worksheets.Cells[rows, column++].Value = sumSub.Sum().ToString();

                                for (int ii = 0; ii < sumSub.Length; ii++)
                                    sumSub[ii] = 0;

                                int i = Convert.ToInt16(aa.dsbtype_id);
                                sumSub[i - 1]++;
                                sum[i - 1]++;
                                std_grade = aa.std_grade;
                                rows++;
                            }
                        }
                        column = 1;
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = (rows - 4).ToString();
                        Worksheets.Cells[rows, column++].Value = std_grade;
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sumSub.Sum().ToString();
                        rows++;

                        column = 2;
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = "รวม";
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum[column - 4];
                        Worksheets.Cells[rows, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Worksheets.Cells[rows, column++].Value = sum.Sum().ToString();

                        for (int i = 5; i <= rows; i++)
                        {
                            for (int j = 1; j <= 12; j++)
                            {
                                BorderCells(ref Worksheets, i, j);
                            }
                        }

                        #endregion

                        Worksheets.Cells.AutoFitColumns();
                        excel.Save();
                    }

                    string attachment = "attachment; filename=" + namefile + ".xls";
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", attachment);
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.Charset = "";
                    streamExcel.WriteTo(Response.OutputStream);

                    Response.End();
                }
                catch { }
            }

            return new EmptyResult();
        }
    }
}
