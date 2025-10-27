# Part D - HOD Comments on Investigation Documentation

## Overview
Part D is where the Head of Department (HOD) reviews the WSHO's investigation findings from Part C and provides comments before forwarding to Head of SBU (HSBU) for final approval.

## Workflow Status
- **From Status**: 03 (Investigation Complete)
- **To Status**: 04 (HOD Commented, forwarded to HSBU)
- **User Role**: HOD (or assigned alternate)

## Form Structure

### Section 1: HOD Comments

#### Fields:
1. **Comments** (textarea, required)
   - HOD's review of investigation findings
   - Comments on recommended corrective actions
   - Any concerns or additional observations
   - MaxLength: Not specified (likely 2000)
   - Error Code: ERR-137

### Section 2: Forward To

#### Fields:
1. **Head of SBU (HSBU)** (dropdown, required)
   - Select HSBU to approve and assign follow-up actions
   - Populated: `get_hsbu_by_sbu(sba_code, sbu_code, department_code, location_code)`
   - Error Code: ERR-133

### Section 3: CC/Email To

#### Fields:
1. **Email To List** (checkbox list)
   - Standard CC list based on organizational structure
   - Default: All selected
   - Method: `get_active_cclist_by_sbu()`

2. **Additional Copy To** (ListView, optional)
   - Add additional recipients beyond standard list
   - Employee search functionality
   - Fields: Employee ID, Name, Designation

### Section 4: Submitter Information (Read-only)

#### Auto-populated fields:
1. **Name**: Current user's name
2. **Employee ID**: Current user ID
3. **Designation**: Current user's designation
4. **Date**: Current date (dd-MMM-yyyy)

## Business Logic

### Initialization (InitPartD):
```csharp
private void InitPartD(string incidentid)
{
    1. Get incident details by ID
    2. Verify status == "03"
    3. Get current user's employee info
    4. Populate HSBU dropdown
    5. Populate CC/Email To list (all checked by default)
    6. Set HOD information (name, ID, designation, date)
}
```

### Validation (btnPartD_Click):
1. **Comments**: Required (ERR-137)
2. **HSBU Selection**: Required (ERR-133)

### Submit Logic (btnPartD_Click):
```csharp
1. Validate required fields

2. Update status from "03" to "04"

3. Create workflow entries:
   - Role: "HSBU"
   - Assigned to: Selected HSBU
   - Comments: HOD's comments

   For CC list:
   - Role: "COPYTO"
   - For each selected person

4. Save workflows: insert_incidents_workflows()

5. Send email notifications:
   - To HSBU
   - To CC list
   - Include incident summary and HOD comments

6. Success:
   - Show message (SUC-001)
   - Redirect to Home
```

## BC (Business Component) Used

### WorkflowIncidentBC
1. `get_incident_by_id()` - Get incident details

### UserBC
1. `GetEmployeeInfoByEmployeeNo()` - Current user info
2. `get_hsbu_by_sbu()` - Get HSBU list
3. `get_active_cclist_by_sbu()` - Get CC list

## Field Mappings (Old vs New)

- `txtPartD_Comment` -> Comment
- `ddlPartD_HSBUID` -> HsbuId
- `chkPartDEmailTo` -> EmailToList (array)
- `lstPartD_CopyTo` -> AdditionalCopyToList (array)

## Error Codes

- ERR-133: HSBU selection is required
- ERR-137: Comments are required
- SUC-001: Successfully submitted

## Workflow After Part D

1. Status: 03 → 04
2. Assigned to: HSBU (Part E)
3. Emails sent to: HSBU, CC list
4. Next step: Part E (HSBU Approval)

## Complexity Level

- **Form Complexity**: Low
- **Validation**: Simple (2 required fields)
- **Business Logic**: Low (comments and forward)
- **Integration**: Low

## UI Implementation Notes

Similar to Part B, this is a simple comment and forward form:
- Single textarea for comments
- Dropdown for HSBU selection
- Standard CC list checkbox
- Submit button to forward to Part E
