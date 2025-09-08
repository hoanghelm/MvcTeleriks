using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIRS.Services.Models
{
    public class RoleModel
    {
        public string UserRoleCode { get; set; } = string.Empty;
        public string UserRoleName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public string UserRoleValue { get; set; } = string.Empty;
    }
}