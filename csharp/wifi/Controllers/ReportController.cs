using SinetWifi.Models;
using SinetWifi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using PagedList;
using OfficeOpenXml;

namespace SinetWifi.Controllers
{
    public class ReportController : BaseController
    {
        public ActionResult Coupon(int? p, string df, string dt)
        {
            int pageSize = 30;

            int pageNumber = Shared.GetPageNumber(p);

            PagedList.IPagedList<UserinfoList> list = null;

            using (DatabaseContext context = new DatabaseContext())
            {
                try
                {
                    ViewBag.DateFrom = df;
                    ViewBag.DateTo = dt;

                    DateTime? dateFrom = null;
                    DateTime? dateTo = null;

                    if (!string.IsNullOrEmpty(df))
                    {
                        dateFrom = Convert.ToDateTime(df, Shared.CultureInfoTh);
                    }

                    if (!string.IsNullOrEmpty(dt))
                    {
                        dateTo = Convert.ToDateTime(dt, Shared.CultureInfoTh).AddDays(1);
                    }

                    var model = (from b in context.UserBillinfo
                                 join i in context.Userinfo on b.username equals i.username
                                 join rg in context.RadUsergroup on b.username equals rg.username
                                 join pl in context.Plan on b.planName equals pl.planName into pl_leftjoin from pl in pl_leftjoin.DefaultIfEmpty()
                                 where b.hotspot_id == UserOnline.HotspotId
                                 orderby b.creationdate descending
                                 select new UserinfoList
                                 {
                                     username = b.username,
                                     firstname = i.firstname,
                                     lastname = i.lastname,
                                     id_card = i.id_card,
                                     mobilephone = i.mobilephone,
                                     email = i.email,
                                     ref1 = i.ref1,
                                     ref2 = i.ref2,
                                     packagename = b.planName,
                                     groupname = rg.groupname,
                                     packageprice = string.IsNullOrEmpty(pl.planCost) ? "0" : pl.planCost,
                                     creationdate = b.creationdate
                                 })
                                 .WhereIf(!string.IsNullOrEmpty(df), w => w.creationdate >= dateFrom)
                                 .WhereIf(!string.IsNullOrEmpty(dt), w => w.creationdate < dateTo);

                    list = model.ToPagedList(pageNumber, pageSize);

                    if ((pageNumber > list.PageCount) && (list.PageCount > 0))
                    {
                        list = model.ToPagedList(list.PageCount, pageSize);
                    }
                }
                catch(Exception ex) 
                {
                    string msgError = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                    ViewBag.Error = msgError;
                    LogManager.Error("Report.Coupon Error -> " + msgError);
                }
            }

            return View(list);
        }

        public ActionResult ExportCouponToExcel(string df, string dt)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                try
                {
                    DateTime? dateFrom = null;
                    DateTime? dateTo = null;

                    if (!string.IsNullOrEmpty(df))
                    {
                        dateFrom = Convert.ToDateTime(df, Shared.CultureInfoTh);
                    }

                    if (!string.IsNullOrEmpty(dt))
                    {
                        dateTo = Convert.ToDateTime(dt, Shared.CultureInfoTh).AddDays(1);
                    }

                    var model = (from b in context.UserBillinfo
                                 join i in context.Userinfo on b.username equals i.username
                                 join rg in context.RadUsergroup on b.username equals rg.username
                                 join pl in context.Plan on b.planName equals pl.planName into pl_leftjoin
                                 from pl in pl_leftjoin.DefaultIfEmpty()
                                 where b.hotspot_id == UserOnline.HotspotId
                                 orderby b.creationdate descending
                                 select new UserinfoList
                                 {
                                     username = b.username,
                                     firstname = i.firstname,
                                     lastname = i.lastname,
                                     id_card = i.id_card,
                                     mobilephone = i.mobilephone,
                                     email = i.email,
                                     ref1 = i.ref1,
                                     ref2 = i.ref2,
                                     packagename = b.planName,
                                     groupname = rg.groupname,
                                     packageprice = string.IsNullOrEmpty(pl.planCost) ? "0" : pl.planCost,
                                     creationdate = b.creationdate
                                 })
                                 .WhereIf(!string.IsNullOrEmpty(df), w => w.creationdate >= dateFrom)
                                 .WhereIf(!string.IsNullOrEmpty(dt), w => w.creationdate < dateTo)
                                 .ToList<UserinfoList>();

                    if (model.Count > 0)
                    {
                        System.IO.MemoryStream streamExcel = new System.IO.MemoryStream();

                        using (var excel = new ExcelPackage(streamExcel))
                        {
                            System.IO.MemoryStream memStream = new System.IO.MemoryStream();

                            var Worksheets = excel.Workbook.Worksheets.Add("Sheet1");

                            Worksheets.Cells[1, 1, 1, 1].Value = "รายงานการออกคูปอง";
                            Worksheets.Cells[1, 1, 1, 1].Style.Font.Bold = true;

                            string[] fieldHeader = { "Create Date", "Username", "ชื่อลูกค้า", "นามสกุล", "เบอร์ติดต่อ", "Email", "Ref1", "Ref2", "Package", "ราคาต้นทุน" ,"หมายเหตุ" };

                            for (int i = 0; i < fieldHeader.Length; i++)
                            {
                                Worksheets.Cells[3, i + 1].Value = fieldHeader[i];
                                Worksheets.Cells[3, i + 1].Style.Font.Bold = true;
                            }

                            int startRows = 3;

                            foreach (var item in model)
                            {
                                Worksheets.Cells[startRows + 1, 1].Value = item.creationdate.ToString("dd/MM/yyyy HH:mm:ss");
                                Worksheets.Cells[startRows + 1, 2].Value = item.username;
                                Worksheets.Cells[startRows + 1, 3].Value = item.firstname;
                                Worksheets.Cells[startRows + 1, 4].Value = item.lastname;
                                Worksheets.Cells[startRows + 1, 5].Value = item.mobilephone;
                                Worksheets.Cells[startRows + 1, 6].Value = item.email;
                                Worksheets.Cells[startRows + 1, 7].Value = item.ref1;
                                Worksheets.Cells[startRows + 1, 8].Value = item.ref2;
                                Worksheets.Cells[startRows + 1, 9].Value = item.packagename;
                                Worksheets.Cells[startRows + 1, 10].Value = double.Parse(item.packageprice).ToString("#,##0.00;($#,##0.00);0.00");
                                Worksheets.Cells[startRows + 1, 11].Value = (item.groupname.Equals(System.Configuration.ConfigurationManager.AppSettings["CouponCancel"].ToString()) ? "คูปองรหัสนี้ถูกยกเลิกไปแล้ว" : "");

                                startRows++;
                            }

                            for (int i = 2; i <= fieldHeader.Length; i++)
                            {
                                Worksheets.Column(i).AutoFit();
                            }
                            excel.Save();
                        }

                        string attachment = "attachment; filename=Coupon.xls";
                        Response.ClearContent();
                        Response.AddHeader("content-disposition", attachment);
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.Charset = "";
                        streamExcel.WriteTo(Response.OutputStream);
                        Response.End();
                    }
                }
                catch (Exception ex)
                {
                    string msgError = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                    LogManager.Error("Report.ExportCouponToExcel Error -> " + msgError);
                }
            }

            return new EmptyResult();
        }
    }
}
