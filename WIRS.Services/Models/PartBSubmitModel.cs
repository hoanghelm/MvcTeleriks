namespace WIRS.Services.Models
{
    public class PartBSubmitModel
    {
        public string IncidentId { get; set; }
        public string InjuredCaseType { get; set; }
        public string ReviewComment { get; set; }
        public string WshoId { get; set; }
        public string AlternateWshoId { get; set; }
        public List<string> EmailToList { get; set; }
        public List<CopyToPersonModel> AdditionalCopyToList { get; set; }
        public string SubmitterName { get; set; }
        public string SubmitterEmpId { get; set; }
        public string SubmitterDesignation { get; set; }
    }

    public class CopyToPersonModel
    {
        public string EmployeeNo { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
    }
}
