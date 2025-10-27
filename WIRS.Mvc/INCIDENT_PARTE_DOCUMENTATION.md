# Part E - HSBU Approval Documentation

## Overview
Part E is where the Head of SBU (HSBU) reviews the investigation and HOD comments, then approves and assigns follow-up actions to WSHO or HOD for implementation of corrective measures.

## Workflow Status
- **From Status**: 04 (HOD Commented)
- **To Status**: 05 (HSBU Approved, forwarded for follow-up)
- **User Role**: HSBU
- **Special**: Can revert back to Part C if investigation needs revision

## Form Structure

### Section 1: HSBU Comments

#### Fields:
1. **Comments** (textarea, required)
   - HSBU's approval comments
   - Agreement with findings and recommendations
   - Additional instructions for follow-up
   - MaxLength: Not specified (likely 2000)
   - Error Code: ERR-134

### Section 2: Assign Follow-Up

#### Fields:
1. **Assign Follow-Up To** (dropdown, required)
   - Select either WSHO or HOD
   - Determines who implements corrective actions (Part F)
   - Options from lookup or user list
   - Error Code: ERR-133

2. **WSHO** (dropdown, optional)
   - Select WSHO if assigning to WSHO role
   - Populated: `get_wsho_by_sbu()`

3. **HOD** (dropdown, optional)
   - Select HOD if assigning to HOD role
   - Populated: `get_hod_by_sbu()`

### Section 3: CC/Email To

#### Fields:
1. **Email To List** (checkbox list)
   - Standard CC list
   - Default: All selected
   - Method: `get_active_cclist_by_sbu()`

2. **Additional Copy To** (ListView, optional)
   - Add extra recipients
   - Employee search functionality

### Section 4: Submitter Information (Read-only)

#### Auto-populated fields:
1. **Name**: Current HSBU's name
2. **Employee ID**: Current HSBU user ID
3. **Designation**: HSBU designation
4. **Date**: Current date (dd-MMM-yyyy)

## Business Logic

### Initialization (InitPartE):
```csharp
private void InitPartE(string incidentid)
{
    1. Get incident details by ID
    2. Verify status == "04"
    3. Get current user's employee info
    4. Populate WSHO dropdown
    5. Populate HOD dropdown
    6. Populate CC/Email To list (all checked)
    7. Set HSBU information
}
```

### Validation (btnPartE_Click):
1. **Comments**: Required (ERR-134)
2. **Follow-Up Assignment**: Required (ERR-133)
   - Either WSHO or HOD must be selected

### Submit Logic (btnPartE_Click):
```csharp
1. Validate required fields

2. Update status from "04" to "05"

3. Create workflow entries:
   - If WSHO selected:
     - Role: "WSHO"
     - Assigned to: Selected WSHO

   - If HOD selected:
     - Role: "HOD"
     - Assigned to: Selected HOD

   - For CC list:
     - Role: "COPYTO"

4. Save workflows

5. Send email notifications:
   - To assigned person (WSHO or HOD)
   - To CC list
   - Include HSBU approval and instructions

6. Success:
   - Show message (SUC-001)
   - Redirect to Home
```

### Revert Logic (btnPartEReverttoPartC):
```csharp
1. Get revert reason from user

2. Update status from "04" back to "02"

3. Create workflow entry:
   - Role: "REVERT"
   - Assigned to: Original WSHO
   - Comments: Revert reason

4. Send email notification:
   - To WSHO
   - Explain what needs to be revised

5. Return to Part C for re-investigation
```

## BC (Business Component) Used

### WorkflowIncidentBC
1. `get_incident_by_id()` - Get incident details

### UserBC
1. `GetEmployeeInfoByEmployeeNo()` - Current user info
2. `get_wsho_by_sbu()` - Get WSHO list
3. `get_hod_by_sbu()` - Get HOD list
4. `get_active_cclist_by_sbu()` - Get CC list

## Field Mappings (Old vs New)

- `txtPartE_Comment` -> Comment
- `ddlPartE_WSHO` -> WshoId (if assigning to WSHO)
- `ddlPartE_HOD` -> HodId (if assigning to HOD)
- `chkPartEEmailTo` -> EmailToList (array)
- `lstPartE_CopyTo` -> AdditionalCopyToList (array)

## Error Codes

- ERR-133: Follow-up assignment is required
- ERR-134: Comments are required
- SUC-001: Successfully submitted

## Workflow After Part E

### After Approval:
1. Status: 04 → 05
2. Assigned to: WSHO or HOD (for Part F)
3. Emails sent to: Assigned person, CC list
4. Next step: Part F (Follow-Up Actions)

### After Revert:
1. Status: 04 → 02
2. Assigned back to: WSHO
3. Returns to: Part C (Investigation)
4. WSHO must revise investigation

## Special Features

### Revert to Part C:
- Button: `btnPartEReverttoPartC`
- Used when investigation is incomplete or needs revision
- Requires revert reason/comments
- WSHO receives notification with revision requirements
- All Part C data is preserved

### Flexible Assignment:
- Can assign follow-up to either WSHO or HOD
- Based on organizational responsibility for corrective actions
- Different roles have different follow-up authorities

## Complexity Level

- **Form Complexity**: Low-Medium
- **Validation**: Simple (2 required fields)
- **Business Logic**: Medium (assignment logic, revert option)
- **Integration**: Medium

## UI Implementation Notes

- Radio buttons or toggle for WSHO vs HOD assignment
- Conditional dropdown display based on assignment choice
- Revert button with confirmation dialog
- Read-only view of Part C findings for reference
