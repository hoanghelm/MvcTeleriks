# Part C - Investigation Documentation

## Overview
Part C is the most comprehensive section where WSHO (Workplace Safety and Health Officer) conducts detailed investigation of the incident, documents injury details, performs root cause analysis, and recommends corrective actions.

## Workflow Status
- **From Status**: 02 (HOD Reviewed)
- **To Status**: 03 (WSHO Investigated, forwarded to HOD for comments)
- **User Role**: WSHO (or assigned alternate)
- **Special**: Can also be accessed by authorized users for adding Medical Certificates (MC) after closure (status 08)

## Form Structure Overview

Part C has 7 major sub-sections:
1. **Part C-1**: Eye Witnesses (reused from Part A)
2. **Part C-2**: Person Interviewed
3. **Part C-3**: Injury Details (for injury incidents)
4. **Part C-4**: Medical Certificates/Leaves
5. **Part C-5**: Incident Classification and Cause Analysis
6. **Part C-6**: Root Cause Analysis
7. **Part C-7**: Recommended Actions and Additional Comments

---

## Part C-1: Eye Witnesses

**Note**: This section reuses the Eye Witnesses functionality from Part A.

- Display list of eye witnesses already added in Part A
- Allow adding additional witnesses if needed
- Uses same employee search and ListView as Part A

---

## Part C-2: Persons Interviewed

### Purpose
Document people interviewed during investigation (beyond eye witnesses).

### Fields:
1. **Name** (textbox)
   - Can use employee search
   - Search button opens: `EmployeeSearch.aspx?SearchType=PartC_PersonInterviewedSearch`

2. **Employee Number/NRIC** (textbox)
   - Auto-populated from search or manual entry

3. **Designation** (textbox)
   - Auto-populated from search or manual entry

4. **Contact Number** (textbox)

5. **Add Button** - Adds person to ListView

### ListView Columns:
- Name
- Employee Number
- Designation
- Contact Number
- Delete button

---

## Part C-3: Injury Details

**Note**: Only visible for injury-type incidents (Part A incident type == "1")

### Section 1: Select Injured Person
- **Dropdown**: Lists all injured persons from Part A
- Used to associate injury details with specific person

### Section 2: Injury Description

#### Fields:
1. **Nature of Injury** (checkbox list)
   - Multiple selection allowed
   - Loaded from lookup: "Nature Of Injury"
   - Examples: Fracture, Cut, Burn, Sprain, etc.

2. **Injured Body Part - Head/Neck/Torso** (checkbox list)
   - Multiple selection
   - Loaded from lookup: "Head Neck Torso"
   - Examples: Head, Neck, Chest, Back, etc.

3. **Injured Body Part - Upper Limbs** (checkbox list)
   - Multiple selection
   - Loaded from lookup: "Upper Limbs"
   - Examples: Shoulder, Arm, Elbow, Hand, Fingers, etc.

4. **Injured Body Part - Lower Limbs** (checkbox list)
   - Multiple selection
   - Loaded from lookup: "Lower Limbs"
   - Examples: Hip, Leg, Knee, Ankle, Foot, Toes, etc.

5. **Injury Description** (textarea)
   - Detailed description of injury
   - MaxLength: Not specified

6. **Add Injury Details** - Adds to ListView

### Injury Details ListView:
- Shows all injury details for all injured persons
- Columns: Injured Person Name, Nature of Injury, Body Parts, Description
- Delete functionality per row

---

## Part C-4: Medical Certificates/Leaves

**Purpose**: Track medical certificates and sick leave days for injured persons.

### Fields:
1. **Select Injured Person** (dropdown)
   - Lists injured persons from Part A

2. **MC/Sick Leave From Date** (datepicker)
   - Start date of medical leave
   - Format: dd-MMM-yyyy

3. **MC/Sick Leave To Date** (datepicker)
   - End date of medical leave
   - Format: dd-MMM-yyyy

4. **Number of Days** (textbox or calculated)
   - Auto-calculated from date range
   - Or manual entry

5. **MC Attachment** (file upload)
   - Upload scanned MC document
   - File types: PDF, JPG, PNG
   - Stored in attachments table

6. **Add MC** - Adds to ListView

### MC ListView:
- Injured Person Name
- From Date
- To Date
- Number of Days
- Has Attachment (Yes/No)
- Delete button
- Download attachment link

### Special Feature - Add MC After Closure:
- Authorized users can add MC even after incident is closed (status 08)
- Special panel `pnlAddExtraMC` visible only for this purpose
- Email notification sent to relevant parties when MC is added

---

## Part C-5: Incident Classification and Cause Analysis

### Section 1: Incident Classification

#### Fields:
1. **Incident Class** (checkbox list)
   - Multiple selection
   - Loaded from lookup: "Incident Class"
   - Examples: Slip/Trip/Fall, Struck by Object, Caught in/between, etc.

2. **Incident Agent** (checkbox list)
   - Multiple selection
   - Loaded from lookup: "Incident Agent"
   - Examples: Machinery, Hand Tools, Chemicals, Electricity, etc.

### Section 2: Root Causes - Unsafe Conditions

#### Fields:
1. **Unsafe Conditions** (checkbox list)
   - Multiple selection
   - Loaded from lookup: "Unsafe Condition"
   - Examples: Poor housekeeping, Inadequate guards, Poor lighting, etc.

### Section 3: Root Causes - Unsafe Acts

#### Fields:
1. **Unsafe Acts** (checkbox list)
   - Multiple selection
   - Loaded from lookup: "Unsafe Act"
   - Examples: Failure to use PPE, Operating without authority, Taking shortcuts, etc.

### Section 4: Contributing Factors

#### Fields:
1. **Factors** (checkbox list)
   - Multiple selection
   - Loaded from lookup: "Factors"
   - Examples: Lack of training, Fatigue, Time pressure, Communication failure, etc.

---

## Part C-6: Root Cause Analysis

### Fields:

1. **Was the incident job-related?** (already from Part A, display-only)
   - Yes/No from Part A

2. **Was employee negligent?** (radio button list, required)
   - Options loaded from lookup: "Negligent"
   - Common values: Yes, No
   - Error Code: ERR-136

3. **If Yes, Comments on Negligence** (textarea, conditional required)
   - Required if negligent == "Yes"
   - MaxLength: Not specified
   - Error Code: ERR-137

4. **Does risk assessment need review?** (radio button list)
   - Options: Yes, No
   - Determines if risk assessment process needs updating

5. **Comments on Risk Assessment Review** (textarea, conditional)
   - Required if risk assessment review == "Yes"

6. **What happened and why?** (textarea, required)
   - Detailed root cause analysis narrative
   - Explain sequence of events and underlying reasons
   - Error Code: ERR-138

7. **Recommended Corrective/Preventive Actions** (textarea, required)
   - List of actions to prevent recurrence
   - MaxLength: Not specified
   - Error Code: ERR-139

---

## Part C-7: Final Submission

### Fields:

1. **Additional Comments** (textarea)
   - Any additional information for HOD
   - Optional

2. **WSHO Submission Info** (read-only)
   - WSHO Name
   - WSHO Employee ID
   - WSHO Designation
   - Submission Date

3. **Forward To** (dropdown, required for Close action)
   - Corporate WSHO (CWSHO) for final closure
   - Populated: `get_c_wsho_by_sbu()`
   - Used if closing incident directly (bypass remaining parts)

### Buttons:

1. **Save** (`btnPartCSave`)
   - Saves progress without changing status
   - Allows WSHO to work on investigation over multiple sessions

2. **Submit to HOD** (`btnPartC`)
   - Validates all required fields
   - Changes status from 02 to 03
   - Forwards to HOD for comments (Part D)
   - Sends email notifications

3. **Close Incident** (`btnPartCClose`)
   - Direct closure option
   - Changes status from 02 to 08
   - Bypasses Parts D, E, F, G
   - Used for minor incidents that don't need full workflow
   - Requires CWSHO selection

---

## Business Logic

### Initialization (InitPartC):

```csharp
private void InitPartC(string incidentid)
{
    1. Get incident details by ID
    2. Check status == "02" (ready for investigation)
    3. Load all lookup checkbox lists:
       - Nature of Injury
       - Head/Neck/Torso
       - Upper Limbs
       - Lower Limbs
       - Incident Class
       - Incident Agent
       - Unsafe Conditions
       - Unsafe Acts
       - Factors
       - Negligent (radio list)
    4. Populate injured person dropdowns from Part A data
    5. Initialize empty tables for:
       - Persons interviewed
       - Injury details
       - Medical certificates
    6. Get current user's employee info
    7. Populate CWSHO dropdown
}
```

### Validation (ValidatePartC):

1. **Negligent**: Required (ERR-136)
2. **Negligent Comments**: Required if negligent == "Yes" (ERR-137)
3. **What Happened and Why**: Required (ERR-138)
4. **Recommended Actions**: Required (ERR-139)
5. **Injury Details**: At least one required if incident type == "1"
6. **Incident Classification**: At least one classification required
7. **Root Causes**: At least one unsafe condition OR unsafe act required

### Save Logic (Save_PartC_Record - Save Only):

```csharp
1. Update incident with:
   - Negligent flag
   - Negligent comments
   - Recommended actions
   - What happened and why comments
   - Status remains "02"

2. Save associated data to XML:
   - Persons interviewed (ireport_injuredpersonXML)
   - Injury details (injury_detailsXML)
   - Cause analysis (cause_analysisXML)
   - Medical leaves (medical_leavesXML)
   - Attachments (incidents_attachmentXML)

3. Call: `WorkflowIncidentBC.save_incident_partc()`

4. No workflow changes
5. No email notifications
6. Stay on same page
```

### Submit Logic (Submit_PartC_Record - Submit to HOD):

```csharp
1. Validate all required fields

2. Update incident:
   - Set negligent, comments, actions, what/why
   - Change status from "02" to "03"

3. Create workflow entries:
   - Create entry for HOD with role "HOD" and comments
   - Create entries for CC list with role "COPYTO"

4. Save all investigation data:
   - Persons interviewed
   - Injury details
   - Cause analysis
   - Medical leaves
   - Attachments

5. Call: `WorkflowIncidentBC.submit_incident_partc()`

6. Send email notifications:
   - To HOD for Part D comments
   - To CC list

7. Show success message and redirect to Home
```

### Close Logic (btnPartCClose - Direct Closure):

```csharp
1. Validate required:
   - Additional comments (ERR-134)
   - CWSHO selection (ERR-135)

2. Update incident:
   - Change status from "02" to "08" (closed)
   - Save all investigation data

3. Create workflow entry:
   - Role: "CLOSE"
   - Assigned to: CWSHO
   - Comments: Additional comments

4. Send email notification:
   - To CWSHO and relevant parties
   - Notify of direct closure

5. Redirect to Home
```

### MC Save Logic (Save_MC):

```csharp
1. Get MC data from form
2. Upload MC attachment if provided
3. Save to medical_leaves table
4. Link attachment to incident
5. Send email notification to stakeholders
6. Refresh MC list
```

---

## BC (Business Component) Used

### WorkflowIncidentBC
Methods:
1. `get_incident_by_id()` - Get incident and all related data
2. `get_incident_partc_id()` - Get Part C specific data
3. `save_incident_partc()` - Save Part C data (status unchanged)
4. `submit_incident_partc()` - Submit Part C (status 02->03)
5. `Save_MC()` - Save medical certificate data
6. `insert_incidents_attachfiles()` - Save attachments
7. `get_emaillist_MCChange()` - Get email list for MC changes

### UserBC
Methods:
1. `GetEmployeeInfoByEmployeeNo()` - Current user info
2. `get_c_wsho_by_sbu()` - Get Corporate WSHO list

---

## Field Mappings (Old vs New)

### Injury Details:
- `ddlInjuredpersonPartC_3` -> SelectedInjuredPersonId
- `chkNatureInj` -> NatureOfInjuryList (array)
- `chkHead` -> HeadNeckTorsoList (array)
- `chkUpper` -> UpperLimbsList (array)
- `chkLower` -> LowerLimbsList (array)
- `txtInjuryDescription` -> InjuryDescription

### Medical Certificates:
- `ddlInjuredpersonPartC_4` -> SelectedInjuredPersonId
- `txtMCFromDate` -> MCFromDate
- `txtMCToDate` -> MCToDate
- `txtMCDays` -> NumberOfDays
- `fileUploadMC` -> MCAttachment

### Cause Analysis:
- `chkIncidentClass` -> IncidentClassList (array)
- `chkIncidentAgent` -> IncidentAgentList (array)
- `chkUC` -> UnsafeConditionList (array)
- `chkUA` -> UnsafeActList (array)
- `chkCF` -> FactorsList (array)

### Root Cause:
- `rdoNegligent` -> IsNegligent
- `txtNegligentComments` -> NegligentComments
- `rdoRiskAssessmentReview` -> NeedsRiskAssessmentReview
- `txtRiskAssessmentComments` -> RiskAssessmentComments
- `txtWhatHappenedWhy` -> WhatHappenedAndWhy
- `txtRecommendedActions` -> RecommendedActions

### Persons Interviewed:
- `txtPartC_PersonInterviewedName` -> Name
- `txtPartC_PersonInterviewedID` -> EmployeeNo
- `txtPartC_PersonInterviewedDesignation` -> Designation
- `txtPartC_PersonInterviewedContactNo` -> ContactNo

---

## Error Codes

- ERR-134: Additional comments required (for close action)
- ERR-135: CWSHO selection required (for close action)
- ERR-136: Negligent field is required
- ERR-137: Negligent comments required if negligent == Yes
- ERR-138: "What happened and why" is required
- ERR-139: Recommended actions is required
- ERR-041: At least one eye witness required (if marked as having witnesses)

---

## Session Data Used

- `SESSION_OBJ_INCIDENT`: Current incident
- `SESSION_LIST_WORKFLOWS`: Workflow entries
- `SESSION_LIST_PartC_PersonInterviewed`: Persons interviewed list
- `SESSION_LIST_PartC_InjuryDetails`: Injury details list
- `SESSION_LIST_PartC_MedicalCertificates`: MC list
- `SESSION_LIST_PartC_CauseAnalysis`: Cause analysis data

---

## Special Features

### 1. Dynamic Sections:
- Injury sections only show for injury-type incidents
- MC section can be accessed after closure by authorized users
- Close button bypasses remaining workflow parts

### 2. Multiple ListViews:
- Eye Witnesses (reused from Part A)
- Persons Interviewed
- Injury Details (per injured person)
- Medical Certificates (per injured person)

### 3. Attachment Handling:
- MC documents uploaded and linked to specific MC records
- File type validation
- Size limits
- Download functionality

### 4. Email Notifications:
- Different recipients based on action (Save vs Submit vs Close)
- Include incident details and investigation summary
- Notify when MC is added post-closure

---

## Workflow After Part C Actions

### After Submit:
1. Status: 02 → 03
2. Assigned to: HOD (for Part D comments)
3. Emails sent to: HOD, CC list
4. Next step: Part D

### After Close:
1. Status: 02 → 08
2. Assigned to: CWSHO
3. Parts D, E, F, G are bypassed
4. Incident closed
5. Can still add MC if needed

### After Save:
1. Status: Remains 02
2. No assignment change
3. No emails
4. WSHO can continue working

---

## Complexity Level

- **Form Complexity**: Very High (7 sub-sections, multiple ListViews)
- **Validation**: High (9+ validation rules)
- **Business Logic**: Very High (multiple save paths, attachments, emails)
- **Integration**: Very High (employee search, file uploads, emails, multiple lookups)
- **Estimated Lines of Code**: 1500+ (old), should be ~800 with AngularJS components

---

## Reusability Strategy

Part C should be broken into reusable AngularJS components:

1. **PersonsInterviewedComponent** - Manage interviewed persons list
2. **InjuryDetailsComponent** - Manage injury details per person
3. **MedicalCertificateComponent** - Manage MC records
4. **CauseAnalysisComponent** - Checkboxes for classifications
5. **RootCauseAnalysisComponent** - Root cause fields

Each component should:
- Have its own controller
- Manage its own data array
- Expose add/delete methods
- Support both edit and read-only modes
- Validate its own fields
