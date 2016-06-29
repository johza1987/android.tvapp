using InternetAccountModel.EDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebExtension.Models
{
    public class Thailand
    {
        private InternetAccountEntities db = null;

        public Thailand(InternetAccountEntities databaseContext)
        {
            this.db = databaseContext;
        }

        public List<MasThProvince> GetAllProvince()
        {
            var list = new List<MasThProvince>();
            try
            {
                list = db.MasThProvince.OrderBy(p => p.ProvinceName).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }
            return list;
        }

        public MasThProvince GetProvince(string provinceName)
        {
            MasThProvince list = null;
            try
            {
                list = db.MasThProvince.FirstOrDefault(p => p.ProvinceName == provinceName);
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }
            return list;
        }

        public List<MasThAmphur> GetAllAmphurs(string provinceName)
        {
            var list = new List<MasThAmphur>();
            try
            {
                list = (from p in db.MasThProvince
                        join a in db.MasThAmphur on p.ProvinceID equals a.ProvinceID
                        where p.ProvinceName == provinceName && !a.AmphurName.Contains("*")
                        orderby a.AmphurName
                        select a).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }
            return list;
        }

        public List<MasThDistrict> GetAllDistricts(string provinceName, string amphurName)
        {
            var list = new List<MasThDistrict>();
            try
            {
                list = (from p in db.MasThProvince
                        join a in db.MasThAmphur on p.ProvinceID equals a.ProvinceID
                        join d in db.MasThDistrict on a.AmphurID equals d.AmphurID
                        where p.ProvinceName == provinceName && a.AmphurName == amphurName && !d.DistrictName.Contains("*")
                        orderby d.DistrictName
                        select d).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }
            return list;
        }

        public string GetZipcode(string provinceName, string amphurName, string districtName)
        {
            string zip = string.Empty;
            try
            {
                zip = (from p in db.MasThProvince
                       join a in db.MasThAmphur on p.ProvinceID equals a.ProvinceID
                       join d in db.MasThDistrict on a.AmphurID equals d.AmphurID
                       join z in db.MasThZipcode on d.DistrictCode equals z.DistrictCode
                       where p.ProvinceName == provinceName && a.AmphurName == amphurName && d.DistrictName == districtName
                       select z.Zipcode).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(MethodBase.GetCurrentMethod().DeclaringType.FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
            }

            return zip;
        }
    }
}