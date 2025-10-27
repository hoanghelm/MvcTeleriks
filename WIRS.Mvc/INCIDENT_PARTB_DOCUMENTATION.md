# Part B - HOD Review Documentation

## Overview
Part B is where the Head of Department (HOD) reviews the incident report submitted in Part A and provides comments before forwarding to WSHO for investigation.

## Workflow Status
- **From Status**: 01 (Part A Submitted)
- **To Status**: 02 (HOD Reviewed, forwarded to WSHO)
- **User Role**: HOD (or assigned alternate)

## Form Structure

### Section 1: Incident Case Type Classification

#### Fields:
1. **Injured Case Type** (radio button list, required)
   - Options loaded from lookup: "Injured Case Type"
   - Common values: Fatal, Lost Time Injury, Medical Treatment, First Aid, etc.
   - Selection determines HR notification requirement
   - Error Code: Not explicitly shown but required for submission

### Section 2: HOD Review and Comments

#### Fields:
1. **Review and Comment** (textarea, required)
   - Free text for HOD's review comments
   - MaxLength: Not specified (likely 2000)
   - Error Code: ERR-134 (required)
   - CSS: txtDescbox

### Section 3: Workflow Assignment

#### Fields:
1. **WSHO** (dropdown, required)
   - Populated based on SBA/SBU/Department/Location from Part A
   - Method: `get_wsho_by_sbu(sba_code, sbu_code, department_code, location_code)`
   - Error Code: ERR-135 (required)
   - Primary assignee for Part C investigation

2. **Alternate WSHO** (dropdown, optional)
   - Populated based on SBA/SBU/Department/Location
   - Method: `get_awsho_by_sbu(sba_code, sbu_code, department_code, location_code)`
   - Alternate person if WSHO is unavailable

### Section 4: CC/Email To List

#### Fields:
1. **Email To (Checkbox list)** - Multiple selection
   - Populated based on organizational structure
   - Method: `get_active_cclist_by_sbu(sba_code, sbu_code, department_code, location_code)`
   - Default: All selected
   - These users receive email notifications

2. **Additional Copy To (ListView)** - Dynamic list
   - Can add additional employees to CC list
   - Fields per entry:
     - Employee ID
     - Name
     - Designation
   - Uses employee search functionality

### Section 5: Submitter Information (Read-only)

#### Auto-populated fields:
1. **Name**: Current user's name from employee database
2. **Employee ID**: Current logged-in user ID
3. **Designation**: Current user's designation
4. **Date of Submission**: Current date (dd-MMM-yyyy format)

## Business Logic

### Initialization (InitPartB):
```csharp
private void InitPartB(string incidentid)
{
    1. Get incident details by ID
    2. Verify status == "01" (only show if ready for HOD review)
    3. Get current user's employee info
    4. Populate WSHO dropdown based on org structure
    5. Populate Alternate WSHO dropdown
    6. Populate CC/Email To list (all checked by default)
    7. Set HOD information (name, ID, designation, date)
}
```

### Validation (in btnPartB_Click):
1. **Review and Comment**: Required (ERR-134)
2. **WSHO Selection**: Required (ERR-135)

### Save Logic (btnPartB_Click):

#### Steps:
1. **Validate Input**
   - Check review comment is not empty
   - Check WSHO is selected

2. **Update Incident**
   - Set `injured_case_type` from radio button selection
   - Update incident status to "02"
   - Call: `WorkflowIncidentBC.update_Incidents()`

3. **Create Workflow Entries**
   - For each selected CC/Email To: Create workflow entry with role "COPYTO"
   - For each additional Copy To person: Create workflow entry with role "COPYTO"
   - For WSHO: Create workflow entry with role "WSHO" and HOD's review comment
   - For Alternate WSHO (if selected): Create workflow entry with role "A_WSHO"

4. **Send Email Notifications**
   - If injured case type == 1 (injury case):
     - Get HR list: `get_hrlist_by_sbu()`
     - Send email to HR team
   - Send email to WSHO with incident details and HOD comments

5. **Insert Workflows**
   - Call: `WorkflowIncidentBC.insert_incidents_workflows(incident_id, workflow_ds)`

6. **Redirect**
   - Show success message (SUC-001)
   - Redirect to Home.aspx

## Data Binding (BindPartB - Read-only View):

When status > 1, show read-only view:
1. Hide edit panel (pnl_PartB)
2. Show view panel (pnl_V_PartB)
3. Display:
   - Injured case type
   - HOD's review and comment
   - HOD's name, ID, designation
   - Submission date
4. Show workflow history grid for Part B submissions

## BC (Business Component) Used

### WorkflowIncidentBC
Methods:
1. `get_incident_by_id()` - Get incident details
2. `update_Incidents()` - Update incident with injured case type
3. `insert_incidents_workflows()` - Insert workflow records
4. `get_wirs_incidents_workflows_by_id(incident_id, "02")` - Get Part B workflow history

### UserBC
Methods:
1. `GetEmployeeInfoByEmployeeNo()` - Get current user's employee info
2. `get_wsho_by_sbu()` - Get WSHO list for dropdown
3. `get_awsho_by_sbu()` - Get alternate WSHO list
4. `get_active_cclist_by_sbu()` - Get CC list for email notifications
5. `get_hrlist_by_sbu()` - Get HR list for injury case notifications

## Field Mappings (Old vs New)

### WebForms Controls -> MVC Model:
- `rdoinjured_case_type` -> InjuredCaseType
- `txtPartBReviewandComment` -> ReviewComment
- `ddlPartB_WSHO` -> WshoId
- `ddlPartB_A_WSHO` -> AlternateWshoId
- `chkPartBEmailTo` -> EmailToList (array)
- `lstPartB_CopyTo` -> AdditionalCopyToList (array)
- `lblPartBHODName` -> SubmitterName (read-only)
- `lblPartBHODID` -> SubmitterEmpId (read-only)
- `lblPartBHODDesignation` -> SubmitterDesignation (read-only)
- `lblPartBHODSubmittionDate` -> SubmissionDate (read-only)

## Workflow After Part B Submit

1. Status changes from "01" to "02"
2. Incident assigned to WSHO for investigation (Part C)
3. Email notifications sent to:
   - WSHO (primary assignee)
   - Alternate WSHO (if assigned)
   - All CC list recipients
   - HR team (if injury case)
4. Original reporter (Part A submitter) is notified
5. Incident moves to Part C for WSHO investigation

## Error Codes

- ERR-134: Review and Comment is required
- ERR-135: WSHO selection is required
- SUC-001: Successfully submitted

## UI Sections

### Edit Mode (Status == 01):
- pnl_PartB: Visible, Enabled
- Fields are editable
- Submit button visible

### Read-only Mode (Status > 01):
- pnl_V_PartB: Visible
- pnl_PartB: Hidden
- Shows submitted data only
- Workflow history grid displayed

## Special Features

### HR Notification Logic:
```csharp
if (injured_case_type == 1) // Injury case
{
    DataSet hrds = userBC.get_hrlist_by_sbu(...);
    SendEmailNotification(..., hrds.Tables[0], ...);
}
```

### Additional Copy To:
- Users can add employees beyond the standard CC list
- Uses ListView with employee search
- Each entry requires: Employee ID, Name, Designation
- All entries are added to workflow with role "COPYTO"

## Session Data Used

- `SESSION_OBJ_INCIDENT`: Current incident being worked on
- `SESSION_LIST_WORKFLOWS`: Temporary storage for workflow entries before DB insert
- `SESSION_OBJ_USERLOGIN`: Current logged-in user info

## Reusability Notes

Part B should be implemented as a reusable component that can:
1. Show in edit mode when HOD is reviewing
2. Show in read-only mode when incident is past Part B
3. Display workflow history for this stage
4. Be integrated into the main incident view page

## Complexity Level

- **Form Complexity**: Low
- **Validation**: Simple (2 required fields)
- **Business Logic**: Medium (workflow creation, email notifications)
- **Integration**: Medium (emails, workflow, user lookups)
