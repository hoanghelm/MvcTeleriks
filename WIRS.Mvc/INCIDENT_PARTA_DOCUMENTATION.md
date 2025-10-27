# Part A - Incident Report Documentation

## Overview
Part A is the initial incident report form where users fill in incident details and submit for review. This is the first step in the workflow process.

## Form Structure

### Section 1: Incident Details

#### Fields:
1. **Incident Type** (dropdown, required)
   - Populated from lookup: "Incident Type"
   - OnChange: Triggers `ddlIncidentType_SelectedIndexChanged`
   - Function: Hides/shows sections based on incident type

2. **If 'Others', please specify** (textbox)
   - Enabled only when Incident Type is "Others"
   - MaxLength: Not specified
   - CSS: txtOtherbox

3. **Incident No** (label, readonly)
   - Visible only when editing existing incident
   - Shows auto-generated incident ID

4. **Date of Incident** (datepicker, required)
   - Format: dd-MMM-yyyy (e.g., 01-Jan-2024)
   - Validation: Must be exactly 11 characters, cannot be future date
   - Error Codes: ERR-002 (required), ERR-010 (invalid format), ERR-011 (future date)

5. **Time (24-hr)** (textbox, required)
   - Format: HHMM (e.g., 1430 for 2:30 PM)
   - MaxLength: 4 characters
   - Validation: Must be 4 digits, valid 24-hour time
   - Error Codes: ERR-003 (required), ERR-012 (invalid format)

6. **Sector** (dropdown)
   - Populated from SBA lookup
   - OnChange: Triggers cascade to populate LOB dropdown
   - Initially disabled (value from user session)

7. **LOB** (dropdown, required)
   - Populated based on selected Sector
   - OnChange: Triggers cascade to populate Department
   - Error Code: ERR-004 (required)

8. **Department** (dropdown)
   - Populated based on selected LOB
   - OnChange: Triggers cascade to populate Location

9. **Location of Incident** (dropdown)
   - Populated based on selected Department and LOB

10. **Exact Location** (textbox)
    - Free text field for specific location details
    - CSS: txtOtherbox

11. **Near-miss / Incident Description** (textarea, required)
    - Max Length: 2000 characters
    - Error Codes: ERR-013 (required), ERR-115 (exceeds max length)
    - CSS: txtDescbox

### Section 2: Particulars of Injured Person(s)

#### Injured Type Selection (Radio buttons):
- **Employee** (default selected)
- **For Contractor/Public**

#### Injured Person Fields:

1. **Name** (textbox, required)
   - Can be populated via Employee Search
   - CSS: _Name
   - Error Code: ERR for missing name

2. **Employee Search Button** (...)
   - Opens popup: `EmployeeSearch.aspx?SearchType=InjuredSearch`
   - Callback: `setInjuredSearch()` function
   - Populates all employee-related fields

3. **Contact Number** (textbox)
   - Auto-populated from employee search

4. **Employee Number** (textbox, required for Employee type)
   - Auto-populated from employee search or manual entry

5. **Age** (hidden field)
   - Stored in `hdnInjured_Age`
   - Auto-populated from employee search

6. **Company** (textbox, visible for Contractor/Public)
   - MaxLength: 50
   - Visible based on Injured Type selection

7. **Race** (textbox)
   - MaxLength: 10
   - Auto-populated from employee search

8. **Nationality** (textbox)
   - MaxLength: 20
   - Auto-populated from employee search

9. **Gender** (textbox)
   - MaxLength: 10
   - Auto-populated from employee search

10. **Designation** (textbox)
    - MaxLength: 40
    - Auto-populated from employee search

11. **Employment Type** (textbox)
    - MaxLength: 10
    - Auto-populated from employee search

12. **Date of Employment** (textbox)
    - Format: dd-MMM-yyyy
    - Auto-populated from employee search

13. **Add Button**
    - Adds injured person to ListView
    - OnClick: `btnAddPartAInjured_Click`

#### Injured Person ListView:
- Displays list of added injured persons
- Columns: Name, Employee Number/NRIC, Race, Gender, Age, Company, Contact No., Nationality, Designation, Employment Type, Date of Employment, Delete
- Delete functionality available per row
- Event: `lstViewInjuredPerson_ItemDeleting`
- **Validation**: At least 1 injured person required if Incident Type = "1"
- Error Code: ERR-132

### Section 3: Additional Incident Information (pnl_PartA_2)

1. **Is this job related?** (radio button list)
   - Options: Yes/No
   - Stores text value

2. **Name of Hospital/Clinic examined at** (textbox)
   - Free text entry

3. **Was the employee working overtime?** (textbox)
   - Free text entry

4. **Official Working Hours** (textbox)
   - Free text entry

5. **Damage Description** (textarea, required for non-injury incidents)
   - Required when Incident Type != "1"
   - Error Code: ERR-141

6. **Are there any eye witnesses?** (radio button list)
   - Options: 1=Yes, 0=No
   - OnChange: Shows/hides eyewitness section
   - **Validation**: If "Yes" selected, at least 1 eyewitness required
   - Error Code: ERR-041

### Section 4: Workflow Recipients (Part A submission)

1. **HOD ID** (dropdown)
   - Populated based on SBU/Department/Location
   - Required for workflow

2. **AHOD** (dropdown)
   - Populated based on SBU/Department/Location
   - Optional

3. **WSHO ID** (dropdown)
   - Populated based on SBU/Department/Location
   - Required for workflow

4. **Copy To** (checkbox list)
   - Populated based on SBU/Department/Location
   - Default: All selected
   - Multiple recipients allowed

## Business Logic

### Initialization (InitPartA):
1. Sets panel visibility (pnl_PartA, pnl_PartA_1, pnl_PartA_2)
2. Populates Incident Type dropdown from lookup
3. Populates SBA dropdown
4. Cascades to populate SBU, Department, Location
5. Populates HOD, AHOD, WSHO, CopyTo lists

### Data Binding (BindPartA):
- When editing existing incident (status > 0):
  - Hides edit panels
  - Shows read-only view (pnl_V_PartA)
  - Displays workflow history
  - Shows injured persons in grid view

### Cascading Dropdowns:
1. **SBA -> SBU**: `ddlsba_SelectedIndexChanged`
   - Calls: `appHelper.Setup_all_sbus(ddlSbu, sba_code)`

2. **SBU -> Department**: `ddlSbu_SelectedIndexChanged`
   - Calls: `appHelper.get_active_departments(string.Empty, sba_code, sbu_code)`

3. **Department -> Location**: `ddlDepartment_SelectedIndexChanged`
   - Calls: `appHelper.get_active_locations(sba_code, sbu_code, department_code)`
   - Also populates: HOD, AHOD, WSHO, CopyTo lists

### Validation (ValidatePartAFileds):
1. Incident Date: Required, valid format, not future date
2. Incident Time: Required, valid 24-hour format
3. LOB (SBU): Required
4. Incident Description: Required, max 1000 characters (duplicate validation in code)
5. Injured Person: At least 1 required if Incident Type = "1"
6. Damage Description: Required if Incident Type != "1"
7. Eyewitnesses: At least 1 required if "Has Eyewitness" = Yes

### Save Logic (Save_PartA_Record):

#### Creates WorkflowIncidentBE object:
- `sba_code`: Selected sector
- `incident_datetime`: Combined date + time (dd-MMM-yyyy HH:mm)
- `sbu_code`: Selected LOB
- `department`: Selected department
- `location`: Selected location
- `exact_location`: Exact location text
- `incident_desc`: Incident description
- `superior_emp_no`: Current user ID
- `superior_name`: Current user name
- `status`: "01" (initial status)
- `damage_description`: Damage description
- `any_eyewitness`: 0 or 1
- `is_working_overtime`: Overtime text
- `is_jobrelated`: Job related (Yes/No text)
- `examined_hospital_clinic_name`: Hospital/clinic name
- `official_working_hrs`: Official working hours

#### Collects Session Data:
- Injured persons: `SESSION_LIST_InjuredPerson`
- Eyewitnesses: `SESSION_LIST_PartC_EyeWitnesses`

#### Creates Workflows:
1. Sets action_role = "HOD", assigns to selected HOD
2. If AHOD selected, sets action_role = "AHOD", assigns to selected AHOD
3. Sets action_role = "WSHO", assigns to selected WSHO
4. Loops through selected CopyTo list, sets action_role = "CC"

#### Calls Business Component:
- `WorkflowIncidentBC.insert_Incidents()`
- Passes XML datasets for:
  - Incident types
  - Injured persons
  - Eyewitnesses
  - Workflows

## BC (Business Component) Used

### WorkflowIncidentBC
Located at: WIRS.DataAccess\Implementations\WorkflowIncidentDataAccess.cs

#### Methods called from Part A:
1. `get_incident_by_id(WorkflowIncidentBE)` - Retrieves incident by ID
2. `insert_Incidents()` - Creates new incident with all related data
3. `update_incidents_header()` - Updates incident header when saving
4. `validate_workflowuser()` - Validates user can work on workflow
5. `validate_user_to_edit_inc()` - Validates user can edit incident
6. `get_wirs_incidents_workflows_by_id()` - Gets workflow history

### UserBC
Located at: WIRS.DataAccess\Implementations\UserDataAccess.cs

#### Methods used:
1. `get_wsho_by_sbu()` - Gets WSHO users for dropdown
2. `get_hod_by_sbu()` - Gets HOD users for dropdown
3. `get_ahod_by_sbu()` - Gets AHOD users for dropdown
4. `get_partA_cclist_by_sbu()` - Gets CC list for dropdown

## Field Mappings (Old vs New)

### WebForms Controls -> MVC Model:
- `txtIncDate` -> IncidentDate
- `txtIncTime` -> IncidentTime
- `ddlIncidentType` -> IncidentType (single or array)
- `ddlsba` -> SbaCode (Sector)
- `ddlSbu` -> SbuCode (LOB)
- `ddlDepartment` -> Department
- `ddlLocation` -> Location
- `txtLocation` -> ExactLocation
- `txtIncDescription` -> IncidentDesc
- `txtIncDescription_2` -> DamageDescription
- `rdoHasEyeWitness` -> AnyEyewitness
- `rdois_job_related` -> IsJobrelated
- `txthospital_name` -> ExaminedHospitalClinicName
- `txtisworkingovertime` -> IsWorkingOvertime
- `txtoffical_work_hrs` -> OfficialWorkingHrs
- `lstViewInjuredPerson` -> InjuredPersons (array)
- `lstViewPartC_EyeWitnesses` -> Eyewitnesses (array)
- `ddlPartA_HODID` -> HodId
- `ddlPartA_AHOD` -> AhodId
- `ddlPartA_WSHOID` -> WshoId
- `chkPartACopyTo` -> CopyToList (array)

## Workflow after Part A Submit

1. Status changes from null/0 to "01"
2. Incident ID is generated
3. Email notifications sent to:
   - HOD
   - AHOD (if assigned)
   - WSHO
   - CC list
4. Incident moves to Part B for HOD review
5. Original creator can still edit via "Save" if WSHO user

## Error Codes

- ERR-002: Incident Date is required
- ERR-003: Incident Time is required
- ERR-004: LOB is required
- ERR-010: Invalid date format
- ERR-011: Incident date cannot be future date
- ERR-012: Invalid time format
- ERR-013: Incident Description is required
- ERR-041: At least one eyewitness is required
- ERR-115: Incident Description exceeds maximum length
- ERR-132: At least one injured person is required
- ERR-141: Damage Description is required

## JavaScript Functions

### Employee Search:
- `SearchInjuredPerson()` - Opens employee search popup
- `setInjuredSearch(id, name, contactno, nric, age, race, nationlity, gender, employmenttype, dateofemployment, empdesignation, empemailaddress)` - Callback to populate fields

### Tab Management:
- jQuery tabs widget used
- Active tab stored in `hdncurrenttab` hidden field

### Date Pickers:
- jQuery UI datepicker
- Format: dd-M-yy
- Year range: 1980-2030
