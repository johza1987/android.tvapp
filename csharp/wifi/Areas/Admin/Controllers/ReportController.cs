using OfficeOpenXml;
using SinetWifi.Common;
using SinetWifi.Controllers;
using SinetWifi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace SinetWifi.Areas.Admin.Controllers
{
    public class ReportController : BaseController
    {
        public ActionResult Coupon(int? p, string df, string dt, string key, int hid = 0)
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
                    ViewBag.HotspotId = hid;
                    ViewBag.KeyWord = key;

                    string CouponCancel = System.Configuration.ConfigurationManager.AppSettings["CouponCancel"].ToString();

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
                                 join h in context.Hotspot on b.hotspot_id equals h.id
                                 join pl in context.Plan on b.planName equals pl.planName into pl_leftjoin
                                 from pl in pl_leftjoin.DefaultIfEmpty()
                                 where rg.groupname != CouponCancel
                                 orderby b.creationdate descending
                                 select new UserinfoList
                                 {
                                     id = b.id,
                                     username = b.username,
                                     firstname = i.firstname,
                                     lastname = i.lastname,
                                     id_card = i.id_card,
                                     mobilephone = i.mobilephone,
                                     email = i.email,
                                     ref1 = i.ref1,
                                     ref2 = i.ref2,
                                     expiredate = (i.expiredate.HasValue) ? i.expiredate : null,
                                     packagename = b.planName,
                                     groupname = rg.groupname,
                                     packageprice = string.IsNullOrEmpty(pl.planCost) ? "0" : pl.planCost,
                                     creationdate = b.creationdate,
                                     hotspot_id = b.hotspot_id,
                                     hotspot_name = h.name
                                 })
                                 .WhereIf(hid > 0, w => w.hotspot_id == hid)
                                 .WhereIf(!string.IsNullOrEmpty(df), w => w.creationdate >= dateFrom)
                                 .WhereIf(!string.IsNullOrEmpty(dt), w => w.creationdate < dateTo)
                                 .WhereIf(!string.IsNullOrEmpty(key),
                                      w =>
                                          w.firstname.Contains(key.Trim())
                                        || w.lastname.Contains(key.Trim())
                                        || w.id_card.Contains(key.Trim())
                                        || w.mobilephone.Contains(key.Trim())
                                        || w.email.Contains(key.Trim())
                                        || w.ref1.Contains(key.Trim())
                                        || w.ref2.Contains(key.Trim())
                                        || w.username.Contains(key.Trim())
                                        || w.hotspot_name.Contains(key.Trim())
                                        );

                    list = model.ToPagedList(pageNumber, pageSize);

                    if ((pageNumber > list.PageCount) && (list.PageCount > 0))
                    {
                        list = model.ToPagedList(list.PageCount, pageSize);
                    }

                    ViewData["Hotspot"] = context.Hotspot.ToList<Hotspot>();
                }
                catch (Exception ex)
                {
                    string msgError = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                    ViewBag.Error = msgError;
                    LogManager.Error("Report.Coupon Error -> " + msgError);
                }
            }

            return View(list);
        }

        public ActionResult ExportCouponToExcel(string df, string dt, string key, int hid = 0)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                try
                {
                    string CouponCancel = System.Configuration.ConfigurationManager.AppSettings["CouponCancel"].ToString();

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
                                 join h in context.Hotspot on b.hotspot_id equals h.id
                                 join pl in context.Plan on b.planName equals pl.planName into pl_leftjoin
                                 from pl in pl_leftjoin.DefaultIfEmpty()
                                 where rg.groupname != CouponCancel
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
                                     expiredate = (i.expiredate.HasValue) ? i.expiredate : null,
                                     packagename = b.planName,
                                     groupname = rg.groupname,
                                     packageprice = string.IsNullOrEmpty(pl.planCost) ? "0" : pl.planCost,
                                     creationdate = b.creationdate,
                                     hotspot_id = b.hotspot_id,
                                     hotspot_name = h.name
                                 })
                                 .WhereIf(hid > 0, w => w.hotspot_id == hid)
                                 .WhereIf(!string.IsNullOrEmpty(df), w => w.creationdate >= dateFrom)
                                 .WhereIf(!string.IsNullOrEmpty(dt), w => w.creationdate < dateTo)
                                 .WhereIf(!string.IsNullOrEmpty(key),
                                      w => 
                                          w.firstname.Contains(key.Trim())
                                        || w.lastname.Contains(key.Trim())
                                        || w.id_card.Contains(key.Trim())
                                        || w.mobilephone.Contains(key.Trim())
                                        || w.email.Contains(key.Trim())
                                        || w.ref1.Contains(key.Trim())
                                        || w.ref2.Contains(key.Trim())
                                        || w.username.Contains(key.Trim())
                                        )
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

                            string[] fieldHeader = { "Create Date", "หอพัก", "Username", "ชื่อลูกค้า", "นามสกุล", "เบอร์ติดต่อ", "Email", "Ref1", "Ref2", "Package", "วันหมดอายุ", "ราคาต้นทุน" };

                            for (int i = 0; i < fieldHeader.Length; i++)
                            {
                                Worksheets.Cells[3, i + 1].Value = fieldHeader[i];
                                Worksheets.Cells[3, i + 1].Style.Font.Bold = true;
                            }

                            int startRows = 3;

                            foreach (var item in model)
                            {
                                Worksheets.Cells[startRows + 1, 1].Value = item.creationdate.ToString("dd/MM/yyyy HH:mm:ss");
                                Worksheets.Cells[startRows + 1, 2].Value = item.hotspot_name;
                                Worksheets.Cells[startRows + 1, 3].Value = item.username;
                                Worksheets.Cells[startRows + 1, 4].Value = item.firstname;
                                Worksheets.Cells[startRows + 1, 5].Value = item.lastname;
                                Worksheets.Cells[startRows + 1, 6].Value = item.mobilephone;
                                Worksheets.Cells[startRows + 1, 7].Value = item.email;
                                Worksheets.Cells[startRows + 1, 8].Value = item.ref1;
                                Worksheets.Cells[startRows + 1, 9].Value = item.ref2;
                                Worksheets.Cells[startRows + 1, 10].Value = item.packagename;
                                Worksheets.Cells[startRows + 1, 11].Value = (item.expiredate.HasValue) ? item.expiredate.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
                                Worksheets.Cells[startRows + 1, 12].Value = double.Parse(item.packageprice).ToString("#,##0.00;($#,##0.00);0.00");

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

        public ActionResult SendMail()
        {
            SendMailToAdmin("ข้อความ");
            SendMailToCustomer("ข้อความ" , "anat.joh@gmail.com");

            return View();
        }

        void SendMailToAdmin(string Msg)
        {
            // โค้ดส่งเมลล์
        }
        void SendMailToCustomer(string Msg, string Email)
        {
            // โค้ดส่งเมลล์
        }
    }
}
