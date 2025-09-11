using System.Data;

namespace WIRS.Services.Models
{
    public class UserCreationModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NonEncryptedPassword { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public DataTable? UserAccess { get; set; }
    }

    public class UserListResult
    {
        public List<UserListItem> Users { get; set; } = new List<UserListItem>();
        public int TotalCount { get; set; }
    }

    public class UserListItem
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string UserRoleName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public string AccountStatusName { get; set; } = string.Empty;
        public DateTime? LastLoginDate { get; set; }
        public DateTime? InactiveDate { get; set; }
        public string SectorName { get; set; } = string.Empty;
        public string LOBName { get; set; } = string.Empty;
    }

    public class UserDetailsModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string UserRoleName { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public string AccountStatusName { get; set; } = string.Empty;
        public DateTime? InactiveDate { get; set; }
        public string InactiveDateString { get; set; } = string.Empty;
        public List<UserAccessDetails> UserAccess { get; set; } = new List<UserAccessDetails>();
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string Creator { get; set; } = string.Empty;
        public string Modifier { get; set; } = string.Empty;
    }

    public class UserAccessDetails
    {
        public string UserRoleCode { get; set; } = string.Empty;
        public string UserRoleName { get; set; } = string.Empty;
        public string SectorCode { get; set; } = string.Empty;
        public string SectorValue { get; set; } = string.Empty;
        public string LOBCode { get; set; } = string.Empty;
        public string LOBValue { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentValue { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public string LocationValue { get; set; } = string.Empty;
    }

    public class UserUpdateRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public string InactiveDate { get; set; } = string.Empty;
        public List<UserAccessItem> UserAccess { get; set; } = new List<UserAccessItem>();
    }

    public class UserCreationRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public List<UserAccessItem> UserAccess { get; set; } = new List<UserAccessItem>();
    }

    public class UserAccessItem
    {
        public string UserRoleCode { get; set; } = string.Empty;
        public string SectorCode { get; set; } = string.Empty;
        public string LobCode { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
    }

    public class EmployeeSearchRequest
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
    }

    public class EmployeeSearchResult
    {
        public List<EmployeeInfo> Employees { get; set; } = new List<EmployeeInfo>();
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class EmployeeInfo
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string CostCentreNo { get; set; } = string.Empty;
        public string CostCentreName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Nric { get; set; } = string.Empty;
        public string Age { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string DateOfEmployment { get; set; } = string.Empty;
    }
}