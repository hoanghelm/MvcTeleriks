namespace WIRS.DataAccess.Entities
{
    public class StatisticsReport
    {
        public int YearFrom { get; set; }
        public int MonthFrom { get; set; }
        public int YearTo { get; set; }
        public int MonthTo { get; set; }
        public int MCdays { get; set; }
        public string UserId { get; set; }
        public string UserRole { get; set; }
    }
}