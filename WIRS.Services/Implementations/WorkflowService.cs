using Microsoft.AspNetCore.Mvc;
using System;
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
                var workflows = await this.GetIncidentWorkflowsAsync(incidentId);
                
                if (dataSet?.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    return null;

                var incident = MapDataSetToDetailModel(dataSet, workflows);
                
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

        private WorkflowIncidentDetailModel MapDataSetToDetailModel(DataSet dataSet, DataSet workflowDs)
        {
            var row = dataSet.Tables[0].Rows[0];
            var incidentTypes = new List<IncidentTypeModel>();
            var injuredPerson = new List<InjuredPersonModel>();
            var eyewitnesses = new List<EyewitnessModel>();
            var workflows = new List<IncidentWorkflowModel>();


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

            if (workflowDs != null && workflowDs.Tables.Count > 0)
            {
                foreach (DataRow item in workflowDs.Tables[0].Rows)
                {
                    workflows.Add(new IncidentWorkflowModel()
                    {
                        ActionCode = item["_actions_code"]?.ToString() ?? string.Empty,
                        ActionRole = item["_actions_role"]?.ToString() ?? string.Empty,
                        From = item["from"]?.ToString() ?? string.Empty,
                        FromName = item["from_name"]?.ToString() ?? string.Empty,
                        FromDesignation = item["from_designation"]?.ToString() ?? string.Empty,
                        To = item["_to"]?.ToString() ?? string.Empty,
                        ToName = item["to_name"]?.ToString() ?? string.Empty,
                        ToDesignation = item["to_designation"]?.ToString() ?? string.Empty,
                        Date = item["Date"]?.ToString() ?? string.Empty,
                        Remarks = item["remarks"]?.ToString() ?? string.Empty,
                    });
                }
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
                Workflows = workflows
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

            dt.Columns.Add("InjuredPerson_name", typeof(string));
            dt.Columns.Add("InjuredPerson_EmpNo", typeof(string));
            dt.Columns.Add("InjuredPerson_NRIC", typeof(string));
            dt.Columns.Add("InjuredPerson_Race", typeof(string));
            dt.Columns.Add("InjuredPerson_Gender", typeof(string));
            dt.Columns.Add("InjuredPerson_ContactNo", typeof(string));
            dt.Columns.Add("InjuredPerson_Age", typeof(string));
            dt.Columns.Add("InjuredPerson_Nationality", typeof(string));
            dt.Columns.Add("InjuredPerson_EmploymentType", typeof(string));
            dt.Columns.Add("InjuredPerson_DateofEmployment", typeof(string));
            dt.Columns.Add("InjuredPerson_Designation", typeof(string));
            dt.Columns.Add("Injured_Company", typeof(string));

            foreach (var person in injuredPersons)
            {
                DataRow row = dt.NewRow();
                row["InjuredPerson_name"] = person.Name ?? "";
                row["InjuredPerson_EmpNo"] = person.EmpNo ?? "";
                row["InjuredPerson_NRIC"] = person.NricFinNo ?? "";
                row["InjuredPerson_Race"] = person.Race ?? "";
                row["InjuredPerson_Gender"] = person.Gender ?? "";
                row["InjuredPerson_ContactNo"] = person.ContactNo ?? "";
                row["InjuredPerson_Age"] = person.Age ?? "";
                row["InjuredPerson_Nationality"] = person.Nationality ?? "";
                row["InjuredPerson_EmploymentType"] = person.EmploymentType ?? "";
                row["InjuredPerson_DateofEmployment"] = person.EmploymentDate ?? "";
                row["InjuredPerson_Designation"] = person.Designation ?? "";
                row["Injured_Company"] = person.Company ?? "";
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

        public async Task<string> SubmitPartBAsync(PartBSubmitModel model, string userId)
        {
            var incident = new WorkflowIncident { incident_id = model.IncidentId };
            var dataset = await _workflowIncidentDataAccess.get_incident_by_id(incident);

            if (dataset == null || dataset.Tables.Count == 0) {
                throw new Exception("The incident not found");
            }

            incident.status = "02";
            incident.injured_case_type = model.InjuredCaseType;

            var errorCode = await _workflowIncidentDataAccess.update_Incidents(incident);

            if (!string.IsNullOrEmpty(errorCode))
            {
                throw new Exception(errorCode);
            }
            else
            {
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

                List<(User, string)> assignedUsers = new List<(User, string)>();

                if (model.EmailToList != null && model.EmailToList.Count > 0)
                {
                    foreach (var emailTo in model.EmailToList)
                    {
                        assignedUsers.Add((new User() { UserId = emailTo, UserRole = "COPYTO"}, string.Empty));
                    }
                }

                if (model.AdditionalCopyToList != null && model.AdditionalCopyToList.Count > 0)
                {
                    foreach (var copyTo in model.AdditionalCopyToList)
                    {
                        assignedUsers.Add((new User() { UserId = copyTo.EmployeeNo, UserRole = "COPYTO" }, string.Empty));
                    }
                }

                if (!string.IsNullOrEmpty(model.WshoId))
                {
                    assignedUsers.Add((new User() { UserId = model.WshoId, UserRole = "WSHO" }, model.ReviewComment));
                }

                if (!string.IsNullOrEmpty(model.AlternateWshoId))
                {
                    assignedUsers.Add((new User() { UserId = model.AlternateWshoId, UserRole = "A_WSHO" }, string.Empty));
                }

                foreach (var assignedUser in assignedUsers)
                {
                    DataRow row = dt.NewRow();
                    row["incident_id"] = incident.incident_id;
                    row["actions_code"] = "02";
                    row["actions_role"] = assignedUser.Item1.UserRole;
                    row["from"] = userId;
                    row["to"] = assignedUser.Item1.UserId;
                    row["remarks"] = assignedUser.Item2;
                    row["Date"] = "";
                    row["attachment"] = "";
                    dt.Rows.Add(row);
                }

                workflow.Tables.Add(dt);

                return await _workflowIncidentDataAccess.insert_incidents_workflows(incident.incident_id, workflow.GetXml());
            }
        }

        public async Task<string> ClosePartBAsync(PartBSubmitModel model, string userId)
        {
            var incident = new WorkflowIncident { incident_id = model.IncidentId };
            var dataset = await _workflowIncidentDataAccess.get_incident_by_id(incident);

            if (dataset == null || dataset.Tables.Count == 0)
            {
                throw new Exception("The incident not found");
            }

            incident.status = "08";


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

            List<(User, string)> assignedUsers = new List<(User, string)>();
            string userAction = "CLOSE";
            assignedUsers.Add((new User() { UserId = userId, UserRole = userAction }, model.ReviewComment));

            if (model.EmailToList != null && model.EmailToList.Count > 0)
            {
                foreach (var emailTo in model.EmailToList)
                {
                    assignedUsers.Add((new User() { UserId = emailTo, UserRole = userAction }, string.Empty));
                }
            }

            if (model.AdditionalCopyToList != null && model.AdditionalCopyToList.Count > 0)
            {
                foreach (var copyTo in model.AdditionalCopyToList)
                {
                    assignedUsers.Add((new User() { UserId = copyTo.EmployeeNo, UserRole = userAction }, string.Empty));
                }
            }

            if (!string.IsNullOrEmpty(model.WshoId))
            {
                assignedUsers.Add((new User() { UserId = model.WshoId, UserRole = userAction }, string.Empty));
            }

            if (!string.IsNullOrEmpty(model.AlternateWshoId))
            {
                assignedUsers.Add((new User() { UserId = model.AlternateWshoId, UserRole = userAction }, string.Empty));
            }

            foreach (var assignedUser in assignedUsers)
            {
                DataRow row = dt.NewRow();
                row["incident_id"] = incident.incident_id;
                row["actions_code"] = incident.status;
                row["actions_role"] = assignedUser.Item1.UserRole;
                row["from"] = userId;
                row["to"] = assignedUser.Item1.UserId;
                row["remarks"] = assignedUser.Item2;
                row["Date"] = "";
                row["attachment"] = "";
                dt.Rows.Add(row);
            }

            workflow.Tables.Add(dt);

            return await _workflowIncidentDataAccess.insert_incidents_workflows(incident.incident_id, workflow.GetXml());
        }

        public async Task<string> SavePartCAsync(PartCSubmitModel model, string userId)
        {
            try
            {
                var incident = new WorkflowIncident
                {
                    incident_id = model.IncidentId,
                    negligent = model.IsNegligent,
                    negligent_comments = model.NegligentComments ?? string.Empty,
                    recommend_action_desc = model.RecommendedActions,
                    what_happened_and_why_comments = model.WhatHappenedAndWhy ?? string.Empty,
                    status = "02",
                    modified_by = userId
                };

                var ireportDs = ConvertIReportToDataSet(model);
                var interviewedDs = ConvertPersonsInterviewedToDataSet(model.PersonsInterviewed);
                var injuryDetailsDs = ConvertInjuryDetailsToDataSet(model);
                var causeAnalysisDs = ConvertCauseAnalysisToDataSet(model);
                var medicalLeavesDs = ConvertMedicalCertificatesToDataSet(model.IncidentId, model.MedicalCertificates);
                var workflowDs = new DataSet();
                var attachmentDs = new DataSet();

                var errorCode = await _workflowIncidentDataAccess.submit_incident_partc(
                    incident,
                    ireportDs.GetXml(),
                    interviewedDs.GetXml(),
                    injuryDetailsDs.GetXml(),
                    causeAnalysisDs.GetXml(),
                    medicalLeavesDs.GetXml(),
                    workflowDs.GetXml(),
                    attachmentDs.GetXml()
                );

                return errorCode;
            }
            catch (Exception)
            {
                return "ERROR_SAVE_PARTC";
            }
        }

        public async Task<string> SubmitPartCAsync(PartCSubmitModel model, string userId)
        {
            try
            {
                var incident = new WorkflowIncident
                {
                    incident_id = model.IncidentId,
                    negligent = model.IsNegligent,
                    negligent_comments = model.AdditionalComments ?? string.Empty,
                    recommend_action_desc = model.RecommendedActions,
                    what_happened_and_why_comments = model.WhatHappenedAndWhy ?? string.Empty,
                    status = "03",
                    modified_by = userId
                };

                var ireportDs = ConvertIReportToDataSet(model);
                var interviewedDs = ConvertPersonsInterviewedToDataSet(model.PersonsInterviewed);
                var injuryDetailsDs = ConvertInjuryDetailsToDataSet(model);
                var causeAnalysisDs = ConvertCauseAnalysisToDataSet(model);
                var medicalLeavesDs = ConvertMedicalCertificatesToDataSet(model.IncidentId, model.MedicalCertificates);
                var workflowDs = CreatePartCWorkflowDataSet(model.IncidentId, userId, model.CwshoId, model.AdditionalComments);
                var attachmentDs = new DataSet();

                var errorCode = await _workflowIncidentDataAccess.submit_incident_partc(
                    incident,
                    ireportDs.GetXml(),
                    interviewedDs.GetXml(),
                    injuryDetailsDs.GetXml(),
                    causeAnalysisDs.GetXml(),
                    medicalLeavesDs.GetXml(),
                    workflowDs.GetXml(),
                    attachmentDs.GetXml()
                );

                return errorCode;
            }
            catch (Exception)
            {
                return "ERROR_SUBMIT_PARTC";
            }
        }

        public Task<string> ClosePartCAsync(PartCCloseModel model, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> SubmitPartDAsync(PartDSubmitModel model, string userId)
        {
            throw new NotImplementedException();
        }

        private DataSet ConvertIReportToDataSet(PartCSubmitModel model)
        {
            var ds = new DataSet();
            var dt = new DataTable("incidents_injured");
            dt.Columns.Add("injured_emp_no", typeof(string));
            dt.Columns.Add("fourth_day_mc_date", typeof(string));
            dt.Columns.Add("ireport_no", typeof(string));
            dt.Columns.Add("ireport_date", typeof(string));

            foreach (var injuredDetail in model.InjuryDetails)
            {
                var row = dt.NewRow();
                row["injured_emp_no"] = injuredDetail.InjuredPersonId ?? string.Empty;
                row["fourth_day_mc_date"] = string.Empty;
                row["ireport_no"] = string.Empty;
                row["ireport_date"] = string.Empty;
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet ConvertPersonsInterviewedToDataSet(List<PersonInterviewedModel> personsInterviewed)
        {
            var ds = new DataSet();
            var dt = new DataTable("PersonInterviewed");
            dt.Columns.Add("empid", typeof(string));
            dt.Columns.Add("empname", typeof(string));
            dt.Columns.Add("empdesignation", typeof(string));
            dt.Columns.Add("empcontactno", typeof(string));

            if (personsInterviewed != null)
            {
                foreach (var person in personsInterviewed)
                {
                    var row = dt.NewRow();
                    row["empid"] = person.EmployeeNo ?? string.Empty;
                    row["empname"] = person.Name ?? string.Empty;
                    row["empdesignation"] = person.Designation ?? string.Empty;
                    row["empcontactno"] = person.ContactNo ?? string.Empty;
                    dt.Rows.Add(row);
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet ConvertInjuryDetailsToDataSet(PartCSubmitModel model)
        {
            var ds = new DataSet();
            var injuredDisplayDt = new DataTable("injured_details_display");
            injuredDisplayDt.Columns.Add("incident_id", typeof(string));
            injuredDisplayDt.Columns.Add("injured_id", typeof(string));
            injuredDisplayDt.Columns.Add("empname", typeof(string));
            injuredDisplayDt.Columns.Add("part_of_body_injured", typeof(string));
            injuredDisplayDt.Columns.Add("nature_injury", typeof(string));
            injuredDisplayDt.Columns.Add("injured_description", typeof(string));
            injuredDisplayDt.Columns.Add("attach_statement", typeof(string));
            injuredDisplayDt.Columns.Add("head_neck_torso", typeof(string));
            injuredDisplayDt.Columns.Add("upper_limbs", typeof(string));
            injuredDisplayDt.Columns.Add("lower_limps", typeof(string));
            foreach (var injury in model.InjuryDetails)
            {
                var row = injuredDisplayDt.NewRow();
                row["incident_id"] = model.IncidentId;
                row["injured_id"] = injury.InjuredPersonId;
                row["empname"] = injury.InjuredPersonName;
                row["part_of_body_injured"] = string.Empty;
                row["nature_injury"] = string.Empty;
                row["injured_description"] = injury.Description;
                row["attach_statement"] = string.Empty;
                row["head_neck_torso"] = string.Empty;
                row["upper_limbs"] = string.Empty;
                row["lower_limps"] = string.Empty;
                injuredDisplayDt.Rows.Add(row);
            }
            ds.Tables.Add(injuredDisplayDt);

            var dt = new DataTable("injured_details");
            dt.Columns.Add("incident_id", typeof(string));
            dt.Columns.Add("injured_id", typeof(string));
            dt.Columns.Add("injury_type", typeof(string));
            dt.Columns.Add("injury_code", typeof(string));
            dt.Columns.Add("injury_name", typeof(string));
            dt.Columns.Add("other_desc", typeof(string));

            if (model.InjuryDetails != null)
            {
                foreach (var injury in model.InjuryDetails)
                {
                    if (injury.HeadNeckTorso != null)
                    {
                        foreach (var code in injury.HeadNeckTorso)
                        {
                            var row = dt.NewRow();
                            row["incident_id"] = model.IncidentId;
                            row["injured_id"] = injury.InjuredPersonId ?? string.Empty;
                            row["injury_type"] = "Head Neck Torso";
                            row["injury_code"] = code;
                            row["injury_name"] = string.Empty;
                            row["other_desc"] = string.Empty;
                            dt.Rows.Add(row);
                        }
                    }

                    if (injury.UpperLimbs != null)
                    {
                        foreach (var code in injury.UpperLimbs)
                        {
                            var row = dt.NewRow();
                            row["incident_id"] = model.IncidentId;
                            row["injured_id"] = injury.InjuredPersonId ?? string.Empty;
                            row["injury_type"] = "Upper Limbs";
                            row["injury_code"] = code;
                            row["injury_name"] = string.Empty;
                            row["other_desc"] = string.Empty;
                            dt.Rows.Add(row);
                        }
                    }

                    if (injury.LowerLimbs != null)
                    {
                        foreach (var code in injury.LowerLimbs)
                        {
                            var row = dt.NewRow();
                            row["incident_id"] = model.IncidentId;
                            row["injured_id"] = injury.InjuredPersonId ?? string.Empty;
                            row["injury_type"] = "Lower Limbs";
                            row["injury_code"] = code;
                            row["injury_name"] = string.Empty;
                            row["other_desc"] = string.Empty;
                            dt.Rows.Add(row);
                        }
                    }

                    if (injury.NatureOfInjury != null)
                    {
                        foreach (var code in injury.NatureOfInjury)
                        {
                            var row = dt.NewRow();
                            row["incident_id"] = model.IncidentId;
                            row["injured_id"] = injury.InjuredPersonId ?? string.Empty;
                            row["injury_type"] = "Nature Of Injury";
                            row["injury_code"] = code;
                            row["injury_name"] = string.Empty;
                            row["other_desc"] = string.Empty;
                            dt.Rows.Add(row);
                        }
                    }
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet ConvertCauseAnalysisToDataSet(PartCSubmitModel model)
        {
            var ds = new DataSet();
            var dt = new DataTable("cause_analysis");
            dt.Columns.Add("lookup_type", typeof(string));
            dt.Columns.Add("lookup_code", typeof(string));
            dt.Columns.Add("type_description", typeof(string));

            if (model.IncidentClassList != null)
            {
                foreach (var code in model.IncidentClassList)
                {
                    var row = dt.NewRow();
                    row["lookup_type"] = "Incident Class";
                    row["lookup_code"] = code;
                    row["type_description"] = string.Empty;
                    dt.Rows.Add(row);
                }
            }

            if (model.IncidentAgentList != null)
            {
                foreach (var code in model.IncidentAgentList)
                {
                    var row = dt.NewRow();
                    row["lookup_type"] = "Incident Agent";
                    row["lookup_code"] = code;
                    row["type_description"] = string.Empty;
                    dt.Rows.Add(row);
                }
            }

            if (model.UnsafeConditionsList != null)
            {
                foreach (var code in model.UnsafeConditionsList)
                {
                    var row = dt.NewRow();
                    row["lookup_type"] = "Unsafe Condition";
                    row["lookup_code"] = code;
                    row["type_description"] = string.Empty;
                    dt.Rows.Add(row);
                }
            }

            if (model.UnsafeActsList != null)
            {
                foreach (var code in model.UnsafeActsList)
                {
                    var row = dt.NewRow();
                    row["lookup_type"] = "Unsafe Act";
                    row["lookup_code"] = code;
                    row["type_description"] = string.Empty;
                    dt.Rows.Add(row);
                }
            }

            if (model.ContributingFactorsList != null)
            {
                foreach (var code in model.ContributingFactorsList)
                {
                    var row = dt.NewRow();
                    row["lookup_type"] = "Factors";
                    row["lookup_code"] = code;
                    row["type_description"] = string.Empty;
                    dt.Rows.Add(row);
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet ConvertMedicalCertificatesToDataSet(string incidentId, List<MedicalCertificateModel> medicalCertificates)
        {
            var ds = new DataSet();
            var dt = new DataTable("injured_mc");
            dt.Columns.Add("incident_id", typeof(string));
            dt.Columns.Add("injured_id", typeof(string));
            dt.Columns.Add("injured_name", typeof(string));
            dt.Columns.Add("hospital_clinic_name", typeof(string));
            dt.Columns.Add("from_date", typeof(string));
            dt.Columns.Add("to_date", typeof(string));
            dt.Columns.Add("no_days", typeof(string));
            dt.Columns.Add("morethan24hrs", typeof(string));
            dt.Columns.Add("morethan24hrs_text", typeof(string));
            dt.Columns.Add("attachment", typeof(string));

            if (medicalCertificates != null)
            {
                foreach (var mc in medicalCertificates)
                {
                    var row = dt.NewRow();
                    row["incident_id"] = incidentId;
                    row["injured_id"] = mc.InjuredPersonId;
                    row["injured_name"] = mc.InjuredPersonName ?? string.Empty;
                    row["hospital_clinic_name"] = string.Empty;
                    row["from_date"] = mc.FromDate ?? string.Empty;
                    row["to_date"] = mc.ToDate ?? string.Empty;
                    row["no_days"] = mc.NumberOfDays.ToString() ?? string.Empty;
                    row["morethan24hrs"] = "0";
                    row["morethan24hrs_text"] = string.Empty;
                    row["attachment"] = mc.AttachmentPath ?? string.Empty;
                    dt.Rows.Add(row);
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet CreatePartCWorkflowDataSet(string incidentId, string fromUserId, string toUserId, string remarks)
        {
            var ds = new DataSet("NewDataSet");
            var dt = new DataTable("incidents_workflows");
            dt.Columns.Add("incident_id", typeof(string));
            dt.Columns.Add("actions_code", typeof(string));
            dt.Columns.Add("actions_role", typeof(string));
            dt.Columns.Add("from", typeof(string));
            dt.Columns.Add("to", typeof(string));
            dt.Columns.Add("remarks", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("attachment", typeof(string));

            var row = dt.NewRow();
            row["incident_id"] = incidentId;
            row["actions_code"] = "03";
            row["actions_role"] = "CWSHO";
            row["from"] = fromUserId;
            row["to"] = toUserId ?? string.Empty;
            row["remarks"] = remarks ?? string.Empty;
            row["Date"] = string.Empty;
            row["attachment"] = string.Empty;
            dt.Rows.Add(row);

            ds.Tables.Add(dt);
            return ds;
        }
    }
}