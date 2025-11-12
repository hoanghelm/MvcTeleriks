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

        [HttpPost]
        public async Task<IActionResult> CreateApi([FromBody] IncidentCreateApiRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null)
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                var createModel = new IncidentCreateModel
                {
                    IncidentDateTime = $"{request.IncidentDate} {request.IncidentTime}",
                    IncidentTime = request.IncidentTime,
                    IncidentDate = request.IncidentDate,
                    SbaCode = request.SectorCode,
                    SbuCode = request.LobCode,
                    Division = request.DepartmentCode ?? string.Empty,
                    Department = request.DepartmentCode ?? string.Empty,
                    Location = request.LocationCode ?? string.Empty,
                    ExactLocation = request.ExactLocation ?? string.Empty,
                    IncidentDesc = request.IncidentDescription,
                    SuperiorName = request.SuperiorName ?? currentUser.UserName,
                    SuperiorEmpNo = request.SuperiorEmpNo ?? currentUser.UserId,
                    SuperiorDesignation = request.SuperiorDesignation ?? string.Empty,
                    IncidentTypes = new List<IncidentTypeModel>()
                    {
                        new IncidentTypeModel()
                        {
                            Type = request.IncidentType
                        }
                    },
                    InjuredPersons = request.InjuredPersons?.Select(ip => new Services.Models.InjuredPersonModel()
                    {
                        Name = ip.Name,
                        EmpNo = ip.EmployeeNo,
                        ContactNo = ip.ContactNo,
                        Age = ip.Age,
                        Company = ip.Company,
                        Race = ip.Race,
                        Nationality = ip.Nationality,
                        Designation = ip.Designation,
                        EmploymentType = ip.EmploymentType,
                        Gender = ip.Gender,
                        EmploymentTypeOther = ip.Type,
                        EmploymentDate = ip.DateOfEmployment,

                    }).ToList() ?? new List<Services.Models.InjuredPersonModel>(),
                    Eyewitnesses = request.Eyewitnesses?.Select(e => new Services.Models.EyewitnessModel()
                    {
                        ContactNo = e.ContactNo,
                        Designation = e.Designation,
                        EmpNo = e.EmployeeNo,
                        Name = e.Name
                    }).ToList() ?? new List<Services.Models.EyewitnessModel>(),
                    AnyEyewitness = request.HasEyeWitness ? 1 : 0,
                    DamageDescription = request.DamageDescription ?? string.Empty,
                    IsWorkingOvertime = request.WorkingOvertime ?? string.Empty,
                    IsJobrelated = request.IsJobRelated ?? string.Empty,
                    ExaminedHospitalClinicName = request.HospitalClinicName ?? string.Empty,
                    OfficialWorkingHrs = request.OfficialWorkingHours ?? string.Empty,
                    HodId = request.HodId,
                    WshoId = request.WshoId ?? string.Empty,
                    AhodId = request.AhodId ?? string.Empty,
                    CopyToList = request.CopyToList ?? new List<string>()
                };

                var result = await _workflowService.CreateIncidentAsync(createModel, currentUser.UserId);

                if (!string.IsNullOrEmpty(result.incidentId) && string.IsNullOrEmpty(result.errorCode))
                {
                    return Json(new { success = true, incidentId = result.incidentId, message = "Incident created successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result.errorCode ?? "Failed to create incident" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while creating the incident", error = ex.Message });
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

        [HttpGet]
        public async Task<IActionResult> Print(string id)
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
                var htmlContent = await _workflowService.GetPrintViewHtmlAsync(id);

                if (string.IsNullOrEmpty(htmlContent))
                {
                    ViewBag.ErrorMessage = "No print data available for this incident.";
                    ViewBag.HtmlContent = string.Empty;
                }
                else
                {
                    ViewBag.HtmlContent = htmlContent;
                }

                ViewBag.IncidentId = id;
                return View();
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An error occurred while loading the print preview.";
                ViewBag.HtmlContent = string.Empty;
                ViewBag.IncidentId = id;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrintContent(string id)
        {
            var currentUser = await GetCurrentUserSessionAsync();
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            try
            {
                var htmlContent = await _workflowService.GetPrintViewHtmlAsync(id);

                if (string.IsNullOrEmpty(htmlContent))
                {
                    return Content("No print data available for this incident.");
                }

                var fullHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>Incident Report - {id}</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            margin: 20px;
            padding: 20px;
        }}
        table {{
            width: 100%;
            border-collapse: collapse !important;
            margin: 10px 0;
            border: 1px solid #000 !important;
        }}
        table th, table td {{
            border: 1px solid #000 !important;
            padding: 8px;
            text-align: left;
        }}
        th {{
            background-color: #f2f2f2;
            font-weight: bold;
        }}
        * {{
            box-sizing: border-box;
        }}
    </style>
</head>
<body>
    {htmlContent}
</body>
</html>";

                return Content(fullHtml, "text/html", System.Text.Encoding.UTF8);
            }
            catch (Exception)
            {
                return Content("An error occurred while generating the print preview.");
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

        [HttpGet]
        public async Task<IActionResult> Update(string id)
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

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetIncidentById(string id)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Incident ID is required" });
                }

                var incident = await _workflowService.GetIncidentByIdAsync(id, currentUser.UserId);
                if (incident == null)
                {
                    return Json(new { success = false, message = "Incident not found" });
                }

                return Json(new { success = true, incident = incident });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error retrieving incident", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPartB([FromBody] PartBSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.ReviewComment))
                {
                    return Json(new { success = false, message = "Review and Comment is required", errorCode = "ERR-134" });
                }

                if (string.IsNullOrEmpty(request.WshoId))
                {
                    return Json(new { success = false, message = "WSHO selection is required", errorCode = "ERR-135" });
                }

                var partBModel = new PartBSubmitModel
                {
                    IncidentId = request.IncidentId,
                    InjuredCaseType = request.InjuredCaseType,
                    ReviewComment = request.ReviewComment,
                    WshoId = request.WshoId,
                    AlternateWshoId = request.AlternateWshoId ?? string.Empty,
                    EmailToList = request.EmailToList ?? new List<string>(),
                    AdditionalCopyToList = request.AdditionalCopyToList?.Select(c => new Services.Models.CopyToPersonModel()
                    {
                        Name = c.Name,
                        EmployeeNo = c.EmployeeNo,
                        Designation = c.Designation
                    }).ToList() ?? new List<Services.Models.CopyToPersonModel>(),
                    SubmitterName = currentUser.UserName,
                    SubmitterEmpId = currentUser.UserId,
                    SubmitterDesignation = string.Empty
                };

                var result = await _workflowService.SubmitPartBAsync(partBModel, currentUser.UserId);

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part B submitted successfully", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting Part B", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClosePartB([FromBody] PartBSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.ReviewComment))
                {
                    return Json(new { success = false, message = "Review and Comment is required", errorCode = "ERR-134" });
                }

                if (string.IsNullOrEmpty(request.WshoId))
                {
                    return Json(new { success = false, message = "WSHO selection is required", errorCode = "ERR-135" });
                }

                var partBModel = new PartBSubmitModel
                {
                    IncidentId = request.IncidentId,
                    InjuredCaseType = request.InjuredCaseType,
                    ReviewComment = request.ReviewComment,
                    WshoId = request.WshoId,
                    AlternateWshoId = request.AlternateWshoId ?? string.Empty,
                    EmailToList = request.EmailToList ?? new List<string>(),
                    AdditionalCopyToList = request.AdditionalCopyToList?.Select(c => new Services.Models.CopyToPersonModel()
                    {
                        Name = c.Name,
                        EmployeeNo = c.EmployeeNo,
                        Designation = c.Designation
                    }).ToList() ?? new List<Services.Models.CopyToPersonModel>(),
                    SubmitterName = currentUser.UserName,
                    SubmitterEmpId = currentUser.UserId,
                    SubmitterDesignation = string.Empty
                };

                var result = await _workflowService.ClosePartBAsync(partBModel, currentUser.UserId);

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part B closed successfully. Incident report has been rejected.", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while closing Part B", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePartC([FromBody] PartCSaveRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                var partCModel = MapPartCRequestToModel(request, currentUser.UserId);
                var result = await _workflowService.SavePartCAsync(partCModel, currentUser.UserId);

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part C saved successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving Part C", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPartC([FromBody] PartCSaveRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                var validationError = ValidatePartC(request);
                if (!string.IsNullOrEmpty(validationError))
                {
                    return Json(new { success = false, message = validationError });
                }

                var partCModel = MapPartCRequestToModel(request, currentUser.UserId);
                var result = await _workflowService.SubmitPartCAsync(partCModel, currentUser.UserId);

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part C submitted successfully", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting Part C", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClosePartC([FromBody] PartCCloseRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.AdditionalComments))
                {
                    return Json(new { success = false, message = "Additional comments required for closure", errorCode = "ERR-134" });
                }

                if (string.IsNullOrEmpty(request.CwshoId))
                {
                    return Json(new { success = false, message = "Corporate WSHO selection required", errorCode = "ERR-135" });
                }

                var partCModel = MapPartCCloseRequestToModel(request, currentUser.UserId);
                var result = await _workflowService.ClosePartCAsync(partCModel, currentUser.UserId);

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Incident closed successfully", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while closing incident", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPartD([FromBody] PartDSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Review & Comment is required", errorCode = "ERR-137" });
                }

                if (string.IsNullOrEmpty(request.HeadLobId))
                {
                    return Json(new { success = false, message = "Name of Head LOB is required", errorCode = "ERR-133" });
                }

                var partDModel = new PartDSubmitModel
                {
                    IncidentId = request.IncidentId,
                    Comments = request.Comments,
                    HsbuId = request.HeadLobId
                };

                var result = await _workflowService.SubmitPartDAsync(partDModel, currentUser.UserId);

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part D submitted successfully to HSBU", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting Part D", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RevertPartDToWSHO([FromBody] PartDSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Review & Comment is required", errorCode = "ERR-137" });
                }

                if (string.IsNullOrEmpty(request.WshoId))
                {
                    return Json(new { success = false, message = "Name of WSHO is required", errorCode = "ERR-135" });
                }

                var result = await _workflowService.RevertPartDToWSHOAsync(request.IncidentId, request.Comments, request.WshoId, currentUser.UserId);

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part D reverted successfully to WSHO", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while reverting Part D", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPartE([FromBody] PartESubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Review & Comment is required", errorCode = "ERR-134" });
                }

                if (string.IsNullOrEmpty(request.HodId))
                {
                    return Json(new { success = false, message = "Name of HOD is required", errorCode = "ERR-133" });
                }

                var selectedEmailTo = request.EmailToList ?? new List<string>();
                var additionalCopyTo = (request.AdditionalCopyToList ?? new List<CopyToPersonModel>())
                    .Select(p => new Services.Models.CopyToPersonModel
                    {
                        EmployeeNo = p.EmployeeNo,
                        Name = p.Name,
                        Designation = p.Designation
                    }).ToList();

                var result = await _workflowService.SubmitPartEAsync(
                    request.IncidentId,
                    request.Comments,
                    request.HodId,
                    selectedEmailTo,
                    additionalCopyTo,
                    currentUser.UserId
                );

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part E submitted successfully to HOD", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting Part E", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RevertPartEToWSHO([FromBody] PartESubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Review & Comment is required", errorCode = "ERR-137" });
                }

                if (string.IsNullOrEmpty(request.WshoId))
                {
                    return Json(new { success = false, message = "Name of WSHO is required", errorCode = "ERR-135" });
                }

                var selectedEmailTo = request.EmailToList ?? new List<string>();
                var additionalCopyTo = (request.AdditionalCopyToList ?? new List<CopyToPersonModel>())
                    .Select(p => new Services.Models.CopyToPersonModel
                    {
                        EmployeeNo = p.EmployeeNo,
                        Name = p.Name,
                        Designation = p.Designation
                    }).ToList();

                var result = await _workflowService.RevertPartEToWSHOAsync(
                    request.IncidentId,
                    request.Comments,
                    request.WshoId,
                    selectedEmailTo,
                    additionalCopyTo,
                    currentUser.UserId
                );

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part E reverted successfully to WSHO", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while reverting Part E", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPartF([FromForm] PartFSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Provide Objective Evidence of Actions Taken is required", errorCode = "ERR-137" });
                }

                if (string.IsNullOrEmpty(request.RiskAssessmentReview))
                {
                    return Json(new { success = false, message = "Risk Assessment Review selection is required", errorCode = "ERR-116" });
                }

                if (string.IsNullOrEmpty(request.WshoId))
                {
                    return Json(new { success = false, message = "Name of WSHO is required", errorCode = "ERR-133" });
                }

                var result = await _workflowService.SubmitPartFAsync(
                    request.IncidentId,
                    request.Comments,
                    request.RiskAssessmentReview,
                    request.WshoId,
                    request.Attachments,
                    request.RiskAttachments,
                    currentUser.UserId
                );

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part F submitted successfully to WSHO", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting Part F", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPartG([FromForm] PartGSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Review & Comment is required", errorCode = "ERR-134" });
                }

                if (string.IsNullOrEmpty(request.CwshoId))
                {
                    return Json(new { success = false, message = "Name of Chairman WSH is required", errorCode = "ERR-133" });
                }

                var result = await _workflowService.SubmitPartGAsync(
                    request.IncidentId,
                    request.Comments,
                    request.CwshoId,
                    request.Attachments,
                    currentUser.UserId
                );

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part G submitted successfully to Chairman WSH", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while submitting Part G", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RevertPartGToHOD([FromForm] PartGSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Review & Comment is required", errorCode = "ERR-134" });
                }

                if (string.IsNullOrEmpty(request.HodId))
                {
                    return Json(new { success = false, message = "Name of HOD is required", errorCode = "ERR-133" });
                }

                var result = await _workflowService.RevertPartGToHODAsync(
                    request.IncidentId,
                    request.Comments,
                    request.HodId,
                    request.Attachments,
                    currentUser.UserId
                );

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part G reverted successfully to HOD", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while reverting Part G", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RevertPartHToWSHO([FromBody] PartHSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Review & Comment is required", errorCode = "ERR-137" });
                }

                if (string.IsNullOrEmpty(request.WshoId))
                {
                    return Json(new { success = false, message = "Name of WSHO is required", errorCode = "ERR-133" });
                }

                var selectedEmailTo = request.EmailToList ?? new List<string>();
                var additionalCopyTo = (request.AdditionalCopyToList ?? new List<CopyToPersonModel>())
                    .Select(p => new Services.Models.CopyToPersonModel
                    {
                        EmployeeNo = p.EmployeeNo,
                        Name = p.Name,
                        Designation = p.Designation
                    }).ToList();

                var result = await _workflowService.RevertPartHToWSHOAsync(
                    request.IncidentId,
                    request.Comments,
                    request.WshoId,
                    selectedEmailTo,
                    additionalCopyTo,
                    currentUser.UserId
                );

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Part H reverted successfully to WSHO", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while reverting Part H", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CloseReport([FromBody] PartHSubmitRequest request)
        {
            try
            {
                var currentUser = await GetCurrentUserSessionAsync();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                if (request == null || string.IsNullOrEmpty(request.IncidentId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(request.Comments))
                {
                    return Json(new { success = false, message = "Review & Comment is required", errorCode = "ERR-137" });
                }

                var selectedEmailTo = request.EmailToList ?? new List<string>();
                var additionalCopyTo = (request.AdditionalCopyToList ?? new List<CopyToPersonModel>())
                    .Select(p => new Services.Models.CopyToPersonModel
                    {
                        EmployeeNo = p.EmployeeNo,
                        Name = p.Name,
                        Designation = p.Designation
                    }).ToList();

                var result = await _workflowService.CloseReportAsync(
                    request.IncidentId,
                    request.Comments,
                    selectedEmailTo,
                    additionalCopyTo,
                    currentUser.UserId
                );

                if (string.IsNullOrEmpty(result) || !result.Contains("ERROR"))
                {
                    return Json(new { success = true, message = "Report closed successfully", successCode = "SUC-001" });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while closing report", error = ex.Message });
            }
        }

        private string ValidatePartC(PartCSaveRequest request)
        {
            if (string.IsNullOrEmpty(request.IsNegligent))
            {
                return "Negligent/ Non Negligent is required (ERR-136)";
            }

            if (string.IsNullOrEmpty(request.AdditionalComments))
            {
                return "Comment is required (ERR-137)";
            }

            if (string.IsNullOrEmpty(request.CwshoId))
            {
                return "Name of Chairman WSH is required (ERR-138)";
            }

            if (string.IsNullOrEmpty(request.RecommendedActions))
            {
                return "Corrective and Preventive Action(s) is required (ERR-139)";
            }

            return string.Empty;
        }

        private PartCSubmitModel MapPartCRequestToModel(PartCSaveRequest request, string userId)
        {
            return new PartCSubmitModel
            {
                IncidentId = request.IncidentId,
                IsNegligent = request.IsNegligent,
                NegligentComments = request.NegligentComments ?? string.Empty,
                NeedsRiskAssessmentReview = request.NeedsRiskAssessmentReview ?? "N",
                RiskAssessmentComments = request.RiskAssessmentComments ?? string.Empty,
                WhatHappenedAndWhy = request.WhatHappenedAndWhy ?? string.Empty,
                RecommendedActions = request.RecommendedActions,
                AdditionalComments = request.AdditionalComments ?? string.Empty,
                CwshoId = request.CwshoId ?? string.Empty,
                PersonsInterviewed = request.PersonsInterviewed?.Select(c => new Services.Models.PersonInterviewedModel()
                {
                    Name = c.Name,
                    ContactNo = c.ContactNo,
                    EmployeeNo = c.EmployeeNo,
                    Designation = c.Designation
                }).ToList() ?? new List<Services.Models.PersonInterviewedModel>(),
                InjuryDetails = request.InjuryDetails?.Select(c => new Services.Models.InjuryDetailModel()
                {
                    Description = c.Description,
                    HeadNeckTorso = c.HeadNeckTorso,
                    InjuredPersonId = c.InjuredPersonId,
                    InjuredPersonName = c.InjuredPersonName,
                    LowerLimbs = c.LowerLimbs,
                    NatureOfInjury = c.NatureOfInjury,
                    UpperLimbs = c.UpperLimbs
                }).ToList() ?? new List<Services.Models.InjuryDetailModel>(),
                MedicalCertificates = request.MedicalCertificates?.Select(c => new Services.Models.MedicalCertificateModel()
                {
                    AttachmentPath = c.AttachmentPath,
                    HasAttachment = c.HasAttachment,
                    InjuredPersonId = c.InjuredPersonId,
                    InjuredPersonName  = c.InjuredPersonName,
                    FromDate = c.FromDate,
                    ToDate = c.ToDate,
                    NumberOfDays = c.NumberOfDays
                }).ToList() ?? new List<Services.Models.MedicalCertificateModel>(),
                IncidentClassList = request.IncidentClassList ?? new List<string>(),
                IncidentAgentList = request.IncidentAgentList ?? new List<string>(),
                UnsafeConditionsList = request.UnsafeConditionsList ?? new List<string>(),
                UnsafeActsList = request.UnsafeActsList ?? new List<string>(),
                ContributingFactorsList = request.ContributingFactorsList ?? new List<string>(),
                SubmitterName = string.Empty,
                SubmitterEmpId = userId,
                SubmitterDesignation = string.Empty
            };
        }

        private PartCCloseModel MapPartCCloseRequestToModel(PartCCloseRequest request, string userId)
        {
            return new PartCCloseModel
            {
                IncidentId = request.IncidentId,
                AdditionalComments = request.AdditionalComments,
                CwshoId = request.CwshoId,
                PartCData = MapPartCRequestToModel(request.PartCData, userId)
            };
        }

        private async Task LoadCreateViewModelDropdowns(IncidentCreateViewModel model)
        {
            model.SbaOptions = await _masterDataService.GetSectors();
            model.SbuOptions = new List<LookupItem>();
            model.DivisionOptions = new List<LookupItem>();
            model.DepartmentOptions = new List<LookupItem>();
            model.LocationOptions = await _masterDataService.GetLocations();

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
            model.SbaOptions = await _masterDataService.GetSectors();
            model.SbuOptions = new List<LookupItem>();
            model.DivisionOptions = new List<LookupItem>();
            model.DepartmentOptions = new List<LookupItem>();
            model.LocationOptions = await _masterDataService.GetLocations();
        }

        private async Task LoadSearchViewModelDropdowns(IncidentSearchViewModel model)
        {
            model.SbaOptions = await _masterDataService.GetSectors();
            model.SbuOptions = new List<LookupItem>();
            model.DivisionOptions = new List<LookupItem>();
        }

        private void SetStagePermissions(IncidentViewViewModel model)
        {
            if (model.Incident?.StagePermissions?.Any() == true)
            {
                var stageA = model.StagePermissions.FirstOrDefault(x => x.Stage == "01");
                var stageB = model.StagePermissions.FirstOrDefault(x => x.Stage == "02");
                var stageC = model.StagePermissions.FirstOrDefault(x => x.Stage == "03");

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
                    model.CurrentStage = "01";
                    model.CurrentStageDescription = "Initial Report";
                }
            }
        }

        private IncidentCreateModel MapViewModelToCreateModel(IncidentCreateViewModel model)
        {
            return new IncidentCreateModel
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

    public class IncidentCreateApiRequest
    {
        public string IncidentType { get; set; }
        public string IncidentOther { get; set; }
        public string IncidentDate { get; set; }
        public string IncidentTime { get; set; }
        public string IncidentDateTime { get; set; }
        public string SectorCode { get; set; }
        public string LobCode { get; set; }
        public string DepartmentCode { get; set; }
        public string LocationCode { get; set; }
        public string ExactLocation { get; set; }
        public string IncidentDescription { get; set; }
        public string DamageDescription { get; set; }
        public string IsJobRelated { get; set; }
        public string HospitalClinicName { get; set; }
        public string WorkingOvertime { get; set; }
        public string OfficialWorkingHours { get; set; }
        public bool HasEyeWitness { get; set; }
        public string HodId { get; set; }
        public string WshoId { get; set; }
        public string AhodId { get; set; }
        public List<InjuredPersonModel> InjuredPersons { get; set; }
        public List<EyewitnessModel> Eyewitnesses { get; set; }
        public List<string> CopyToList { get; set; }
        public string SuperiorEmpNo { get; set; }
        public string SuperiorName { get; set; }
        public string SuperiorDesignation { get; set; }
    }

    public class InjuredPersonModel
    {
        public string Name { get; set; }
        public string EmployeeNo { get; set; }
        public string ContactNo { get; set; }
        public string Age { get; set; }
        public string Company { get; set; }
        public string Race { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string Designation { get; set; }
        public string EmploymentType { get; set; }
        public string DateOfEmployment { get; set; }
        public string Type { get; set; }
    }

    public class EyewitnessModel
    {
        public string Name { get; set; }
        public string EmployeeNo { get; set; }
        public string Designation { get; set; }
        public string ContactNo { get; set; }
    }

    public class PartBSubmitRequest
    {
        public string IncidentId { get; set; }
        public string InjuredCaseType { get; set; }
        public string ReviewComment { get; set; }
        public string WshoId { get; set; }
        public string AlternateWshoId { get; set; }
        public List<string> EmailToList { get; set; }
        public List<CopyToPersonModel> AdditionalCopyToList { get; set; }
    }

    public class CopyToPersonModel
    {
        public string EmployeeNo { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
    }

    public class PartCSaveRequest
    {
        public string IncidentId { get; set; }
        public string IsNegligent { get; set; }
        public string NegligentComments { get; set; }
        public string NeedsRiskAssessmentReview { get; set; }
        public string RiskAssessmentComments { get; set; }
        public string WhatHappenedAndWhy { get; set; }
        public string RecommendedActions { get; set; }
        public string AdditionalComments { get; set; }
        public string CwshoId { get; set; }
        public List<PersonInterviewedModel> PersonsInterviewed { get; set; }
        public List<InjuryDetailModel> InjuryDetails { get; set; }
        public List<MedicalCertificateModel> MedicalCertificates { get; set; }
        public List<string> IncidentClassList { get; set; }
        public List<string> IncidentAgentList { get; set; }
        public List<string> UnsafeConditionsList { get; set; }
        public List<string> UnsafeActsList { get; set; }
        public List<string> ContributingFactorsList { get; set; }
    }

    public class PartCCloseRequest
    {
        public string IncidentId { get; set; }
        public string AdditionalComments { get; set; }
        public string CwshoId { get; set; }
        public PartCSaveRequest PartCData { get; set; }
    }

    public class PersonInterviewedModel
    {
        public string Name { get; set; }
        public string EmployeeNo { get; set; }
        public string Designation { get; set; }
        public string ContactNo { get; set; }
    }

    public class InjuryDetailModel
    {
        public string InjuredPersonId { get; set; }
        public string InjuredPersonName { get; set; }
        public List<string> NatureOfInjury { get; set; }
        public List<string> HeadNeckTorso { get; set; }
        public List<string> UpperLimbs { get; set; }
        public List<string> LowerLimbs { get; set; }
        public string Description { get; set; }
    }

    public class MedicalCertificateModel
    {
        public string InjuredPersonId { get; set; }
        public string InjuredPersonName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int NumberOfDays { get; set; }
        public string AttachmentPath { get; set; }
        public bool HasAttachment { get; set; }
    }

    public class PartDSubmitRequest
    {
        public string IncidentId { get; set; }
        public string Comments { get; set; }
        public string WshoId { get; set; }
        public string HeadLobId { get; set; }
        public string ActionType { get; set; }
    }

    public class PartESubmitRequest
    {
        public string IncidentId { get; set; }
        public string Comments { get; set; }
        public string WshoId { get; set; }
        public string HodId { get; set; }
        public List<string> EmailToList { get; set; }
        public List<CopyToPersonModel> AdditionalCopyToList { get; set; }
    }

    public class PartFSubmitRequest
    {
        public string IncidentId { get; set; }
        public string Comments { get; set; }
        public string RiskAssessmentReview { get; set; }
        public string WshoId { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public List<IFormFile> RiskAttachments { get; set; }
    }

    public class PartGSubmitRequest
    {
        public string IncidentId { get; set; }
        public string Comments { get; set; }
        public string HodId { get; set; }
        public string CwshoId { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }

    public class PartHSubmitRequest
    {
        public string IncidentId { get; set; }
        public string Comments { get; set; }
        public string WshoId { get; set; }
        public List<string> EmailToList { get; set; }
        public List<CopyToPersonModel> AdditionalCopyToList { get; set; }
    }
}