using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using WIRS.DataAccess.Interfaces;
using WIRS.Services.Interfaces;

namespace WIRS.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IWorkflowIncidentDataAccess _workflowIncidentDataAccess;
        private readonly IEmployeeDataAccess _employeeDataAccess;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionService _encryptionService;

        public NotificationService(
            IWorkflowIncidentDataAccess workflowIncidentDataAccess,
            IEmployeeDataAccess employeeDataAccess,
            IConfiguration configuration,
            IEncryptionService encryptionService)
        {
            _workflowIncidentDataAccess = workflowIncidentDataAccess;
            _employeeDataAccess = employeeDataAccess;
            _configuration = configuration;
            _encryptionService = encryptionService;
        }

        public async Task<string> SendWorkflowEmailNotificationAsync(
            string incidentId,
            string actionFrom,
            string actionTo,
            DataTable workflowsDataTable,
            string remarks)
        {
            string errorCode = string.Empty;

            try
            {
                var emailData = await _workflowIncidentDataAccess.get_sendemaildata_by_id(incidentId, actionFrom, actionTo);

                if (emailData == null || emailData.Tables.Count == 0 || emailData.Tables[0].Rows.Count == 0)
                {
                    return "ERR-111";
                }

                var emailConfig = emailData.Tables[0].Rows[0];

                string subject = GetStringValue(emailConfig["emailsubject"]);
                string emailHeader = GetStringValue(emailConfig["emailheader"]);
                string commentsLabel = GetStringValue(emailConfig["commentslabel"]);
                string emailLink = GetStringValue(emailConfig["email_link"]);
                string injuredPersonInfo = GetStringValue(emailConfig["injured_person_info"]);

                if (string.IsNullOrEmpty(emailLink))
                {
                    emailLink = _configuration["EmailSettings:LoginLink"] ?? string.Empty;
                }

                string emailFrom = _configuration["EmailSettings:MailFrom"] ?? string.Empty;
                string emailTemplateLink = _configuration["EmailSettings:TemplateLink"] ?? string.Empty;
                string loginLink = _configuration["EmailSettings:LoginLink"] ?? string.Empty;

                if (workflowsDataTable != null && workflowsDataTable.Rows.Count > 0)
                {
                    foreach (DataRow workflow in workflowsDataTable.Rows)
                    {
                        string recipientUserId = workflow["to"]?.ToString() ?? string.Empty;
                        string actionsRole = workflow["actions_role"]?.ToString() ?? string.Empty;

                        if (string.IsNullOrEmpty(recipientUserId))
                            continue;

                        var employee = new DataAccess.Entities.Employee { EmpID = recipientUserId };
                        var (emailTo, userRole, employeeErrorCode) = await _employeeDataAccess.GetEmployeeEmailAddress(employee);

                        if (string.IsNullOrEmpty(emailTo) || !string.IsNullOrEmpty(employeeErrorCode))
                            continue;

                        string emailBody = await GetEmailTemplateAsync(emailTemplateLink, "IncidentSubmitted.htm");

                        emailBody = emailBody.Replace("[emailheader]", emailHeader);
                        emailBody = emailBody.Replace("[incidentid]", GetStringValue(emailConfig["incident_id"]));
                        emailBody = emailBody.Replace("[inctype]", await GetIncidentTypeListByIdAsync(incidentId));
                        emailBody = emailBody.Replace("[datetime]", GetStringValue(emailConfig["incident_datetime"]));
                        emailBody = emailBody.Replace("[USERNAME]", GetStringValue(emailConfig["superior_name"]));
                        emailBody = emailBody.Replace("[sbu]", GetStringValue(emailConfig["sbu_name"]));
                        emailBody = emailBody.Replace("[department]", GetStringValue(emailConfig["department_name"]));
                        emailBody = emailBody.Replace("[location]", GetStringValue(emailConfig["location_name"]));
                        emailBody = emailBody.Replace("[incDesc]", GetStringValue(emailConfig["incident_desc"]));
                        emailBody = emailBody.Replace("[Designation]", GetStringValue(emailConfig["superior_designation"]));
                        emailBody = emailBody.Replace("[creatorname]", GetStringValue(emailConfig["superior_name"]));
                        emailBody = emailBody.Replace("[submittedon]", GetStringValue(emailConfig["creation_date"]));
                        emailBody = emailBody.Replace("[Remarks]", remarks);
                        emailBody = emailBody.Replace("[Comments]", commentsLabel);
                        emailBody = emailBody.Replace("[injured_person_info]", injuredPersonInfo);
                        emailBody = emailBody.Replace("[LINK]", loginLink);

                        await SendEmailAsync(emailFrom, emailTo, subject, emailBody);
                    }
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return "ERR-111";
            }
        }

        private string GetStringValue(object value)
        {
            return value?.ToString() ?? string.Empty;
        }

        private async Task<string> GetEmailTemplateAsync(string templatePath, string templateName)
        {
            string fullPath = Path.Combine(templatePath, templateName);

            if (!File.Exists(fullPath))
            {
                return string.Empty;
            }

            return await File.ReadAllTextAsync(fullPath);
        }

        private async Task<string> GetIncidentTypeListByIdAsync(string incidentId)
        {
            return string.Empty;
        }

        private async Task SendEmailAsync(string from, string to, string subject, string body)
        {
            string smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "localhost";
            int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "25");
            bool enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "false");
            string smtpUser = _configuration["EmailSettings:SmtpUser"] ?? string.Empty;
            string smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? string.Empty;

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.Normal;

                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.EnableSsl = enableSsl;

                    if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword))
                    {
                        smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                    }

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}
