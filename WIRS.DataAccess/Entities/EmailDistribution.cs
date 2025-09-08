using System;

namespace WIRS.DataAccess.Entities
{
    public class EmailDistribution
    {
        public string EmpName { get; set; }
        public string EmpNo { get; set; }
        public string sba_code { get; set; }
        public string sbu_code { get; set; }
        public string Group { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string InactiveDate { get; set; }
        public string Creator { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Modifiedby { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}