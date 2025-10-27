# WIRS Incident Report Migration Instructions

## Overview
Migrate the `Create_Incident_Report` WebForms page to ASP.NET Core MVC using AngularJS and Kendo UI. This is a complex, multi-stage workflow system for incident reporting and management.

---

## Project Structure

### Source (Old WebForms)
- **Location**: `WIRS.Mvc\old-webforms\`
- **Files**:
  - `Create_Incident_Report.aspx` (2500+ lines)
  - `Create_Incident_Report.aspx.cs` (8000+ lines)
  - `CommonFunc.cs` (shared utilities)

### Target (New MVC)
- **View**: `WIRS.Mvc\Views\Incident\Create.cshtml`
- **Controller**: `WIRS.Mvc\Controllers\IncidentController.cs`
- **JavaScript**:
  - `wwwroot/js/incident/incident-create-app.js`
  - `wwwroot/js/incident/incident-create-service.js`
  - `wwwroot/js/incident/incident-create-controller.js`

---

## Documentation References

### ⚠️ IMPORTANT: Read These First!

Before starting any migration work, **READ THESE DOCUMENTS**:

1. **`MIGRATION_STATUS.md`** - Current status, what's done, what's remaining
2. **Part-specific documentation** (detailed field mappings and logic):
   - `INCIDENT_PARTA_DOCUMENTATION.md` - Initial report (Status: 00 → 01) ✅ MIGRATED
   - `INCIDENT_PARTB_DOCUMENTATION.md` - HOD Review (Status: 01 → 02)
   - `INCIDENT_PARTC_DOCUMENTATION.md` - Investigation (Status: 02 → 03)
   - `INCIDENT_PARTD_DOCUMENTATION.md` - HOD Comments (Status: 03 → 04)
   - `INCIDENT_PARTE_DOCUMENTATION.md` - HSBU Approval (Status: 04 → 05)
   - `INCIDENT_PARTF_DOCUMENTATION.md` - Follow-up Actions (Status: 05 → 06)
   - `INCIDENT_PARTG_DOCUMENTATION.md` - Verification (Status: 06 → 07)
   - `INCIDENT_PARTH_DOCUMENTATION.md` - Final Closure (Status: 07 → 08)

Each documentation file contains:
- Complete field listings with validation rules
- Business logic explanation
- BC (Business Component) to DataAccess mappings
- Error codes
- Session data usage
- Workflow transitions
- Old field to new field mappings

---

## Architecture Overview

### Data Flow
```
User Input (AngularJS)
    ↓
Angular Service (API calls)
    ↓
MVC Controller
    ↓
Service Layer (WIRS.Services)
    ↓
DataAccess Layer (WIRS.DataAccess)
    ↓
PostgreSQL Database
```

### Technology Stack
- **Frontend**: AngularJS 1.8.2 + Kendo UI
- **Backend**: ASP.NET Core MVC
- **Styling**: Tailwind CSS
- **Database**: PostgreSQL (existing)

---

## Business Component (BC) to DataAccess Mapping

### Pattern
Old code uses BC (Business Component) classes that map to DataAccess classes:

```csharp
// OLD (WebForms)
WorkflowIncidentBC ibc = new WorkflowIncidentBC();
WorkflowIncidentBE ibe = new WorkflowIncidentBE();
DataSet ds = ibc.get_incident_by_id(ibe);

// NEW (MVC)
// BC calls are replaced by:
// 1. Controller receives request
// 2. Calls Service layer (WIRS.Services)
// 3. Service calls DataAccess (WIRS.DataAccess)
```

### Common BC Mappings:
| Old BC Class | New DataAccess | Notes |
|--------------|----------------|-------|
| `WorkflowIncidentBC` | `WorkflowIncidentDataAccess` | All incident CRUD and workflows |
| `UserBC` | `UserDataAccess` | User lookups, employee search |
| `CommonFunc` | Various `MasterDataService` methods | Dropdowns, lookups |
| `ErrorMessageBC` | Not needed (use validation in controller) | Error code lookups |

### Finding Existing Implementations:
1. **Check Controller first**: `WIRS.Mvc\Controllers\` - May already have API endpoint
2. **Check Service**: `WIRS.Services\Implementations\` - May have business logic
3. **Check DataAccess**: `WIRS.DataAccess\Implementations\` - Data layer always here

---

## Incident Workflow System

### Status Flow
```
00 (Draft)
  ↓ Submit (Part A)
01 (Pending HOD Review)
  ↓ HOD Review (Part B)
02 (Pending WSHO Investigation)
  ↓ WSHO Investigate (Part C)
03 (Pending HOD Comment)
  ↓ HOD Comment (Part D)
04 (Pending HSBU Approval)
  ↓ HSBU Approve (Part E)
05 (Pending Follow-up)
  ↓ Implement Actions (Part F)
06 (Pending Verification)
  ↓ Verify Actions (Part G)
07 (Pending Closure)
  ↓ Close Incident (Part H)
08 (Closed)
```

### Part Summary

| Part | Name | User Role | Complexity | Lines of Code (est.) |
|------|------|-----------|------------|---------------------|
| **A** | Initial Report | Any User | High | 600 ✅ |
| **B** | HOD Review | HOD | Low | 200 |
| **C** | Investigation | WSHO | Very High | 1500 |
| **D** | HOD Comment | HOD | Low | 150 |
| **E** | HSBU Approval | HSBU | Medium | 250 |
| **F** | Follow-up | WSHO/HOD | Medium | 400 |
| **G** | Verification | WSHO/CWSHO | Medium | 300 |
| **H** | Closure | CWSHO | Medium | 350 |

---

## Migration Strategy

### Phase 1: Part A (Initial Report) ✅ COMPLETE
**Status**: 80% complete - UI and logic done, API endpoints needed

**What's Done**:
- ✅ Create.cshtml with full AngularJS form
- ✅ AngularJS app, service, and controller
- ✅ All form fields and validation
- ✅ Cascading dropdowns
- ✅ Injured persons management
- ✅ Eye witnesses management
- ✅ Kendo UI integration

**What's Remaining**:
- ⏳ Add API endpoints (10-15 endpoints)
- ⏳ Test integration
- ⏳ Fix bugs found during testing

### Phase 2: Parts B-H (Workflow Stages)
**Status**: Documentation complete, implementation pending

**Recommended Order**:
1. Part B (simplest - comments and forward)
2. Part D (similar to Part B)
3. Part E (adds approval logic)
4. Part F (adds file uploads)
5. Part G (similar to Part F)
6. Part H (final closure logic)
7. Part C (most complex - save for last)

---

## Reusability Requirements

### Part A Component Reusability
Part A must work in TWO modes:

1. **Create Mode** (Status: 00 → 01)
   - All fields editable
   - Validation active
   - Submit button enabled
   - Used in: Create.cshtml

2. **Read-Only Mode** (Status: >= 01)
   - All fields disabled/read-only
   - Show submitted data
   - Used in: View.cshtml (when viewing existing incidents)

### Implementation Pattern
```javascript
// AngularJS Component
app.component('partA', {
    bindings: {
        incident: '<',
        mode: '@',  // 'edit' or 'view'
        onSubmit: '&'
    },
    templateUrl: '/templates/incident/part-a.html',
    controller: PartAController
});
```

### Reusable Services
Create separate services for:
- **IncidentService**: Incident CRUD operations
- **MasterDataService**: Dropdowns (sectors, LOBs, departments, locations)
- **UserService**: User lookups (HOD, WSHO, employee search)
- **AttachmentService**: File uploads/downloads
- **WorkflowService**: Workflow operations

---

## Coding Rules

### 1. No Comments
- ❌ `// Get user info`
- ✅ Clear variable names: `currentUserInfo` not `info`

### 2. Hard-coded vs Database Values
- **Hard-code**: Yes/No options, simple static lists
- **Database**: Sectors, LOBs, Departments, Locations, Users, Lookups

### 3. API Endpoints Pattern
```csharp
// Controller action
[HttpGet]
public async Task<IActionResult> GetSectors()
{
    var sectors = await _masterDataService.GetSectors();
    return Json(new { success = true, data = sectors });
}

// AngularJS call
IncidentService.getSectors()
    .then(function(data) {
        vm.sectors = data;
    });
```

### 4. Error Handling
```javascript
// Always handle errors
IncidentService.createIncident(data)
    .then(function(response) {
        vm.successMessage = 'Incident created successfully';
    })
    .catch(function(error) {
        vm.errorMessage = error.message || 'An error occurred';
    });
```

### 5. Validation
- Client-side: AngularJS + Kendo UI validation
- Server-side: Controller + Service validation
- Never trust client-side validation alone

---

## File Naming Conventions

### AngularJS Files
- `incident-create-app.js` - Module definition
- `incident-create-service.js` - API services
- `incident-create-controller.js` - Page controller
- `partB-component.js` - Reusable Part B component
- `partC-investigation-component.js` - Reusable Part C component

### Views
- `Create.cshtml` - Part A only
- `View.cshtml` - View all parts (read-only + workflow actions)
- `_PartAComponent.cshtml` - Partial view for Part A (if needed)

### CSS
- `incident-create.css` - Part A styling
- `incident-view.css` - View page styling
- `incident-workflow.css` - Workflow components styling

---

## Common Patterns from Old Code

### 1. Employee Search
```csharp
// OLD
openEmployeeSearchwindow('InjuredSearch');
function setInjuredSearch(id, name, contactno, ...) {
    // Populate fields
}

// NEW
// Use existing modal: _EmployeeSearchModal.cshtml
// Or create AngularJS directive
vm.searchEmployee = function() {
    employeeSearchModal.open('InjuredSearch')
        .then(function(employee) {
            vm.injured.name = employee.name;
            vm.injured.empId = employee.id;
        });
};
```

### 2. Cascading Dropdowns
```javascript
// OLD: Server-side cascading with postback
// NEW: Client-side cascading
vm.onSectorChange = function() {
    vm.selectedLOB = null;
    vm.selectedDepartment = null;

    IncidentService.getLOBs(vm.selectedSector)
        .then(function(data) {
            vm.lobs = data;
        });
};
```

### 3. ListView/GridView
```csharp
// OLD: ListView with server-side data binding
// NEW: Kendo Grid with client-side data
vm.injuredGridOptions = {
    dataSource: vm.injuredPersons,
    columns: [
        { field: 'name', title: 'Name' },
        { field: 'employeeNo', title: 'Employee No' },
        { command: { text: 'Delete', click: deleteRow }, title: '' }
    ]
};
```

### 4. Session Data
```csharp
// OLD: Session stores temporary data
AppSession.SetSession("SESSION_LIST_InjuredPerson", list);

// NEW: AngularJS arrays in controller scope
vm.injuredPersons = [];
vm.addInjuredPerson = function(person) {
    vm.injuredPersons.push(person);
};
```

### 5. Validation with Error Codes
```csharp
// OLD: Error code system (ERR-001, ERR-002, etc.)
errorCode = "ERR-002";
baseHelper.popUpMessageSetfocus(Page, "Error", errorCode, "0", control.ClientID);

// NEW: Direct validation messages
if (!vm.incident.incidentDate) {
    vm.validationMessage = 'Incident Date is required';
    return false;
}
```

---

## Testing Checklist

For each part, test:

- [ ] Page loads without errors
- [ ] All dropdowns populate correctly
- [ ] Cascading dropdowns work
- [ ] Data can be added to grids/lists
- [ ] Data can be deleted from grids/lists
- [ ] Form validation shows correct errors
- [ ] Form submits successfully
- [ ] Data is saved to database
- [ ] Workflow is created correctly
- [ ] Email notifications sent (if applicable)
- [ ] Read-only mode works
- [ ] Revert functionality works (if applicable)

---

## Next Steps for Current Work

1. **Complete Part A API Endpoints** (see MIGRATION_STATUS.md for list)
2. **Test Part A end-to-end**
3. **Fix any bugs found**
4. **Move to Part B** (use INCIDENT_PARTB_DOCUMENTATION.md)
5. **Repeat for remaining parts**

---

## Important Notes

### Do NOT:
- ❌ Change database schema
- ❌ Modify stored procedures (unless absolutely necessary)
- ❌ Change workflow status values (must match existing system)
- ❌ Remove existing functionality from old code (until fully migrated)

### DO:
- ✅ Refer to documentation files before starting each part
- ✅ Check MIGRATION_STATUS.md for current status
- ✅ Update MIGRATION_STATUS.md after completing tasks
- ✅ Create small, focused AngularJS components
- ✅ Write reusable services
- ✅ Test thoroughly
- ✅ Document any deviations from old logic

---

## Quick Reference Links

### Documentation Files
- `MIGRATION_STATUS.md` - Overall status
- `INCIDENT_PARTA_DOCUMENTATION.md` through `INCIDENT_PARTH_DOCUMENTATION.md` - Part-specific docs

### Key Directories
- **Old Code**: `WIRS.Mvc\old-webforms\`
- **Views**: `WIRS.Mvc\Views\Incident\`
- **Controllers**: `WIRS.Mvc\Controllers\`
- **Services**: `WIRS.Services\Implementations\`
- **DataAccess**: `WIRS.DataAccess\Implementations\`
- **JavaScript**: `WIRS.Mvc\wwwroot\js\incident\`
- **CSS**: `WIRS.Mvc\wwwroot\css\incident\`

### Key Files
- Main view: `Views/Incident/Create.cshtml`
- Controller: `Controllers/IncidentController.cs`
- Service: `Services/Implementations/IncidentService.cs`
- DataAccess: `DataAccess/Implementations/WorkflowIncidentDataAccess.cs`

---

## Support and Questions

If unclear about:
- **Field mappings**: Check part-specific documentation
- **Business logic**: Check old .aspx.cs file + documentation
- **BC methods**: Search in DataAccess folder
- **Existing APIs**: Check Controllers and Services folders
- **Status codes**: See workflow status flow above
- **Error codes**: Listed in each part's documentation

Remember: **Read the relevant documentation file FIRST** before asking questions or making changes!
