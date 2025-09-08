namespace WIRS.Shared.Enums
{
    public enum UserRole
    {
        User = 1,
        Supervisor = 2,
        WSHO = 3,
        AssistantWSHO = 4,
        Management = 5,
        SeniorManagement = 6,
        DepartmentHead = 7,
        HOD = 8,
        Administrator = 9
    }

    public static class UserRoleExtensions
    {
        public static string GetRoleName(this UserRole role)
        {
            return role switch
            {
                UserRole.User => "User",
                UserRole.Supervisor => "Supervisor",
                UserRole.WSHO => "WSHO",
                UserRole.AssistantWSHO => "Assistant WSHO",
                UserRole.Management => "Management",
                UserRole.SeniorManagement => "Senior Management",
                UserRole.DepartmentHead => "Department Head",
                UserRole.HOD => "HOD",
                UserRole.Administrator => "Administrator",
                _ => "Unknown"
            };
        }

        public static UserRole FromString(string roleString)
        {
            return roleString switch
            {
                "1" => UserRole.User,
                "2" => UserRole.Supervisor,
                "3" => UserRole.WSHO,
                "4" => UserRole.AssistantWSHO,
                "5" => UserRole.Management,
                "6" => UserRole.SeniorManagement,
                "7" => UserRole.DepartmentHead,
                "8" => UserRole.HOD,
                "9" => UserRole.Administrator,
                _ => UserRole.User
            };
        }

        public static bool IsManagement(this UserRole role)
        {
            return role >= UserRole.Management;
        }

        public static bool IsWSHO(this UserRole role)
        {
            return role == UserRole.WSHO || role == UserRole.AssistantWSHO || role >= UserRole.SeniorManagement;
        }

        public static bool IsAdmin(this UserRole role)
        {
            return role == UserRole.Administrator;
        }
    }
}