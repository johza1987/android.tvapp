using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Xml;
using InternetAccountModel.EDM;

namespace WebExtension.Models
{
  
    interface ICustomPrincipal : IPrincipal
    {
        string UserCode { get; set; }
        DateTime ExpDate { get; set; }
        string ClientInfo { get; set; }
    }

    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }

        public string UserCode { get; set; }
        public DateTime ExpDate { get; set; }
        public string ClientInfo { get; set; }
    }

    public class CustomPrincipalSerializeModel
    {
        public string UserCode { get; set; }
        public DateTime ExpDate { get; set; }
        public string ClientInfo { get; set; }
    }

    public class PermissionDetail
    {
        public List<string> Province { get; set; }
        public List<string> Permission { get; set; } 
    }

    public class Authen
    {
        public UserLogon GetUserLogon()
        {
            UserLogon user = null;
            using (var db = new InternetAccountEntities())
            {
                var x = HttpContext.Current.User as CustomPrincipal;
                string str = "";
                if (x != null)
                    str = x.UserCode;

                //string str = System.Web.HttpContext.Current.User.Identity.Name;
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        var entity = db.Users.FirstOrDefault(r => r.UserCode == str);


                        if (entity != null)
                        {
                            var role = db.MasUserGroup.FirstOrDefault(r => r.Code == entity.UserGroupCode);

                            if (role != null)
                            {
                                var branch = db.Branches.FirstOrDefault(c => c.BranchCode == entity.BranchCode);
                                user = new UserLogon()
                                {
                                    UserCode = entity.UserCode,
                                    Username = entity.Username,
                                    FirstName = entity.FirstName,
                                    LastName = entity.LastName,
                                    LastLogin = entity.LastLogin,
                                    LastChangePassword = entity.LastChangePassword,
                                    Disable = entity.Disable,
                                    Default = entity.Default,
                                    UserGroupCode = entity.UserGroupCode,
                                    BranchCode = entity.BranchCode,
                                    Branches = branch,
                                    BranchId = branch != null ? branch.id : 0
                                };

                                if (string.IsNullOrEmpty(role.AccessAreaCode))
                                {
                                    user.AccessAreaCode = new List<string>()
                                    {
                                        user.Branches.AreaCode
                                    };
                                }
                                else if (role.AccessAreaCode == "00")
                                {
                                    user.AccessAreaCode = db.Areas.Select(r => r.AreaCode).ToList();
                                }
                                else
                                {
                                    user.AccessAreaCode = role.AccessAreaCode.Split(',').ToList();
                                }

                                //user.PermissionCode = db.MasUserGroupPermissions.Where(r => r.UserGroupCode == entity.UserGroupCode).Select(r => r.PermissionCode).ToList();

                                user.UserId = entity.UserId;
                                user.UserCode = entity.UserCode;
                                user.Username = entity.Username;
                       
                                user.RoleId = entity.RoleId;
                                user.FirstName = entity.FirstName;
                                user.LastName = entity.LastName;
                                user.EmailAddress = entity.EmailAddress;
                                user.PhoneNo = entity.PhoneNo;
                                user.Photo = entity.PhoneNo;
                                user.BranchId = entity.BranchId;
                                user.Branches = db.Branches.FirstOrDefault(r => r.BranchCode == entity.BranchCode);
                                user.BranchCode = entity.BranchCode;
                                user.Default = entity.Default;
                                user.Disable = entity.Disable;
                                user.LastLogin = entity.LastLogin;
                                user.LastChangePassword = entity.LastChangePassword;
                                user.EnableSetting = entity.EnableSetting;
                                user.UserGroupCode = entity.UserGroupCode;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
                    }
                }
            }
            return user;
        }

        public PermissionDetail GetPermission(string userCode = null)
        {
            PermissionDetail permission = null;
            using (var db = new InternetAccountEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(userCode))
                    {
                        var x = HttpContext.Current.User as CustomPrincipal;
                        if (x != null)
                            userCode = x.UserCode;
                    }

                    var group = (from t1 in db.Users
                                 join t2 in db.MasUserGroup on t1.UserGroupCode equals t2.Code
                                 where t1.UserCode == userCode
                                 select t2.AccessAreaCode).FirstOrDefault();

                    var province = new List<string>();
                    if (group != null)
                    {
                        if (group.Contains("00"))
                        {
                            province = db.Areas.Select(c => c.AreaCode).ToList();
                        }
                        else
                        {
                            province = group.Split(',').ToList();
                        }
                    }
                    else
                    {
                        province = (from t1 in db.Users
                                    join t2 in db.Branches on t1.BranchCode equals t2.BranchCode
                                    join t3 in db.Areas on t2.AreaCode equals t3.AreaCode
                                    where t1.UserCode == userCode
                                    where t1.Disable == false
                                    select t3.AreaCode).ToList();
                    }

                    var per = (from t1 in db.Users
                               join t2 in db.MasUserGroupPermissions on t1.UserGroupCode equals t2.UserGroupCode
                               where t1.UserCode == userCode
                               select t2.PermissionCode).ToList();
                    permission = new PermissionDetail() { Province = province, Permission = per };

                }
                catch (Exception ex)
                {
                    Log.Error(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " Error -> " + ex.Message);
                }
            }
            return permission;
        }
    }

}