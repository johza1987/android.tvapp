using FTTxModel.EDM;
using InternetAccountModel.EDM;
using OfficeOpenXml;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WebExtension.Common;
using WebExtension.Models;

namespace WebExtension.Controllers
{
    public class CustomerController : BaseController 
    {
        public ActionResult Index()
        {
            try
            {
                var permission = UserPermission;
                using (var db = new InternetAccountEntities())
                {
                    var area = db.Areas.Where(r => permission.Province.Contains(r.AreaCode)).ToList();

                    ViewBag.Area = area;

                    var thailand = new Thailand(db);
                    var province = thailand.GetAllProvince();
                    string selectProvince = (area != null ? area[0].AreaName : province[0].ProvinceName);
                    if (!province.Exists(p => p.ProvinceName == selectProvince))
                    {
                        selectProvince = province[0].ProvinceName;
                    }
                    if (province.Count > 0)
                    {
                        var amphur = thailand.GetAllAmphurs(selectProvince);
                        var district = thailand.GetAllDistricts(selectProvince, amphur[0].AmphurName);
                        ViewData["_Province"] = province.Select(p => new DropdownEntity
                        {
                            Id = p.ProvinceCode.ToString(),
                            Name = p.ProvinceName,
                            Selected = (p.ProvinceName == selectProvince ? true : false)
                        }).ToList();
                        ViewData["_Amphur"] = amphur.Select(p => new DropdownEntity
                        {
                            Id = p.AmphurName,
                            Name = p.AmphurName
                        }).ToList();
                        ViewData["_District"] = district.Select(p => new DropdownEntity
                        {
                            Id = p.DistrictName,
                            Name = p.DistrictName
                        }).ToList();
                        ViewData["_Zipcode"] =
                            thailand.GetZipcode(selectProvince, amphur[0].AmphurName, district[0].DistrictName)
                                .ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }
            return View();
        }       
        public JsonResult CustomerSave(BusinessPartnerModel model, int mode = 0)
        {
            var jsonReturn = new JsonResponse();
            using (var db = new FTTxEntities())
            {
                try
                {
                    string responseMessage = String.Empty;
                    var sap = new FTTx();

                    model.Cellular = new StringBuilder(System.Text.RegularExpressions.Regex.Replace(model.Cellular, "[^0-9]", "")).ToString();

                    if (mode == 0)
                    {
                        string prefix = "C";
                        int newCode = 1;
                        var lastCardCode = db.OCRD.Where(r => r.CardCode.Substring(0, 3) == prefix + model.ProvinceId).OrderByDescending(r => r.CardCode.Substring(3)).Select(r => r.CardCode.Substring(3)).FirstOrDefault();
                        if (lastCardCode != null)
                            newCode = int.Parse(lastCardCode) + 1;

                        model.CustomerCode = string.Format("{0}{1}{2}", prefix, model.ProvinceId, newCode.ToString().PadLeft(6, '0'));

                        sap.Connecting(out responseMessage);
                        if (sap.company.Connected)
                        {
                            SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)sap.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                            bp.GroupCode = 103;
                            bp.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                            bp.CardCode = model.CustomerCode;
                            bp.CardName = model.CustomerName;
                            bp.FederalTaxID = model.CardNo;
                            bp.Frozen =  model.FrozenFor == "Y" ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;
                            bp.Address = model.Address;
                            bp.Block = model.District;
                            bp.County = model.Amphur;
                            bp.City = model.ProvinceName;
                            bp.ZipCode = model.Zipcode;
                            bp.Cellular = model.Cellular;
                            bp.EmailAddress = model.Email;

                            bp.MailAddress = model.Address;
                            bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
                            bp.Addresses.AddressName = "Ship to";
                            bp.Addresses.Block = model.District;
                            bp.Addresses.County = model.Amphur;
                            bp.Addresses.City = model.ProvinceName;
                            bp.Addresses.ZipCode = model.Zipcode;
                            if (!string.IsNullOrEmpty(model.Cellular))
                                bp.UserFields.Fields.Item("U_InsTel").Value = model.Cellular;

                            if (!string.IsNullOrEmpty(model.Address))
                                bp.UserFields.Fields.Item("U_StmAddr").Value = bp.Address;
                            if (!string.IsNullOrEmpty(model.District))
                                bp.UserFields.Fields.Item("U_StmBlck").Value = model.District;
                            if (!string.IsNullOrEmpty(model.Amphur))
                                bp.UserFields.Fields.Item("U_StmCnty").Value = model.Amphur;
                            if (!string.IsNullOrEmpty(model.ProvinceName))
                                bp.UserFields.Fields.Item("U_StmCity").Value = model.ProvinceName;
                            if (!string.IsNullOrEmpty(model.Zipcode))
                                bp.UserFields.Fields.Item("U_StmZipC").Value = model.Zipcode;
                            if (!string.IsNullOrEmpty(model.Cellular))
                                bp.UserFields.Fields.Item("U_Bphone").Value = model.Cellular;

                            if (0 != bp.Add())
                            {
                                jsonReturn = new JsonResponse { status = false, message = sap.company.GetLastErrorDescription() };
                            }
                            else
                            {
                                jsonReturn = new JsonResponse { status = true, message = "บันทึกข้อมูลเรียบร้อยแล้ว" };
                            }

                            sap.company.Disconnect();
                        }
                        else
                        {
                            jsonReturn = new JsonResponse() { status = false, message = responseMessage };
                        }
                    }
                    else
                    {
                        sap.Connecting(out responseMessage);
                        if (sap.company.Connected)
                        {
                            SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)sap.company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                            bp.GetByKey(model.CustomerCode);

                            bp.CardName = model.CustomerName;
                            bp.FederalTaxID = model.CardNo;
                            bp.Frozen = model.FrozenFor == "Y" ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;
                            bp.Address = model.Address;
                            bp.Block = model.District;
                            bp.County = model.Amphur;
                            bp.City = model.ProvinceName;
                            bp.ZipCode = model.Zipcode;
                            bp.Cellular = model.Cellular;
                            bp.EmailAddress = model.Email;

                            bp.MailAddress = model.Address;
                            bp.Addresses.Block = model.District;
                            bp.Addresses.County = model.Amphur;
                            bp.Addresses.City = model.ProvinceName;
                            bp.Addresses.ZipCode = model.Zipcode;
                            if (!string.IsNullOrEmpty(model.Cellular))
                                bp.UserFields.Fields.Item("U_InsTel").Value = model.Cellular;

                            if (!string.IsNullOrEmpty(model.Address))
                                bp.UserFields.Fields.Item("U_StmAddr").Value = bp.Address;
                            if (!string.IsNullOrEmpty(model.District))
                                bp.UserFields.Fields.Item("U_StmBlck").Value = model.District;
                            if (!string.IsNullOrEmpty(model.Amphur))
                                bp.UserFields.Fields.Item("U_StmCnty").Value = model.Amphur;
                            if (!string.IsNullOrEmpty(model.ProvinceName))
                                bp.UserFields.Fields.Item("U_StmCity").Value = model.ProvinceName;
                            if (!string.IsNullOrEmpty(model.Zipcode))
                                bp.UserFields.Fields.Item("U_StmZipC").Value = model.Zipcode;
                            if (!string.IsNullOrEmpty(model.Cellular))
                                bp.UserFields.Fields.Item("U_Bphone").Value = model.Cellular;

                            if (0 != bp.Update())
                            {
                                jsonReturn = new JsonResponse { status = false, message = sap.company.GetLastErrorDescription() };
                            }
                            else
                            {
                                jsonReturn = new JsonResponse { status = true, message = "บันทึกข้อมูลเรียบร้อยแล้ว" };
                            }

                            sap.company.Disconnect();
                        }
                        else
                        {
                            jsonReturn = new JsonResponse() { status = false, message = responseMessage };
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonReturn = new JsonResponse { status = false, message = ex.Message };
                    Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
                }
            }
            return Json(jsonReturn);
        }
        public JsonResult CustomerListLoad(DatableOption option, FilterCustomer model)
        {
            var jsonReturn = new JsonDataTable();

            try
            {
                using (var db = new FTTxEntities())
                {
                    var endDate = DateTime.Now.Date;
                    if (model.EndDate != null)
                        endDate = model.EndDate.Value.AddDays(1);

                    var query = (from t1 in db.OCRD
                                 where (model.ProvinceCode.Count == 0 || model.ProvinceCode.Contains(t1.CardCode.Substring(1, 2)))
                                 where (model.StartDate == null || t1.CreateDate >= model.StartDate)
                                 where (model.EndDate == null || t1.CreateDate < endDate)
                                 where (model.Status.Count == 0 || model.Status.Contains(t1.frozenFor))
                                 where t1.GroupCode == 103
                                 where (string.IsNullOrEmpty(model.Query)
                                 || (t1.CardCode.Contains(model.Query))
                                 || (t1.CardName.Contains(model.Query))
                                 || (t1.Cellular.Contains(model.Query))
                                 || (t1.E_Mail.Contains(model.Query))
                                 )
                                 select new 
                                 {
                                     CardCode = t1.CardCode ?? "",
                                     CardName = t1.CardName ?? "",
                                     CardNo= t1.LicTradNum ?? "",
                                     Cellular = t1.Cellular ?? "",
                                     CreateDate = t1.CreateDate,
                                     EMail = t1.E_Mail ?? "",
                                     Address = t1.Address ?? "",
                                     District = t1.Block ?? "",
                                     Amphur = t1.County ?? "",
                                     Province = t1.City ?? "",
                                     ZipCode = t1.ZipCode ?? "",
                                     Status = t1.frozenFor ?? ""
                                 });

                    if (option.order.Count > 0)
                    {
                        var sorting = int.Parse(option.order[0].FirstOrDefault(r => r.Key == "column").Value);
                        var dir = option.order[0].FirstOrDefault(r => r.Key == "dir").Value;

                        switch (sorting)
                        {
                            case 1: query = (dir.ToLower() == "asc" ? query.OrderBy(r => r.CardName) : query.OrderByDescending(r => r.CardName)); break;
                            case 2: query = (dir.ToLower() == "asc" ? query.OrderBy(r => r.Cellular) : query.OrderByDescending(r => r.Cellular)); break;
                            case 3: query = (dir.ToLower() == "asc" ? query.OrderBy(r => r.EMail) : query.OrderByDescending(r => r.EMail)); break;
                            case 5: query = (dir.ToLower() == "asc" ? query.OrderBy(r => r.CreateDate) : query.OrderByDescending(r => r.CreateDate)); break;
                            case 6: query = (dir.ToLower() == "asc" ? query.OrderBy(r => r.Status) : query.OrderByDescending(r => r.Status)); break;
                            default: query = (dir.ToLower() == "asc" ? query.OrderBy(r => r.CardCode) : query.OrderByDescending(r => r.CardCode)); break;
                        }
                    }

                    var data = query.Skip(option.start).Take(option.length).ToList();
                    var count = query.Count();

                    jsonReturn = new JsonDataTable { status = true, message = "Ok", data = data, draw = option.draw, recordsTotal = count, recordsFiltered = count };
                }
            }
            catch (Exception ex)
            {
                jsonReturn = new JsonDataTable { status = false, message = ex.Message, data = new string[0], draw = option.draw, recordsTotal = 0 };
                Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }

            return Json(jsonReturn);
        }
        public ActionResult CustomerExport(FilterCustomer model)
        {
            try
            {
                using (var db = new FTTxEntities())
                {
                    var endDate = DateTime.Now.Date;
                    if (model.EndDate != null)
                        endDate = model.EndDate.Value.AddDays(1);

                    var query = (from t1 in db.OCRD
                                 where (model.ProvinceCode.Count == 0 || model.ProvinceCode.Contains(t1.CardCode.Substring(1, 2)))
                                 where (model.StartDate == null || t1.CreateDate >= model.StartDate)
                                 where (model.EndDate == null || t1.CreateDate < endDate)
                                 where (model.Status.Count == 0 || model.Status.Contains(t1.frozenFor))
                                 where t1.GroupCode == 103
                                 where (string.IsNullOrEmpty(model.Query)
                                 || (t1.CardCode.Contains(model.Query))
                                 || (t1.CardName.Contains(model.Query))
                                 || (t1.Cellular.Contains(model.Query))
                                 || (t1.E_Mail.Contains(model.Query))
                                 )
                                 select new
                                 {
                                     CardCode = t1.CardCode ?? "",
                                     CardName = t1.CardName ?? "",
                                     CardNo = t1.LicTradNum ?? "",
                                     Cellular = t1.Cellular ?? "",
                                     CreateDate = t1.CreateDate,
                                     EMail = t1.E_Mail ?? "",
                                     Address = t1.Address ?? "",
                                     District = t1.Block ?? "",
                                     Amphur = t1.County ?? "",
                                     Province = t1.City ?? "",
                                     ZipCode = t1.ZipCode ?? "",
                                     Status = t1.frozenFor ?? ""
                                 });

                    var entity = query.ToList();

                    if (entity.Count > 0)
                    {
                        System.IO.MemoryStream streamExcel = new System.IO.MemoryStream();

                        using (var excel = new ExcelPackage(streamExcel))
                        {
                            System.IO.MemoryStream memStream = new System.IO.MemoryStream();

                            var sheets1 = excel.Workbook.Worksheets.Add("Sheet1");

                            sheets1.Cells[1, 1, 1, 1].Value = "รายชื่อลูกค้า";

                            string[] fieldHeader1 = { "รหัสลูกค้า", "ชื่อลูกค้า", "เบอร์โทรศัพท์", "Email", "ที่อยู่", "วันที่สร้างลูกค้า" ,"บัตรประชาชน/พาสปอร์ต/เลขที่นิติบุคคล", "สถานะ"};

                            for (int i = 0; i < fieldHeader1.Length; i++)
                            {
                                sheets1.Cells[3, i + 1].Value = fieldHeader1[i];
                            }

                            int startRows = 3;

                            foreach (var item in entity)
                            {
                                sheets1.Cells[startRows + 1, 1].Value = item.CardCode;
                                sheets1.Cells[startRows + 1, 2].Value = item.CardName;
                                sheets1.Cells[startRows + 1, 3].Value = item.Cellular;
                                sheets1.Cells[startRows + 1, 4].Value = item.EMail;
                                sheets1.Cells[startRows + 1, 5].Value = string.Format("{0} {1} {2} {3} {4}", item.Address ?? "", item.District ?? "", item.Amphur ?? "", item.Province ?? "", item.ZipCode ?? "");
                                sheets1.Cells[startRows + 1, 6].Value = item.CreateDate != null ? item.CreateDate.Value.ToString("dd/MM/yyyy") : "";
                                sheets1.Cells[startRows + 1, 7].Value = item.CardNo;
                                sheets1.Cells[startRows + 1, 8].Value = item.Status == "Y" ? "Active" : "Inactive";
                                startRows++;
                            }

                            for (int i = 2; i <= fieldHeader1.Length; i++)
                            {
                                sheets1.Column(i).AutoFit();
                            }

                            excel.Save();
                        }

                        string attachment = "attachment; filename=CustomerExport-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xls";
                        Response.ClearContent();
                        Response.AddHeader("content-disposition", attachment);
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.Charset = "";
                        streamExcel.WriteTo(Response.OutputStream);
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }

            return new EmptyResult();
        }
    }
}