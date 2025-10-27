using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.Services.Models;

namespace WIRS.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel?> GetUserById(string userId);
        Task<UserModel?> GetUserByEIP(string eipId);
        Task<bool> CheckUserExists(string userId);
        Task<UserCreationModel?> ValidateUserExists(string userId);
        Task<string> CreateUser(UserCreationRequest request, string creatorId);
        Task<EmployeeSearchResult> SearchEmployees(string empId, string empName, int pageNo = 1, int pageSize = 10);
        
        Task<UserListResult> SearchUsers(string currentUserId, string sector, string lob, string userId, string userName, string userRole);
        Task<UserDetailsModel?> GetUserDetails(string userId);
        Task<string> UpdateUser(UserUpdateRequest request, string modifierUserId);
        Task<string> InactiveUser(string userId, string modifierUserId);
        Task<string> ResetPassword(string userId, string modifierUserId);
    }
}