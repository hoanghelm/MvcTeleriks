# Part G - Verification Documentation

## Overview
Part G is where WSHO or Corporate WSHO (CWSHO) verifies that the corrective and preventive actions from Part F have been properly implemented and are effective.

## Workflow Status
- **From Status**: 06 (Actions Completed)
- **To Status**: 07 (Actions Verified, ready for closure)
- **User Role**: WSHO or CWSHO (whoever was assigned in Part F)
- **Special**: Can revert back to Part F if actions are incomplete/inadequate

## Form Structure

### Section 1: Verification Findings

#### Fields:
1. **Verification Comments** (textarea, required)
   - Confirmation that actions were implemented
   - Assessment of action effectiveness
   - Any gaps or additional requirements
   - MaxLength: Not specified (likely 2000)
   - Error Code: ERR-134

2. **Verification Date** (datepicker, auto-filled)
   - Date of verification
   - Default: Current date
   - Format: dd-MMM-yyyy

### Section 2: Verification Evidence/Attachments

#### Purpose: Upload verification evidence (optional but recommended)

#### Fields:
1. **Attachment Description** (textbox)
   - Description of verification evidence
   - E.g., "Verification inspection photos", "Effectiveness check results"

2. **File Upload** (file input)
   - Photos of implemented actions
   - Inspection checklists
   - Test results
   - Multiple attachments allowed

3. **Add Attachment** - Adds to ListView

### Attachment ListView:
- Description
- File Name
- Upload Date
- Uploaded By
- Download link
- Delete button

### Section 3: Forward For Closure

#### Fields:
1. **Forward To** (dropdown, required)
   - Corporate WSHO (CWSHO) for final closure
   - Must be CWSHO level for closure authority
   - Populated: `get_c_wsho_by_sbu()`
   - Error Code: ERR-133

2. **HOD (Optional)** (dropdown)
   - Can notify HOD of closure
   - Populated: `get_hod_by_sbu()`

### Section 4: Submitter Information (Read-only)

#### Auto-populated fields:
1. **Name**: Current WSHO/CWSHO name
2. **Employee ID**: Current user ID
3. **Designation**: WSHO/CWSHO designation
4. **Date**: Current date (dd-MMM-yyyy)

## Business Logic

### Initialization (InitPartG):
```csharp
private void InitPartG(string incidentid)
{
    1. Get incident details by ID
    2. Verify status == "06"
    3. Get current user's employee info
    4. Display Part F actions taken (read-only reference)
    5. Display Part F attachments (for review)
    6. Populate CWSHO dropdown for closure
    7. Populate HOD dropdown (optional)
    8. Initialize attachment list
    9. Set user information
}
```

### Validation (btnPartG_Click):
1. **Verification Comments**: Required (ERR-134)
2. **Forward To (CWSHO)**: Required (ERR-133)

### Submit Logic (btnPartG_Click):
```csharp
1. Validate required fields

2. Update status from "06" to "07"

3. Save verification attachments:
   - Upload files
   - Insert attachment records
   - Link to incident

4. Create workflow entry:
   - Role: "CWSHO"
   - Assigned to: Selected CWSHO
   - Comments: Verification comments

   If HOD selected:
   - Role: "HOD" (notification only)
   - No action required

5. Send email notifications:
   - To CWSHO for final closure
   - To HOD if selected (informational)
   - Include verification summary

6. Success:
   - Show message (SUC-001)
   - Redirect to Home
```

### Revert Logic (btnPartGReverttoPartF):
```csharp
1. Prompt for revert reason

2. Update status from "06" back to "05"

3. Create workflow entry:
   - Role: "REVERT"
   - Assigned to: Part F submitter (WSHO/HOD)
   - Comments: What needs to be corrected

4. Send email notification:
   - To Part F submitter
   - Explain what needs revision

5. Return to Part F for action revision
```

## BC (Business Component) Used

### WorkflowIncidentBC
1. `get_incident_by_id()` - Get incident details
2. `insert_incidents_attachfiles()` - Save verification attachments

### UserBC
1. `GetEmployeeInfoByEmployeeNo()` - Current user info
2. `get_c_wsho_by_sbu()` - Get CWSHO list for closure
3. `get_hod_by_sbu()` - Get HOD list (optional notification)

## Field Mappings (Old vs New)

- `txtPartG_Comment` -> VerificationComments
- `txtPartG_VerificationDate` -> VerificationDate
- `ddlPartG_CWSHO` -> CwshoId
- `ddlPartG_HOD` -> HodId (optional)
- `lstPartGAttach` -> Attachments (array)

## Error Codes

- ERR-133: CWSHO selection is required
- ERR-134: Verification comments are required
- SUC-001: Successfully submitted

## Workflow After Part G

### After Verification:
1. Status: 06 → 07
2. Assigned to: CWSHO (for Part H closure)
3. Emails sent to: CWSHO, HOD (if selected)
4. Next step: Part H (Final Closure)

### After Revert:
1. Status: 06 → 05
2. Assigned back to: Part F submitter
3. Returns to: Part F (Follow-Up Actions)
4. Actions must be revised/completed

## Special Features

### Revert to Part F:
- Button: `btnPartGReverttoPartF`
- Used when actions are incomplete or ineffective
- Requires detailed revert reason
- Original submitter notified
- Must re-implement and resubmit

### Verification Evidence:
- Photos of implemented controls
- Effectiveness test results
- Inspection reports
- Employee feedback
- Risk assessment updates

### Optional HOD Notification:
- Keeps HOD informed of closure
- No action required from HOD
- Good practice for transparency

## Session Data

- `SESSION_LIST_G_ATTACHMENTS`: Temporary attachment list
- `SESSION_OBJ_INCIDENT`: Current incident

## Complexity Level

- **Form Complexity**: Medium
- **Validation**: Simple (2 required fields)
- **Business Logic**: Medium (revert logic, attachments)
- **Integration**: Medium (file uploads, references to Part F)

## UI Implementation Notes

- Display Part F actions in read-only for reference
- Show Part F attachments (download/view)
- File upload for verification evidence
- Revert button with confirmation dialog
- Timeline view showing all previous parts
- Conditional HOD notification checkbox
