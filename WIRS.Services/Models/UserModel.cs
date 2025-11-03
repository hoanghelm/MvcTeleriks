namespace WIRS.Services.Models
{
    public class UserModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public int UnsuccessfulLogin { get; set; }
        public string SbaName { get; set; } = string.Empty;
        public string SbuName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;

        public string RoleName => GetRoleName(UserRole);
        public string RoleValue => GetRoleValue(UserRole);
        public int RoleSortOrder => GetRoleSortOrder(UserRole);
        public bool IsHigherAuthority => CheckHigherAuthority(UserRole);
        public bool IsWSHRole => CheckWSHRole(UserRole);
        public bool IsManagementRole => CheckManagementRole(UserRole);
        public bool IsAdministrativeRole => CheckAdministrativeRole(UserRole);

        private static string GetRoleName(string roleCode)
        {
            return roleCode switch
            {
                "1" => "HR",
                "2" => "View Only",
                "3" => "WSHO",
                "4" => "Alternate WSHO",
                "5" => "HOD",
                "6" => "Chairman WSH",
                "7" => "Head of LOB",
                "8" => "Alternate HOD",
                "9" => "WSH Administrator",
                _ => "Unknown"
            };
        }

        private static string GetRoleValue(string roleCode)
        {
            return roleCode switch
            {
                "1" => "COPYTO",
                "2" => "COPYTO",
                "3" => "WSHO",
                "4" => "A_WSHO",
                "5" => "HOD",
                "6" => "CWSHO",
                "7" => "HSBU",
                "8" => "AHOD",
                "9" => "SYS",
                _ => "UNKNOWN"
            };
        }

        private static int GetRoleSortOrder(string roleCode)
        {
            return roleCode switch
            {
                "6" => 1, // Chairman WSH
                "7" => 2, // Head of LOB
                "5" => 3, // HOD
                "8" => 4, // Alternate HOD
                "3" => 5, // WSHO
                "4" => 6, // Alternate WSHO
                "1" => 7, // HR
                "2" => 8, // View Only
                "9" => 9, // WSH Administrator
                _ => 10
            };
        }

        private static bool CheckHigherAuthority(string roleCode)
        {
            return roleCode is "5" or "6" or "7" or "8" or "9";
        }

        private static bool CheckWSHRole(string roleCode)
        {
            return roleCode is "3" or "4" or "6";
        }

        private static bool CheckManagementRole(string roleCode)
        {
            return roleCode is "5" or "7" or "8";
        }

        private static bool CheckAdministrativeRole(string roleCode)
        {
            return roleCode is "1" or "9";
        }
    }
}