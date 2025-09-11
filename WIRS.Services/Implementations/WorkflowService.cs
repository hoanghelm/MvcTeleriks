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

        public async Task<(string incidentId, string errorCode)> CreateIncidentAsync(WorkflowIncidentCreateModel model, string userId)
        {
            try
            {
                var workflowIncident = MapCreateModelToEntity(model, userId);
                var incidentTypeXml = ConvertIncidentTypesToXml(model.IncidentTypes);
                var injuredPersonXml = ConvertInjuredPersonsToXml(model.InjuredPersons);
                var eyewitnessXml = ConvertEyewitnessesToXml(model.Eyewitnesses);
                var workflowXml = CreateInitialWorkflowXml(userId);

                return await _workflowIncidentDataAccess.insert_Incidents(
                    workflowIncident,
                    incidentTypeXml,
                    injuredPersonXml,
                    eyewitnessXml,
                    workflowXml);
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
                        Stage = "A",
                        StageDescription = "Initial Report",
                        CanView = true,
                        CanEdit = await CanUserEditIncidentAsync(incidentId, userId),
                        IsCurrentStage = currentStatus == "A" || string.IsNullOrEmpty(currentStatus),
                        RequiredRole = "Reporter"
                    });

                    permissions.Add(new IncidentStagePermissionModel
                    {
                        Stage = "B",
                        StageDescription = "Investigation",
                        CanView = !string.IsNullOrEmpty(currentStatus),
                        CanEdit = await CanUserWorkflowIncidentAsync(incidentId, userId) && (currentStatus == "B" || currentStatus == "C"),
                        IsCurrentStage = currentStatus == "B",
                        RequiredRole = "Investigator"
                    });

                    permissions.Add(new IncidentStagePermissionModel
                    {
                        Stage = "C",
                        StageDescription = "Final Report",
                        CanView = currentStatus == "C" || currentStatus == "D",
                        CanEdit = await CanUserWorkflowIncidentAsync(incidentId, userId) && currentStatus == "C",
                        IsCurrentStage = currentStatus == "C",
                        RequiredRole = "Approver"
                    });
                }
            }
            catch (Exception)
            {
                // Return default permissions on error
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

        private WorkflowIncident MapCreateModelToEntity(WorkflowIncidentCreateModel model, string userId)
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
                status = "A",
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
                WhatHappenedAndWhyComments = row["what_happened_and_why_comments"]?.ToString() ?? ""
            };
        }

        private string ConvertIncidentTypesToXml(List<IncidentTypeModel> incidentTypes)
        {
            if (incidentTypes?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(incidentTypes);
        }

        private string ConvertInjuredPersonsToXml(List<InjuredPersonModel> injuredPersons)
        {
            if (injuredPersons?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(injuredPersons);
        }

        private string ConvertEyewitnessesToXml(List<EyewitnessModel> eyewitnesses)
        {
            if (eyewitnesses?.Any() != true) return string.Empty;
            
            return JsonSerializer.Serialize(eyewitnesses);
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

        private string CreateInitialWorkflowXml(string userId)
        {
            var initialWorkflow = new List<IncidentWorkflowModel>
            {
                new()
                {
                    Status = "A",
                    UserId = userId,
                    ActionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ActionType = "CREATE",
                    Comments = "Incident created"
                }
            };
            
            return JsonSerializer.Serialize(initialWorkflow);
        }
    }
}