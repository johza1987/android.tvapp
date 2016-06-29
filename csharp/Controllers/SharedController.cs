using InternetAccountModel.EDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebExtension.Models;
using WebGrease;

namespace WebExtension.Controllers
{
    public class SharedController : BaseController
    {
        private InternetAccountEntities db = new InternetAccountEntities();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult LoadAmphur(string provinceName)
        {
            var jsonResponse = new JsonResponse { status = false, total = 0 };
            try
            {
                Thailand thailand = new Thailand(db);
                var entity = thailand.GetAllAmphurs(provinceName).Select(p => new DropdownEntity()
                {
                    Id = p.AmphurName,
                    Name = p.AmphurName
                }).ToList();
                jsonResponse = new JsonResponse() { status = true, message = "Ok", data = entity };

            }
            catch (Exception ex)
            {
                jsonResponse = new JsonResponse() { status = false, message = ex.Message };
                Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }
            return Json(jsonResponse);
        }

        public JsonResult LoadDistrict(string provinceName, string amphurName)
        {
            var jsonResponse = new JsonResponse { status = false, total = 0 };
            try
            {
                Thailand thailand = new Thailand(db);
                var entity = thailand.GetAllDistricts(provinceName, amphurName).Select(p => new DropdownEntity()
                {
                    Id = p.DistrictName,
                    Name = p.DistrictName
                }).ToList();
                jsonResponse = new JsonResponse() { status = true, message = "Ok", data = entity };

            }
            catch (Exception ex)
            {
                jsonResponse = new JsonResponse() { status = false, message = ex.Message };
                Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }
            return Json(jsonResponse);
        }

        public JsonResult LoadZipcode(string provinceName, string amphurName, string districtName)
        {
            var jsonResponse = new JsonResponse { status = false, total = 0 };
            try
            {
                Thailand thailand = new Thailand(db);
                var zipcode = thailand.GetZipcode(provinceName, amphurName, districtName);
                jsonResponse = new JsonResponse() { status = true, message = "Ok", data = zipcode };

            }
            catch (Exception ex)
            {
                jsonResponse = new JsonResponse() { status = false, message = ex.Message };
                Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }
            return Json(jsonResponse);
        }
    }
}