using System.Data;

namespace WIRS.Services.Interfaces
{
    public interface INotificationService
    {
        Task<string> SendWorkflowEmailNotificationAsync(
            string incidentId,
            string actionFrom,
            string actionTo,
            DataTable workflowsDataTable,
            string remarks
        );
    }
}
