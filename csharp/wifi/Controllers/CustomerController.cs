using SinetWifi.Common;
using SinetWifi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net.Mail;
using OfficeOpenXml;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data.Entity;

namespace SinetWifi.Controllers
{
    public class CustomerController : BaseController
    {
        public int CouponLimit = string.IsNullOrEmpty(ConfigurationManager.AppSettings["CouponLimit"].ToString()) ? 1 : int.Parse(ConfigurationManager.AppSettings["CouponLimit"]);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(int? p, string key)
        {
            IPagedList<UserinfoList> list = new List<UserinfoList>().ToPagedList(1,1);

            try
            {
                ViewBag.KeyWord = key;

                int pageSize = 30;

                int pageNumber = Shared.GetPageNumber(p);

                using (DatabaseContext context = new DatabaseContext())
                {
                    var model = (from u in context.BatchUser
                                 where u.hotspot_id == UserOnline.HotspotId
                                 orderby u.id descending
                                 select new UserinfoList
                                 {
                                     id = u.id,
                                     firstname = u.firstname,
                                     lastname = u.lastname,
                                     id_card = u.id_card,
                                     passport_id = u.passport_id,
                                     mobilephone = u.mobilephone,
                                     email = u.email,
                                     ref1 = u.ref1,
                                     ref2 = u.ref2
                                 }).WhereIf(!string.IsNullOrEmpty(key),
                                          w =>
                                              w.firstname.Contains(key.Trim())
                                            || w.lastname.Contains(key.Trim())
                                            || w.id_card.Contains(key.Trim())
                                            || w.passport_id.Contains(key.Trim())
                                            || w.mobilephone.Contains(key.Trim())
                                            || w.email.Contains(key.Trim())
                                            || w.ref1.Contains(key.Trim())
                                            || w.ref2.Contains(key.Trim())
                                            );

                    list = model.ToPagedList(pageNumber, pageSize);

                    if ((pageNumber > list.PageCount) && (list.PageCount > 0))
                    {
                        list = model.ToPagedList(list.PageCount, pageSize);
                    }
                }
            }
            catch(Exception ex)
            {
                ViewBag.Error = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                LogManager.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }

            return View(list);
        }

        public ActionResult ExportCustomerToExcel(string key)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                try
                {
                    var model = (from u in context.BatchUser
                                 where u.hotspot_id == UserOnline.HotspotId
                                 orderby u.id descending
                                 select new UserinfoList
                                 {
                                     id = u.id,
                                     firstname = u.firstname,
                                     lastname = u.lastname,
                                     id_card = u.id_card,
                                     passport_id = u.passport_id,
                                     mobilephone = u.mobilephone,
                                     email = u.email,
                                     ref1 = u.ref1,
                                     ref2 = u.ref2
                                 }).WhereIf(!string.IsNullOrEmpty(key),
                                          w =>
                                              w.firstname.Contains(key.Trim())
                                            || w.lastname.Contains(key.Trim())
                                            || w.id_card.Contains(key.Trim())
                                            || w.passport_id.Contains(key.Trim())
                                            || w.mobilephone.Contains(key.Trim())
                                            || w.email.Contains(key.Trim())
                                            || w.ref1.Contains(key.Trim())
                                            || w.ref2.Contains(key.Trim())
                                            ).ToList<UserinfoList>();

                    if (model.Count > 0)
                    {
                        System.IO.MemoryStream streamExcel = new System.IO.MemoryStream();

                        using (var excel = new ExcelPackage(streamExcel))
                        {
                            System.IO.MemoryStream memStream = new System.IO.MemoryStream();

                            var Worksheets = excel.Workbook.Worksheets.Add("Sheet1");

                            Worksheets.Cells[1, 1, 1, 1].Value = "รายชื่อลูกค้า";
                            Worksheets.Cells[1, 1, 1, 1].Style.Font.Bold = true;

                            string[] fieldHeader = { "ชื่อ", "นามสกุล", "เลขประจำตัวประชาชน", "เลขที่หนังสือเดินทาง", "เบอร์ติดต่อ", "Email", "Ref1", "Ref2" };

                            for (int i = 0; i < fieldHeader.Length; i++)
                            {
                                Worksheets.Cells[3, i + 1].Value = fieldHeader[i];
                                Worksheets.Cells[3, i + 1].Style.Font.Bold = true;
                            }

                            int startRows = 3;

                            foreach (var item in model)
                            {
                                Worksheets.Cells[startRows + 1, 1].Value = item.firstname;
                                Worksheets.Cells[startRows + 1, 2].Value = item.lastname;
                                Worksheets.Cells[startRows + 1, 3].Value = item.id_card;
                                Worksheets.Cells[startRows + 1, 4].Value = item.passport_id;
                                Worksheets.Cells[startRows + 1, 5].Value = item.mobilephone;
                                Worksheets.Cells[startRows + 1, 6].Value = item.email;
                                Worksheets.Cells[startRows + 1, 7].Value = item.ref1;
                                Worksheets.Cells[startRows + 1, 8].Value = item.ref2;

                                startRows++;
                            }

                            for (int i = 2; i <= fieldHeader.Length; i++)
                            {
                                Worksheets.Column(i).AutoFit();
                            }
                            excel.Save();
                        }

                        string attachment = "attachment; filename=Customer.xls";
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
                    LogManager.Error("ExportCustomerToExcel Error -> " + msgError);
                }
            }

            return new EmptyResult();
        }

        public ActionResult Create()
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var plan = (from hp in context.HotspotPlan
                            join pl in context.Plan on hp.plan_id equals pl.id
                            join pr in context.Profile on pl.planName equals pr.plan_name
                            where hp.hotspot_id == UserOnline.HotspotId
                            select new PlanList
                            {
                                id = pl.id,
                                planName = pl.planName
                            }).Distinct();

                ViewData["Plan"] = plan.ToList<PlanList>();
            }

            ViewData["CouponLimit"] = CouponLimit;

            return View();
        }

        [HttpPost]
        public JsonResult Create(BatchUser model)
        {
            JsonResponse json = new JsonResponse { Status = false };

            string MsgError = string.Empty;

            List<CouponList> coupon = new List<CouponList>();

            int id;

            if (ModelState.IsValid)
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    try
                    {
                        if (model.quantity <= CouponLimit && model.quantity > 0)
                        {
                            var plan = context.Plan.Find(model.packageId);

                            string planname = (plan != null) ? plan.planName : string.Empty;

                            var profile = context.Profile.Where(p => p.plan_name.Equals(planname)).FirstOrDefault();

                            string groupname = (profile != null) ? profile.profile_name : string.Empty;

                            var BatchUser = new BatchUser
                            {
                                batch_name = DateTime.Now.ToString("yyMMddHHmmssfff", Shared.CultureInfo),
                                batch_status = "Pending",
                                hotspot_id = UserOnline.HotspotId,
                                firstname = model.firstname,
                                lastname = model.lastname,
                                id_card = model.id_card,
                                passport_id = model.passport_id,
                                ref1 = model.ref1,
                                ref2 = model.ref2,
                                mobilephone = model.mobilephone,
                                email = model.email,
                                creationdate = DateTime.Now,
                                creationby = UserOnline.Username
                            };

                            context.BatchUser.Add(BatchUser);

                            context.SaveChanges();

                            id = BatchUser.id;

                            for (int i = 0; i < model.quantity; i++)
                            {
                                string password = Shared.RandomPassword();

                                var doc = context.GenerateCode.Find(1);

                                if (doc != null)
                                {
                                    string code = string.Empty;

                                    int run = doc.last_number;

                                    if (doc.last_gendate == null)
                                    {
                                        doc.last_gendate = DateTime.Now;
                                    }
                                    else
                                    {
                                        if (DateTime.Now.Year != ((DateTime)doc.last_gendate).Year)
                                        {
                                            doc.last_gendate = DateTime.Now;
                                            run = 0;
                                        }
                                    }

                                    run++;
                                    doc.last_number = run;
                                    code = (string.IsNullOrEmpty(doc.prefix) ? "" : doc.prefix.Trim()) + doc.last_gendate.ToString("yy", Shared.CultureInfo) + run.ToString().PadLeft(doc.digit_number, '0');
                                    context.Entry(doc).State = EntityState.Modified;

                                    context.Userinfo.Add(new Userinfo
                                    {
                                        username = code,
                                        firstname = model.firstname,
                                        lastname = model.lastname,
                                        id_card = model.id_card,
                                        passport_id = model.passport_id,
                                        ref1 = model.ref1,
                                        ref2 = model.ref2,
                                        mobilephone = model.mobilephone,
                                        email = model.email,
                                        creationdate = DateTime.Now,
                                        creationby = UserOnline.Username
                                    });

                                    context.UserBillinfo.Add(new UserBillinfo
                                    {
                                        username = code,
                                        planName = planname,
                                        hotspot_id = UserOnline.HotspotId,
                                        batch_id = id,
                                        creationdate = DateTime.Now,
                                        creationby = UserOnline.Username
                                    });

                                    context.RadUsergroup.Add(new RadUsergroup
                                    {
                                        username = code,
                                        groupname = groupname,
                                        priority = 0
                                    });

                                    context.RadCheck.Add(new RadCheck
                                    {
                                        username = code,
                                        attribute = "Cleartext-Password",
                                        op = ":=",
                                        value = password
                                    });

                                    context.SaveChanges();

                                    coupon.Add(new CouponList
                                    {
                                        package = planname,
                                        username = code,
                                        password = password
                                    });

                                    json.Status = true;
                                }
                                else
                                {
                                    json.Status = false;
                                    json.Message = "Generate_code Table not initial document running for coupon";
                                }
                            }

                            if (json.Status == true)
                            {
                                EmailSentCreateCopon(id, coupon);
                                SMSSentCreateCopon(id, model.mobilephone, coupon);
                            }
                        }
                        else
                        {
                            json.Status = false;
                            json.Message = "Can not create more coupon than " + CouponLimit + " items.";
                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        string msg = string.Empty;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            msg += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);

                            foreach (var ve in eve.ValidationErrors)
                            {
                                msg += string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        json.Status = false;
                        json.Message = msg;
                        LogManager.Error("Customer.Create Error -> " + msg);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                        json.Status = false;
                        json.Message = msg;
                        LogManager.Error("Customer.Create Error -> " + msg);
                    }
                }
            }
            else
            {
                json.Status = false;

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        json.Message += error.ErrorMessage + " ";
                    }
                }
            }

            json.Message = Shared.GetMsg(json.Status, json.Message);

            return Json(json);
        }

        [HttpPost]
        public JsonResult Update(BatchUser model)
        {
            JsonResponse json = new JsonResponse { Status = false };

            string MsgError = string.Empty;

            if (ModelState.IsValid)
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    try
                    {
                        var userinfo = context.BatchUser.Find(model.id);

                        userinfo.firstname = model.firstname;
                        userinfo.lastname = model.lastname;
                        userinfo.id_card = model.id_card;
                        userinfo.passport_id = model.passport_id;
                        userinfo.mobilephone = model.mobilephone;
                        userinfo.email = model.email;
                        userinfo.ref1 = model.ref1;
                        userinfo.ref2 = model.ref2;

                        context.Entry(userinfo).State = EntityState.Modified;
                        context.SaveChanges();
                        
                        json.Status = true;
                    }
                    catch (DbEntityValidationException e)
                    {
                        string msg = string.Empty;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            msg += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);

                            foreach (var ve in eve.ValidationErrors)
                            {
                                msg += string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        json.Status = false;
                        json.Message = msg;
                        LogManager.Error("Customer.Update Error -> " + msg);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                        json.Status = false;
                        json.Message = msg;
                        LogManager.Error("Customer.Update Error -> " + msg);
                    }
                }
            }
            else
            {
                json.Status = false;

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        json.Message += error.ErrorMessage + " ";
                    }
                }
            }

            json.Message = Shared.GetMsg(json.Status, json.Message);

            return Json(json);
        }

        public ActionResult Details(int id)
        {
            BatchUser model = new BatchUser();

            using (DatabaseContext context = new DatabaseContext())
            {
                try
                {
                    var plan = (from hp in context.HotspotPlan
                                join pl in context.Plan on hp.plan_id equals pl.id
                                join pr in context.Profile on pl.planName equals pr.plan_name
                                where hp.hotspot_id == UserOnline.HotspotId
                                select new PlanList
                                {
                                    id = pl.id,
                                    planName = pl.planName
                                }).Distinct();

                    ViewData["Plan"] = plan.ToList<PlanList>();

                    model = context.BatchUser.Where(u => u.id == id).FirstOrDefault();

                    var coupon = (from bill in context.UserBillinfo
                                  join rd in context.RadCheck on bill.username equals rd.username
                                  join rg in context.RadUsergroup on bill.username equals rg.username
                                  join user in context.Userinfo on bill.username equals user.username
                                  where bill.batch_id == id
                                  orderby bill.id descending
                                  select new CouponList
                                  {
                                      username = rd.username,
                                      password = rd.value,
                                      package = bill.planName,
                                      groupname = rg.groupname,
                                      creationdate = bill.creationdate,
                                      firstaccttime = user.firstaccttime,
                                      expiredate = user.expiredate
                                  });

                    ViewData["Coupon"] = coupon.ToList<CouponList>();
                }
                catch(Exception ex)
                {
                    string msgError = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                    ViewBag.Error = msgError;
                    LogManager.Error("Report.Coupon Error -> " + msgError);
                }
            }

            ViewData["CouponLimit"] = CouponLimit;

            return View(model);
        }

        [HttpPost]
        public JsonResult Coupon(int id, int packageId, int quantity)
        {
            JsonResponse json = new JsonResponse { Status = false };

            string MsgError = string.Empty;

            List<CouponList> coupon = new List<CouponList>();

            if (ModelState.IsValid)
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    try
                    {
                        if (quantity <= CouponLimit && quantity > 0)
                        {
                            var model = context.BatchUser.Where(b => b.id == id).FirstOrDefault();
                            var plan = context.Plan.Find(packageId);
                            string planname = (plan != null) ? plan.planName : string.Empty;
                            var profile = context.Profile.Where(p => p.plan_name.Equals(planname)).FirstOrDefault();
                            string groupname = (profile != null) ? profile.profile_name : string.Empty;

                            for (int i = 0; i < quantity; i++)
                            {
                                string password = Shared.RandomPassword();

                                var doc = context.GenerateCode.Find(1);

                                if (doc != null)
                                {
                                    string code = string.Empty;

                                    int run = doc.last_number;

                                    if (doc.last_gendate == null)
                                    {
                                        doc.last_gendate = DateTime.Now;
                                    }
                                    else
                                    {
                                        if (DateTime.Now.Year != ((DateTime)doc.last_gendate).Year)
                                        {
                                            doc.last_gendate = DateTime.Now;
                                            run = 0;
                                        }
                                    }

                                    run++;
                                    doc.last_number = run;
                                    code = (string.IsNullOrEmpty(doc.prefix) ? "" : doc.prefix.Trim()) + doc.last_gendate.ToString("yy", Shared.CultureInfo) + run.ToString().PadLeft(doc.digit_number, '0');
                                    context.Entry(doc).State = EntityState.Modified;

                                    context.Userinfo.Add(new Userinfo
                                    {
                                        username = code,
                                        firstname = model.firstname,
                                        lastname = model.lastname,
                                        id_card = model.id_card,
                                        passport_id = model.passport_id,
                                        ref1 = model.ref1,
                                        ref2 = model.ref2,
                                        mobilephone = model.mobilephone,
                                        email = model.email,
                                        creationdate = DateTime.Now,
                                        creationby = UserOnline.Username
                                    });

                                    context.UserBillinfo.Add(new UserBillinfo
                                    {
                                        username = code,
                                        planName = planname,
                                        hotspot_id = UserOnline.HotspotId,
                                        batch_id = id,
                                        creationdate = DateTime.Now,
                                        creationby = UserOnline.Username
                                    });

                                    context.RadUsergroup.Add(new RadUsergroup
                                    {
                                        username = code,
                                        groupname = groupname,
                                        priority = 0
                                    });

                                    context.RadCheck.Add(new RadCheck
                                    {
                                        username = code,
                                        attribute = "Cleartext-Password",
                                        op = ":=",
                                        value = password
                                    });

                                    context.SaveChanges();

                                    coupon.Add(new CouponList
                                    {
                                        package = planname,
                                        username = code,
                                        password = password
                                    });

                                    json.Status = true;
                                }
                                else
                                {
                                    json.Status = false;
                                    json.Message = "ไม่สามารถสร้างรหัส Batch User ได้ !!";
                                }
                            }

                            if (json.Status == true)
                            {
                                EmailSentCreateCopon(id, coupon);
                                SMSSentCreateCopon(id, model.mobilephone, coupon);
                            }
                        }
                        else
                        {
                            json.Status = false;
                            json.Message = "Can not create more coupon than 5 items.";
                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        string msg = string.Empty;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            msg += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);

                            foreach (var ve in eve.ValidationErrors)
                            {
                                msg += string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        json.Status = false;
                        json.Message = msg;
                        LogManager.Error("Customer.Coupon Create Error -> " + msg);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                        json.Status = false;
                        json.Message = msg;
                        LogManager.Error("Customer.Coupon Create Error -> " + msg);
                    }
                }
            }
            else
            {
                json.Status = false;

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        json.Message += error.ErrorMessage + " ";
                    }
                }
            }

            json.Message = Shared.GetMsg(json.Status, json.Message);

            return Json(json);
        }

        [HttpPost]
        public JsonResult CancelCoupon(string id)
        {
            JsonResponse json = new JsonResponse { Status = false };

            string MsgError = string.Empty;

            if (!string.IsNullOrEmpty(id))
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    try
                    {
                        var usergroup = context.RadUsergroup.Where(i => i.username.Equals(id)).FirstOrDefault();

                        if (usergroup != null)
                        {
                            usergroup.groupname = ConfigurationManager.AppSettings["CouponCancel"].ToString();

                            context.Entry(usergroup).State = EntityState.Modified;

                            context.SaveChanges();

                            json.Status = true;
                        }
                    }
                    catch (DbEntityValidationException e)
                    {
                        string msg = string.Empty;
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            msg += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);

                            foreach (var ve in eve.ValidationErrors)
                            {
                                msg += string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        json.Status = false;
                        json.Message = msg;
                        LogManager.Error("Customer.CancelCoupon Error -> " + msg);
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                        json.Status = false;
                        json.Message = msg;
                        LogManager.Error("Customer.CancelCoupon Error -> " + msg);
                    }
                }
            }
            else
            {
                json.Status = false;
                json.Message = "ไม่สามารถบันทึกข้อมูลได้ เนื่องจากรหัสคูปองเป็นค่าว่าง";
            }

            json.Message = Shared.GetMsg(json.Status, json.Message);

            return Json(json);
        }

        #region Send SMS with MailBit Gateway
        public string SMSSentCreateCopon(int id, string mobilephone, List<CouponList> coupon)
        {
            string sMsgError = string.Empty;

            try
            {
                mobilephone = string.IsNullOrEmpty(mobilephone) ? "" : mobilephone;

                if (ConfigurationManager.AppSettings["SMSEnable"].Equals("true") && !string.IsNullOrEmpty(mobilephone) && mobilephone.Length == 10)
                {
                    string URL = ConfigurationManager.AppSettings["SMSMAILBITURL"];
                    string USER = ConfigurationManager.AppSettings["SMSMAILBITUSER"];
                    string PASS = ConfigurationManager.AppSettings["SMSMAILBITPASS"];
                    string SENDERID = ConfigurationManager.AppSettings["SMSMAILBITSENDERID"];

                    foreach (var item in coupon)
                    {
                        StringBuilder ToThaiMobile = new StringBuilder(mobilephone);
                        string TO = ToThaiMobile.Remove(0, 1).Insert(0, "66").ToString(); // Example: 66882975234;
                        string CONTENT = "Welcome to Sinet WiFi, Username: " + item.username + " Password: " + item.password; ;
                        string PARAMS = string.Format("?user={0}&password={1}&sid={2}&msisdn={3}&msg={4}&fl={5}&dc={6}", USER, PASS, SENDERID, TO, CONTENT, "0", "8");
                        String res = string.Empty;
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + PARAMS);
                        request.ContentType = "application/json; charset=UTF-8";
                        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            res = reader.ReadToEnd();
                        }
                        try
                        {
                            var data = JsonConvert.DeserializeObject<SMSMailBITResponse>(res);

                            if (data.ErrorCode != "000")
                            {
                                EmailSentError(System.Reflection.MethodBase.GetCurrentMethod().Name, "SMS to Mobile Number: " + mobilephone + " ,BatchUser: " + id + " ,Error: " + data.ErrorMessage);
                            }

                            LogManager.More("SMS to Mobile Number: " + mobilephone + " ,Result: " + data.ErrorMessage + " ,Message: " + CONTENT + " ,SMSGateWayResponse: " + res, "SMS");
                        }
                        catch (WebException ex)
                        {
                            sMsgError += "This program is expected to throw WebException on successful run." + "\n\nException Message :" + ex.Message;

                            if (ex.Status == WebExceptionStatus.ProtocolError)
                            {
                                sMsgError += string.Format(" ,Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode);
                                sMsgError += string.Format(" ,Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription);
                            }

                            LogManager.Error("SMS to Mobile Number Error > " + mobilephone + " ,BatchUser: " + id + " ,Error: " + sMsgError);
                        }
                        catch (Exception ex)
                        {
                            sMsgError += ex.Message.ToString() + " --> SMS Gateway Response: " + res;
                            LogManager.Error("SMS to Mobile Number Error > " + mobilephone + " ,BatchUser: " + id + " ,Error: " + sMsgError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sMsgError += ex.Message.ToString();
                LogManager.Error("SMS to Mobile Number Error > " + mobilephone + " ,BatchUser: " + id + " ,Error: " + sMsgError);
            }

            if (!string.IsNullOrEmpty(sMsgError))
            {
                EmailSentError(System.Reflection.MethodBase.GetCurrentMethod().Name, "SMS to Mobile Number: " + mobilephone + " ,BatchUser: " + id + " ,Error: " + sMsgError);
            }

            return sMsgError;
        }
        #endregion

        #region Send SMS with AIS Gateway
        //public string SMSSentCreateCopon(int id, string mobilephone, List<CouponList> coupon)
        //{
        //    string sMsgError = string.Empty;

        //    try
        //    {
        //        mobilephone = string.IsNullOrEmpty(mobilephone) ? "" : mobilephone;

        //        if (ConfigurationManager.AppSettings["SMSEnable"].Equals("true") && !string.IsNullOrEmpty(mobilephone) && mobilephone.Length == 10)
        //        {
        //            var thaiMobilephone = new StringBuilder(mobilephone);

        //            string URL = ConfigurationManager.AppSettings["SMSURL"];
        //            string CMD = ConfigurationManager.AppSettings["SMSCMD"];
        //            string FROM = ConfigurationManager.AppSettings["SMSFROM"];
        //            string TO = thaiMobilephone.Remove(0, 1).Insert(0, "66").ToString(); // Example: 66882975234;
        //            string CHARGE = ConfigurationManager.AppSettings["SMSCHARGE"];
        //            string CODE = ConfigurationManager.AppSettings["SMSCODE"];
        //            string CTYPE = ConfigurationManager.AppSettings["SMSCTYPE"]; //TEXT = For English SMS, UNICODE = For Thai SMS
        //            string REPORT = ConfigurationManager.AppSettings["SMSREPORT"];

        //            foreach (var item in coupon)
        //            {
        //                string Message = "Welcome to Sinet WiFi, Username: " + item.username + " Password: " + item.password;
        //                string CONTENT = string.Empty;
        //                string responseFromServer = string.Empty;

        //                try
        //                {
        //                    if (CTYPE.Equals("UNICODE"))
        //                    {
        //                        System.Text.Encoding encoding = System.Text.Encoding.BigEndianUnicode;
        //                        byte[] byteArrayContent = encoding.GetBytes(Message);
        //                        CONTENT = "%" + BitConverter.ToString(byteArrayContent).Replace("-", "%");
        //                    }
        //                    else
        //                    {
        //                        CONTENT = Message;
        //                    }

        //                    using (WebClient wc = new WebClient())
        //                    {
        //                        NameValueCollection formData = new NameValueCollection();

        //                        formData["CMD"] = CMD;
        //                        formData["FROM"] = FROM;
        //                        formData["TO"] = TO;
        //                        formData["CHARGE"] = CHARGE;
        //                        formData["CODE"] = CODE;
        //                        formData["CONTENT"] = CONTENT;
        //                        formData["CTYPE"] = CTYPE;
        //                        formData["REPORT"] = REPORT;

        //                        byte[] responseBytes = wc.UploadValues(URL, "POST", formData);
        //                        responseFromServer = Encoding.UTF8.GetString(responseBytes);
        //                        wc.Dispose();

        //                        int xmlStartIndex = responseFromServer.IndexOf("<XML>");
        //                        int xmlLastIndex = responseFromServer.Length - xmlStartIndex;

        //                        XmlDocument xml = new XmlDocument();
        //                        xml.LoadXml(responseFromServer.Substring(xmlStartIndex, xmlLastIndex));
        //                        XmlNodeList xnList = xml.SelectNodes("/XML");

        //                        string STATUS = string.Empty;
        //                        string DETAIL = string.Empty;
        //                        string SMID = string.Empty;

        //                        foreach (XmlNode xn in xnList)
        //                        {
        //                            STATUS = xn["STATUS"].InnerText;
        //                            DETAIL = xn["DETAIL"].InnerText;
        //                            SMID = xn["SMID"].InnerText;
        //                        }

        //                        LogManager.More("SMS to Mobile Number: " + mobilephone + " ,Status: " + STATUS + " ,Details: " + DETAIL + " ,SMID: " + SMID + " ,BatchUser: " + id + " ,Message: " + Message, "SMS");

        //                        if (!STATUS.Equals("OK"))
        //                        {
        //                            EmailSentError(System.Reflection.MethodBase.GetCurrentMethod().Name, "SMS to Mobile Number: " + mobilephone + " ,BatchUser: " + id + " ,Error: " + DETAIL);
        //                        }
        //                    }
        //                }
        //                catch (WebException ex)
        //                {
        //                    sMsgError += "This program is expected to throw WebException on successful run." + "\n\nException Message :" + ex.Message;

        //                    if (ex.Status == WebExceptionStatus.ProtocolError)
        //                    {
        //                        sMsgError += string.Format(" ,Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode);
        //                        sMsgError += string.Format(" ,Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription);
        //                    }

        //                    LogManager.Error("SMS to Mobile Number Error > " + mobilephone + " ,BatchUser: " + id + " ,Error: " + sMsgError);
        //                }
        //                catch (Exception ex)
        //                {
        //                    sMsgError += ex.Message.ToString() + " --> SMS Gateway Response: " + responseFromServer;
        //                    LogManager.Error("SMS to Mobile Number Error > " + mobilephone + " ,BatchUser: " + id + " ,Error: " + sMsgError);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sMsgError += ex.Message.ToString();
        //        LogManager.Error("SMS to Mobile Number Error > " + mobilephone + " ,BatchUser: " + id + " ,Error: " + sMsgError);
        //    }

        //    if (!string.IsNullOrEmpty(sMsgError))
        //    {
        //        EmailSentError(System.Reflection.MethodBase.GetCurrentMethod().Name, "SMS to Mobile Number: " + mobilephone + " ,BatchUser: " + id + " ,Error: " + sMsgError);
        //    }

        //    return sMsgError;
        //}
        #endregion

        public string EmailSentCreateCopon(int id , List<CouponList> coupon)
        {
            string sMsgError = string.Empty;

            try
            {
                if (ConfigurationManager.AppSettings["MailEnable"].Equals("true"))
                {
                    string sHost = ConfigurationManager.AppSettings["MailHost"].ToString();

                    int sPort = Convert.ToInt16(ConfigurationManager.AppSettings["MailPort"].ToString());

                    using (DatabaseContext context = new DatabaseContext())
                    {
                        try
                        {
                            var model = context.BatchUser.Where(u => u.id == id).FirstOrDefault();

                            if (model != null && coupon.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(model.email) && !string.IsNullOrEmpty(model.mobilephone))
                                {
                                    MailMessage myMailMessage = new MailMessage();

                                    string DearName = string.Empty;
                                    string PackageName = coupon[0].package;
                                    string sCouponDetails = string.Empty;

                                    myMailMessage.To.Add(new MailAddress(model.email));

                                    DearName += "คุณ" + model.firstname + " " + model.lastname;

                                    // Obtains the e-mail address of the person sending the message. 
                                    string sMailFrom = ConfigurationManager.AppSettings["MailForm"].ToString();
                                    string sMailFromDisplay = ConfigurationManager.AppSettings["MailFormDisplay"].ToString();
                                    string sMailFromPassword = ConfigurationManager.AppSettings["MailFormPassword"].ToString();
                                    myMailMessage.From = new MailAddress(sMailFrom, sMailFromDisplay);

                                    // Obtains the subject of the e-mail message 
                                    myMailMessage.Subject = "ชื่อผู้ใช้และรหัสผ่านสำหรับใช้งาน Sinet Wifi";

                                    string sBody = "&nbsp;&nbsp;เรียน ลูกค้าผู้มีอุปการะคุณ,<br/>";
                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                                    sBody = sBody + "&nbsp;&nbsp; ขอขอบคุณที่เลือกใช้บริการ Internet Sinet Wifi - Package " + PackageName + " <br/>";
                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;ชื่อผู้ใช้และรหัสผ่านสำหรับใช้งาน Sinet Wifi ของคุณ ถูกส่ง SMS ไปยังเบอร์ " + string.Format("{0}-{1}", model.mobilephone.Substring(0, 3), model.mobilephone.Substring(3,7)) + "<br/>";

                                    sCouponDetails += "<table width=\"380\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse: collapse; border: 1px solid black;\">";
                                    sCouponDetails += "<thead><tr style=\"background-color:#d9d9d9;\"><th style=\"border: 1px solid black;\"><strong>#</strong></th><th  style=\"border: 1px solid black;\"><strong>Username</strong></th><th  style=\"border: 1px solid black;\"><strong>Password</strong></th></tr></thead>";
                                    sCouponDetails += "<tbody>";

                                    int count = 0;
                                    foreach (var item in coupon)
                                    {
                                        count++;
                                        sCouponDetails += "<tr><td width=\"40\" style=\"text-align:center;border: 1px solid black;\">" + count + "</td><td width=\"170\" style=\"text-align:center;border: 1px solid black;\">" + item.username + "</td><td width=\"170\" style=\"text-align:center;border: 1px solid black;\">" + item.password + "</td></tr>";
                                        //sCouponDetails += "&nbsp;&nbsp;" + count + ". Username: " + item.username + "    &nbsp;&nbsp;Password: " + item.password + "<br/>";
                                    }

                                    sCouponDetails += "</tbody></table>";

                                    sBody += sCouponDetails;

                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;ด้วยความนับถือ<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;Sinet FTTx<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;Website : http://www.sinetfttx.com <br/>";
                                    sBody = sBody + "&nbsp;&nbsp;<img src=\"http://www.sinetfttx.com/wp-content/themes/sinet/images/sinet-logo.png\"><br/>";

                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "<br/>";

                                    sBody = sBody + "&nbsp;&nbsp;Dear Valued Customer,<br/>";
                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                                    sBody = sBody + "&nbsp;&nbsp; Thank you for choosing our internet service - Sinet FTTx - Package " + PackageName + " <br/>";
                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;Username and password for your Sinet Wifi were sent by SMS to mobile phone number " + string.Format("{0}-{1}", model.mobilephone.Substring(0, 3), model.mobilephone.Substring(3, 7)) + ". <br/>";

                                    sBody += sCouponDetails;

                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;Sincerely Yours,<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;Sinet FTTx<br/>";
                                    sBody = sBody + "&nbsp;&nbsp;Website : http://www.sinetfttx.com <br/>";
                                    sBody = sBody + "&nbsp;&nbsp;<img src=\"http://www.sinetfttx.com/wp-content/themes/sinet/images/sinet-logo.png\"><br/>";

                                    // Obtains the body of the e-mail message. 
                                    myMailMessage.Body = sBody;

                                    // The default format is Text.                     
                                    myMailMessage.IsBodyHtml = true;
                                    myMailMessage.Priority = MailPriority.High;

                                    SmtpClient myMailClient = new SmtpClient();
                                    myMailClient.Credentials = new System.Net.NetworkCredential(sMailFrom, sMailFromPassword);
                                    myMailClient.Host = sHost;
                                    myMailClient.Port = sPort;
                                    myMailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    object userState = myMailMessage;

                                    try
                                    {
                                        myMailClient.Send(myMailMessage);
                                    }
                                    catch (System.Net.Mail.SmtpException ex)
                                    {
                                        sMsgError = sMsgError + ex.Message;
                                    }

                                    myMailMessage.Dispose();

                                    int order = 0;
                                    string LogCouponDetails = string.Empty;

                                    foreach (var item in coupon)
                                    {
                                        order++;
                                        LogCouponDetails += order + ".Username: " + item.username + " Password: " + item.password + " ";
                                    }

                                    LogManager.More("Mail to " + model.email + " , BatchUser: " + id + " , Coupon: " + LogCouponDetails, "Email");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sMsgError += "Email Sent CreateCopon Error > BatchUser: " + id + " ,Msg: " + ex.Message;
                            LogManager.Error(sMsgError);
                        }
                    }
                }
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                sMsgError += "Email Sent CreateCopon Error > BatchUser: " + id + " ,Msg: " + ex.Message;
                LogManager.Error(sMsgError);
            }
            catch (Exception ex)
            {
                sMsgError += "Email Sent CreateCopon Error > BatchUser: " + id + " ,Msg: " + ex.Message;
                LogManager.Error(sMsgError);
            }

            if (!string.IsNullOrEmpty(sMsgError))
            {
                EmailSentError(System.Reflection.MethodBase.GetCurrentMethod().Name, "BatchUser: " + id + " ,Msg: " + sMsgError);
            }

            return sMsgError;
        }

        public string EmailSentError(string function, string err)
        {
            string sMsgError = string.Empty;

            try
            {
                string sHost = ConfigurationManager.AppSettings["MailHost"].ToString();

                int sPort = Convert.ToInt16(ConfigurationManager.AppSettings["MailPort"].ToString());

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MailTo"].ToString()))
                {
                    MailMessage myMailMessage = new MailMessage();

                    // Obtains the e-mail address of the person the e-mail is being sent to. 
                    string[] aMailTo = ConfigurationManager.AppSettings["MailTo"].ToString().Split(';');
                    for (int NumberOfEmailsTo = 0; NumberOfEmailsTo < aMailTo.Length; NumberOfEmailsTo++)
                    {
                        if (!string.IsNullOrEmpty(aMailTo[NumberOfEmailsTo]))
                            myMailMessage.To.Add(new MailAddress(aMailTo[NumberOfEmailsTo]));
                    }

                    // Obtains the e-mail address of the person the e-mail is being sent Cc. 
                    string[] aMailCc = ConfigurationManager.AppSettings["MailCc"].ToString().Split(';');
                    for (int NumberOfEmailsCc = 0; NumberOfEmailsCc < aMailCc.Length; NumberOfEmailsCc++)
                    {
                        if (!string.IsNullOrEmpty(aMailCc[NumberOfEmailsCc]))
                            myMailMessage.CC.Add(new MailAddress(aMailCc[NumberOfEmailsCc]));
                    }

                    // Obtains the e-mail address of the person the e-mail is being sent Cc. 
                    string[] aMailBcc = ConfigurationManager.AppSettings["MailBcc"].ToString().Split(';');
                    for (int NumberOfEmailsBcc = 0; NumberOfEmailsBcc < aMailBcc.Length; NumberOfEmailsBcc++)
                    {
                        if (!string.IsNullOrEmpty(aMailBcc[NumberOfEmailsBcc]))
                            myMailMessage.Bcc.Add(new MailAddress(aMailBcc[NumberOfEmailsBcc]));
                    }

                    // Obtains the e-mail address of the person sending the message. 
                    string sMailFrom = ConfigurationManager.AppSettings["MailForm"].ToString();
                    string sMailFromDisplay = ConfigurationManager.AppSettings["MailFormDisplay"].ToString();
                    string sMailFromPassword = ConfigurationManager.AppSettings["MailFormPassword"].ToString();
                    myMailMessage.From = new MailAddress(sMailFrom, sMailFromDisplay);

                    // Obtains the subject of the e-mail message 
                    myMailMessage.Subject = "Web Sinet wifi has error !!";

                    string sBody = "&nbsp;&nbsp;เรียน ผู้เกี่ยวข้อง, มีข้อผิดพลาดในการทำงานของ Web Sinet Wifi<br/><br/>";

                    sBody += "&nbsp;&nbsp;รายละเอียดดังนี้<br/>";
                    sBody += "&nbsp;&nbsp;DateTime: " + DateTime.Now + "<br/>";
                    sBody += "&nbsp;&nbsp;Function: " + function + "<br/>";
                    sBody += "&nbsp;&nbsp;Error: " + err + "<br/><br/>";

                    sBody = sBody + "&nbsp;&nbsp;ด้วยความนับถือ<br/>";
                    sBody = sBody + "&nbsp;&nbsp;Sinet FTTx<br/>";
                    sBody = sBody + "&nbsp;&nbsp;Website : http://www.sinetfttx.com <br/>";
                    sBody = sBody + "<br/>";
                    sBody = sBody + "<br/>";
                    sBody = sBody + "<br/>";

                    // Obtains the body of the e-mail message. 
                    myMailMessage.Body = sBody;

                    // The default format is Text.                     
                    myMailMessage.IsBodyHtml = true;
                    myMailMessage.Priority = MailPriority.High;

                    SmtpClient myMailClient = new SmtpClient();
                    myMailClient.Credentials = new System.Net.NetworkCredential(sMailFrom, sMailFromPassword);
                    myMailClient.Host = sHost;
                    myMailClient.Port = sPort;
                    myMailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    object userState = myMailMessage;

                    try
                    {
                        myMailClient.Send(myMailMessage);
                    }
                    catch (System.Net.Mail.SmtpException ex)
                    {
                        sMsgError = sMsgError + ex.Message;
                    }

                    myMailMessage.Dispose();
                }
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                sMsgError += "Sinet Wifi Error > " + ex.Message;
                LogManager.Error(sMsgError);
            }
            catch (Exception ex)
            {
                sMsgError += "Sinet Wifi Error > " + ex.Message;
                LogManager.Error(sMsgError);
            }

            return sMsgError;
        }
        
        public ActionResult TestSMS()
        {
            string sMsgError = string.Empty;

            try
            {
                var ToThaiMobile = new StringBuilder("0882975234");

                string URL = ConfigurationManager.AppSettings["SMSURL"];
                string CMD = ConfigurationManager.AppSettings["SMSCMD"];
                string FROM = ConfigurationManager.AppSettings["SMSFROM"];
                string TO = ToThaiMobile.Remove(0, 1).Insert(0, "66").ToString(); // Example: 66882975234;
                string CHARGE = ConfigurationManager.AppSettings["SMSCHARGE"];
                string CODE = ConfigurationManager.AppSettings["SMSCODE"];
                string CTYPE = ConfigurationManager.AppSettings["SMSCTYPE"]; //TEXT = For English SMS, UNICODE = For Thai SMS
                string REPORT = ConfigurationManager.AppSettings["SMSREPORT"];

                string CONTENT = "ลูกค้ารหัส {1} มีค่าบริการ SINET รอบ {2} ค้างชำระเกินกำหนดแล้ว จำนวน {3} บาท กรุณาชำระที่ Sinet Shop ขอบคุณค่ะ";

                if (CTYPE.Equals("UNICODE"))
                {
                    System.Text.Encoding encoding = System.Text.Encoding.BigEndianUnicode;
                    byte[] byteArrayContent = encoding.GetBytes(CONTENT);
                    CONTENT = "%" + BitConverter.ToString(byteArrayContent).Replace("-", "%");
                }

                using (WebClient wc = new WebClient())
                {
                    NameValueCollection formData = new NameValueCollection();
                    formData["CMD"] = CMD;
                    formData["FROM"] = FROM;
                    formData["TO"] = TO;
                    formData["CHARGE"] = CHARGE;
                    formData["CODE"] = CODE;
                    formData["CONTENT"] = CONTENT;
                    formData["CTYPE"] = CTYPE;
                    formData["REPORT"] = REPORT;

                    byte[] responseBytes = wc.UploadValues(URL, "POST", formData);
                    string responseFromServer = Encoding.UTF8.GetString(responseBytes);
                    wc.Dispose();

                    int xmlStartIndex = responseFromServer.IndexOf("<XML>");
                    int xmlLastIndex = responseFromServer.Length - xmlStartIndex;

                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(responseFromServer.Substring(xmlStartIndex, xmlLastIndex));
                    XmlNodeList xnList = xml.SelectNodes("/XML");

                    string STATUS = string.Empty;
                    string DETAIL = string.Empty;
                    string SMID = string.Empty;

                    foreach (XmlNode xn in xnList)
                    {
                        STATUS = xn["STATUS"].InnerText;
                        DETAIL = xn["DETAIL"].InnerText;
                        SMID = xn["SMID"].InnerText;
                    }
                }
            }
            catch (WebException ex)
            {
                sMsgError += "This program is expected to throw WebException on successful run." + "\n\nException Message :" + ex.Message;

                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    sMsgError += string.Format(" ,Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode);
                    sMsgError += string.Format(" ,Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription);
                }

                LogManager.Error("TestSMS > " + sMsgError);
            }
            catch (Exception ex)
            {
                sMsgError += ex.Message;

                LogManager.Error("TestSMS > " + sMsgError);
            }

            return View();
        }
    }
}
