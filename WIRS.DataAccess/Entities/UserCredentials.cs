using System;
using System.Data;

namespace WIRS.DataAccess.Entities
{
    public class UserCredentials
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NonEncryptedPassword { get; set; }
        public string PasswordExpiryDate { get; set; }
        public string PasswordHistory { get; set; }
        public string LastLoginDate { get; set; }
        public int? UnsuccessfulLogin { get; set; }
        public string AccountStatus { get; set; }
        public string AccountStatusDescription { get; set; }
        public string InactiveDate { get; set; }
        public string Creator { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Modifiedby { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public DataTable UserAccess { get; set; }
        public string sbaname { get; set; }
        public string sbuname { get; set; }

    }
}