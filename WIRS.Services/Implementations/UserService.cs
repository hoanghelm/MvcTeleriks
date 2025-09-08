using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WIRS.DataAccess.Implementations;
using WIRS.DataAccess.Interfaces;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserDataAccess _userDataAccess;
        private readonly IDataMapperService _dataMapper;

        public UserService(IUserDataAccess userDataAccess, IDataMapperService dataMapper)
        {
            _userDataAccess = userDataAccess;
            _dataMapper = dataMapper;
        }

        public async Task<UserModel?> GetUserById(string userId)
        {
            try
            {
                var userBE = await _userDataAccess.GetUserByUserID(userId);

                if (userBE?.UserId == null) return null;

                return new UserModel
                {
                    UserId = userBE.UserId,
                    UserName = userBE.UserName,
                    UserRole = userBE.UserRole,
                    AccountStatus = userBE.AccountStatus,
                    UnsuccessfulLogin = userBE.UnsuccessfulLogin ?? 0,
                    SbaName = userBE.sbaname ?? string.Empty
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserModel?> GetUserByEIP(string eipId)
        {
            try
            {
                var userBE = await _userDataAccess.GetUserByUserEIP(eipId);

                if (userBE?.UserId == null) return null;

                return new UserModel
                {
                    UserId = userBE.UserId,
                    UserName = userBE.UserName,
                    UserRole = userBE.UserRole,
                    AccountStatus = userBE.AccountStatus,
                    UnsuccessfulLogin = userBE.UnsuccessfulLogin ?? 0,
                    SbaName = userBE.sbaname ?? string.Empty
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CheckUserExists(string userId)
        {
            try
            {
                return await _userDataAccess.CheckUserExists(userId);
            }
            catch
            {
                return false;
            }
        }

        public async Task<IncidentDataModel> GetIncidentDataAsync(string userId, string userRole)
        {
            try
            {
                var dataset = await _userDataAccess.GetInfoByUserID(userId, userRole);

                var incidents = _dataMapper.MapDataSetToIncidents(dataset, 0);
                var pendingIncidents = _dataMapper.MapDataSetToIncidents(dataset, 1);

                return await Task.FromResult(new IncidentDataModel
                {
                    Incidents = incidents,
                    PendingIncidents = pendingIncidents
                });
            }
            catch
            {
                return new IncidentDataModel();
            }
        }
    }
}