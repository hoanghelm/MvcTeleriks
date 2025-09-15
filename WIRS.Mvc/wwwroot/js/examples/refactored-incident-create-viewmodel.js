// Refactored Incident Create ViewModel using the new framework
class RefactoredIncidentCreateViewModel extends BaseViewModel {
    constructor() {
        super();
        this.isEditMode = false;
        this.injuredPersonIndex = 0;
        this.eyewitnessIndex = 0;
        this.dropdowns = {};
        this.init();
    }

    init() {
        this.setupData();
        this.setupValidation();
        this.setupDropdowns();
        this.setupEventHandlers();
        this.loadInitialData();
    }

    setupData() {
        // Observable data properties
        this.data.incidentDate = null;
        this.data.incidentTime = null;
        this.data.selectedSba = null;
        this.data.selectedSbu = null;
        this.data.selectedDivision = null;
        this.data.selectedDepartment = null;
        this.data.selectedLocation = null;
        this.data.exactLocation = '';
        this.data.incidentDesc = '';
        this.data.superiorEmpNo = '';
        this.data.superiorName = '';
        this.data.superiorDesignation = '';
        this.data.anyEyewitness = null;
        this.data.damageDescription = '';
        this.data.isWorkingOvertime = null;
        this.data.isJobrelated = null;
        this.data.examinedHospitalClinicName = '';
        this.data.injuredCaseType = null;
    }

    setupValidation() {
        this.addValidationRule('incidentDate', { required: true, message: 'Incident date is required' });
        this.addValidationRule('incidentTime', { required: true, message: 'Incident time is required' });
        this.addValidationRule('selectedSba', { required: true, message: 'SBA is required' });
        this.addValidationRule('selectedSbu', { required: true, message: 'SBU is required' });
        this.addValidationRule('selectedDepartment', { required: true, message: 'Department is required' });
        this.addValidationRule('selectedLocation', { required: true, message: 'Location is required' });
        this.addValidationRule('exactLocation', { required: true, message: 'Exact location is required' });
        this.addValidationRule('incidentDesc', { required: true, message: 'Incident description is required' });
        this.addValidationRule('superiorEmpNo', { required: true, message: 'Superior employee number is required' });
        this.addValidationRule('superiorName', { required: true, message: 'Superior name is required' });

        // Add employee number validation
        FrameworkUtils.setupValidationRules(this);
    }

    setupDropdowns() {
        const factory = window.DropdownFactory;

        // Create all standard incident dropdowns using the factory
        const incidentDropdowns = factory.createStandardIncidentDropdowns(
            '#ddlSbaCode',
            '#ddlSbuCode',
            '#ddlDivision',
            '#ddlDepartment',
            '#ddlLocation',
            this
        );

        // Store dropdown references
        this.dropdowns = { ...incidentDropdowns };

        // Create Yes/No dropdowns
        this.dropdowns.anyEyewitness = factory.createYesNoDropdown('#ddlAnyEyewitness', this, {
            bindTo: 'anyEyewitness',
            onChange: (value) => this.onEyewitnessChange(value)
        });

        this.dropdowns.isWorkingOvertime = factory.createYesNoDropdown('#ddlIsWorkingOvertime', this, {
            bindTo: 'isWorkingOvertime'
        });

        this.dropdowns.isJobrelated = factory.createYesNoDropdown('#ddlIsJobrelated', this, {
            bindTo: 'isJobrelated'
        });

        // Injured case type dropdown (assuming API endpoint exists)
        this.dropdowns.injuredCaseType = factory.createApiDropdown('#ddlInjuredCaseType', '/MasterData/GetInjuredCaseTypes', this, {
            bindTo: 'injuredCaseType',
            optionLabel: 'Select Case Type'
        });

        // Setup cascading relationships with proper dependency clearing
        this.setupCascadingLogic();
    }

    setupCascadingLogic() {
        // Override the clearDependents method for each dropdown
        this.dropdowns.sba.clearDependents = () => {
            this.dropdowns.sbu.clear();
            this.dropdowns.division.clear();
            this.dropdowns.department.clear();
        };

        this.dropdowns.sbu.clearDependents = () => {
            this.dropdowns.division.clear();
            this.dropdowns.department.clear();
        };

        this.dropdowns.division.clearDependents = () => {
            this.dropdowns.department.clear();
        };

        // Subscribe to changes for logging or other side effects
        this.subscribe('selectedSba', (value) => {
            console.log('SBA changed to:', value);
        });

        this.subscribe('selectedSbu', (value) => {
            console.log('SBU changed to:', value);
        });
    }

    setupEventHandlers() {
        $(document).ready(() => {
            this.bindEvents();
        });
    }

    bindEvents() {
        // Form submission
        $("#createIncidentForm").on("submit", (e) => this.onSubmit(e));

        // Text field bindings
        $("#txtExactLocation").on("input", (e) => {
            this.data.exactLocation = $(e.target).val();
        });

        $("#txtIncidentDesc").on("input", (e) => {
            this.data.incidentDesc = $(e.target).val();
        });

        $("#txtSuperiorEmpNo").on("input", (e) => {
            this.data.superiorEmpNo = $(e.target).val();
        });

        $("#txtSuperiorName").on("input", (e) => {
            this.data.superiorName = $(e.target).val();
        });

        $("#txtSuperiorDesignation").on("input", (e) => {
            this.data.superiorDesignation = $(e.target).val();
        });

        $("#txtDamageDescription").on("input", (e) => {
            this.data.damageDescription = $(e.target).val();
        });

        $("#txtExaminedHospitalClinicName").on("input", (e) => {
            this.data.examinedHospitalClinicName = $(e.target).val();
        });
    }

    loadInitialData() {
        const now = new Date();
        this.data.incidentDate = now;
        this.data.incidentTime = now;

        // Update Kendo controls
        const datePicker = $("#dpIncidentDate").data("kendoDatePicker");
        const timePicker = $("#tpIncidentTime").data("kendoTimePicker");

        if (datePicker) datePicker.value(now);
        if (timePicker) timePicker.value(now);
    }

    onEyewitnessChange(value) {
        const hasEyewitness = value === "1";
        if (hasEyewitness) {
            $("#eyewitnessSection").show();
        } else {
            $("#eyewitnessSection").hide();
            this.clearEyewitnesses();
        }
    }

    onSubmit(e) {
        e.preventDefault();

        if (!this.validateForm()) {
            showErrorNotification("Please correct the validation errors before submitting.");
            return false;
        }

        this.submitForm();
        return false;
    }

    validateForm() {
        // Use the built-in validation
        const isValid = this.validateAll();

        if (!this.validateInjuredPersons()) {
            return false;
        }

        if (this.data.anyEyewitness === "1") {
            if (!this.validateEyewitnesses()) {
                return false;
            }
        }

        // Display validation errors if any
        if (!isValid) {
            const errors = this.getValidationErrors();
            for (const property in errors) {
                console.error(`Validation error for ${property}:`, errors[property]);
                // You can display these errors in the UI as needed
            }
        }

        return isValid;
    }

    validateInjuredPersons() {
        const injuredRows = $(".injured-person-row");
        let hasValidInjured = false;

        injuredRows.each(function(index) {
            const empNo = $(this).find(".injured-emp-no").val();
            const name = $(this).find(".injured-name").val();

            if (empNo && name) {
                hasValidInjured = true;
            }
        });

        if (!hasValidInjured) {
            showErrorNotification("At least one injured person must be specified.");
            return false;
        }

        return true;
    }

    validateEyewitnesses() {
        const eyewitnessRows = $(".eyewitness-row");
        let hasValidEyewitness = false;

        eyewitnessRows.each(function(index) {
            const empNo = $(this).find(".eyewitness-emp-no").val();
            const name = $(this).find(".eyewitness-name").val();

            if (empNo && name) {
                hasValidEyewitness = true;
            }
        });

        if (!hasValidEyewitness) {
            showErrorNotification("At least one eyewitness must be specified when 'Any Eyewitness' is set to Yes.");
            return false;
        }

        return true;
    }

    async submitForm() {
        showLoadingNotification("Creating incident report...");

        const formData = this.collectFormData();

        try {
            const apiService = FrameworkUtils.getApiService();
            const response = await apiService.post('/Incident/Create', formData);

            if (response.success) {
                showSuccessNotification("Incident report created successfully!");
                if (response.incidentId) {
                    setTimeout(() => {
                        window.location.href = '/Incident/View/' + response.incidentId;
                    }, 1500);
                } else {
                    setTimeout(() => {
                        window.location.href = '/Home/Index';
                    }, 1500);
                }
            } else {
                hideLoadingNotification();
                showErrorNotification(response.message || "Failed to create incident report.");
            }
        } catch (error) {
            hideLoadingNotification();
            console.error('Error creating incident:', error);
            showErrorNotification("An error occurred while creating the incident report.");
        }
    }

    collectFormData() {
        return {
            IncidentDateTime: this.combineDateTime(),
            IncidentTime: this.data.incidentTime,
            IncidentDate: this.data.incidentDate,
            SbaCode: this.data.selectedSba,
            SbuCode: this.data.selectedSbu,
            Division: this.data.selectedDivision,
            Department: this.data.selectedDepartment,
            Location: this.data.selectedLocation,
            ExactLocation: this.data.exactLocation,
            IncidentDesc: this.data.incidentDesc,
            SuperiorName: this.data.superiorName,
            SuperiorEmpNo: this.data.superiorEmpNo,
            SuperiorDesignation: this.data.superiorDesignation,
            AnyEyewitness: this.data.anyEyewitness,
            DamageDescription: this.data.damageDescription,
            IsWorkingOvertime: this.data.isWorkingOvertime,
            IsJobrelated: this.data.isJobrelated,
            ExaminedHospitalClinicName: this.data.examinedHospitalClinicName,
            InjuredCaseType: this.data.injuredCaseType,
            InjuredPersons: this.collectInjuredPersons(),
            Eyewitnesses: this.collectEyewitnesses()
        };
    }

    combineDateTime() {
        if (this.data.incidentDate && this.data.incidentTime) {
            const combined = new Date(this.data.incidentDate);
            const time = this.data.incidentTime;
            combined.setHours(time.getHours(), time.getMinutes(), time.getSeconds());
            return combined.toISOString();
        }
        return null;
    }

    collectInjuredPersons() {
        const injuredPersons = [];

        $(".injured-person-row").each(function(index) {
            const empNo = $(this).find(".injured-emp-no").val();
            const name = $(this).find(".injured-name").val();
            const contact = $(this).find(".injured-contact").val();

            if (empNo || name) {
                injuredPersons.push({
                    EmpNo: empNo || '',
                    Name: name || '',
                    ContactNo: contact || '',
                    NricFinNo: $(this).find(".injured-nric").val() || '',
                    Company: $(this).find(".injured-company").val() || '',
                    Age: $(this).find(".injured-age").val() || '',
                    Race: $(this).find(".injured-race").val() || '',
                    Gender: $(this).find(".injured-gender").val() || '',
                    Nationality: $(this).find(".injured-nationality").val() || '',
                    Designation: $(this).find(".injured-designation").val() || '',
                    EmploymentType: $(this).find(".injured-employment-type").val() || ''
                });
            }
        });

        return injuredPersons;
    }

    collectEyewitnesses() {
        const eyewitnesses = [];

        $(".eyewitness-row").each(function(index) {
            const empNo = $(this).find(".eyewitness-emp-no").val();
            const name = $(this).find(".eyewitness-name").val();
            const contact = $(this).find(".eyewitness-contact").val();

            if (empNo || name) {
                eyewitnesses.push({
                    EmpNo: empNo || '',
                    Name: name || '',
                    ContactNo: contact || '',
                    Designation: $(this).find(".eyewitness-designation").val() || ''
                });
            }
        });

        return eyewitnesses;
    }

    clearEyewitnesses() {
        $(".eyewitness-row").not(":first").remove();
        $(".eyewitness-row:first .eyewitness-emp-no").val("");
        $(".eyewitness-row:first .eyewitness-name").val("");
        $(".eyewitness-row:first .eyewitness-contact").val("");
    }

    // Injured person and eyewitness management methods remain the same
    addInjuredPerson() {
        this.injuredPersonIndex++;
        const newRow = this.createInjuredPersonRow(this.injuredPersonIndex);
        $("#injuredPersonsSection").append(newRow);
        this.updateRemoveButtons(".injured-person-row", ".remove-injured-btn");
    }

    removeInjuredPerson(index) {
        $(`.injured-person-row[data-index="${index}"]`).remove();
        this.updateRemoveButtons(".injured-person-row", ".remove-injured-btn");
    }

    addEyewitness() {
        this.eyewitnessIndex++;
        const newRow = this.createEyewitnessRow(this.eyewitnessIndex);
        $("#eyewitnessesSection").append(newRow);
        this.updateRemoveButtons(".eyewitness-row", ".remove-eyewitness-btn");
    }

    removeEyewitness(index) {
        $(`.eyewitness-row[data-index="${index}"]`).remove();
        this.updateRemoveButtons(".eyewitness-row", ".remove-eyewitness-btn");
    }

    updateRemoveButtons(rowSelector, buttonSelector) {
        const rows = $(rowSelector);
        const removeButtons = $(buttonSelector);

        if (rows.length > 1) {
            removeButtons.show();
        } else {
            removeButtons.hide();
        }
    }

    // Row creation methods and other utility methods remain the same as original...
    // (createInjuredPersonRow, createEyewitnessRow, openSuperiorSearch, etc.)

    saveDraft() {
        console.log('Save draft functionality to be implemented');
        showInfoNotification("Save draft functionality will be implemented in the next phase.");
    }

    cancelForm() {
        if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
            window.location.href = "/Home/Index";
        }
    }

    // Cleanup method
    destroy() {
        // Destroy all dropdowns
        for (const key in this.dropdowns) {
            if (this.dropdowns[key] && typeof this.dropdowns[key].destroy === 'function') {
                this.dropdowns[key].destroy();
            }
        }
        this.dropdowns = {};
    }
}

// Initialize the refactored ViewModel
$(document).ready(function() {
    // Wait for framework to be ready
    $(document).on('framework:ready', function() {
        window.refactoredIncidentCreateVM = new RefactoredIncidentCreateViewModel();
    });
});