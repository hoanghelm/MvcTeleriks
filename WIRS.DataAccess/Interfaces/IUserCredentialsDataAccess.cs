using System.Data;
using System.Threading.Tasks;
using WIRS.DataAccess.Entities;

namespace WIRS.DataAccess.Interfaces
{
    public interface IUserCredentialsDataAccess
    {
        Task<(DataSet accessList, string errorCode)> GetUsersAccessSBU(string userId, string userRole, string sbaCode);

        Task<string> SaveUsers(UserCredentials userCredentials);

        Task<DataSet> SeachUsers(UserCredentials userCredentials, string loginId);

        Task<string> GetUser(UserCredentials userCredentials);

        Task InactiveUsers(UserCredentials userCredentials);

        Task<DataSet> GetAllUserRole();

        Task<string> UpdateUsers(UserCredentials userCredentials);

        Task ResetPassword(UserCredentials userCredentials);

        Task ValidateUserExists(UserCredentials userCredentials);

        Task<string> ValidatUserName(UserCredentials userCredentials);

        Task<string> ValidateUserResign(UserCredentials userCredentials);

        Task<string> GetUserRoleName(string userRoleCode);

        Task<DataSet> SearchUsers(string currentUserId, string sector, string lob, string userId, string userName, string userRole);
    }
}