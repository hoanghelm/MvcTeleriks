using System.Threading.Tasks;

namespace WIRS.DataAccess.Interfaces
{
    public interface IErrorMessageDataAccess
    {
        Task<string> GetErrorMessage(string errorCode);
    }
}