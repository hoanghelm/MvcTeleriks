using System.Data;
using System.Threading.Tasks;
using WIRS.DataAccess.Entities;

namespace WIRS.DataAccess.Interfaces
{
    public interface IEmailDistributionDataAccess
    {
        Task<string> SaveEmailUsers(EmailDistribution emailBE);

        Task<DataSet> SeachEmailUsers(EmailDistribution emailBE);

        Task<string> GetEmailUsers(EmailDistribution emailBE);

        Task<string> UpdateEmailUsers(EmailDistribution emailBE);
    }
}