# Part H - Final Closure Documentation

## Overview
Part H is the final stage where Corporate WSHO (CWSHO) performs final review, confirms incident closure, and sends notifications to all stakeholders. This completes the incident lifecycle.

## Workflow Status
- **From Status**: 07 (Actions Verified)
- **To Status**: 08 (Incident Closed)
- **User Role**: Corporate WSHO (CWSHO)
- **Special**: Can revert back to Part G if verification is inadequate

## Form Structure

### Section 1: Final Closure Comments

#### Fields:
1. **Closure Comments** (textarea, required)
   - Final summary of incident handling
   - Confirmation of action effectiveness
   - Any lessons learned
   - Final recommendations
   - MaxLength: Not specified (likely 2000)
   - Error Code: ERR-137

2. **Closure Date** (datepicker, auto-filled)
   - Date of closure
   - Default: Current date
   - Format: dd-MMM-yyyy

### Section 2: Final Notifications

#### Fields:
1. **Notify** (checkbox list)
   - Standard notification list
   - Includes: Original reporter, HOD, WSHO, HSBU, etc.
   - Default: All selected
   - Method: `get_active_cclist_by_sbu()`

2. **Additional Copy To** (ListView, optional)
   - Add extra recipients for closure notification
   - Employee search functionality
   - Fields: Employee ID, Name, Designation

### Section 3: Incident Summary (Read-only)

#### Display for final review:
1. **Incident ID**
2. **Incident Date/Time**
3. **Location and Department**
4. **Incident Type and Description**
5. **Injured Persons** (if applicable)
6. **Investigation Summary** (from Part C)
7. **Actions Taken** (from Part F)
8. **Verification Status** (from Part G)
9. **Total Days to Close**

### Section 4: Submitter Information (Read-only)

#### Auto-populated fields:
1. **Name**: CWSHO name
2. **Employee ID**: CWSHO user ID
3. **Designation**: Corporate WSHO
4. **Date**: Current date (dd-MMM-yyyy)

## Business Logic

### Initialization (InitPartH):
```csharp
private void InitPartH(string incidentid)
{
    1. Get incident details by ID
    2. Verify status == "07"
    3. Get current user's employee info
    4. Load complete incident summary (all parts A-G)
    5. Calculate total days from report to closure
    6. Populate notification list (all checked)
    7. Set CWSHO information
}
```

### Validation (btnPartH_Click):
1. **Closure Comments**: Required (ERR-137)

### Submit Logic (btnPartH_Click):
```csharp
1. Validate closure comments

2. Update incident:
   - Change status from "07" to "08" (CLOSED)
   - Set closure_date
   - Set closed_by = current user ID
   - Calculate total_days_to_close

3. Create final workflow entry:
   - Role: "CLOSE"
   - Assigned to: Original creator (for information)
   - Comments: Closure comments

   For notification list:
   - Role: "COPYTO"
   - Each selected person

4. Generate closure report (optional):
   - PDF summary of entire incident lifecycle
   - All parts A-H included
   - Attachments indexed
   - Store for compliance/audit

5. Send email notifications:
   - To all selected recipients
   - Include:
     * Incident summary
     * Closure comments
     * Link to view complete incident
     * Lessons learned

6. Update statistics/dashboards:
   - Incident closed count
   - Days to close metrics
   - Department safety statistics

7. Archive incident data (if configured):
   - Move to closed incidents archive
   - Update reporting tables

8. Success:
   - Show closure message (SUC-001)
   - Redirect to Home
```

### Revert Logic (btnPartHReverttoPartG):
```csharp
1. Prompt for revert reason

2. Update status from "07" back to "06"

3. Create workflow entry:
   - Role: "REVERT"
   - Assigned to: Part G verifier
   - Comments: What needs correction

4. Send email notification:
   - To Part G verifier
   - Explain verification gaps

5. Return to Part G for re-verification
```

## BC (Business Component) Used

### WorkflowIncidentBC
1. `get_incident_by_id()` - Get complete incident details
2. `get_wirs_incidents_workflows_by_id()` - Get all workflow history
3. `update_Incidents()` - Update to closed status
4. `insert_incidents_workflows()` - Insert closure workflows
5. `generate_incident_report()` - Generate PDF (optional)

### UserBC
1. `GetEmployeeInfoByEmployeeNo()` - Current user info
2. `get_active_cclist_by_sbu()` - Get notification list
3. `get_all_incident_stakeholders()` - Get all involved users

## Field Mappings (Old vs New)

- `txtPartH_Comment` -> ClosureComments
- `txtPartH_ClosureDate` -> ClosureDate
- `chkPartHEmailTo` -> NotifyList (array)
- `lstPartH_CopyTo` -> AdditionalNotifyList (array)

## Error Codes

- ERR-137: Closure comments are required
- SUC-001: Incident successfully closed

## Workflow After Part H

### After Closure:
1. Status: 07 → 08 (CLOSED)
2. No further workflow actions
3. Emails sent to: All stakeholders
4. Incident archived
5. Statistics updated

### After Revert:
1. Status: 07 → 06
2. Assigned back to: Part G verifier
3. Returns to: Part G (Verification)

## Special Features

### Complete Incident Summary:
- Read-only view of all Parts A-G
- Timeline of all workflow actions
- All comments from each stage
- All attachments accessible
- Total time metrics

### Comprehensive Notifications:
- Original reporter knows incident is resolved
- All workflow participants notified
- HOD and HSBU informed of closure
- Safety team updated
- HR informed (for injury cases)

### Audit Trail:
- Complete history of who did what and when
- All comments preserved
- All attachments retained
- Workflow sequence documented
- Useful for compliance audits

### Closure Report (Optional):
- PDF generation of complete incident
- Suitable for regulatory submission
- Archival purposes
- Trend analysis

### Post-Closure Actions:
- Update safety statistics
- Feed into safety metrics dashboard
- Contribute to monthly safety reports
- Input for safety committee meetings

## Session Data

- `SESSION_OBJ_INCIDENT`: Final incident state
- `SESSION_LIST_WORKFLOWS`: All workflow history

## Complexity Level

- **Form Complexity**: Medium (comprehensive summary display)
- **Validation**: Simple (1 required field)
- **Business Logic**: High (notifications, reporting, statistics)
- **Integration**: Very High (email, reporting, dashboard updates, archival)

## UI Implementation Notes

### Closure Summary View:
- Accordion/tab view showing all parts
- Expandable sections for each workflow stage
- Timeline visualization
- Metrics display (days to close, participants, etc.)
- Download complete report button

### Notification Section:
- Select all/deselect all for checkbox list
- Preview notification email
- Add custom message to notification

### Confirmation:
- Final confirmation dialog before closing
- "Are you sure?" prompt
- Cannot be undone (unless reopened by admin)

## Post-Closure Features

### Incident Can Still:
- Be viewed in read-only mode (status 08)
- Have Medical Certificates added (Part C special feature)
- Be referenced by incident ID
- Be reopened by authorized users (status 08 → 02)
- Be included in reports and statistics

### Incident Cannot:
- Have workflow actions modified
- Be deleted (audit requirement)
- Have investigation details changed
- Have status changed (except by admin)

## Metrics Calculated at Closure

1. **Days to Close**: Report date to closure date
2. **Days per Stage**: Time spent in each part
3. **Number of Reverts**: How many times incident was sent back
4. **Number of Stakeholders**: Total people involved
5. **Attachment Count**: Total documents uploaded
6. **Injury Case Flag**: Used in injury statistics

## Closure Checklist

Before closing, system validates:
- [ ] Part A: Initial report complete
- [ ] Part B: HOD review done
- [ ] Part C: Investigation complete with all required sections
- [ ] Part D: HOD comments on investigation
- [ ] Part E: HSBU approval given
- [ ] Part F: Corrective actions implemented with evidence
- [ ] Part G: Actions verified by WSHO/CWSHO
- [ ] Part H: Final closure comments provided

## Reusability Notes

Part H should include:
1. **IncidentSummaryComponent**: Reusable display of complete incident
2. **WorkflowTimelineComponent**: Visual timeline of all actions
3. **NotificationManagerComponent**: Handle notification list
4. **ClosureConfirmationDialog**: Final confirmation before closure

## Integration Points

- **Email Service**: Send bulk notifications
- **Reporting Engine**: Generate PDF reports
- **Dashboard Service**: Update real-time statistics
- **Archive Service**: Move to historical data
- **Audit Service**: Log closure action
- **HR System**: Update injury records (if injury case)
- **Compliance System**: Flag for regulatory reporting (if required)
