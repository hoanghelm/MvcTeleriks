# Telerik Dropdown Framework Migration Guide

## Overview
This guide helps you migrate from the old ViewBag-based and mixed dropdown approaches to the new unified JavaScript framework.

## Before vs After

### Old Approach (ViewBag-based)
```csharp
// Controller
ViewBag.Locations = await _masterDataService.GetLocationsAsync();
```

```html
@{
    var locations = ViewBag.Locations as IEnumerable<dynamic> ?? new List<dynamic>();
}

@(Html.Kendo().DropDownList()
    .Name("LocationCode")
    .DataTextField("Value")
    .DataValueField("Code")
    .OptionLabel("Select Location")
    .BindTo(locations)
    .HtmlAttributes(new { @class = "form-control" })
)
```

### New Approach (Framework-based)
```html
@(Html.Kendo().DropDownList()
    .Name("LocationCode")
    .DataTextField("Value")
    .DataValueField("Code")
    .OptionLabel("Select Location")
    .HtmlAttributes(new { @class = "form-control" })
    .AutoBind(false)
)

<script>
$(document).on('framework:ready', function() {
    const locationDropdown = window.DropdownFactory.createLocationDropdown('#LocationCode');
});
</script>
```

## Migration Steps

### Step 1: Include Framework Scripts
Add to your layout file or specific pages:
```html
@Html.Partial("_FrameworkScripts")
```

### Step 2: Remove ViewBag Dependencies
**Before:**
```csharp
public async Task<IActionResult> Create()
{
    ViewBag.Locations = await _masterDataService.GetLocationsAsync();
    ViewBag.UserRoles = await _masterDataService.GetUserRolesAsync();
    return View();
}
```

**After:**
```csharp
public IActionResult Create()
{
    // No need to load dropdown data in controller
    return View();
}
```

### Step 3: Update View Models
**Before:**
```javascript
var userCreateViewModel = (function () {
    function loadMasterData() {
        $.get('/MasterData/GetUserRoles', function(data) {
            $("#ddlUserRole").data("kendoDropDownList").setDataSource(data);
        });
    }
    // ... rest of code
})();
```

**After:**
```javascript
class UserCreateViewModel extends BaseViewModel {
    constructor() {
        super();
        this.setupDropdowns();
    }

    setupDropdowns() {
        const factory = window.DropdownFactory;
        this.dropdowns = {
            userRole: factory.createUserRoleDropdown('#ddlUserRole', this, {
                bindTo: 'selectedUserRole'
            })
        };
    }
}
```

### Step 4: Update Razor Views
**Before:**
```html
@Html.Partial("~/Views/Shared/Dropdown/LocationDropDown.cshtml")
```

**After:**
```html
@(Html.Kendo().DropDownList()
    .Name("LocationCode")
    .DataTextField("Value")
    .DataValueField("Code")
    .OptionLabel("Select Location")
    .HtmlAttributes(new { @class = "form-control" })
    .AutoBind(false)
)

<script>
$(document).on('framework:ready', function() {
    window.DropdownFactory.createLocationDropdown('#LocationCode');
});
</script>
```

## Common Patterns

### 1. Cascading Dropdowns
**Before:**
```javascript
$("#ddlSbaCode").data("kendoDropDownList").bind("change", function(e) {
    var selectedSba = e.sender.value();
    if (selectedSba) {
        $.get('/MasterData/GetSbusBySba?sbaCode=' + selectedSba, function(data) {
            $("#ddlSbuCode").data("kendoDropDownList").setDataSource(data);
        });
    }
});
```

**After:**
```javascript
const factory = window.DropdownFactory;
const sbaDropdown = factory.createSbaDropdown('#ddlSbaCode', this);
const sbuDropdown = factory.createSbuDropdown('#ddlSbuCode', '#ddlSbaCode', this);
// Cascading is handled automatically
```

### 2. Data Binding
**Before:**
```javascript
function collectFormData() {
    return {
        LocationCode: $("#ddlLocation").data("kendoDropDownList").value(),
        UserRole: $("#ddlUserRole").data("kendoDropDownList").value()
    };
}
```

**After:**
```javascript
// With ViewModel binding, data is automatically available
function collectFormData() {
    return {
        LocationCode: this.data.selectedLocation,
        UserRole: this.data.selectedUserRole
    };
}
```

### 3. Validation
**Before:**
```javascript
function validateForm() {
    var location = $("#ddlLocation").data("kendoDropDownList").value();
    if (!location) {
        showError("Location is required");
        return false;
    }
    return true;
}
```

**After:**
```javascript
// Setup validation rules
this.addValidationRule('selectedLocation', {
    required: true,
    message: 'Location is required'
});

// Validate
function validateForm() {
    return this.validateAll(); // Handles all validation automatically
}
```

## File-by-File Migration

### 1. Incident Create ViewModel
- **Old file:** `incident-create-viewmodel.js`
- **New file:** `refactored-incident-create-viewmodel.js` (see examples)
- **Key changes:**
  - Extends `BaseViewModel`
  - Uses `DropdownFactory` for all dropdowns
  - Observable data properties
  - Built-in validation

### 2. User Management ViewModels
- **Files to update:** `user-create-viewmodel.js`, `user-update-viewmodel.js`
- **Key changes:**
  - Replace manual AJAX calls with factory methods
  - Use `createUserManagementDropdowns` bulk method
  - Remove ViewBag dependencies

### 3. Partial Views
- **Old:** `Views/Shared/Dropdown/*.cshtml`
- **New:** Use `_RefactoredDropdowns.cshtml` as template
- **Key changes:**
  - Remove ViewBag usage
  - Set `AutoBind(false)`
  - Add JavaScript initialization

## Testing Your Migration

### 1. Verify Framework Loading
```javascript
$(document).ready(function() {
    $(document).on('framework:ready', function() {
        console.log('Framework loaded successfully');
        console.log('Available services:', Object.keys(window.ServiceContainer.services));
    });
});
```

### 2. Test Dropdown Functionality
```javascript
// Test dropdown creation
const testDropdown = window.DropdownFactory.createLocationDropdown('#testDropdown');
console.log('Dropdown created:', testDropdown);

// Test data loading
testDropdown.refresh();
```

### 3. Verify API Calls
- Check browser network tab for proper API calls
- Ensure "Code" property is maintained (uppercase)
- Verify caching is working

## Troubleshooting

### Common Issues

1. **Framework not loaded**
   - Ensure `_FrameworkScripts.cshtml` is included
   - Wait for `framework:ready` event

2. **Dropdown not initializing**
   - Check element selector is correct
   - Ensure element exists when script runs

3. **API calls failing**
   - Verify endpoint URLs are correct
   - Check for CORS issues
   - Ensure proper authentication

4. **Cascading not working**
   - Verify parent selector is correct
   - Check `cascadeFromField` parameter
   - Ensure parent dropdown has value

### Debug Tools
```javascript
// Enable debug logging
window.DROPDOWN_DEBUG = true;

// Check service container
console.log(window.ServiceContainer);

// Check dropdown instances
console.log(window.DropdownService.instances);

// Test API service
window.FrameworkUtils.getApiService().get('/MasterData/GetLocations')
    .then(data => console.log('API test successful:', data))
    .catch(error => console.error('API test failed:', error));
```

## Performance Benefits

### Before Migration
- Multiple ViewBag calls per page load
- No caching of dropdown data
- Repeated API calls for same data
- Manual dependency management

### After Migration
- API calls only when needed
- Built-in caching (30-minute TTL)
- Automatic dependency clearing
- Optimized loading patterns

## Best Practices

1. **Always wait for framework:ready**
2. **Use factory methods for standard dropdowns**
3. **Extend BaseViewModel for observable data**
4. **Implement proper cleanup in destroy() methods**
5. **Use validation rules for better UX**
6. **Cache dropdown instances when needed**

## Rollback Plan

If issues occur during migration:

1. **Keep old files as backup**
2. **Use feature flags to toggle between old/new**
3. **Migrate page by page, not all at once**
4. **Test thoroughly in staging environment**

```javascript
// Feature flag example
if (window.USE_NEW_DROPDOWN_FRAMEWORK) {
    // New framework code
    window.DropdownFactory.createLocationDropdown('#location');
} else {
    // Old jQuery code
    $("#location").kendoDropDownList({ /* old config */ });
}
```