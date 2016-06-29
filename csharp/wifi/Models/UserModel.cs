using SinetWifi.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace SinetWifi.Models
{
    [Table("operators")]
    public class Operator
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
    
    public class Login
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class UserOnline
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Location { get; set; }
        public string CustomerCode { get; set; }
        public int HotspotId { get; set; }

        public static UserOnline GetUserData(HttpRequest Request)
        {
            /*
            ***** Example Get Obeject *****
            UserOnline user = UserOnline.GetUserData(Request);
            user.UserName.ToString();
            user.Name.ToString();
            user.Role.ToString();

            ***** Example Attribute Obeject *****
            UserOnline.GetUserData(Request).UserName.ToString();*/
            
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        var decrypted = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);

                        UserOnline userData = new JavaScriptSerializer().Deserialize<UserOnline>(decrypted.UserData);

                        userData.Username = System.Web.HttpContext.Current.User.Identity.Name.ToString();

                        using (DatabaseContext context = new DatabaseContext())
                        {
                            if (userData.Role.Equals(Shared.RoleName.Admintstrator, StringComparison.OrdinalIgnoreCase))
                            {
                                var admin = context.Operator.Where(u => u.username.Equals(userData.Username)).FirstOrDefault();
                                userData.FullName = admin.firstname + " " + admin.lastname;
                                userData.ID = admin.id;
                                userData.Role = Shared.RoleName.Admintstrator;
                            }
                            else
                            {
                                var user = context.HotspotUser.Where(u => u.username.Equals(userData.Username)).FirstOrDefault();
                                userData.FullName = user.fullname;
                                userData.ID = user.id;
                                userData.Location = user.Hotspot.type;
                                userData.CustomerCode = user.Hotspot.code;
                                userData.HotspotId = user.Hotspot.id;
                                userData.Role = Shared.RoleName.StandardUser;
                            }
                        }

                        //Set the Pricipal with our user specific details
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(decrypted.Name, "Forms"), userData.Role.Split(';'));

                        return userData;
                    }
                    catch (Exception ex)
                    {
                        //somehting went wrong
                    }
                }
            }

            return new UserOnline();
        }

        public static UserOnline GetUserDataFromCookie(HttpRequest Request)
        {
            /*
            ***** Example Get Obeject *****
            UserOnline user = UserOnline.GetUserData(Request);
            user.UserName.ToString();
            user.Name.ToString();
            user.Role.ToString();

            ***** Example Attribute Obeject *****
            UserOnline.GetUserData(Request).UserName.ToString();*/

            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        var decrypted = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);

                        UserOnline userData = new JavaScriptSerializer().Deserialize<UserOnline>(decrypted.UserData);

                        return userData;
                    }
                    catch (Exception ex)
                    {
                        //somehting went wrong
                    }
                }
            }

            return new UserOnline();
        }
    }
}