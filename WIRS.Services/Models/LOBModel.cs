using System.ComponentModel.DataAnnotations;

namespace WIRS.Services.Models
{
    public class LOBModel
    {
        [Required(ErrorMessage = "Sector is required")]
        public string SbaCode { get; set; } = string.Empty;

        public string? SbaName { get; set; }

        [Required(ErrorMessage = "LOB Code is required")]
        [StringLength(10, ErrorMessage = "LOB Code cannot exceed 10 characters")]
        public string SbuCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "LOB Name is required")]
        [StringLength(100, ErrorMessage = "LOB Name cannot exceed 100 characters")]
        public string SbuName { get; set; } = string.Empty;

        public string? InactiveDate { get; set; }

        public string? Uid { get; set; }
    }

    public class LocationModel
    {
        [Required(ErrorMessage = "Sector is required")]
        public string SbaCode { get; set; } = string.Empty;

        public string? SbaName { get; set; }

        [Required(ErrorMessage = "LOB is required")]
        public string SbuCode { get; set; } = string.Empty;

        public string? SbuName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string DepartmentCode { get; set; } = string.Empty;

        public string? DepartmentName { get; set; }

        [Required(ErrorMessage = "Location Code is required")]
        [StringLength(10, ErrorMessage = "Location Code cannot exceed 10 characters")]
        public string LocationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location Name is required")]
        [StringLength(100, ErrorMessage = "Location Name cannot exceed 100 characters")]
        public string LocationName { get; set; } = string.Empty;

        public string? InactiveDate { get; set; }

        public string? Uid { get; set; }
    }

    public class DepartmentModel
    {
        [Required(ErrorMessage = "Sector is required")]
        public string SbaCode { get; set; } = string.Empty;

        public string? SbaName { get; set; }

        [Required(ErrorMessage = "LOB is required")]
        public string SbuCode { get; set; } = string.Empty;

        public string? SbuName { get; set; }

        [Required(ErrorMessage = "Department Code is required")]
        [StringLength(10, ErrorMessage = "Department Code cannot exceed 10 characters")]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department Name is required")]
        [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters")]
        public string DepartmentName { get; set; } = string.Empty;

        public string? InactiveDate { get; set; }

        public string? Uid { get; set; }

        public string? CodeType { get; set; }
    }

    public class CopyToListModel
    {
        [Required(ErrorMessage = "Sector is required")]
        public string SbaCode { get; set; } = string.Empty;

        public string? SbaName { get; set; }

        [Required(ErrorMessage = "LOB is required")]
        public string SbuCode { get; set; } = string.Empty;

        public string? SbuName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string DepartmentCode { get; set; } = string.Empty;

        public string? DepartmentName { get; set; }

        public string? LocationCode { get; set; } = string.Empty;

        public string? LocationName { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        [StringLength(8, ErrorMessage = "User ID must be exactly 8 characters")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "User Name is required")]
        [StringLength(100, ErrorMessage = "User Name cannot exceed 100 characters")]
        public string UserName { get; set; } = string.Empty;

        public string? InactiveDate { get; set; }

        public string? Uid { get; set; }
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
    }
}