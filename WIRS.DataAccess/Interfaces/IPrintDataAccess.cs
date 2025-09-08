using System.Data;
using System.Threading.Tasks;

namespace WIRS.DataAccess.Interfaces
{
    public interface IPrintDataAccess
    {
        Task<DataSet> PrintIncident(string incidentID);

        Task<DataSet> PrintIncidentType(string incidentID);
    }
}