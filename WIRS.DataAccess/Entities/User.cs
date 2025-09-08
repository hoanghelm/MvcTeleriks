using System;

namespace WIRS.DataAccess.Entities
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string sbaname { get; set; }
        public string sbuname { get; set; }
        public string Designation { get; set; }
        public string costcentreno { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public string PasswordHistory { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int? UnsuccessfulLogin { get; set; }
        public string AccountStatus { get; set; }
        public string AccountStatusDescription { get; set; }
        public DateTime? InactiveDate { get; set; }
        public string Creator { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Modifiedby { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}