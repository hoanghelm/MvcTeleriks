namespace WIRS.Services.Models
{
    public class LookupItem
    {
        public string Code { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class MasterDataResponse
    {
        public List<LookupItem>? UserRoles { get; set; }
        public List<LookupItem>? Sectors { get; set; }
        public List<LookupItem>? LOBs { get; set; }
        public List<LookupItem>? Departments { get; set; }
        public List<LookupItem>? Locations { get; set; }
    }

    public class MasterDataRequest
    {
        public bool IncludeUserRoles { get; set; } = false;
        public bool IncludeSectors { get; set; } = false;
        public bool IncludeLocations { get; set; } = false;
        public string? SectorCode { get; set; }
        public string? LOBCode { get; set; }
    }
}