using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SinetWifi.Models
{
    public class SMSModel
    {
        public string phonenumber { get; set; }
        public string message { get; set; }
        public string error { get; set; }
        public SMSSendResult results { get; set; }
    }

    public class SMSSendResult
    {
        public string status { get; set; }
        public string details { get; set; }
        public string smid { get; set; }
    }

    public class SMSTemplate
    {
        public int id { get; set; }
        public string title { get; set; }
        public string message { get; set; }
    }

    #region MailBIT SMS GateWay
    public class SMSMailBITResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string JobId { get; set; }
        public List<SMSMailBITMessageData> MessageData { get; set; }
    }
    public class SMSMailBITMessageData
    {
        public string Number { get; set; }
        public List<SMSMailBITMessageParts> MessageParts { get; set; }
    }
    public class SMSMailBITMessageParts
    {
        public string MsgId { get; set; }
        public int PartId { get; set; }
        public string Text { get; set; }
    }
    public static class SMS
    {
        public static string SMSMailBITGetMsgError(string code)
        {
            string msg = string.Empty;

            switch (code)
            {
                case "000": msg = "Success"; break;
                case "001": msg = "Account details cannot be blank"; break;
                case "002": msg = "Username or password cannot be blank"; break;
                case "003": msg = "SenderId cannot be blank"; break;
                case "004": msg = "Message cannot be blank"; break;
                case "005": msg = "Message properties cannot be blank"; break;
                case "006": msg = "ServerError#Error message"; break;
                case "007": msg = "Invalid username or password"; break;
                case "008": msg = "Account inactive"; break;
                case "009": msg = "Account lock"; break;
                case "010": msg = "Unauthorized API access"; break;
                case "011": msg = "Unauthorized IP address"; break;
                case "012": msg = "Message length violation"; break;
                case "013": msg = "Invalid mobile numbers"; break;
                case "014": msg = "Account locked due to spam message contact support"; break;
                case "015": msg = "Invalid SednerId"; break;
                case "016": msg = "Transactional account not active"; break;
                case "017": msg = "Invalid groupid"; break;
                case "018": msg = "Cannot send multi message to group"; break;
                case "019": msg = "Invalid schedule date"; break;
                case "020": msg = "Message or mobile number cannot be blank"; break;
                case "021": msg = "Insufficient credits"; break;
                case "022": msg = "Invalid jobid"; break;
                case "023": msg = "Parameter missing"; break;
                case "024": msg = "Invalid template or template mismatch"; break;
                default: msg = "Miss match error code."; break;
            }

            return msg;
        }
    }
    #endregion
}