using Npgsql;
using System;
using System.Data;
using System.Linq;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;
using WIRS.Shared.Extensions;
using WIRS.Shared.Helpers;

namespace WIRS.DataAccess.Implementations
{
    public class WorkflowIncidentDataAccess : IWorkflowIncidentDataAccess
    {
        private readonly IDBHelper _dBHelper;

        public WorkflowIncidentDataAccess(IDBHelper dBHelper)
        {
            _dBHelper = dBHelper;
        }

        public async Task<(string incident_ID, string error_Code)> insert_Incidents(WorkflowIncident incidents, string incidentype_dsXML, string InjuredPerson_dsXML, string eyewitness_dsXML, string workflow_dsXML)
        {
            string incident_ID = string.Empty;
            string error_Code = string.Empty;
            using (NpgsqlConnection con = _dBHelper.GetConnection())
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                try
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SELECT * FROM dbo.spc_insert_incidents(@p_incident_datetime, @p_sba_code, @p_sbu_code, @p_division, @p_department, " +
                                      "@p_location, @p_exact_location, @p_incident_desc, @p_superior_name, @p_superior_emp_no, @p_status, " +
                                      "@p_incidentype_dsXML, @p_InjuredPerson_dsXML, @p_workflow_dsXML, @p_eyewitness_dsXML, @p_damage_description, " +
                                      "@p_any_eyewitness, @p_is_working_overtime, @p_is_jobrelated, @p_examined_hospital_clinic_name, " +
                                      "@p_official_working_hrs)";

                    DateTime parsedDate;
                    var incidentDateValue = DateTime.TryParse(incidents.incident_datetime, out parsedDate) ? (object)parsedDate : DBNull.Value;

                    cmd.AddParameter("@p_incident_datetime", NpgsqlTypes.NpgsqlDbType.Timestamp, incidentDateValue);
                    cmd.AddParameter("@p_sba_code", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.sba_code) ? DBNull.Value : (object)incidents.sba_code);
                    cmd.AddParameter("@p_sbu_code", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.sbu_code) ? DBNull.Value : (object)incidents.sbu_code);
                    cmd.AddParameter("@p_division", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.division) ? DBNull.Value : (object)incidents.division);
                    cmd.AddParameter("@p_department", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.department) ? DBNull.Value : (object)incidents.department);
                    cmd.AddParameter("@p_location", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.location) ? DBNull.Value : (object)incidents.location);
                    cmd.AddParameter("@p_exact_location", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.exact_location) ? DBNull.Value : (object)incidents.exact_location);
                    cmd.AddParameter("@p_incident_desc", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.incident_desc) ? DBNull.Value : (object)incidents.incident_desc);
                    cmd.AddParameter("@p_superior_name", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.superior_name) ? DBNull.Value : (object)incidents.superior_name);
                    cmd.AddParameter("@p_superior_emp_no", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.superior_emp_no) ? DBNull.Value : (object)incidents.superior_emp_no);
                    cmd.AddParameter("@p_status", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.status) ? DBNull.Value : (object)incidents.status);

                    cmd.AddParameter("@p_incidentype_dsXML", NpgsqlTypes.NpgsqlDbType.Xml, string.IsNullOrEmpty(incidentype_dsXML) ? DBNull.Value : (object)incidentype_dsXML);
                    cmd.AddParameter("@p_InjuredPerson_dsXML", NpgsqlTypes.NpgsqlDbType.Xml, string.IsNullOrEmpty(InjuredPerson_dsXML) ? DBNull.Value : (object)InjuredPerson_dsXML);
                    cmd.AddParameter("@p_workflow_dsXML", NpgsqlTypes.NpgsqlDbType.Xml, string.IsNullOrEmpty(workflow_dsXML) ? DBNull.Value : (object)workflow_dsXML);
                    cmd.AddParameter("@p_eyewitness_dsXML", NpgsqlTypes.NpgsqlDbType.Xml, string.IsNullOrEmpty(eyewitness_dsXML) ? DBNull.Value : (object)eyewitness_dsXML);
                    cmd.AddParameter("@p_damage_description", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.damage_description) ? DBNull.Value : (object)incidents.damage_description);
                    cmd.AddParameter("@p_any_eyewitness", NpgsqlTypes.NpgsqlDbType.Integer, incidents.any_eyewitness.HasValue ? (object)incidents.any_eyewitness.Value : DBNull.Value);
                    cmd.AddParameter("@p_is_working_overtime", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.is_working_overtime) ? DBNull.Value : (object)incidents.is_working_overtime);
                    cmd.AddParameter("@p_is_jobrelated", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.is_jobrelated) ? DBNull.Value : (object)incidents.is_jobrelated);
                    cmd.AddParameter("@p_examined_hospital_clinic_name", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.examined_hospital_clinic_name) ? DBNull.Value : (object)incidents.examined_hospital_clinic_name);
                    cmd.AddParameter("@p_official_working_hrs", NpgsqlTypes.NpgsqlDbType.Varchar, string.IsNullOrEmpty(incidents.official_working_hrs) ? DBNull.Value : (object)incidents.official_working_hrs);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            incident_ID = reader["p_incident_id"] == DBNull.Value ? string.Empty : reader["p_incident_id"].ToString();
                            error_Code = reader["p_errCode"] == DBNull.Value ? string.Empty : reader["p_errCode"].ToString();
                        }
                        else
                        {
                            incident_ID = string.Empty;
                            error_Code = "No response from function";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return (incident_ID, error_Code);
            }
        }

        public async Task<string> update_incidents_injured(string incident_id, string InjuredPerson_dsXML)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_update_incidents_injured";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@incident_id", incident_id);
                cmd.Parameters.AddWithValue("@InjuredPerson_dsXML", InjuredPerson_dsXML == null || InjuredPerson_dsXML == "" ? DBNull.Value : (object)InjuredPerson_dsXML);

                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 1000;

                cmd.ExecuteNonQuery();
                error_Code = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<string> update_incidents_header(WorkflowIncident incidents, string incidentype_dsXML, string eyewitness_dsXML)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_update_incidents_header";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@incident_id", incidents.incident_id);
                cmd.Parameters.AddWithValue("@incident_datetime", incidents.incident_datetime == null || incidents.incident_datetime == string.Empty ? DBNull.Value : (object)incidents.incident_datetime);
                cmd.Parameters.AddWithValue("@sba_code", incidents.sba_code == null || incidents.sba_code == string.Empty ? DBNull.Value : (object)incidents.sba_code);
                cmd.Parameters.AddWithValue("@sbu_code", incidents.sbu_code == null || incidents.sbu_code == string.Empty ? DBNull.Value : (object)incidents.sbu_code);
                cmd.Parameters.AddWithValue("@division", incidents.division == null || incidents.division == string.Empty ? DBNull.Value : (object)incidents.division);
                cmd.Parameters.AddWithValue("@department", incidents.department == null || incidents.department == string.Empty ? DBNull.Value : (object)incidents.department);
                cmd.Parameters.AddWithValue("@location", incidents.location == null || incidents.location == string.Empty ? DBNull.Value : (object)incidents.location);
                cmd.Parameters.AddWithValue("@exact_location", incidents.exact_location == null || incidents.exact_location == string.Empty ? DBNull.Value : (object)incidents.exact_location);
                cmd.Parameters.AddWithValue("@incident_desc", incidents.incident_desc == null || incidents.incident_desc == string.Empty ? DBNull.Value : (object)incidents.incident_desc);
                cmd.Parameters.AddWithValue("@modified_by", incidents.modified_by == null || incidents.modified_by == string.Empty ? DBNull.Value : (object)incidents.modified_by);
                cmd.Parameters.AddWithValue("@incidentype_dsXML", incidentype_dsXML == null || incidentype_dsXML == "" ? DBNull.Value : (object)incidentype_dsXML);

                cmd.Parameters.AddWithValue("@eyewitness_dsXML", eyewitness_dsXML == null || eyewitness_dsXML == "" ? DBNull.Value : (object)eyewitness_dsXML);
                cmd.Parameters.AddWithValue("@damage_description", incidents.damage_description == null || incidents.damage_description == string.Empty ? DBNull.Value : (object)incidents.damage_description);
                cmd.Parameters.AddWithValue("@any_eyewitness", incidents.any_eyewitness.HasValue ? incidents.any_eyewitness : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@is_working_overtime", incidents.is_working_overtime == null || incidents.is_working_overtime == string.Empty ? DBNull.Value : (object)incidents.is_working_overtime);
                cmd.Parameters.AddWithValue("@is_jobrelated", incidents.is_jobrelated == null || incidents.is_jobrelated == string.Empty ? DBNull.Value : (object)incidents.is_jobrelated);
                cmd.Parameters.AddWithValue("@examined_hospital_clinic_name", incidents.examined_hospital_clinic_name == null || incidents.examined_hospital_clinic_name == string.Empty ? DBNull.Value : (object)incidents.examined_hospital_clinic_name);
                cmd.Parameters.AddWithValue("@official_working_hrs", incidents.official_working_hrs == null || incidents.official_working_hrs == string.Empty ? DBNull.Value : (object)incidents.official_working_hrs);

                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 1000;

                cmd.ExecuteNonQuery();
                error_Code = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<string> update_Incidents(WorkflowIncident incidents)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_update_incidents";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.incident_id);
                cmd.AddParameter("@p_negligent", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.negligent == null || incidents.negligent == string.Empty ? DBNull.Value : (object)incidents.negligent);
                cmd.AddParameter("@p_negligent_comments", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.negligent_comments == null || incidents.negligent_comments == string.Empty ? DBNull.Value : (object)incidents.negligent_comments);
                cmd.AddParameter("@p_risk_assessment_review", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.risk_assessment_review == null || incidents.risk_assessment_review == string.Empty ? DBNull.Value : (object)incidents.risk_assessment_review);
                cmd.AddParameter("@p_risk_assessment_review_comments", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.risk_assessment_review_comments == null || incidents.risk_assessment_review_comments == string.Empty ? DBNull.Value : (object)incidents.risk_assessment_review_comments);
                cmd.AddParameter("@p_recommend_action_desc", NpgsqlTypes.NpgsqlDbType.Text, incidents.recommend_action_desc == null || incidents.recommend_action_desc == string.Empty ? DBNull.Value : (object)incidents.recommend_action_desc);
                cmd.AddParameter("@p_modified_by", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.modified_by == null || incidents.modified_by == string.Empty ? DBNull.Value : (object)incidents.modified_by);
                cmd.AddParameter("@p_injured_case_type", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.injured_case_type == null || incidents.injured_case_type == string.Empty ? DBNull.Value : (object)incidents.injured_case_type);

                error_Code = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        //public void submit_incident_partc(WorkflowIncident incidents, string eyewitnessesXML, string interviewedXML, string injury_detailsXML, string cause_analysisXML, string medical_leavesXML, string workflowsdsXML, string incidents_attachmentXML, out String error_Code)
        public async Task<string> submit_incident_partc(WorkflowIncident incidents, string ireport_injuredpersonXML, string interviewedXML, string injury_detailsXML, string cause_analysisXML, string medical_leavesXML, string workflowsdsXML, string incidents_attachmentXML)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_insert_incident_partc";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.incident_id);
                cmd.AddParameter("@p_negligent", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.negligent);
                cmd.AddParameter("@p_negligent_comments", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.negligent_comments);
                cmd.AddParameter("@p_recommend_action_desc", NpgsqlTypes.NpgsqlDbType.Text, incidents.recommend_action_desc);
                cmd.AddParameter("@p_what_why", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.what_happened_and_why_comments == null || incidents.what_happened_and_why_comments == string.Empty ? DBNull.Value : (object)incidents.what_happened_and_why_comments);
                cmd.AddParameter("@p_status", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.status);
                cmd.AddParameter("@p_modified_by", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.modified_by);
                cmd.AddParameter("@p_injuredpersonxml", NpgsqlTypes.NpgsqlDbType.Xml, ireport_injuredpersonXML == null || ireport_injuredpersonXML == "" ? DBNull.Value : (object)ireport_injuredpersonXML);
                cmd.AddParameter("@p_interviewedxml", NpgsqlTypes.NpgsqlDbType.Xml, interviewedXML == null || interviewedXML == "" ? DBNull.Value : (object)interviewedXML);
                cmd.AddParameter("@p_injury_detailsxml", NpgsqlTypes.NpgsqlDbType.Xml, injury_detailsXML == null || injury_detailsXML == "" ? DBNull.Value : (object)injury_detailsXML);
                cmd.AddParameter("@p_cause_analysisxml", NpgsqlTypes.NpgsqlDbType.Xml, cause_analysisXML == null || cause_analysisXML == "" ? DBNull.Value : (object)cause_analysisXML);
                cmd.AddParameter("@p_medical_leavesxml", NpgsqlTypes.NpgsqlDbType.Xml, medical_leavesXML == null || medical_leavesXML == "" ? DBNull.Value : (object)medical_leavesXML);
                cmd.AddParameter("@p_incidents_attachmentxml", NpgsqlTypes.NpgsqlDbType.Xml, incidents_attachmentXML == null || incidents_attachmentXML == "" ? DBNull.Value : (object)incidents_attachmentXML);
                cmd.AddParameter("@p_workflowsdsxml", NpgsqlTypes.NpgsqlDbType.Xml, workflowsdsXML == null || workflowsdsXML == "" ? DBNull.Value : (object)workflowsdsXML);

                error_Code = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<string> Save_MC(WorkflowIncident incidents, string medical_leavesXML, string incidents_attachmentXML)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_save_medical_leaves";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@incident_id", incidents.incident_id);
                cmd.Parameters.AddWithValue("@modified_by", incidents.modified_by);
                cmd.Parameters.AddWithValue("@medical_leavesXML", medical_leavesXML == null || medical_leavesXML == "" ? DBNull.Value : (object)medical_leavesXML);
                cmd.Parameters.AddWithValue("@incidents_attachmentXML", incidents_attachmentXML == null || incidents_attachmentXML == "" ? DBNull.Value : (object)incidents_attachmentXML);

                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 1000;

                cmd.ExecuteNonQuery();
                error_Code = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        //public void save_incident_partc(WorkflowIncident incidents, string eyewitnessesXML, string interviewedXML, string injury_detailsXML, string cause_analysisXML, string medical_leavesXML, string incidents_attachmentXML, out String error_Code)
        public async Task<string> save_incident_partc(WorkflowIncident incidents, string ireport_injuredpersonXML, string interviewedXML, string injury_detailsXML, string cause_analysisXML, string medical_leavesXML, string incidents_attachmentXML)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_save_incident_partc";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.incident_id);
                cmd.AddParameter("@p_negligent", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.negligent == null || incidents.negligent == string.Empty ? DBNull.Value : (object)incidents.negligent);
                cmd.AddParameter("@p_negligent_comments", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.negligent_comments == null || incidents.negligent_comments == string.Empty ? DBNull.Value : (object)incidents.negligent_comments);
                cmd.AddParameter("@p_recommend_action_desc", NpgsqlTypes.NpgsqlDbType.Text, incidents.recommend_action_desc == null || incidents.recommend_action_desc == string.Empty ? DBNull.Value : (object)incidents.recommend_action_desc);
                cmd.AddParameter("@p_what_why", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.what_happened_and_why_comments == null || incidents.what_happened_and_why_comments == string.Empty ? DBNull.Value : (object)incidents.what_happened_and_why_comments);
                cmd.AddParameter("@p_status", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.status);
                cmd.AddParameter("@p_modified_by", NpgsqlTypes.NpgsqlDbType.Varchar, incidents.modified_by);
                cmd.AddParameter("@p_injuredpersonxml", NpgsqlTypes.NpgsqlDbType.Xml, ireport_injuredpersonXML == null || ireport_injuredpersonXML == "" ? DBNull.Value : (object)ireport_injuredpersonXML);
                cmd.AddParameter("@p_interviewedxml", NpgsqlTypes.NpgsqlDbType.Xml, interviewedXML == null || interviewedXML == "" ? DBNull.Value : (object)interviewedXML);
                cmd.AddParameter("@p_injury_detailsxml", NpgsqlTypes.NpgsqlDbType.Xml, injury_detailsXML == null || injury_detailsXML == "" ? DBNull.Value : (object)injury_detailsXML);
                cmd.AddParameter("@p_cause_analysisxml", NpgsqlTypes.NpgsqlDbType.Xml, cause_analysisXML == null || cause_analysisXML == "" ? DBNull.Value : (object)cause_analysisXML);
                cmd.AddParameter("@p_medical_leavesxml", NpgsqlTypes.NpgsqlDbType.Xml, medical_leavesXML == null || medical_leavesXML == "" ? DBNull.Value : (object)medical_leavesXML);
                cmd.AddParameter("@p_incidents_attachmentxml", NpgsqlTypes.NpgsqlDbType.Xml, incidents_attachmentXML == null || incidents_attachmentXML == "" ? DBNull.Value : (object)incidents_attachmentXML);

                error_Code = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<string> update_incidents_attachfiles(int uid, string created_by)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_update_incidents_attachfiles";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_uid", uid);
                cmd.Parameters.AddWithValue("@p_created_by", created_by == null || created_by == "" ? DBNull.Value : (object)created_by);

                error_Code = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<string> insert_incidents_attachfiles(string strXML, string created_by)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_insert_attachfiles";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@p_strxml", strXML == null || strXML == "" ? DBNull.Value : (object)strXML);
                cmd.Parameters.AddWithValue("@p_created_by", created_by == null || created_by == "" ? DBNull.Value : (object)created_by);
                error_Code = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<string> insert_incidents_workflows(string incident_id, string strXML)//, string actions_code, string actions, string from, string to, string remarks, string Date, string attachment, out String error_Code)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_insert_workflows";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);
                cmd.AddParameter("@p_str_xml", NpgsqlTypes.NpgsqlDbType.Xml, strXML == null || strXML == "" ? DBNull.Value : (object)strXML);

                error_Code = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<string> insert_incidents_injured(string incident_id, string injured_emp_no, string injured_name, string injured_nric_fin_no,
        string injured_company, string injured_contact_no, string injured_age, string injured_race,
        string injured_race_oth, string injured_gender, string injured_nationality,
        string injured_designation, string injured_employment_type, string injured_employment_type_oth,
        string remarks)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            string error_Code = string.Empty;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_insert_incidents_injured";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@incident_id", incident_id);
                cmd.Parameters.AddWithValue("@injured_emp_no", injured_emp_no == null || injured_emp_no == string.Empty ? DBNull.Value : (object)injured_emp_no);
                cmd.Parameters.AddWithValue("@injured_name", injured_name == null || injured_name == string.Empty ? DBNull.Value : (object)injured_name);
                cmd.Parameters.AddWithValue("@injured_nric_fin_no", injured_nric_fin_no == null || injured_nric_fin_no == string.Empty ? DBNull.Value : (object)injured_nric_fin_no);
                cmd.Parameters.AddWithValue("@injured_company", injured_company == null || injured_company == string.Empty ? DBNull.Value : (object)injured_company);
                cmd.Parameters.AddWithValue("@injured_contact_no", injured_contact_no == null || injured_contact_no == string.Empty ? DBNull.Value : (object)injured_contact_no);
                cmd.Parameters.AddWithValue("@injured_age", injured_age == null || injured_age == string.Empty ? DBNull.Value : (object)injured_age);
                cmd.Parameters.AddWithValue("@injured_race", injured_race == null || injured_race == string.Empty ? DBNull.Value : (object)injured_race);
                cmd.Parameters.AddWithValue("@injured_race_oth", injured_race_oth == null || injured_race_oth == string.Empty ? DBNull.Value : (object)injured_race_oth);
                cmd.Parameters.AddWithValue("@injured_gender", injured_gender == null || injured_gender == string.Empty ? DBNull.Value : (object)injured_gender);
                cmd.Parameters.AddWithValue("@injured_nationality", injured_nationality == null || injured_nationality == string.Empty ? DBNull.Value : (object)injured_nationality);
                cmd.Parameters.AddWithValue("@injured_designation", injured_designation == null || injured_designation == string.Empty ? DBNull.Value : (object)injured_designation);
                cmd.Parameters.AddWithValue("@injured_employment_type", injured_employment_type == null || injured_employment_type == string.Empty ? DBNull.Value : (object)injured_employment_type);
                cmd.Parameters.AddWithValue("@injured_employment_type_oth", injured_employment_type_oth == null || injured_employment_type_oth == string.Empty ? DBNull.Value : (object)injured_employment_type_oth);
                cmd.Parameters.AddWithValue("@remarks", remarks == null || remarks == string.Empty ? DBNull.Value : (object)remarks);

                NpgsqlParameter err_Code = cmd.Parameters.Add("@errCode", NpgsqlTypes.NpgsqlDbType.Varchar);
                err_Code.Direction = ParameterDirection.Output;
                err_Code.Size = 1000;

                error_Code = cmd.Parameters["@errCode"].Value == DBNull.Value ? string.Empty : (string)cmd.Parameters["@errCode"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return error_Code;
        }

        public async Task<DataSet> get_printview_incident_by_id(string incident_id)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            //DataTable inctypedt = new DataTable();
            //DataTable injurydetailsdt = new DataTable();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_print_incidents_id";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return ds;
        }

        public async Task<DataSet> get_incident_by_id(WorkflowIncident incidents)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            DataSet ds = new DataSet();
            DataTable inctypedt = new DataTable();
            DataTable injurydetailsdt = new DataTable();
            string incident_id = incidents.incident_id;

            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.CommandText = "spc_get_incidents_by_incident_id";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);

                    ds = cmd.ExecuteDataSet(_dBHelper.GetConnection());

                    if (ds.Tables.Count > 0)
                    {
                        inctypedt.Merge(ds.Tables[0]);
                        injurydetailsdt.Merge(ds.Tables[1]);
                    }

                    incidents.incident_id = ds.Tables[0].Rows[0]["incident_id"].ToString();
                    incidents.incident_datetime = ds.Tables[0].Rows[0]["incident_datetime"].ToString();
                    incidents.incident_date = ds.Tables[0].Rows[0]["incident_date"].ToString();
                    incidents.incident_time = ds.Tables[0].Rows[0]["incident_time"].ToString();
                    incidents.sba_code = ds.Tables[0].Rows[0]["sba_code"].ToString();
                    incidents.sbu_code = ds.Tables[0].Rows[0]["sbu_code"].ToString();
                    incidents.sbu_name = ds.Tables[0].Rows[0]["sbu_name"].ToString();
                    if (ds.Tables[0].Rows[0]["division"] != DBNull.Value)
                    {
                        incidents.division = ds.Tables[0].Rows[0]["division"].ToString();
                        //incidents.division = ds.Tables[0].Rows[0]["division"].ToString();
                    }
                    else
                    {
                        incidents.division = null;
                        //incidents.division = ds.Tables[0].Rows[0]["division"].ToString();
                    }
                    if (ds.Tables[0].Rows[0]["department"] != DBNull.Value)
                    {
                        incidents.department = ds.Tables[0].Rows[0]["department"].ToString();
                        incidents.department_name = ds.Tables[0].Rows[0]["department_name"].ToString();
                    }
                    else
                    {
                        incidents.department = null;
                        incidents.department_name = string.Empty;
                    }
                    if (ds.Tables[0].Rows[0]["location"] != DBNull.Value)
                    {
                        incidents.location = ds.Tables[0].Rows[0]["location"].ToString();
                        incidents.location_name = ds.Tables[0].Rows[0]["location_name"].ToString();
                    }
                    else
                    {
                        incidents.location = null;
                        incidents.location_name = string.Empty;
                    }
                    if (ds.Tables[0].Rows[0]["exact_location"] != DBNull.Value)
                    {
                        incidents.exact_location = ds.Tables[0].Rows[0]["exact_location"].ToString();
                    }
                    else
                    {
                        incidents.exact_location = string.Empty;
                    }
                    incidents.incident_desc = ds.Tables[0].Rows[0]["incident_desc"].ToString();
                    incidents.superior_name = ds.Tables[0].Rows[0]["superior_name"].ToString();
                    incidents.superior_emp_no = ds.Tables[0].Rows[0]["superior_emp_no"].ToString();
                    incidents.superior_designation = ds.Tables[0].Rows[0]["superior_designation"].ToString();

                    incidents.created_by = ds.Tables[0].Rows[0]["created_by"].ToString();
                    incidents.creation_date = ds.Tables[0].Rows[0]["creation_date"].ToString();
                    //incidents.incident_desc = ds.Tables[0].Rows[0][""].ToString();

                    if (ds.Tables[0].Rows[0]["status"] != DBNull.Value)
                    {
                        incidents.status = ds.Tables[0].Rows[0]["status"].ToString();
                    }
                    else
                    {
                        incidents.status = null;
                    }

                    if (ds.Tables[0].Rows[0]["hod_id"] != DBNull.Value)
                    {
                        incidents.hod_id = ds.Tables[0].Rows[0]["hod_id"].ToString();
                    }
                    else
                    {
                        incidents.hod_id = null;
                    }

                    if (ds.Tables[0].Rows[0]["ahod_id"] != DBNull.Value)
                    {
                        incidents.ahod_id = ds.Tables[0].Rows[0]["ahod_id"].ToString();
                    }
                    else
                    {
                        incidents.ahod_id = null;
                    }

                    if (ds.Tables[0].Rows[0]["wsho_id"] != DBNull.Value)
                    {
                        incidents.wsho_id = ds.Tables[0].Rows[0]["wsho_id"].ToString();
                    }
                    else
                    {
                        incidents.wsho_id = null;
                    }

                    if (ds.Tables[0].Rows[0]["awsho_id"] != DBNull.Value)
                    {
                        incidents.awsho_id = ds.Tables[0].Rows[0]["awsho_id"].ToString();
                    }
                    else
                    {
                        incidents.awsho_id = null;
                    }

                    if (ds.Tables[0].Rows[0]["hhod_id"] != DBNull.Value)
                    {
                        incidents.hhod_id = ds.Tables[0].Rows[0]["hhod_id"].ToString();
                    }
                    else
                    {
                        incidents.hhod_id = null;
                    }

                    if (ds.Tables[0].Rows[0]["cwsho_id"] != DBNull.Value)
                    {
                        incidents.cwsho_id = ds.Tables[0].Rows[0]["cwsho_id"].ToString();
                    }
                    else
                    {
                        incidents.cwsho_id = null;
                    }

                    if (ds.Tables[0].Rows[0]["negligent"] != DBNull.Value)
                    {
                        incidents.negligent = ds.Tables[0].Rows[0]["negligent"].ToString();
                    }
                    else
                    {
                        incidents.negligent = null;
                    }

                    if (ds.Tables[0].Rows[0]["negligent_comments"] != DBNull.Value)
                    {
                        incidents.negligent_comments = ds.Tables[0].Rows[0]["negligent_comments"].ToString();
                    }
                    else
                    {
                        incidents.negligent_comments = null;
                    }

                    if (ds.Tables[0].Rows[0]["risk_assessment_review"] != DBNull.Value)
                    {
                        incidents.risk_assessment_review = ds.Tables[0].Rows[0]["risk_assessment_review"].ToString();
                    }
                    else
                    {
                        incidents.risk_assessment_review = null;
                    }

                    if (ds.Tables[0].Rows[0]["risk_assessment_review_comments"] != DBNull.Value)
                    {
                        incidents.risk_assessment_review_comments = ds.Tables[0].Rows[0]["risk_assessment_review_comments"].ToString();
                    }
                    else
                    {
                        incidents.risk_assessment_review_comments = null;
                    }

                    if (ds.Tables[0].Rows[0]["risk_assessment_review_desc"] != DBNull.Value)
                    {
                        incidents.risk_assessment_review_desc = ds.Tables[0].Rows[0]["risk_assessment_review_desc"].ToString();
                    }
                    else
                    {
                        incidents.risk_assessment_review_desc = null;
                    }

                    if (ds.Tables[0].Rows[0]["recommend_action_desc"] != DBNull.Value)
                    {
                        incidents.recommend_action_desc = ds.Tables[0].Rows[0]["recommend_action_desc"].ToString();
                    }
                    else
                    {
                        incidents.recommend_action_desc = null;
                    }

                    if (ds.Tables[0].Rows[0]["what_why"] != DBNull.Value)
                    {
                        incidents.what_happened_and_why_comments = ds.Tables[0].Rows[0]["what_why"].ToString();
                    }
                    else
                    {
                        incidents.what_happened_and_why_comments = null;
                    }

                    if (ds.Tables[0].Rows[0]["modified_by"] != DBNull.Value)
                    {
                        incidents.modified_by = ds.Tables[0].Rows[0]["modified_by"].ToString();
                    }
                    else
                    {
                        incidents.modified_by = null;
                    }
                    if (ds.Tables[0].Rows[0]["modify_date"] != DBNull.Value)
                    {
                        incidents.modify_date = ds.Tables[0].Rows[0]["modify_date"].ToString();
                    }
                    else
                    {
                        incidents.modify_date = null;
                    }

                    if (ds.Tables[0].Rows[0]["any_eyewitness"] != DBNull.Value)
                    {
                        incidents.any_eyewitness = Convert.ToInt16(ds.Tables[0].Rows[0]["any_eyewitness"].ToString());
                    }
                    else
                    {
                        incidents.any_eyewitness = null;
                    }

                    if (ds.Tables[0].Rows[0]["damage_description"] != DBNull.Value)
                    {
                        incidents.damage_description = ds.Tables[0].Rows[0]["damage_description"].ToString();
                    }
                    else
                    {
                        incidents.damage_description = null;
                    }

                    if (ds.Tables[0].Rows[0]["is_working_overtime"] != DBNull.Value)
                    {
                        incidents.is_working_overtime = ds.Tables[0].Rows[0]["is_working_overtime"].ToString();
                    }
                    else
                    {
                        incidents.is_working_overtime = null;
                    }

                    if (ds.Tables[0].Rows[0]["is_jobrelated"] != DBNull.Value)
                    {
                        incidents.is_jobrelated = ds.Tables[0].Rows[0]["is_jobrelated"].ToString();
                    }
                    else
                    {
                        incidents.is_jobrelated = null;
                    }

                    if (ds.Tables[0].Rows[0]["examined_hospital_clinic_name"] != DBNull.Value)
                    {
                        incidents.examined_hospital_clinic_name = ds.Tables[0].Rows[0]["examined_hospital_clinic_name"].ToString();
                    }
                    else
                    {
                        incidents.examined_hospital_clinic_name = null;
                    }
                    if (ds.Tables[0].Rows[0]["official_working_hrs"] != DBNull.Value)
                    {
                        incidents.official_working_hrs = ds.Tables[0].Rows[0]["official_working_hrs"].ToString();
                    }
                    else
                    {
                        incidents.official_working_hrs = null;
                    }

                    if (ds.Tables[0].Rows[0]["injured_case_type"] != DBNull.Value)
                    {
                        incidents.injured_case_type = ds.Tables[0].Rows[0]["injured_case_type"].ToString();
                    }
                    else
                    {
                        incidents.injured_case_type = null;
                    }

                    //if (ds.Tables[0].Rows[0][""] != DBNull.Value)
                    //{
                    //    incidents. = ds.Tables[0].Rows[0][""].ToString();
                    //}
                    //else
                    //{
                    //    incidents. = null;
                    //}
                    //if (ds.Tables[0].Rows[0][""] != DBNull.Value)
                    //{
                    //    incidents. = ds.Tables[0].Rows[0][""].ToString();
                    //}
                    //else
                    //{
                    //    incidents. = null;
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public async Task<DataSet> get_sendemaildata_by_id(string incident_id, string aFrom, string aTo)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            DataTable inctypedt = new DataTable();
            DataTable injurydetailsdt = new DataTable();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_get_sendemaildata_by_incident_id";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);
                cmd.AddParameter("@p_actionfrom", NpgsqlTypes.NpgsqlDbType.Varchar, aFrom);
                cmd.AddParameter("@p_actionto", NpgsqlTypes.NpgsqlDbType.Varchar, aTo);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return ds;
        }

        public async Task<DataSet> validate_user_to_edit_inc(string incident_id, string login_id, string changemode)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_validate_user_to_edit_inc";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);
                cmd.AddParameter("@p_login_id", NpgsqlTypes.NpgsqlDbType.Varchar, login_id);
                cmd.AddParameter("@p_change_mode", NpgsqlTypes.NpgsqlDbType.Varchar, changemode);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
            return ds;
        }

        public async Task<DataSet> validate_workflowuser(string incident_id, string login_id)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_validate_workflow";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);
                cmd.AddParameter("@p_login_id", NpgsqlTypes.NpgsqlDbType.Varchar, login_id);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
            return ds;
        }

        public async Task<DataSet> get_emaillist_MCChange(string incident_id, string login_id)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_get_emaillist_mc_change";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);
                //cmd.Parameters.AddWithValue("@login_id", login_id);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
            return ds;
        }

        public async Task<DataSet> get_injured_person_injury_description(string incident_id, string injured_id)
        {
            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.CommandText = "spc_get_injured_person_injury_description";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);
                    cmd.AddParameter("@p_injured_id", NpgsqlTypes.NpgsqlDbType.Varchar, injured_id);

                    ds = cmd.ExecuteDataSet(_dBHelper.GetConnection());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public async Task<DataSet> get_wirs_incidents_workflows_by_id(string incident_id, string status)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            DataSet ds = new DataSet();

            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.CommandText = "spc_get_wirs_incidents_workflows_by_id";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);
                    cmd.AddParameter("@p_status", NpgsqlTypes.NpgsqlDbType.Varchar, status == null || status == string.Empty ? DBNull.Value : (object)status);

                    ds = cmd.ExecuteDataSet(_dBHelper.GetConnection());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public async Task<DataSet> get_incident_partc_id(WorkflowIncident incidents)
        {
            DataSet ds = new DataSet();
            DataTable inctypedt = new DataTable();
            DataTable injurydetailsdt = new DataTable();
            string incident_id = incidents.incident_id;

            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.CommandText = "spc_get_partc_investigation";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);

                    ds = cmd.ExecuteDataSet(_dBHelper.GetConnection());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public async Task<DataSet> get_incidents_attachedfiles_by_id(string incident_id, string attachment_type, string reference_code)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_get_incidents_attachfiles";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_incident_id", NpgsqlTypes.NpgsqlDbType.Varchar, incident_id);
                cmd.AddParameter("@p_attachment_type", NpgsqlTypes.NpgsqlDbType.Varchar, attachment_type == null || attachment_type == string.Empty ? DBNull.Value : (object)attachment_type);
                cmd.AddParameter("@p_reference_code", NpgsqlTypes.NpgsqlDbType.Varchar, reference_code == null || reference_code == string.Empty ? DBNull.Value : (object)reference_code);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
            return ds;
        }

        public async Task<DataSet> search_incidents(string userid, string userrolecode, string incidentid, string sba, string sbu, string division, string incdatefrom, string incdateto, DataTable dt)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            string[] incidentTypeArray = dt.AsEnumerable()
            .Select(row => row["incident_type"].ToString())
            .ToArray();

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_search_incidents";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@p_userid", NpgsqlTypes.NpgsqlDbType.Varchar, userid);
                cmd.AddParameter("@p_userrolecode", NpgsqlTypes.NpgsqlDbType.Char, userrolecode);
                cmd.AddParameter("@p_incidentid", NpgsqlTypes.NpgsqlDbType.Varchar, incidentid);
                cmd.AddParameter("@p_sba", NpgsqlTypes.NpgsqlDbType.Varchar, sba);
                cmd.AddParameter("@p_sbu", NpgsqlTypes.NpgsqlDbType.Varchar, sbu);
                cmd.AddParameter("@p_division", NpgsqlTypes.NpgsqlDbType.Varchar, division);
                cmd.AddParameter("@p_incdatefrom", NpgsqlTypes.NpgsqlDbType.Varchar, incdatefrom);
                cmd.AddParameter("@p_incdateto", NpgsqlTypes.NpgsqlDbType.Varchar, incdateto);
                cmd.Parameters.AddWithValue("@p_incidenttype", NpgsqlTypes.NpgsqlDbType.Text | NpgsqlTypes.NpgsqlDbType.Array, incidentTypeArray);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }

            return ds;
        }

        public async Task<DataSet> isvalid_ireportinformation(string incident_id)
        {
            NpgsqlConnection con = _dBHelper.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "spc_isvalid_ireportinformation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@incident_id", incident_id);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter
                {
                    SelectCommand = cmd
                };
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Dispose();
                con.Close();
                con.Dispose();
            }
            return ds;
        }
    }
}