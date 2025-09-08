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
        Task<IncidentDataModel> GetIncidentDataAsync(string userId, string userRole);
    }
}