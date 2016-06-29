using Newtonsoft.Json;
using SinetWifi.Common;
using SinetWifi.Controllers;
using SinetWifi.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace SinetWifi.Areas.Admin.Controllers
{
    public class SMSController : BaseController
    {
        public ActionResult Message()
        {
            //string path = HttpContext.Server.MapPath(ConfigurationManager.AppSettings["SMSTemplate"].ToString()); 

            //if (!System.IO.File.Exists(path))
            //{
            //    System.IO.File.WriteAllText(path, string.Empty);
            //}
            //else
            //{
            //    string data = System.IO.File.ReadAllText(path);
            //    var result = JsonConvert.DeserializeObject<List<SMSTemplate>>(data);

            //    ViewData["SMSTemplate"] = result;
            //}

            return RedirectToAction("Coupon", "Report");
        }

        [HttpPost]
        public JsonResult Create(SMSTemplate model)
        {
            JsonResponse json = new JsonResponse { Status = false };

            if (ModelState.IsValid)
            {
                string path = HttpContext.Server.MapPath(ConfigurationManager.AppSettings["SMSTemplate"].ToString()); 

                var result = JsonConvert.DeserializeObject<List<SMSTemplate>>(System.IO.File.ReadAllText(path));

                if (result != null)
                {
                    result.Add(new SMSTemplate { id = result.Count + 1, title = model.title, message = model.message });
                }
                else
                {
                    result = new List<SMSTemplate>();
                    result.Add(new SMSTemplate { id = 1, title = model.title, message = model.message });
                }

                System.IO.File.WriteAllText(path, new JavaScriptSerializer().Serialize(result));

                json.Status = true;
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
        public JsonResult Delete(int id)
        {
            JsonResponse json = new JsonResponse { Status = false };

            string MsgError = string.Empty;

            string path = HttpContext.Server.MapPath(ConfigurationManager.AppSettings["SMSTemplate"].ToString());

            var result = JsonConvert.DeserializeObject<List<SMSTemplate>>(System.IO.File.ReadAllText(path));

            var list = new List<SMSTemplate>();

            if (result != null)
            {
                int i = 1;

                foreach (var item in result)
                {
                    if (item.id != id)
                    {
                        item.id = i;
                        list.Add(item);
                        i++;
                    }
                }

                json.Status = true;

                System.IO.File.WriteAllText(path, new JavaScriptSerializer().Serialize(list));
            }

            json.Message = Shared.GetMsg(json.Status, json.Message);

            return Json(json);
        }

        [HttpPost]
        public JsonResult Update(SMSTemplate model)
        {
            JsonResponse json = new JsonResponse { Status = false };

            string MsgError = string.Empty;

            string path = HttpContext.Server.MapPath(ConfigurationManager.AppSettings["SMSTemplate"].ToString());

            var result = JsonConvert.DeserializeObject<List<SMSTemplate>>(System.IO.File.ReadAllText(path));

            var list = new List<SMSTemplate>();

            if (result != null)
            {
                foreach (var item in result)
                {
                    if (item.id == model.id)
                    {
                        item.title = model.title;
                        item.message = model.message;
                    }
                }

                json.Status = true;

                System.IO.File.WriteAllText(path, new JavaScriptSerializer().Serialize(result));
            }

            json.Message = Shared.GetMsg(json.Status, json.Message);

            return Json(json);
        }

        [HttpPost]
        public WrappedJsonResult ImportFile(HttpPostedFileWrapper file, string optSmsTemplate)
        {
            JsonResponse json = new JsonResponse { Status = false };

            try
            {
                if (file != null || file.ContentLength > 0)
                {
                    int MaxContentLength = 1024 * 1024 * 3; //3 MB
                    string[] AllowedFileExtensions = new string[] { ".csv" };
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);

                    if (!AllowedFileExtensions.Contains(fileExtension))
                    {
                        json.Message = "Please select file of type : " + string.Join(", ", AllowedFileExtensions);
                    }
                    else if (file.ContentLength > MaxContentLength)
                    {
                        json.Message = "Maximum allowed file size is " + MaxContentLength + " bytes";
                    }
                    else
                    {
                        string TempFolderPath = "~/Temp/";

                        string fileLocation = string.Format("{0}/{1}", Server.MapPath(TempFolderPath), file.FileName);

                        if (System.IO.File.Exists(fileLocation))
                            System.IO.File.Delete(fileLocation);

                        if (!Directory.Exists(Server.MapPath(TempFolderPath)))
                            Directory.CreateDirectory(Server.MapPath(TempFolderPath));

                        file.SaveAs(fileLocation);

                        // Load data
                        DataTable data = new DataTable();

                        CSVReader csv = new CSVReader();

                        data = csv.GetDataTable(fileLocation , false);

                        List<SMSModel> list = new List<SMSModel>();

                        foreach (DataRow r in data.Rows)
                        {
                            string sMessage = string.Empty;
                            string sMsgErr = string.Empty;

                            int number = 0;

                            if (string.IsNullOrEmpty(r[0].ToString()))
                                sMsgErr += " หมายเลขโทรศัพท์ต้องไม่เป็นค่าว่าง";
                            else if (!int.TryParse(r[0].ToString(), out number))
                                sMsgErr += " หมายเลขโทรศัพท์ต้องเป็นตัวเลขเท่านั้น";
                            else if (r[0].ToString().Length != 10)
                                sMsgErr += " หมายเลขโทรศัพท์ต้องเป็นตัวเลขจำนวน 10 หลัก เท่านั้น";

                            try
                            {
                                sMessage = String.Format(optSmsTemplate, r.ItemArray);
                            }
                            catch
                            {
                                sMsgErr += " จำนวนพารามิเตอร์ {parameter} ในข้อความ ต้องมีจำนวนน้อยกว่าหรือเท่ากับจำนวนคอลัมน์ข้อมูลใน Excel";
                            }

                            list.Add(new SMSModel { phonenumber = r[0].ToString(), message = sMessage, error = sMsgErr });
                        }

                        if (System.IO.File.Exists(fileLocation))
                        {
                            try
                            {
                                System.IO.File.Delete(fileLocation);
                            }
                            catch (System.IO.IOException e)
                            {
                                json.Message = "ERROR import excel file : " + e.Message;
                            }
                        }

                        json.Data = list;

                        json.Status = true;
                    }
                }
                else
                {
                    json.Message = "Plese select Excel File.";
                }
            }
            catch (Exception ex)
            {
                json = new JsonResponse { Status = false, Message = ex.Message, Total = 0 };
            }

            json.Message = Shared.GetMsg(json.Status, json.Message);

            return new WrappedJsonResult
            {
                Data = Json(json)
            };
        }

        #region Send SMS with AIS Gateway
        //[HttpPost]
        //public JsonResult SendMessage(List<SMSModel> list)
        //{
        //    JsonResponse json = new JsonResponse { Status = false };

        //    string sMsgError = string.Empty;

        //    try
        //    {
        //        if (ConfigurationManager.AppSettings["SMSEnable"].Equals("true"))
        //        {                   
        //            bool SMS_ERROR = false;
        //            string URL = ConfigurationManager.AppSettings["SMSURL"];
        //            string CMD = ConfigurationManager.AppSettings["SMSCMD"];
        //            string FROM = ConfigurationManager.AppSettings["SMSFROM"];
        //            string CHARGE = ConfigurationManager.AppSettings["SMSCHARGE"];
        //            string CODE = ConfigurationManager.AppSettings["SMSCODE"];
        //            string CTYPE = ConfigurationManager.AppSettings["SMSCTYPE"]; //TEXT = For English SMS, UNICODE = For Thai SMS
        //            string REPORT = ConfigurationManager.AppSettings["SMSREPORT"];
 
        //            foreach (var item in list)
        //            {
        //                StringBuilder ToThaiMobile = new StringBuilder(item.phonenumber);

        //                string TO = ToThaiMobile.Remove(0, 1).Insert(0, "66").ToString(); // Example: 66882975234;

        //                string CONTENT = item.message;

        //                if (CTYPE.Equals("UNICODE"))
        //                {
        //                    System.Text.Encoding encoding = System.Text.Encoding.BigEndianUnicode;
        //                    byte[] byteArrayContent = encoding.GetBytes(CONTENT);
        //                    CONTENT = "%" + BitConverter.ToString(byteArrayContent).Replace("-", "%");
        //                }

        //                using (WebClient wc = new WebClient())
        //                {
        //                    NameValueCollection formData = new NameValueCollection();

        //                    formData["CMD"] = CMD;
        //                    formData["FROM"] = FROM;
        //                    formData["TO"] = TO;
        //                    formData["CHARGE"] = CHARGE;
        //                    formData["CODE"] = CODE;
        //                    formData["CONTENT"] = CONTENT;
        //                    formData["CTYPE"] = CTYPE;
        //                    formData["REPORT"] = REPORT;

        //                    byte[] responseBytes = wc.UploadValues(URL, "POST", formData);
        //                    //string responseFromServer = "E:CONNECTION";
        //                    string responseFromServer = Encoding.UTF8.GetString(responseBytes);
        //                    wc.Dispose();
        //                    int xmlStartIndex = responseFromServer.IndexOf("<XML>");
        //                    int xmlLastIndex = responseFromServer.Length - xmlStartIndex;

        //                    try
        //                    {
        //                        XmlDocument xml = new XmlDocument();
        //                        xml.LoadXml(responseFromServer.Substring(xmlStartIndex, xmlLastIndex));
        //                        XmlNodeList xnList = xml.SelectNodes("/XML");

        //                        foreach (XmlNode xn in xnList)
        //                        {
        //                            item.results = new SMSSendResult();
        //                            item.results.status = xn["STATUS"].InnerText;
        //                            item.results.details = xn["DETAIL"].InnerText;
        //                            item.results.smid = xn["SMID"].InnerText;
        //                        }

        //                        LogManager.More("SMS to Mobile Number: " + item.phonenumber + " ,Status: " + item.results.status + " ,Details: " + item.results.details + " ,SMID: " + item.results.smid + " ,Message: " + item.message, "ImportSMS");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        SMS_ERROR = true;
        //                        sMsgError = "ไม่สามารถส่งข้อความได้ กรุณาติดต่อผู้ดูแลเว็ปไซต์ --> Response: " + responseFromServer + ", Exception: " + ex.Message;
        //                        LogManager.Error("SMS.SendMessage Error -> " + sMsgError);
        //                        break;
        //                    }
        //                }
        //            }

        //            if (SMS_ERROR == true)
        //            {
        //                json.Status = false;
        //                json.Message = sMsgError;
        //            }
        //            else
        //            {
        //                json.Status = true;
        //                json.Data = list;
        //                json.Message = "ส่งข้อความ จำนวน " + list.Count + " ข้อความ เรียบร้อยแล้ว !!";
        //            }
        //        }
        //        else
        //        {
        //            json.Status = false;
        //            json.Message = "Function send message disabled. You have to enable in config.";
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        sMsgError += "This program is expected to throw WebException on successful run." + "\n\nException Message :" + ex.Message;

        //        if (ex.Status == WebExceptionStatus.ProtocolError)
        //        {
        //            sMsgError += string.Format(" ,Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode);
        //            sMsgError += string.Format(" ,Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription);
        //        }

        //        json.Status = false;
        //        json.Message = sMsgError;

        //        LogManager.Error("SMS.SendMessage Error > " + sMsgError);
        //    }
        //    catch (Exception ex)
        //    {
        //        sMsgError += ex.Message;

        //        json.Status = false;
        //        json.Message = sMsgError;

        //        LogManager.Error("SMS.SendMessage Error > " + sMsgError);
        //    }

        //    json.Message = Shared.GetMsg(json.Status, json.Message);

        //    return Json(json);
        //}
        #endregion

        #region Send SMS with MailBit Gateway
        [HttpPost]
        public JsonResult SendMessage(List<SMSModel> list)
        {
            JsonResponse json = new JsonResponse { Status = false };
            string sMsgError = string.Empty;
            try
            {
                if (ConfigurationManager.AppSettings["SMSEnable"].Equals("true"))
                {
                    bool SMS_ERROR = false;
                    string URL = ConfigurationManager.AppSettings["SMSMAILBITURL"];
                    string USER = ConfigurationManager.AppSettings["SMSMAILBITUSER"];
                    string PASS = ConfigurationManager.AppSettings["SMSMAILBITPASS"];
                    string SENDERID = ConfigurationManager.AppSettings["SMSMAILBITSENDERID"];

                    foreach (var item in list)
                    {
                        StringBuilder ToThaiMobile = new StringBuilder(item.phonenumber);
                        string TO = ToThaiMobile.Remove(0, 1).Insert(0, "66").ToString(); // Example: 66882975234;
                        string CONTENT = item.message;
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
                            var data = new JavaScriptSerializer().Deserialize<SMSMailBITResponse>(res);
                            item.results = new SMSSendResult();
                            item.results.status = data.ErrorCode == "000" ? "OK" : "ERROR";
                            item.results.details = data.ErrorMessage;
                            item.results.smid = data.JobId;
                            LogManager.More("SMS to Mobile Number: " + item.phonenumber + " ,Result: " + data.ErrorMessage + " ,Message: " + item.message + " ,SMSGateWayResponse: " + res, "ImportSMS");
                        }
                        catch (Exception ex)
                        {
                            SMS_ERROR = true;
                            sMsgError = "ไม่สามารถส่งข้อความได้ กรุณาติดต่อผู้ดูแลเว็ปไซต์ --> Response: " + res + ", Exception: " + ex.Message;
                            LogManager.Error("SMS.SendMessage Error -> " + sMsgError);
                            break;
                        }
                    }

                    if (SMS_ERROR == true)
                    {
                        json.Status = false;
                        json.Message = sMsgError;
                    }
                    else
                    {
                        json.Status = true;
                        json.Data = list;
                        json.Message = "ส่งข้อความ จำนวน " + list.Count + " ข้อความ เรียบร้อยแล้ว !!";
                    }
                }
                else
                {
                    json.Status = false;
                    json.Message = "Function send message disabled. You have to enable in config.";
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

                json.Status = false;
                json.Message = sMsgError;

                LogManager.Error("SMS.SendMessage Error > " + sMsgError);
            }
            catch (Exception ex)
            {
                sMsgError += ex.Message;

                json.Status = false;
                json.Message = sMsgError;

                LogManager.Error("SMS.SendMessage Error > " + sMsgError);
            }

            json.Message = Shared.GetMsg(json.Status, json.Message);

            return Json(json);
        }
        #endregion
    }
}
