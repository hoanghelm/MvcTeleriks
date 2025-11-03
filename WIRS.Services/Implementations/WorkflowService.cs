using System.Data;
using System.Text.Json;
using WIRS.DataAccess.Entities;
using WIRS.DataAccess.Interfaces;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;

namespace WIRS.Services.Implementations
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowIncidentDataAccess _workflowIncidentDataAccess;
        private readonly IDataMapperService _dataMapper;

        public WorkflowService(IWorkflowIncidentDataAccess workflowIncidentDataAccess, IDataMapperService dataMapper)
        {
            _workflowIncidentDataAccess = workflowIncidentDataAccess;
            _dataMapper = dataMapper;
        }

        public async Task<(string incidentId, string errorCode)> CreateIncidentAsync(IncidentCreateModel model, string userId)
        {
            try
            {
                var incident = MapCreateModelToEntity(model, userId);
                var incidentTypeXml = ConvertIncidentTypesToXml(model.IncidentTypes);
                var injuredPersonXml = ConvertInjuredPersonsToXml(model.InjuredPersons);
                var eyewitnessXml = ConvertEyewitnessesToXml(model.Eyewitnesses);

                // init workflow
                DataSet workflow = new DataSet("NewDataSet");
                DataTable dt = new DataTable("incidents_workflows");

                dt.Columns.Add("incident_id", typeof(string));
                dt.Columns.Add("actions_code", typeof(string));
                dt.Columns.Add("actions_role", typeof(string));
                dt.Columns.Add("from", typeof(string));
                dt.Columns.Add("to", typeof(string));
                dt.Columns.Add("remarks", typeof(string));
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("attachment", typeof(string));

                List<User> assignedUsers = new List<User>();
                assignedUsers.Add(new User() { UserId = model.HodId, UserRole = "HOD" });
                
                if(!string.IsNullOrEmpty(model.AhodId))
                {
                    assignedUsers.Add(new User() { UserId = model.AhodId, UserRole = "AHOD" });
                }

                if (!string.IsNullOrEmpty(model.WshoId))
                {
                    assignedUsers.Add(new User() { UserId = model.WshoId, UserRole = "WSHO" });
                }

                if (model.CopyToList != null)
                {
                    foreach (var copyTo in model.CopyToList)
                    {
                        assignedUsers.Add(new User() { UserId = copyTo, UserRole = "COPYTO" });
                    }
                }

                foreach (var assignedUser in assignedUsers)
                {
                    DataRow row = dt.NewRow();
                    row["incident_id"] = ""; // for creation
                    row["actions_code"] = "01";
                    row["actions_role"] = assignedUser.UserRole;
                    row["from"] = userId;
                    row["to"] = assignedUser.UserId;
                    row["remarks"] = "";
                    row["Date"] = "";
                    row["attachment"] = "";
                    dt.Rows.Add(row);
                }

                workflow.Tables.Add(dt);

                return await _workflowIncidentDataAccess.insert_Incidents(
                    incident,
                    incidentTypeXml,
                    injuredPersonXml,
                    eyewitnessXml,
                    workflow.GetXml());
            }
            catch (Exception)
            {
                return (string.Empty, "ERROR_CREATE_INCIDENT");
            }
        }

        public async Task<WorkflowIncidentDetailModel?> GetIncidentByIdAsync(string incidentId, string userId)
        {
            try
            {
                var workflowIncident = new WorkflowIncident { incident_id = incidentId };
                var dataSet = await _workflowIncidentDataAccess.get_incident_by_id(workflowIncident);
                
                if (dataSet?.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    return null;

                var incident = MapDataSetToDetailModel(dataSet);
                
                incident.CanEdit = await CanUserEditIncidentAsync(incidentId, userId);
                incident.CanWorkflow = await CanUserWorkflowIncidentAsync(incidentId, userId);
                incident.StagePermissions = await GetIncidentStagePermissionsAsync(incidentId, userId);

                return incident;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CanUserEditIncidentAsync(string incidentId, string userId, string changeMode = "")
        {
            try
            {
                var result = await _workflowIncidentDataAccess.validate_user_to_edit_inc(incidentId, userId, changeMode);
                return result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CanUserWorkflowIncidentAsync(string incidentId, string userId)
        {
            try
            {
                var result = await _workflowIncidentDataAccess.validate_workflowuser(incidentId, userId);
                return result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> UpdateIncidentAsync(WorkflowIncidentUpdateModel model, string userId)
        {
            try
            {
                var workflowIncident = MapUpdateModelToEntity(model, userId);
                var incidentTypeXml = ConvertIncidentTypesToXml(model.IncidentTypes);
                var eyewitnessXml = ConvertEyewitnessesToXml(model.Eyewitnesses);

                return await _workflowIncidentDataAccess.update_incidents_header(workflowIncident, incidentTypeXml, eyewitnessXml);
            }
            catch (Exception)
            {
                return "ERROR_UPDATE_INCIDENT";
            }
        }

        public async Task<string> SubmitIncidentPartCAsync(WorkflowIncidentPartCModel model, string userId)
        {
            try
            {
                var workflowIncident = new WorkflowIncident 
                { 
                    incident_id = model.IncidentId,
                    modified_by = userId,
                    modify_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var injuredPersonXml = ConvertInjuredPersonsToXml(new List<InjuredPersonModel>());
                var interviewedXml = ConvertIntervieweesToXml(model.Interviewees);
                var injuryDetailsXml = ConvertInjuryDetailsToXml(model.InjuryDetails);
                var causeAnalysisXml = ConvertCauseAnalysisToXml(model.CauseAnalysis);
                var medicalLeavesXml = ConvertMedicalLeavesToXml(model.MedicalLeaves);
                var workflowsXml = ConvertWorkflowsToXml(model.Workflows);
                var attachmentsXml = ConvertAttachmentsToXml(model.Attachments);

                return await _workflowIncidentDataAccess.submit_incident_partc(
                    workflowIncident,
                    injuredPersonXml,
                    interviewedXml,
                    injuryDetailsXml,
                    causeAnalysisXml,
                    medicalLeavesXml,
                    workflowsXml,
                    attachmentsXml);
            }
            catch (Exception)
            {
                return "ERROR_SUBMIT_PARTC";
            }
        }

        public async Task<DataSet> GetIncidentWorkflowsAsync(string incidentId, string status = "")
        {
            try
            {
                return await _workflowIncidentDataAccess.get_wirs_incidents_workflows_by_id(incidentId, status);
            }
            catch (Exception)
            {
                return new DataSet();
            }
        }

        public async Task<List<IncidentStagePermissionModel>> GetIncidentStagePermissionsAsync(string incidentId, string userId)
        {
            var permissions = new List<IncidentStagePermissionModel>();

            try
            {
                var incident = new WorkflowIncident { incident_id = incidentId };
                var dataSet = await _workflowIncidentDataAccess.get_incident_by_id(incident);
                
                if (dataSet?.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    var currentStatus = dataSet.Tables[0].Rows[0]["status"]?.ToString() ?? "";
                    
                    permissions.Add(new IncidentStagePermissionModel
                    {
                        Stage = "01",
                        StageDescription = "Initial Report",
                        CanView = true,
                        CanEdit = await CanUserEditIncidentAsync(incidentId, userId),
                        IsCurrentStage = currentStatus == "01" || string.IsNullOrEmpty(currentStatus),
                        RequiredRole = "Reporter"
                    });

                    permissions.Add(new IncidentStagePermissionModel
                    {
                        Stage = "02",
                        StageDescription = "Investigation",
                        CanView = !string.IsNullOrEmpty(currentStatus),
                        CanEdit = await CanUserWorkflowIncidentAsync(incidentId, userId) && (currentStatus == "02" || currentStatus == "03"),
                        IsCurrentStage = currentStatus == "02",
                        RequiredRole = "Investigator"
                    });

                    permissions.Add(new IncidentStagePermissionModel
                    {
                        Stage = "03",
                        StageDescription = "Final Report",
                        CanView = currentStatus == "03" || currentStatus == "04",
                        CanEdit = await CanUserWorkflowIncidentAsync(incidentId, userId) && currentStatus == "03",
                        IsCurrentStage = currentStatus == "03",
                        RequiredRole = "Approver"
                    });
                }
            }
            catch (Exception)
            {

            }

            return permissions;
        }

        public async Task<DataSet> SearchIncidentsAsync(string userId, string userRoleCode, IncidentSearchModel searchCriteria)
        {
            try
            {
                return await _workflowIncidentDataAccess.search_incidents(
                    userId,
                    userRoleCode,
                    searchCriteria.IncidentId,
                    searchCriteria.Sba,
                    searchCriteria.Sbu,
                    searchCriteria.Division,
                    searchCriteria.IncDateFrom,
                    searchCriteria.IncDateTo,
                    searchCriteria.SearchFilters ?? new DataTable());
            }
            catch (Exception)
            {
                return new DataSet();
            }
        }

        private WorkflowIncident MapCreateModelToEntity(IncidentCreateModel model, string userId)
        {
            return new WorkflowIncident
            {
                incident_datetime = model.IncidentDateTime,
                incident_time = model.IncidentTime,
                incident_date = model.IncidentDate,
                sba_code = model.SbaCode,
                sbu_code = model.SbuCode,
                division = model.Division,
                department = model.Department,
                location = model.Location,
                exact_location = model.ExactLocation,
                incident_desc = model.IncidentDesc,
                superior_name = model.SuperiorName,
                superior_emp_no = model.SuperiorEmpNo,
                superior_designation = model.SuperiorDesignation,
                any_eyewitness = model.AnyEyewitness,
                damage_description = model.DamageDescription,
                is_working_overtime = model.IsWorkingOvertime,
                is_jobrelated = model.IsJobrelated,
                examined_hospital_clinic_name = model.ExaminedHospitalClinicName,
                official_working_hrs = model.OfficialWorkingHrs,
                injured_case_type = model.InjuredCaseType,
                status = "01",
                created_by = userId,
                creation_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        private WorkflowIncident MapUpdateModelToEntity(WorkflowIncidentUpdateModel model, string userId)
        {
            return new WorkflowIncident
            {
                incident_id = model.IncidentId,
                incident_datetime = model.IncidentDateTime,
                incident_time = model.IncidentTime,
                incident_date = model.IncidentDate,
                sba_code = model.SbaCode,
                sbu_code = model.SbuCode,
                division = model.Division,
                department = model.Department,
                location = model.Location,
                exact_location = model.ExactLocation,
                incident_desc = model.IncidentDesc,
                superior_name = model.SuperiorName,
                superior_emp_no = model.SuperiorEmpNo,
                superior_designation = model.SuperiorDesignation,
                any_eyewitness = model.AnyEyewitness,
                damage_description = model.DamageDescription,
                is_working_overtime = model.IsWorkingOvertime,
                is_jobrelated = model.IsJobrelated,
                examined_hospital_clinic_name = model.ExaminedHospitalClinicName,
                official_working_hrs = model.OfficialWorkingHrs,
                injured_case_type = model.InjuredCaseType,
                negligent = model.Negligent,
                negligent_comments = model.NegligentComments,
                recommend_action_desc = model.RecommendActionDesc,
                risk_assessment_review = model.RiskAssessmentReview,
                risk_assessment_review_desc = model.RiskAssessmentReviewDesc,
                risk_assessment_review_comments = model.RiskAssessmentReviewComments,
                what_happened_and_why_comments = model.WhatHappenedAndWhyComments,
                status = model.Status,
                modified_by = userId,
                modify_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        private WorkflowIncidentDetailModel MapDataSetToDetailModel(DataSet dataSet)
        {
            var row = dataSet.Tables[0].Rows[0];
            var incidentTypes = new List<IncidentTypeModel>();
            var injuredPerson = new List<InjuredPersonModel>();
            var eyewitnesses = new List<EyewitnessModel>();

            foreach (DataRow typeRow in dataSet.Tables[1].Rows)
            {
                incidentTypes.Add(new IncidentTypeModel()
                {
                    Type = typeRow["lookup_code"]?.ToString(),
                    Description = typeRow["lookup_value"]?.ToString()
                });
            }

            foreach (DataRow injuredRow in dataSet.Tables[2].Rows)
            {
                injuredPerson.Add(new InjuredPersonModel()
                {
                    EmpNo = injuredRow["injured_emp_no"]?.ToString() ?? string.Empty,
                    Name = injuredRow["injured_name"]?.ToString() ?? string.Empty,
                    NricFinNo = injuredRow["injured_nric_fin_no"]?.ToString() ?? string.Empty,
                    Company = injuredRow["injured_company"]?.ToString() ?? string.Empty,
                    ContactNo = injuredRow["injured_contact_no"]?.ToString() ?? string.Empty,
                    Age = injuredRow["injured_age"]?.ToString() ?? string.Empty,
                    Race = injuredRow["injured_race"]?.ToString() ?? string.Empty,
                    RaceOther = injuredRow["injured_race_oth"]?.ToString() ?? string.Empty,
                    Gender = injuredRow["injured_gender"]?.ToString() ?? string.Empty,
                    Nationality = injuredRow["injured_nationality"]?.ToString() ?? string.Empty,
                    Designation = injuredRow["injured_designation"]?.ToString() ?? string.Empty,
                    EmploymentType = injuredRow["injured_employment_type"]?.ToString() ?? string.Empty,
                    EmploymentTypeOther = injuredRow["injured_employment_type_oth"]?.ToString() ?? string.Empty,
                    EmploymentDate = injuredRow["injured_employment_date"]?.ToString() ?? string.Empty,
                    Remarks = injuredRow["Remarks"]?.ToString() ?? string.Empty
                });
            }

            foreach (DataRow eyeRow in dataSet.Tables[3].Rows)
            {
                eyewitnesses.Add(new EyewitnessModel()
                {
                    EmpNo = eyeRow["empid"]?.ToString() ?? string.Empty,
                    Name = eyeRow["empname"]?.ToString() ?? string.Empty,
                    ContactNo = eyeRow["empcontactno"]?.ToString() ?? string.Empty,
                    Designation = eyeRow["empdesignation"]?.ToString() ?? string.Empty
                });
            }

            return new WorkflowIncidentDetailModel
            {
                IncidentId = row["incident_id"]?.ToString() ?? "",
                IncidentDateTime = row["incident_datetime"]?.ToString() ?? "",
                IncidentTime = row["incident_time"]?.ToString() ?? "",
                IncidentDate = row["incident_date"]?.ToString() ?? "",
                SbaCode = row["sba_code"]?.ToString() ?? "",
                SbuCode = row["sbu_code"]?.ToString() ?? "",
                SbuName = row["sbu_name"]?.ToString() ?? "",
                Division = row["division"]?.ToString() ?? "",
                Department = row["department"]?.ToString() ?? "",
                DepartmentName = row["department_name"]?.ToString() ?? "",
                Location = row["location"]?.ToString() ?? "",
                LocationName = row["location_name"]?.ToString() ?? "",
                ExactLocation = row["exact_location"]?.ToString() ?? "",
                IncidentDesc = row["incident_desc"]?.ToString() ?? "",
                SuperiorName = row["superior_name"]?.ToString() ?? "",
                SuperiorEmpNo = row["superior_emp_no"]?.ToString() ?? "",
                SuperiorDesignation = row["superior_designation"]?.ToString() ?? "",
                Status = row["status"]?.ToString() ?? "",
                CreatedBy = row["created_by"]?.ToString() ?? "",
                CreationDate = row["creation_date"]?.ToString() ?? "",
                ModifiedBy = row["modified_by"]?.ToString() ?? "",
                ModifyDate = row["modify_date"]?.ToString() ?? "",
                AnyEyewitness = row["any_eyewitness"] != DBNull.Value ? Convert.ToInt32(row["any_eyewitness"]) : null,
                DamageDescription = row["damage_description"]?.ToString() ?? "",
                IsWorkingOvertime = row["is_working_overtime"]?.ToString() ?? "",
                IsJobrelated = row["is_jobrelated"]?.ToString() ?? "",
                ExaminedHospitalClinicName = row["examined_hospital_clinic_name"]?.ToString() ?? "",
                OfficialWorkingHrs = row["official_working_hrs"]?.ToString() ?? "",
                InjuredCaseType = row["injured_case_type"]?.ToString() ?? "",
                Negligent = row["negligent"]?.ToString() ?? "",
                NegligentComments = row["negligent_comments"]?.ToString() ?? "",
                RecommendActionDesc = row["recommend_action_desc"]?.ToString() ?? "",
                RiskAssessmentReview = row["risk_assessment_review"]?.ToString() ?? "",
                RiskAssessmentReviewDesc = row["risk_assessment_review_desc"]?.ToString() ?? "",
                RiskAssessmentReviewComments = row["risk_assessment_review_comments"]?.ToString() ?? "",
                WhatHappenedAndWhyComments = row["what_why"]?.ToString() ?? "",
                IncidentTypes = incidentTypes,
                InjuredPersons = injuredPerson,
                Eyewitnesses = eyewitnesses,
            };
        }

        private string ConvertIncidentTypesToXml(List<IncidentTypeModel> incidentTypes)
        {
            if (incidentTypes?.Any() != true) return string.Empty;

            DataSet ds = new DataSet("NewDataSet");
            DataTable dt = new DataTable("incidenttype");

            dt.Columns.Add("incident_type", typeof(string));
            dt.Columns.Add("type_description", typeof(string));

            foreach (var type in incidentTypes)
            {
                DataRow row = dt.NewRow();
                row["incident_type"] = type.Type ?? "";
                row["type_description"] = type.Description ?? "";
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            return ds.GetXml();
        }

        private string ConvertInjuredPersonsToXml(List<InjuredPersonModel> injuredPersons)
        {
            if (injuredPersons?.Any() != true) return string.Empty;

            DataSet ds = new DataSet("NewDataSet");
            DataTable dt = new DataTable("Table1");

            dt.Columns.Add("injured_name", typeof(string));
            dt.Columns.Add("injured_emp_no", typeof(string));
            dt.Columns.Add("injured_nric_fin_no", typeof(string));
            dt.Columns.Add("injured_race", typeof(string));
            dt.Columns.Add("injured_gender", typeof(string));
            dt.Columns.Add("injured_contact_no", typeof(string));
            dt.Columns.Add("injured_age_text", typeof(string));
            dt.Columns.Add("injured_nationality", typeof(string));
            dt.Columns.Add("injured_employment_type", typeof(string));
            dt.Columns.Add("injured_employment_date_text", typeof(string));
            dt.Columns.Add("injured_designation", typeof(string));
            dt.Columns.Add("injured_company", typeof(string));

            foreach (var person in injuredPersons)
            {
                DataRow row = dt.NewRow();
                row["injured_name"] = person.Name ?? "";
                row["injured_emp_no"] = person.EmpNo ?? "";
                row["injured_nric_fin_no"] = person.NricFinNo ?? "";
                row["injured_race"] = person.Race ?? "";
                row["injured_gender"] = person.Gender ?? "";
                row["injured_contact_no"] = person.ContactNo ?? "";
                row["injured_age_text"] = person.Age ?? "";
                row["injured_nationality"] = person.Nationality ?? "";
                row["injured_employment_type"] = person.EmploymentType ?? "";
                row["injured_employment_date_text"] = person.EmploymentDate ?? "";
                row["injured_designation"] = person.Designation ?? "";
                row["injured_company"] = person.Company ?? "";
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            return ds.GetXml();
        }

        private string ConvertEyewitnessesToXml(List<EyewitnessModel> eyewitnesses)
        {
            if (eyewitnesses?.Any() != true) return string.Empty;

            DataSet ds = new DataSet("NewDataSet");
            DataTable dt = new DataTable("EyeWitnesses");

            dt.Columns.Add("empid", typeof(string));
            dt.Columns.Add("empname", typeof(string));
            dt.Columns.Add("empdesignation", typeof(string));
            dt.Columns.Add("empcontactno", typeof(string));

            foreach (var eyewitness in eyewitnesses)
            {
                DataRow row = dt.NewRow();
                row["empid"] = eyewitness.EmpNo ?? "";
                row["empname"] = eyewitness.Name ?? "";
                row["empdesignation"] = eyewitness.Designation ?? "";
                row["empcontactno"] = eyewitness.ContactNo ?? "";
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            return ds.GetXml();
        }

        private string ConvertIntervieweesToXml(List<IntervieweeModel> interviewees)
        {
            if (interviewees?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(interviewees);
        }

        private string ConvertInjuryDetailsToXml(List<InjuryDetailModel> injuryDetails)
        {
            if (injuryDetails?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(injuryDetails);
        }

        private string ConvertCauseAnalysisToXml(List<CauseAnalysisModel> causeAnalysis)
        {
            if (causeAnalysis?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(causeAnalysis);
        }

        private string ConvertMedicalLeavesToXml(List<MedicalLeaveModel> medicalLeaves)
        {
            if (medicalLeaves?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(medicalLeaves);
        }

        private string ConvertWorkflowsToXml(List<IncidentWorkflowModel> workflows)
        {
            if (workflows?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(workflows);
        }

        private string ConvertAttachmentsToXml(List<IncidentAttachmentModel> attachments)
        {
            if (attachments?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(attachments);
        }

        public Task<string> SubmitPartBAsync(PartBSubmitModel model, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> SavePartCAsync(PartCSubmitModel model, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> SubmitPartCAsync(PartCSubmitModel model, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> ClosePartCAsync(PartCCloseModel model, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> SubmitPartDAsync(PartDSubmitModel model, string userId)
        {
            throw new NotImplementedException();
        }
    }
}