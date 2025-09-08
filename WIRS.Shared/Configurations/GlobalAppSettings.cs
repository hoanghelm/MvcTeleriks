using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIRS.Shared.Configurations
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string LogoutUri { get; set; }
        public string RedirectUri { get; set; }
        public string UseAWS_SM { get; set; }
        public string AWS_SearchKey { get; set; }
        public string Identity { get; set; }
        public string NRIC { get; set; }
        public string FIN { get; set; }
        public string ITSupportEmail { get; set; }
        public string MailFrom { get; set; }
        public string TemplateLink { get; set; }
        public string Login_Link { get; set; }
        public int MaxLoginError { get; set; }
        public int CertiMaxFileSize_MB { get; set; }
        public int DOMaxFileSize_MB { get; set; }
        public string Temp_Supporting_Documents { get; set; }
        public string Supporting_Documents { get; set; }
        public string Instruction { get; set; }
        public int RandomPasswordLength { get; set; }
        public int NoOldPasswords { get; set; }
        public string PasswordHash { get; set; }
        public string SaltKey { get; set; }
        public string VIKey { get; set; }
        public string Req_Link { get; set; }
        public int LastDay { get; set; }
        public int MCDays { get; set; }
        public string UserGuide { get; set; }
    }

    public class AzureADSettings
    {
        public string ClientId { get; set; }
        public string Tenant { get; set; }
        public string Endpoint { get; set; }
        public string LogoutRedirectUri { get; set; }
        public string Logout { get; set; }
        public string Metadata { get; set; }
    }

    public class ElasticApmSettings
    {
        public string ServiceName { get; set; }
        public string Environment { get; set; }
        public string ServerUrl { get; set; }
        public string ApiKey { get; set; }
        public string GlobalLabels { get; set; }
        public string LogLevel { get; set; }
    }
}