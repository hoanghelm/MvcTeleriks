# Update Incident Page Refactoring Plan

## Overview
The update incident controller is too large (~1327 lines). We need to split it into modular files.

## Current Structure
```
incident-update-controller.js (1327 lines)
├── Part A logic
├── Part B logic
├── Part C logic
├── Part D logic
└── Common utilities
```

## Proposed Structure
```
incident-update-app.js (existing - defines module)
incident-update-controller.js (main controller - ~200 lines)
├── incident-update-part-a.js (~300 lines)
├── incident-update-part-b.js (~250 lines)
├── incident-update-part-c.js (~400 lines)
├── incident-update-part-d.js (~200 lines)
└── incident-update-service.js (existing)
```

## Implementation Strategy

### Option 1: Angular Services Pattern (Recommended)
Create separate services for each part that the main controller injects:

```javascript
// incident-update-part-a-service.js
angular.module('incidentUpdateApp')
    .factory('PartAService', function() {
        return {
            initialize: function(vm, incident) { },
            addInjuredPerson: function(vm) { },
            // ...
        };
    });

// Main controller
function IncidentUpdateController($window, PartAService, PartBService, ...) {
    var vm = this;

    PartAService.initialize(vm, vm.incident);
    vm.addInjuredPerson = function() {
        PartAService.addInjuredPerson(vm);
    };
}
```

### Option 2: Mixins Pattern
Create separate files that extend the controller prototype:

```javascript
// incident-update-part-a-mixin.js
(function() {
    angular.module('incidentUpdateApp')
        .factory('PartAMixin', function() {
            return {
                initPartA: function() { },
                addInjuredPerson: function() { }
            };
        });
})();

// Main controller
angular.extend(vm, PartAMixin, PartBMixin, ...);
```

## Tasks

### Phase 1: Refactor Controller ✅
- [x] Extract Part A functions to `incident-update-part-a-service.js`
- [ ] Extract Part B functions to `incident-update-part-b-service.js`
- [ ] Extract Part C functions to `incident-update-part-c-service.js`
- [ ] Extract Part D functions to `incident-update-part-d-service.js`
- [ ] Update main controller to use services
- [ ] Update Update.cshtml script references

### Phase 2: Fix Kendo Directives in Parts B, C, D
- [ ] Part B: Replace HTML controls with Kendo directives
  - Replace `<textarea>` with `kendo-text-area`
  - Replace `<select>` with `kendo-drop-down-list`
  - Add proper k-options configuration
- [ ] Part C: Same as Part B
- [ ] Part D: Same as Part B

### Phase 3: Add Workflow Tables
- [x] Part A: Workflow table added
- [ ] Part B: Add workflow table with action code '02'
- [ ] Part C: Add workflow table with action code '03'
- [ ] Part D: Add workflow table with action code '04'

## Benefits
1. **Maintainability**: Easier to find and update part-specific code
2. **Testability**: Can test each part independently
3. **Performance**: Lazy load parts as needed
4. **Collaboration**: Multiple developers can work on different parts
5. **Readability**: Each file is focused on single responsibility

## Files to Modify
- `WIRS.Mvc/Views/Incident/Update.cshtml` - Add new script references
- `WIRS.Mvc/wwwroot/js/incident/incident-update-controller.js` - Slim down main controller
- `WIRS.Mvc/Views/Incident/_PartB.cshtml` - Update to use Kendo directives
- `WIRS.Mvc/Views/Incident/_PartC.cshtml` - Update to use Kendo directives
- `WIRS.Mvc/Views/Incident/_PartD.cshtml` - Update to use Kendo directives

## New Files to Create
- `WIRS.Mvc/wwwroot/js/incident/incident-update-part-a-service.js`
- `WIRS.Mvc/wwwroot/js/incident/incident-update-part-b-service.js`
- `WIRS.Mvc/wwwroot/js/incident/incident-update-part-c-service.js`
- `WIRS.Mvc/wwwroot/js/incident/incident-update-part-d-service.js`
