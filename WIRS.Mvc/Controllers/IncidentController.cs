using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using WIRS.Mvc.ViewModels;
using WIRS.Services.Auth;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;
using WIRS.Shared.Enums;
using WIRS.Shared.Models;

namespace WIRS.Mvc.Controllers
{
    public class IncidentController : BaseController
    {
        private readonly IWorkflowService _workflowService;
        private readonly IMasterDataService _masterDataService;

        public IncidentController(IAuthService authService, IWorkflowService workflowService, IMasterDataService masterDataService) : base(authService)
        {
            _workflowService = workflowService;
            _masterDataService = masterDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var viewModel = new IncidentCreateViewModel();
            
            try
            {
                await LoadCreateViewModelDropdowns(viewModel);
            }
            catch (Exception)
            {
                viewModel.ValidationMessage = "Error loading form data. Please try again.";
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IncidentCreateViewModel model)
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                await LoadCreateViewModelDropdowns(model);
                model.ValidationMessage = "Please correct the errors below.";
                return View(model);
            }

            try
            {
                var createModel = MapViewModelToCreateModel(model);
                var result = await _workflowService.CreateIncidentAsync(createModel, currentUser.UserId);

                if (!string.IsNullOrEmpty(result.incidentId) && string.IsNullOrEmpty(result.errorCode))
                {
                    return RedirectToAction("View", new { id = result.incidentId });
                }
                else
                {
                    await LoadCreateViewModelDropdowns(model);
                    model.ValidationMessage = "Failed to create incident. Please try again.";
                    return View(model);
                }
            }
            catch (Exception)
            {
                await LoadCreateViewModelDropdowns(model);
                model.ValidationMessage = "An error occurred while creating the incident.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> View(string id)
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var incident = await _workflowService.GetIncidentByIdAsync(id, currentUser.UserId);
                if (incident == null)
                {
                    return NotFound();
                }

                var viewModel = new IncidentViewViewModel
                {
                    Incident = incident,
                    StagePermissions = incident.StagePermissions,
                    WorkflowHistory = incident.Workflows,
                    Attachments = incident.Attachments,
                    CanEdit = incident.CanEdit,
                    CanWorkflow = incident.CanWorkflow
                };

                await LoadViewViewModelDropdowns(viewModel);
                SetStagePermissions(viewModel);
                SetCurrentStage(viewModel);

                return View(viewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIncident(IncidentViewViewModel model)
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please correct the errors below." });
            }

            try
            {
                if (model.Incident == null)
                {
                    return Json(new { success = false, message = "Invalid incident data" });
                }

                var updateModel = MapViewModelToUpdateModel(model);
                var result = await _workflowService.UpdateIncidentAsync(updateModel, currentUser.UserId);

                if (string.IsNullOrEmpty(result) || result.Contains("ERROR"))
                {
                    return Json(new { success = false, message = "Failed to update incident" });
                }

                return Json(new { success = true, message = "Incident updated successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating the incident" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitWorkflow(WorkflowActionViewModel model)
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            if (string.IsNullOrEmpty(model.IncidentId) || string.IsNullOrEmpty(model.Action))
            {
                return Json(new { success = false, message = "Invalid workflow data" });
            }

            try
            {
                string result = "";

                switch (model.Action.ToUpper())
                {
                    case "SUBMIT_PARTC":
                        if (model.PartCData != null)
                        {
                            result = await _workflowService.SubmitIncidentPartCAsync(model.PartCData, currentUser.UserId);
                        }
                        break;
                    default:
                        return Json(new { success = false, message = "Invalid workflow action" });
                }

                if (string.IsNullOrEmpty(result) || result.Contains("ERROR"))
                {
                    return Json(new { success = false, message = "Failed to submit workflow" });
                }

                return Json(new { success = true, message = "Workflow submitted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while processing the workflow" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var viewModel = new IncidentSearchViewModel();
            
            try
            {
                await LoadSearchViewModelDropdowns(viewModel);
            }
            catch (Exception)
            {
                // Continue with empty dropdowns
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SearchIncidents(IncidentSearchViewModel model)
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            try
            {
                var searchCriteria = new IncidentSearchModel
                {
                    IncidentId = model.IncidentId,
                    Sba = model.SbaCode,
                    Sbu = model.SbuCode,
                    Division = model.Division,
                    IncDateFrom = model.IncDateFrom,
                    IncDateTo = model.IncDateTo
                };

                var searchResult = await _workflowService.SearchIncidentsAsync(currentUser.UserId, UserRoleExtensions.GetRoleName(currentUser.UserRole), searchCriteria);
                var incidents = MapDataSetToSummaryList(searchResult);

                return Json(new { success = true, data = incidents });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while searching incidents" });
            }
        }

        private async Task LoadCreateViewModelDropdowns(IncidentCreateViewModel model)
        {
            model.SbaOptions = await _masterDataService.GetSectors(); // Using Sectors as SBA
            model.SbuOptions = new List<LookupItem>(); // Will be populated via AJAX
            model.DivisionOptions = new List<LookupItem>(); // Will be populated via AJAX  
            model.DepartmentOptions = new List<LookupItem>(); // Will be populated via AJAX
            model.LocationOptions = await _masterDataService.GetLocations();
            
            // Static dropdown options
            model.YesNoOptions = new List<LookupItem>
            {
                new LookupItem { Code = "1", Value = "Yes" },
                new LookupItem { Code = "0", Value = "No" }
            };
            
            model.OvertimeOptions = new List<LookupItem>
            {
                new LookupItem { Code = "Y", Value = "Yes" },
                new LookupItem { Code = "N", Value = "No" }
            };
            
            model.JobRelatedOptions = new List<LookupItem>
            {
                new LookupItem { Code = "Y", Value = "Yes" },
                new LookupItem { Code = "N", Value = "No" }
            };
        }

        private async Task LoadViewViewModelDropdowns(IncidentViewViewModel model)
        {
            model.SbaOptions = await _masterDataService.GetSectors(); // Using Sectors as SBA
            model.SbuOptions = new List<LookupItem>(); // Will be populated via AJAX
            model.DivisionOptions = new List<LookupItem>(); // Will be populated via AJAX
            model.DepartmentOptions = new List<LookupItem>(); // Will be populated via AJAX
            model.LocationOptions = await _masterDataService.GetLocations();
        }

        private async Task LoadSearchViewModelDropdowns(IncidentSearchViewModel model)
        {
            model.SbaOptions = await _masterDataService.GetSectors(); // Using Sectors as SBA
            model.SbuOptions = new List<LookupItem>(); // Will be populated via AJAX
            model.DivisionOptions = new List<LookupItem>(); // Will be populated via AJAX
        }

        private void SetStagePermissions(IncidentViewViewModel model)
        {
            if (model.Incident?.StagePermissions?.Any() == true)
            {
                var stageA = model.StagePermissions.FirstOrDefault(x => x.Stage == "A");
                var stageB = model.StagePermissions.FirstOrDefault(x => x.Stage == "B");
                var stageC = model.StagePermissions.FirstOrDefault(x => x.Stage == "C");

                model.CanViewPartA = stageA?.CanView ?? true;
                model.CanViewPartB = stageB?.CanView ?? false;
                model.CanViewPartC = stageC?.CanView ?? false;
                
                model.CanEditPartA = stageA?.CanEdit ?? false;
                model.CanEditPartB = stageB?.CanEdit ?? false;
                model.CanEditPartC = stageC?.CanEdit ?? false;
            }
        }

        private void SetCurrentStage(IncidentViewViewModel model)
        {
            if (model.Incident != null)
            {
                var currentStagePermission = model.StagePermissions.FirstOrDefault(x => x.IsCurrentStage);
                if (currentStagePermission != null)
                {
                    model.CurrentStage = currentStagePermission.Stage;
                    model.CurrentStageDescription = currentStagePermission.StageDescription;
                }
                else
                {
                    model.CurrentStage = "A";
                    model.CurrentStageDescription = "Initial Report";
                }
            }
        }

        private WorkflowIncidentCreateModel MapViewModelToCreateModel(IncidentCreateViewModel model)
        {
            return new WorkflowIncidentCreateModel
            {
                IncidentDateTime = model.IncidentDateTime,
                IncidentTime = model.IncidentTime,
                IncidentDate = model.IncidentDate,
                SbaCode = model.SbaCode,
                SbuCode = model.SbuCode,
                Division = model.Division,
                Department = model.Department,
                Location = model.Location,
                ExactLocation = model.ExactLocation,
                IncidentDesc = model.IncidentDesc,
                SuperiorName = model.SuperiorName,
                SuperiorEmpNo = model.SuperiorEmpNo,
                SuperiorDesignation = model.SuperiorDesignation,
                IncidentTypes = model.IncidentTypes,
                InjuredPersons = model.InjuredPersons,
                Eyewitnesses = model.Eyewitnesses,
                AnyEyewitness = model.AnyEyewitness,
                DamageDescription = model.DamageDescription,
                IsWorkingOvertime = model.IsWorkingOvertime,
                IsJobrelated = model.IsJobrelated,
                ExaminedHospitalClinicName = model.ExaminedHospitalClinicName,
                OfficialWorkingHrs = model.OfficialWorkingHrs,
                InjuredCaseType = model.InjuredCaseType
            };
        }

        private WorkflowIncidentUpdateModel MapViewModelToUpdateModel(IncidentViewViewModel model)
        {
            if (model.Incident == null)
                throw new ArgumentNullException(nameof(model.Incident));

            return new WorkflowIncidentUpdateModel
            {
                IncidentId = model.Incident.IncidentId,
                IncidentDateTime = model.Incident.IncidentDateTime,
                IncidentTime = model.Incident.IncidentTime,
                IncidentDate = model.Incident.IncidentDate,
                SbaCode = model.Incident.SbaCode,
                SbuCode = model.Incident.SbuCode,
                Division = model.Incident.Division,
                Department = model.Incident.Department,
                Location = model.Incident.Location,
                ExactLocation = model.Incident.ExactLocation,
                IncidentDesc = model.Incident.IncidentDesc,
                SuperiorName = model.Incident.SuperiorName,
                SuperiorEmpNo = model.Incident.SuperiorEmpNo,
                SuperiorDesignation = model.Incident.SuperiorDesignation,
                IncidentTypes = model.Incident.IncidentTypes,
                InjuredPersons = model.Incident.InjuredPersons,
                Eyewitnesses = model.Incident.Eyewitnesses,
                AnyEyewitness = model.Incident.AnyEyewitness,
                DamageDescription = model.Incident.DamageDescription,
                IsWorkingOvertime = model.Incident.IsWorkingOvertime,
                IsJobrelated = model.Incident.IsJobrelated,
                ExaminedHospitalClinicName = model.Incident.ExaminedHospitalClinicName,
                OfficialWorkingHrs = model.Incident.OfficialWorkingHrs,
                InjuredCaseType = model.Incident.InjuredCaseType,
                Status = model.Incident.Status,
                Negligent = model.Incident.Negligent,
                NegligentComments = model.Incident.NegligentComments,
                RecommendActionDesc = model.Incident.RecommendActionDesc,
                RiskAssessmentReview = model.Incident.RiskAssessmentReview,
                RiskAssessmentReviewDesc = model.Incident.RiskAssessmentReviewDesc,
                RiskAssessmentReviewComments = model.Incident.RiskAssessmentReviewComments,
                WhatHappenedAndWhyComments = model.Incident.WhatHappenedAndWhyComments
            };
        }

        private List<IncidentSummaryModel> MapDataSetToSummaryList(DataSet dataSet)
        {
            var incidents = new List<IncidentSummaryModel>();

            if (dataSet?.Tables?.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    incidents.Add(new IncidentSummaryModel
                    {
                        IncidentId = row["incident_id"]?.ToString() ?? "",
                        IncidentDateTime = row["incident_datetime"]?.ToString() ?? "",
                        SbuName = row["sbu_name"]?.ToString() ?? "",
                        DepartmentName = row["department_name"]?.ToString() ?? "",
                        LocationName = row["location_name"]?.ToString() ?? "",
                        IncidentDesc = row["incident_desc"]?.ToString() ?? "",
                        CreatedBy = row["created_by"]?.ToString() ?? "",
                        Status = row["status"]?.ToString() ?? "",
                        StatusDesc = row["status_desc"]?.ToString() ?? "",
                        SubmittedOn = row["submitted_on"]?.ToString() ?? "",
                        CanView = true, 
                        CanEdit = row["can_edit"]?.ToString() == "1"
                    });
                }
            }

            return incidents;
        }
    }
}