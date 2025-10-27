namespace WIRS.Services.Models
{
    public class PartDSubmitModel
    {
        public string IncidentId { get; set; }
        public string Comments { get; set; }
        public string HsbuId { get; set; }
        public List<string> EmailToList { get; set; }
        public List<CopyToPersonModel> AdditionalCopyToList { get; set; }
        public string SubmitterName { get; set; }
        public string SubmitterEmpId { get; set; }
        public string SubmitterDesignation { get; set; }
    }
}
