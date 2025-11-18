namespace WIRS.Shared.Configuration
{
    public class AppSettings
    {
        public bool UseMockData { get; set; } = true;
        public string DatabaseConnectionString { get; set; } = string.Empty;
        public int MaxLoginError { get; set; } = 3;
        public MockAdminSettings MockAdmin { get; set; } = new MockAdminSettings();
        public string ApiBasePath { get; set; } = string.Empty;
        public string ITSupportEmail { get; set; } = string.Empty;
    }

    public class MockAdminSettings
    {
        public string Username { get; set; } = "admin";
        public string Password { get; set; } = "admin";
    }
}