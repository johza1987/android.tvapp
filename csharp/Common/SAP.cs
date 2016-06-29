using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebExtension.Common
{
    public class FTTx
    {
        public SAPbobsCOM.Company company = new SAPbobsCOM.Company();
        public FTTx()
        {
            company.Server = ConfigurationManager.AppSettings["SAP_Server"];
            company.DbUserName = ConfigurationManager.AppSettings["SAP_DBUserName"];
            company.DbPassword = ConfigurationManager.AppSettings["SAP_DBPassword"];
            company.CompanyDB = ConfigurationManager.AppSettings["SAP_DBCompany"];
            company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;
            company.UserName = ConfigurationManager.AppSettings["SAP_UserName"];
            company.Password = ConfigurationManager.AppSettings["SAP_Password"];
            company.LicenseServer = ConfigurationManager.AppSettings["SAP_LicenseServer"];
        }

        public SAPbobsCOM.Company GetCompany()
        {
            return company;
        }

        public void Connecting(out string message)
        {
            message = string.Empty;
            if(company.Connected)
                company.Disconnect();
            if (!company.Connected)
            {
                if (company.Connect() != 0)
                    message = company.GetLastErrorDescription();
            }
        }
    }
}
