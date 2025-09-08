namespace WIRS.DataAccess.Entities
{
    public class ManHours
    {
        public string SBU { get; set; }
        public string SBUName { get; set; }
        public int fromMonth { get; set; }
        public int toMonth { get; set; }
        public int fromYear { get; set; }
        public int toYear { get; set; }
        public string ModifyDate { get; set; }
    }
}