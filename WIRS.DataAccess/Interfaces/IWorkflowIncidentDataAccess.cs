using System;
using System.Data;
using System.Threading.Tasks;
using WIRS.DataAccess.Entities;

namespace WIRS.DataAccess.Interfaces
{
    public interface IWorkflowIncidentDataAccess
    {
        Task<(string incident_ID, string error_Code)> insert_Incidents(WorkflowIncident incidents, string incidentype_dsXML, string InjuredPerson_dsXML, string eyewitness_dsXML, string workflow_dsXML);

        Task<string> update_incidents_injured(string incident_id, string InjuredPerson_dsXML);

        Task<string> update_incidents_header(WorkflowIncident incidents, string incidentype_dsXML, string eyewitness_dsXML);

        Task<string> update_Incidents(WorkflowIncident incidents);

        Task<string> submit_incident_partc(WorkflowIncident incidents, string ireport_injuredpersonXML, string interviewedXML, string injury_detailsXML, string cause_analysisXML, string medical_leavesXML, string workflowsdsXML, string incidents_attachmentXML);

        Task<string> Save_MC(WorkflowIncident incidents, string medical_leavesXML, string incidents_attachmentXML);

        Task<string> save_incident_partc(WorkflowIncident incidents, string ireport_injuredpersonXML, string interviewedXML, string injury_detailsXML, string cause_analysisXML, string medical_leavesXML, string incidents_attachmentXML);

        Task<string> update_incidents_attachfiles(int uid, string created_by);

        Task<string> insert_incidents_attachfiles(string strXML, string created_by);

        Task<string> insert_incidents_workflows(string incident_id, string strXML);

        Task<string> insert_incidents_injured(string incident_id, string injured_emp_no, string injured_name, string injured_nric_fin_no, string injured_company, string injured_contact_no, string injured_age, string injured_race, string injured_race_oth, string injured_gender, string injured_nationality, string injured_designation, string injured_employment_type, string injured_employment_type_oth, string remarks);

        Task<DataSet> get_printview_incident_by_id(string incident_id);

        Task<DataSet> get_incident_by_id(WorkflowIncident incidents);

        Task<DataSet> get_sendemaildata_by_id(string incident_id, string aFrom, string aTo);

        Task<DataSet> validate_user_to_edit_inc(string incident_id, string login_id, string changemode);

        Task<DataSet> validate_workflowuser(string incident_id, string login_id);

        Task<DataSet> get_emaillist_MCChange(string incident_id, string login_id);

        Task<DataSet> get_injured_person_injury_description(string incident_id, string injured_id);

        Task<DataSet> get_wirs_incidents_workflows_by_id(string incident_id, string status);

        Task<DataSet> get_incident_partc_id(WorkflowIncident incidents);

        Task<DataSet> get_incidents_attachedfiles_by_id(string incident_id, string attachment_type, string reference_code);

        Task<DataSet> search_incidents(string userid, string userrolecode, string incidentid, string sba, string sbu, string division, string incdatefrom, string incdateto, DataTable dt);

        Task<DataSet> isvalid_ireportinformation(string incident_id);
    }
}