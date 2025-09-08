using System;
using System.Data;
using System.Threading.Tasks;
using WIRS.DataAccess.Entities;

namespace WIRS.DataAccess.Interfaces
{
    public interface IEmployeeDataAccess
    {
        Task<DataSet> GetEmployeeByName(Employee employee);

        Task<string> ValidateEmployee(Employee employee);

        Task<(string emailAddress, string userRoleName, string errorCode)> GetEmployeeEmailAddress(Employee employee);

        Task<string> ValidateNextUser(string empNo);
    }
}