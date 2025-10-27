# Part D - HOD Comments API Implementation Summary

**Last Updated**: 2025-10-26
**Status**: 95% Complete (UI, API, Client-side ✅ | Service Layer ⏳)
**Complexity**: Low (Similar to Part B)

---

## Overview

Part D allows the Head of Department (HOD) to review the WSHO's investigation findings from Part C and provide comments before forwarding to HSBU (Head of SBU) for final approval.

**Workflow**: Status 03 (Investigation Complete) → 04 (HOD Commented)
**User Role**: HOD or Alternate HOD
**Estimated Effort**: 0.5 days (service layer implementation remaining)

---

## Table of Contents

1. [API Endpoints](#api-endpoints)
2. [Request/Response Models](#requestresponse-models)
3. [Business Logic](#business-logic)
4. [Frontend Implementation](#frontend-implementation)
5. [Files Created/Modified](#files-createdmodified)
6. [Testing Checklist](#testing-checklist)
7. [Service Layer Implementation Guide](#service-layer-implementation-guide)

---

## API Endpoints

### 1. Submit Part D

**Endpoint**: `POST /Incident/SubmitPartD`
**Purpose**: Submit HOD comments and forward to HSBU
**Status Change**: 03 → 04
**Authentication**: Required (HOD or Alternate HOD)

#### Request Body

```json
{
  "incidentId": "INC20250001",
  "comments": "I have reviewed the WSHO's investigation findings...",
  "hsbuId": "12345678",
  "emailToList": ["87654321", "11223344"],
  "additionalCopyToList": [
    {
      "employeeNo": "99887766",
      "name": "John Doe",
      "designation": "Manager"
    }
  ]
}
```

#### Request Model (`PartDSubmitRequest`)

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `incidentId` | string | Yes | Incident ID |
| `comments` | string | Yes | HOD's comments on investigation (ERR-137) |
| `hsbuId` | string | Yes | HSBU user ID (ERR-133) |
| `emailToList` | string[] | No | Array of user IDs for standard CC list |
| `additionalCopyToList` | CopyToPersonModel[] | No | Additional recipients |

**CopyToPersonModel**:
- `employeeNo` (string): Employee number
- `name` (string): Full name
- `designation` (string): Job title

#### Success Response

```json
{
  "success": true,
  "message": "Part D submitted successfully to HSBU",
  "successCode": "SUC-001"
}
```

#### Error Responses

**Missing Comments (ERR-137)**:
```json
{
  "success": false,
  "message": "Comments are required",
  "errorCode": "ERR-137"
}
```

**Missing HSBU (ERR-133)**:
```json
{
  "success": false,
  "message": "HSBU selection is required",
  "errorCode": "ERR-133"
}
```

**User Not Authenticated**:
```json
{
  "success": false,
  "message": "User not authenticated"
}
```

**General Error**:
```json
{
  "success": false,
  "message": "An error occurred while submitting Part D",
  "error": "Detailed error message"
}
```

---

### 2. Get HSBUs

**Endpoint**: `GET /User/GetHSBUs`
**Purpose**: Get list of HSBUs for selection
**Authentication**: Required

#### Query Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `sectorCode` | string | Yes | Sector code |
| `lobCode` | string | Yes | Line of Business code |
| `departmentCode` | string | No | Department code |
| `locationCode` | string | No | Location code |

#### Example Request

```
GET /User/GetHSBUs?sectorCode=SEC01&lobCode=LOB01&departmentCode=DEPT01&locationCode=LOC01
```

#### Success Response

```json
[
  {
    "userId": "12345678",
    "userName": "Jane Smith",
    "designation": "Head of SBU",
    "email": "jane.smith@company.com"
  },
  {
    "userId": "87654321",
    "userName": "Bob Johnson",
    "designation": "Head of SBU",
    "email": "bob.johnson@company.com"
  }
]
```

---

### 3. Get Part D Copy To List

**Endpoint**: `GET /User/GetPartACopyTo` (Reuses Part A endpoint)
**Purpose**: Get standard CC list for Part D
**Authentication**: Required

#### Query Parameters

Same as GetHSBUs endpoint

#### Example Request

```
GET /User/GetPartACopyTo?sectorCode=SEC01&lobCode=LOB01
```

#### Success Response

```json
[
  {
    "userId": "11223344",
    "userName": "Alice Wong",
    "designation": "HR Manager",
    "email": "alice.wong@company.com"
  },
  {
    "userId": "55667788",
    "userName": "Charlie Brown",
    "designation": "Safety Officer",
    "email": "charlie.brown@company.com"
  }
]
```

---

## Request/Response Models

### Controller Models

**Location**: `WIRS.Mvc/Controllers/IncidentController.cs`

```csharp
public class PartDSubmitRequest
{
    public string IncidentId { get; set; }
    public string Comments { get; set; }
    public string HsbuId { get; set; }
    public List<string> EmailToList { get; set; }
    public List<CopyToPersonModel> AdditionalCopyToList { get; set; }
}

public class CopyToPersonModel
{
    public string EmployeeNo { get; set; }
    public string Name { get; set; }
    public string Designation { get; set; }
}
```

### Service Models

**Location**: `WIRS.Services/Models/PartDSubmitModel.cs`

```csharp
public class PartDSubmitModel
{
    public string IncidentId { get; set; }
    public string Comments { get; set; }
    public string HsbuId { get; set; }
    public List<string> EmailToList { get; set; }
    public List<CopyToPersonModel> AdditionalCopyToList { get; set; }
    public string SubmitterName { get; set; }
    public string SubmitterEmpId { get; set; }
    public string SubmitterDesignation { get; set; }
}
```

---

## Business Logic

### Submit Part D Workflow

**File**: `IncidentController.cs` → `SubmitPartD()`

```
1. Validate user authentication
2. Validate request data (not null, incident ID exists)
3. Validate required fields:
   - Comments (ERR-137)
   - HSBU ID (ERR-133)
4. Map request to PartDSubmitModel
5. Set submitter information from current user
6. Call WorkflowService.SubmitPartDAsync()
7. Return success/error response
```

### Service Layer Logic (To Be Implemented)

**File**: `WorkflowService.cs` → `SubmitPartDAsync()`

```csharp
Task<string> SubmitPartDAsync(PartDSubmitModel model, string userId)
{
    1. Validate incident exists and status = "03"
    2. Validate user is HOD or Alternate HOD

    3. Update incident record:
       - Set partd_comments = model.Comments
       - Set status = "04"
       - Set partd_submitted_by = userId
       - Set partd_submitted_date = current date
       - Set hsbu_id = model.HsbuId

    4. Create workflow entries:
       a. HSBU workflow:
          - Role: "HSBU"
          - Assigned to: model.HsbuId
          - Comments: model.Comments
          - Status: "04"

       b. CC/Email To workflows:
          - Role: "COPYTO"
          - For each selected person in EmailToList
          - For each person in AdditionalCopyToList

    5. Call stored procedure: insert_incidents_workflows()

    6. Send email notifications:
       - To: HSBU
       - CC: EmailToList + AdditionalCopyToList
       - Subject: "Incident [ID] - HOD Comments for Review"
       - Body: Include incident summary, HOD comments, link to Part E

    7. Return:
       - Empty string on success
       - "ERROR: message" on failure
}
```

### Permission Checks

**Who can edit Part D**:
- Incident status must be "03"
- Current user must be:
  - HOD (incident.hodId = currentUser.userId), OR
  - Alternate HOD (incident.ahodId = currentUser.userId)

**Who can view Part D**:
- Incident status >= "03"

---

## Frontend Implementation

### Files Created/Modified

#### 1. Views/Incident/_PartD.cshtml (NEW)

**Purpose**: Part D form partial view
**Size**: ~350 lines

**Features**:
- HOD comments textarea with character count (2000 max)
- HSBU dropdown (required)
- Standard email to checkbox list (default all selected)
- Additional copy to dynamic table with employee search
- Submitter information (read-only, auto-populated)
- Submit button
- Read-only mode for status > 03
- Validation messages
- Success messages

**Sections**:
1. HOD Comments (textarea, required)
2. Forward To (HSBU dropdown, required)
3. CC/Email To (checkbox list + additional recipients table)
4. Submitter Information (read-only)

#### 2. wwwroot/js/incident/incident-update-service.js (MODIFIED)

**Added Methods**:

```javascript
function getHSBUs(sectorCode, lobCode, departmentCode, locationCode)
function getPartDCopyToList(sectorCode, lobCode, departmentCode, locationCode)
function submitPartD(partDData)
```

#### 3. wwwroot/js/incident/incident-update-controller.js (MODIFIED)

**Added Data Structure**:

```javascript
vm.partD = {
    isReadOnly: false,
    comments: '',
    hsbuId: '',
    emailToList: [],
    additionalCopyToList: [],
    submitterName: '',
    submitterEmpId: '',
    submitterDesignation: '',
    submittedDate: '',
    currentDate: '',
    validationMessage: '',
    successMessage: '',
    isSubmitting: false
};

vm.hsbuList = [];
```

**Added Functions**:
- `canViewPartD()` - Check if user can view Part D
- `canEditPartD()` - Check if user can edit Part D
- `loadPartDData()` - Load Part D data
- `determinePartDMode()` - Set read-only mode based on status
- `loadPartDReadOnlyData()` - Load submitted data for display
- `loadHSBUs()` - Load HSBU dropdown
- `loadPartDEmailToList()` - Load CC list
- `getHsbuName()` - Get HSBU name for display
- `openEmployeeSearchForPartD()` - Open employee search modal
- `removeAdditionalCopyToFromPartD()` - Remove additional recipient
- `submitPartD()` - Submit Part D
- `cancelPartD()` - Cancel and return to home

#### 4. Views/Incident/Update.cshtml (MODIFIED)

**Added**:
- Part D tab button (conditional visibility)
- Part D tab content pane with _PartD partial

#### 5. Controllers/IncidentController.cs (MODIFIED)

**Added**:
- `SubmitPartD()` endpoint with validation
- `PartDSubmitRequest` model

#### 6. Controllers/UserController.cs (MODIFIED)

**Added**:
- `GetHSBUs()` endpoint

#### 7. Services/Models/PartDSubmitModel.cs (NEW)

**Purpose**: Service layer model for Part D submission

#### 8. Services/Interfaces/IWorkflowService.cs (MODIFIED)

**Added**:
```csharp
Task<string> SubmitPartDAsync(PartDSubmitModel model, string userId);
```

---

## Testing Checklist

### Functional Testing

#### Part D Access Control
- [ ] Part D tab only visible when status >= 03
- [ ] Part D edit mode only available when status = 03
- [ ] Only HOD or Alternate HOD can edit Part D
- [ ] Other users see read-only mode

#### Part D Form - Edit Mode (Status = 03)
- [ ] Comments textarea is enabled
- [ ] Comments character count displays correctly (0/2000)
- [ ] HSBU dropdown populates with correct users
- [ ] Email To list populates with all checkboxes selected
- [ ] Additional recipients table starts empty
- [ ] Employee search button works (TODO: integrate with modal)
- [ ] Add recipient adds to table
- [ ] Remove recipient deletes from table
- [ ] Submitter info auto-populates (name, ID, designation, date)
- [ ] Submit button enabled when form is valid
- [ ] Submit button disabled when form is invalid

#### Part D Form - Read-Only Mode (Status > 03)
- [ ] Comments displayed as read-only text
- [ ] HSBU displayed as read-only text
- [ ] Selected email recipients displayed as bullet list
- [ ] Additional recipients displayed as bullet list
- [ ] Submitter info displayed as read-only
- [ ] Submit button hidden
- [ ] Edit controls hidden

#### Part D Validation
- [ ] Empty comments shows error (ERR-137)
- [ ] No HSBU selected shows error (ERR-133)
- [ ] Comments > 2000 characters prevented
- [ ] Form validation prevents submission when invalid
- [ ] Confirmation dialog shown before submission

#### Part D Submission
- [ ] Valid submission changes status from 03 to 04
- [ ] Success message displayed
- [ ] Redirects to home after 2 seconds
- [ ] Selected email recipients saved
- [ ] Additional recipients saved
- [ ] Submitter information saved

### Integration Testing

#### API Endpoints
- [ ] POST /Incident/SubmitPartD returns success for valid data
- [ ] POST /Incident/SubmitPartD returns ERR-137 for empty comments
- [ ] POST /Incident/SubmitPartD returns ERR-133 for missing HSBU
- [ ] POST /Incident/SubmitPartD returns error for unauthenticated user
- [ ] POST /Incident/SubmitPartD returns error for non-HOD user
- [ ] GET /User/GetHSBUs returns list of HSBUs
- [ ] GET /User/GetPartACopyTo returns CC list

#### Database
- [ ] Incident status updated from 03 to 04
- [ ] HOD comments saved to database
- [ ] HSBU ID saved to database
- [ ] Part D submitter info saved (name, ID, date)
- [ ] HSBU workflow entry created (role=HSBU, status=04)
- [ ] CC workflow entries created for all selected recipients
- [ ] Additional copy to workflow entries created

#### Email Notifications
- [ ] Email sent to HSBU
- [ ] Email sent to all selected CC recipients
- [ ] Email sent to all additional recipients
- [ ] Email contains incident ID and summary
- [ ] Email contains HOD comments
- [ ] Email contains link to Part E (HSBU review)

### Browser Testing
- [ ] Part D works in Chrome
- [ ] Part D works in Firefox
- [ ] Part D works in Edge
- [ ] Part D works in Safari
- [ ] Responsive design works on mobile
- [ ] Responsive design works on tablet
- [ ] Tab navigation works correctly

### Error Handling
- [ ] Network error shows user-friendly message
- [ ] Server error shows user-friendly message
- [ ] Missing data shows validation error
- [ ] Invalid incident ID shows error
- [ ] Concurrent edit shows appropriate error

---

## Service Layer Implementation Guide

### IWorkflowService Interface

**File**: `WIRS.Services/Interfaces/IWorkflowService.cs`

```csharp
Task<string> SubmitPartDAsync(PartDSubmitModel model, string userId);
```

### WorkflowService Implementation

**File**: `WIRS.Services/Implementations/WorkflowService.cs`

```csharp
public async Task<string> SubmitPartDAsync(PartDSubmitModel model, string userId)
{
    try
    {
        // 1. Validate incident exists and status
        var incident = await _workflowDataAccess.GetIncidentByIdAsync(model.IncidentId);
        if (incident == null)
        {
            return "ERROR: Incident not found";
        }

        if (incident.Status != "03")
        {
            return "ERROR: Incident status must be 03 to submit Part D";
        }

        // 2. Validate user is HOD or Alternate HOD
        if (incident.HodId != userId && incident.AhodId != userId)
        {
            return "ERROR: Only HOD or Alternate HOD can submit Part D";
        }

        // 3. Update incident with Part D data
        var updateSuccess = await _workflowDataAccess.UpdateIncidentPartDAsync(
            model.IncidentId,
            model.Comments,
            model.HsbuId,
            userId,
            DateTime.Now
        );

        if (!updateSuccess)
        {
            return "ERROR: Failed to update incident";
        }

        // 4. Change status from 03 to 04
        await _workflowDataAccess.UpdateIncidentStatusAsync(model.IncidentId, "04");

        // 5. Create HSBU workflow entry
        await _workflowDataAccess.InsertWorkflowAsync(new WorkflowModel
        {
            IncidentId = model.IncidentId,
            Role = "HSBU",
            AssignedTo = model.HsbuId,
            Comments = model.Comments,
            Status = "04",
            CreatedBy = userId,
            CreatedDate = DateTime.Now
        });

        // 6. Create CC workflow entries
        foreach (var emailTo in model.EmailToList)
        {
            await _workflowDataAccess.InsertWorkflowAsync(new WorkflowModel
            {
                IncidentId = model.IncidentId,
                Role = "COPYTO",
                AssignedTo = emailTo,
                Comments = "Part D - HOD Comments",
                Status = "04",
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            });
        }

        // 7. Create additional copy to workflow entries
        foreach (var person in model.AdditionalCopyToList)
        {
            await _workflowDataAccess.InsertWorkflowAsync(new WorkflowModel
            {
                IncidentId = model.IncidentId,
                Role = "COPYTO",
                AssignedTo = person.EmployeeNo,
                Comments = "Part D - HOD Comments (Additional)",
                Status = "04",
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            });
        }

        // 8. Send email notifications
        var emailRecipients = new List<string> { model.HsbuId };
        emailRecipients.AddRange(model.EmailToList);
        emailRecipients.AddRange(model.AdditionalCopyToList.Select(p => p.EmployeeNo));

        await _emailService.SendPartDNotificationAsync(
            model.IncidentId,
            incident,
            model.Comments,
            emailRecipients
        );

        return string.Empty; // Success
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error submitting Part D for incident {IncidentId}", model.IncidentId);
        return $"ERROR: {ex.Message}";
    }
}
```

### Data Access Methods Needed

**IWorkflowIncidentDataAccess** additions:

```csharp
Task<bool> UpdateIncidentPartDAsync(
    string incidentId,
    string comments,
    string hsbuId,
    string submitterId,
    DateTime submittedDate
);
```

### Stored Procedure

**Name**: `update_incident_partd`

```sql
CREATE OR REPLACE FUNCTION update_incident_partd(
    p_incident_id VARCHAR,
    p_comments TEXT,
    p_hsbu_id VARCHAR,
    p_submitter_id VARCHAR,
    p_submitted_date TIMESTAMP
)
RETURNS BOOLEAN AS $$
BEGIN
    UPDATE incidents
    SET
        partd_comments = p_comments,
        hsbu_id = p_hsbu_id,
        partd_submitted_by = p_submitter_id,
        partd_submitted_date = p_submitted_date,
        status = '04',
        updated_by = p_submitter_id,
        updated_date = CURRENT_TIMESTAMP
    WHERE incident_id = p_incident_id
    AND status = '03';

    RETURN FOUND;
END;
$$ LANGUAGE plpgsql;
```

---

## Summary

### Part D Implementation Status

| Component | Status | Notes |
|-----------|--------|-------|
| UI (_PartD.cshtml) | ✅ Complete | 350 lines, edit + read-only modes |
| API Endpoint | ✅ Complete | SubmitPartD with validation |
| AngularJS Service | ✅ Complete | 3 methods added |
| AngularJS Controller | ✅ Complete | 200+ lines, full logic |
| Controller Models | ✅ Complete | PartDSubmitRequest |
| Service Models | ✅ Complete | PartDSubmitModel |
| Service Interface | ✅ Complete | SubmitPartDAsync added |
| User Endpoints | ✅ Complete | GetHSBUs added |
| Update.cshtml Integration | ✅ Complete | Part D tab added |
| Service Implementation | ⏳ Pending | WorkflowService.SubmitPartDAsync |
| Data Access Layer | ⏳ Pending | UpdateIncidentPartDAsync |
| Email Notifications | ⏳ Pending | SendPartDNotificationAsync |
| Testing | ⏳ Pending | Full test suite |

### Lines of Code

- `_PartD.cshtml`: ~350 lines
- Controller additions: ~55 lines
- Service models: ~15 lines
- AngularJS controller: ~200 lines added
- AngularJS service: ~30 lines added
- **Total: ~650 lines of new code**

### Complexity Assessment

**Overall Complexity**: Low

- Simple form with 2 required fields
- Standard dropdown and checkbox list
- Similar to Part B (slightly simpler)
- No complex validation rules
- No conditional sections
- Estimated implementation time: 0.5 days (service layer only)

### Comparison to Other Parts

| Aspect | Part B | Part D | Difference |
|--------|--------|--------|------------|
| UI Lines | 250 | 350 | +100 (more detailed read-only mode) |
| Required Fields | 2 | 2 | Same |
| Dropdowns | 2 | 1 | Simpler |
| Dynamic Tables | 1 | 1 | Same |
| Validation Rules | 2 | 2 | Same |
| Complexity | Low | Low | Same |

---

## Next Steps

1. **Implement Service Layer** (0.5 days)
   - WorkflowService.SubmitPartDAsync()
   - DataAccess.UpdateIncidentPartDAsync()
   - Email notification service

2. **Test Part D End-to-End** (0.5 day)
   - Test submission flow
   - Verify database records
   - Test email notifications
   - Test permission checks

3. **Move to Part E** (2-3 days)
   - HSBU Approval with revert functionality

---

## Error Codes Reference

| Code | Message | Trigger |
|------|---------|---------|
| ERR-133 | HSBU selection is required | No HSBU selected |
| ERR-137 | Comments are required | Empty comments field |
| SUC-001 | Successfully submitted | Part D submitted successfully |

---

**Document End**
