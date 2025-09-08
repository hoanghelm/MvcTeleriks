namespace WIRS.DataAccess.Entities
{
    /// <summary>
    /// Summary description for Upload
    /// </summary>
    public class Upload
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public string GeneratedFileName { get; set; }
        public string DestinationFilePath { get; set; }
        public string DestinationFolderPath { get; set; }
        public string IncidentId { get; set; }
        public bool NewUploadFlag { get; set; }
    }

    public class FileUploadBE
    {
        public string attachment_type { get; set; }
        public string reference_code { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }
        public string file_extension { get; set; }
        public string file_id { get; set; }
        public string localfile_path { get; set; }
        public string incident_id { get; set; }
        public bool isNew { get; set; }
    }
}