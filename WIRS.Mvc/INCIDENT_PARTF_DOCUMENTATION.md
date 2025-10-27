# Part F - Follow-Up Actions Documentation

## Overview
Part F is where the assigned person (WSHO or HOD from Part E) implements and documents corrective and preventive actions, uploads evidence of completion, and confirms risk assessment review if needed.

## Workflow Status
- **From Status**: 05 (HSBU Approved)
- **To Status**: 06 (Actions Completed, forwarded for verification)
- **User Role**: WSHO or HOD (whoever was assigned in Part E)

## Form Structure

### Section 1: Corrective/Preventive Actions Implementation

#### Fields:
1. **Actions Taken** (textarea, required)
   - Describe implementation of recommended actions from Part C
   - Evidence of completion
   - Any challenges or deviations from plan
   - MaxLength: Not specified (likely 2000)
   - Error Code: ERR-137

2. **Implementation Date** (datepicker)
   - When actions were completed
   - Format: dd-MMM-yyyy

### Section 2: Risk Assessment Review

#### Fields:
1. **Has Risk Assessment been reviewed?** (radio button, required)
   - Yes/No
   - Based on requirement from Part C
   - Error Code: ERR-116

2. **Risk Assessment Review Comments** (textarea, conditional)
   - Required if "Yes" selected
   - Describe changes made to risk assessment
   - New controls implemented

### Section 3: Attachments

#### Purpose: Upload evidence of corrective actions

#### Fields:
1. **Attachment Description** (textbox)
   - Brief description of what's being attached
   - E.g., "Photos of new guard installed", "Training records"

2. **File Upload** (file input)
   - Supported types: PDF, JPG, PNG, DOCX, XLSX
   - Max size: Varies by configuration
   - Multiple attachments allowed

3. **Add Attachment** - Adds to ListView

### Attachment ListView:
- Description
- File Name
- Upload Date
- Uploaded By
- Download link
- Delete button (before submission)

### Section 4: Forward For Verification

#### Fields:
1. **Forward To** (dropdown, required)
   - Select WSHO or CWSHO for verification
   - CWSHO for major incidents
   - WSHO for minor incidents
   - Populated: `get_wsho_by_sbu()` or `get_c_wsho_by_sbu()`
   - Error Code: ERR-133

### Section 5: Submitter Information (Read-only)

#### Auto-populated fields:
1. **Name**: Current user's name
2. **Employee ID**: Current user ID
3. **Designation**: Current user's designation
4. **Date**: Current date (dd-MMM-yyyy)

## Business Logic

### Initialization (InitPartF):
```csharp
private void InitPartF(string incidentid)
{
    1. Get incident details by ID
    2. Verify status == "05"
    3. Get current user's employee info
    4. Populate WSHO dropdown for verification
    5. Display recommended actions from Part C (read-only)
    6. Initialize attachment list
    7. Set user information
}
```

### Validation (btnPartF_Click):
1. **Actions Taken**: Required (ERR-137)
2. **Risk Assessment Review**: Required (ERR-116)
3. **Forward To**: Required (ERR-133)
4. **Attachments**: At least one attachment recommended (warning if none)

### Submit Logic (btnPartF_Click):
```csharp
1. Validate required fields

2. Update incident:
   - Set risk_assessment_review flag
   - Set risk_assessment_review_comments
   - Change status from "05" to "06"
   - Update modified_by and modified_date

3. Save attachments:
   - Upload files to server/storage
   - Insert attachment records
   - Link to incident ID

4. Create workflow entry:
   - Role: "WSHO" or "CWSHO"
   - Assigned to: Selected person for verification
   - Comments: Actions taken summary

5. Send email notifications:
   - To verifier (WSHO/CWSHO)
   - Include actions taken and attachment links

6. Success:
   - Show message (SUC-001)
   - Redirect to Home
```

## BC (Business Component) Used

### WorkflowIncidentBC
1. `get_incident_by_id()` - Get incident details
2. `update_Incidents()` - Update risk assessment review status
3. `insert_incidents_attachfiles()` - Save attachments

### UserBC
1. `GetEmployeeInfoByEmployeeNo()` - Current user info
2. `get_wsho_by_sbu()` - Get WSHO list for verification
3. `get_c_wsho_by_sbu()` - Get CWSHO list

## Field Mappings (Old vs New)

- `txtPartF_Comment` -> ActionsTaken
- `rdorisk` -> RiskAssessmentReviewed
- `txtPartF_RiskComments` -> RiskAssessmentComments
- `txtPartF_ImplementationDate` -> ImplementationDate
- `ddlPartF_WSHO` -> VerifierWshoId
- `lstPartF_Attachments` -> Attachments (array)

## Attachment Structure

### Attachment Model:
```csharp
{
    AttachmentId: Guid/String,
    IncidentId: String,
    FileName: String,
    OriginalFileName: String,
    Description: String,
    FileType: String,
    FileSize: Long,
    UploadedBy: String,
    UploadedDate: DateTime,
    FilePath: String
}
```

## Error Codes

- ERR-116: Risk assessment review selection is required
- ERR-133: Forward to selection is required
- ERR-137: Actions taken is required
- SUC-001: Successfully submitted

## Workflow After Part F

1. Status: 05 → 06
2. Assigned to: WSHO or CWSHO (for Part G verification)
3. Emails sent to: Verifier
4. Next step: Part G (Verification)

## Special Features

### Attachment Management:
- Multiple file uploads supported
- Files stored securely
- Download links in emails
- Version control if resubmitted
- File type validation
- Size limits enforced

### Risk Assessment Integration:
- Links back to Part C risk assessment question
- Ensures follow-through on identified needs
- Documentation required if review conducted

### Evidence Documentation:
- Photos of corrective actions
- Training completion certificates
- Updated procedures/SOPs
- Equipment inspection reports
- Risk assessment updates

## Session Data

- `SESSION_LIST_F_ATTACHMENTS`: Temporary attachment list before DB save
- `SESSION_OBJ_INCIDENT`: Current incident

## Complexity Level

- **Form Complexity**: Medium
- **Validation**: Medium (3 required fields + conditional)
- **Business Logic**: Medium (file uploads, attachment management)
- **Integration**: High (file storage, email attachments)

## UI Implementation Notes

- File upload component with drag-and-drop
- Attachment preview thumbnails
- Progress bar for uploads
- File size/type validation before upload
- Conditional risk assessment section
- Read-only display of recommended actions from Part C for reference
