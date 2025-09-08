namespace WIRS.Services.Models
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public UserModel? User { get; set; }
        public bool RequiresPasswordChange { get; set; }
        public LoginErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public enum LoginErrorType
    {
        None,
        InvalidCredentials,
        AccountLocked,
        AccountInactive,
        NoAccess,
        SystemError
    }
}