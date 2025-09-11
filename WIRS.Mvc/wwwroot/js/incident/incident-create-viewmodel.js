class IncidentCreateViewModel {
    constructor() {
        this.isEditMode = false;
        this.injuredPersonIndex = 0;
        this.eyewitnessIndex = 0;
        this.validation = new ValidationManager();
        this.init();
    }

    init() {
        this.setupValidation();
        this.setupEventHandlers();
        this.loadInitialData();
    }

    setupValidation() {
        this.validation.addRule('IncidentDate', { required: true, message: 'Incident date is required' });
        this.validation.addRule('IncidentTime', { required: true, message: 'Incident time is required' });
        this.validation.addRule('SbaCode', { required: true, message: 'SBA is required' });
        this.validation.addRule('SbuCode', { required: true, message: 'SBU is required' });
        this.validation.addRule('Department', { required: true, message: 'Department is required' });
        this.validation.addRule('Location', { required: true, message: 'Location is required' });
        this.validation.addRule('ExactLocation', { required: true, message: 'Exact location is required' });
        this.validation.addRule('IncidentDesc', { required: true, message: 'Incident description is required' });
        this.validation.addRule('SuperiorEmpNo', { required: true, message: 'Superior employee number is required' });
        this.validation.addRule('SuperiorName', { required: true, message: 'Superior name is required' });
    }

    setupEventHandlers() {
        $(document).ready(() => {
            this.bindEvents();
        });
    }

    bindEvents() {
        $("#ddlSbaCode").data("kendoDropDownList").bind("change", (e) => this.onSbaChange(e));
        $("#ddlSbuCode").data("kendoDropDownList").bind("change", (e) => this.onSbuChange(e));
        $("#ddlDivision").data("kendoDropDownList").bind("change", (e) => this.onDivisionChange(e));
        $("#ddlAnyEyewitness").data("kendoDropDownList").bind("change", (e) => this.onEyewitnessChange(e));
        
        $("#createIncidentForm").on("submit", (e) => this.onSubmit(e));
    }

    loadInitialData() {
        const now = new Date();
        $("#dpIncidentDate").data("kendoDatePicker").value(now);
        $("#tpIncidentTime").data("kendoTimePicker").value(now);
    }

    onSbaChange(e) {
        const selectedSba = e.sender.value();
        if (selectedSba) {
            this.loadSbusBySba(selectedSba);
        } else {
            this.clearDropdown("#ddlSbuCode");
            this.clearDropdown("#ddlDivision");
            this.clearDropdown("#ddlDepartment");
        }
    }

    onSbuChange(e) {
        const selectedSbu = e.sender.value();
        if (selectedSbu) {
            this.loadDivisionsBySbu(selectedSbu);
            this.loadDepartmentsBySbu(selectedSbu);
        } else {
            this.clearDropdown("#ddlDivision");
            this.clearDropdown("#ddlDepartment");
        }
    }

    onDivisionChange(e) {
        const selectedDivision = e.sender.value();
        if (selectedDivision) {
            this.loadDepartmentsByDivision(selectedDivision);
        }
    }

    onEyewitnessChange(e) {
        const hasEyewitness = e.sender.value() === "1";
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
        const isValid = this.validation.validateAll();
        
        if (!this.validateInjuredPersons()) {
            return false;
        }

        if ($("#ddlAnyEyewitness").data("kendoDropDownList").value() === "1") {
            if (!this.validateEyewitnesses()) {
                return false;
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

    submitForm() {
        showLoadingNotification("Creating incident report...");
        
        const formData = this.collectFormData();
        
        $.ajax({
            url: '/Incident/Create',
            type: 'POST',
            data: formData,
            success: (response) => {
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
            },
            error: (xhr) => {
                hideLoadingNotification();
                console.error('Error creating incident:', xhr);
                showErrorNotification("An error occurred while creating the incident report.");
            }
        });
    }

    collectFormData() {
        const formData = {
            IncidentDateTime: this.combineDateTime(),
            IncidentTime: $("#tpIncidentTime").data("kendoTimePicker").value(),
            IncidentDate: $("#dpIncidentDate").data("kendoDatePicker").value(),
            SbaCode: $("#ddlSbaCode").data("kendoDropDownList").value(),
            SbuCode: $("#ddlSbuCode").data("kendoDropDownList").value(),
            Division: $("#ddlDivision").data("kendoDropDownList").value(),
            Department: $("#ddlDepartment").data("kendoDropDownList").value(),
            Location: $("#ddlLocation").data("kendoDropDownList").value(),
            ExactLocation: $("#txtExactLocation").val(),
            IncidentDesc: $("#txtIncidentDesc").val(),
            SuperiorName: $("#txtSuperiorName").val(),
            SuperiorEmpNo: $("#txtSuperiorEmpNo").val(),
            SuperiorDesignation: $("#txtSuperiorDesignation").val(),
            AnyEyewitness: $("#ddlAnyEyewitness").data("kendoDropDownList").value(),
            DamageDescription: $("#txtDamageDescription").val(),
            IsWorkingOvertime: $("#ddlIsWorkingOvertime").data("kendoDropDownList").value(),
            IsJobrelated: $("#ddlIsJobrelated").data("kendoDropDownList").value(),
            ExaminedHospitalClinicName: $("#txtExaminedHospitalClinicName").val(),
            InjuredCaseType: $("#ddlInjuredCaseType").data("kendoDropDownList").value(),
            InjuredPersons: this.collectInjuredPersons(),
            Eyewitnesses: this.collectEyewitnesses()
        };

        return formData;
    }

    combineDateTime() {
        const date = $("#dpIncidentDate").data("kendoDatePicker").value();
        const time = $("#tpIncidentTime").data("kendoTimePicker").value();
        
        if (date && time) {
            const combined = new Date(date);
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

    loadSbusBySba(sbaCode) {
        // Implementation would call server endpoint to get SBUs by SBA
        console.log('Loading SBUs for SBA:', sbaCode);
    }

    loadDivisionsBySbu(sbuCode) {
        // Implementation would call server endpoint to get divisions by SBU
        console.log('Loading divisions for SBU:', sbuCode);
    }

    loadDepartmentsBySbu(sbuCode) {
        // Implementation would call server endpoint to get departments by SBU
        console.log('Loading departments for SBU:', sbuCode);
    }

    loadDepartmentsByDivision(divisionCode) {
        // Implementation would call server endpoint to get departments by division
        console.log('Loading departments for division:', divisionCode);
    }

    clearDropdown(selector) {
        const dropdown = $(selector).data("kendoDropDownList");
        if (dropdown) {
            dropdown.setDataSource([]);
            dropdown.value("");
        }
    }

    clearEyewitnesses() {
        $(".eyewitness-row").not(":first").remove();
        $(".eyewitness-row:first .eyewitness-emp-no").val("");
        $(".eyewitness-row:first .eyewitness-name").val("");
        $(".eyewitness-row:first .eyewitness-contact").val("");
    }

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

    createInjuredPersonRow(index) {
        return `
            <div class="injured-person-row" data-index="${index}">
                <div class="grid grid-cols-1 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-6">
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-gray-700">Employee Number</label>
                        <div class="flex gap-2">
                            <div class="flex-1">
                                <input type="text" name="InjuredPersons[${index}].EmpNo" class="injured-emp-no k-textbox" placeholder="Employee number" />
                            </div>
                            <button type="button" class="btn-primary search-injured-btn" data-index="${index}" onclick="incidentCreateVM.openInjuredPersonSearch(${index})">
                                <svg class='w-4 h-4' fill='none' stroke='currentColor' viewBox='0 0 24 24'><path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z'></path></svg>
                            </button>
                        </div>
                    </div>
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-gray-700">Injured Name</label>
                        <input type="text" name="InjuredPersons[${index}].Name" class="injured-name k-textbox" placeholder="Injured person's name" readonly />
                    </div>
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-gray-700">Contact Number</label>
                        <input type="text" name="InjuredPersons[${index}].ContactNo" class="injured-contact k-textbox" placeholder="Contact number" />
                    </div>
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-gray-700">Actions</label>
                        <div class="flex gap-2">
                            <button type="button" class="btn-danger remove-injured-btn" data-index="${index}" onclick="incidentCreateVM.removeInjuredPerson(${index})">Remove</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    createEyewitnessRow(index) {
        return `
            <div class="eyewitness-row" data-index="${index}">
                <div class="grid grid-cols-1 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-6">
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-gray-700">Employee Number</label>
                        <div class="flex gap-2">
                            <div class="flex-1">
                                <input type="text" name="Eyewitnesses[${index}].EmpNo" class="eyewitness-emp-no k-textbox" placeholder="Employee number" />
                            </div>
                            <button type="button" class="btn-primary search-eyewitness-btn" data-index="${index}" onclick="incidentCreateVM.openEyewitnessSearch(${index})">
                                <svg class='w-4 h-4' fill='none' stroke='currentColor' viewBox='0 0 24 24'><path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z'></path></svg>
                            </button>
                        </div>
                    </div>
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-gray-700">Eyewitness Name</label>
                        <input type="text" name="Eyewitnesses[${index}].Name" class="eyewitness-name k-textbox" placeholder="Eyewitness name" readonly />
                    </div>
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-gray-700">Contact Number</label>
                        <input type="text" name="Eyewitnesses[${index}].ContactNo" class="eyewitness-contact k-textbox" placeholder="Contact number" />
                    </div>
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-gray-700">Actions</label>
                        <div class="flex gap-2">
                            <button type="button" class="btn-danger remove-eyewitness-btn" data-index="${index}" onclick="incidentCreateVM.removeEyewitness(${index})">Remove</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    openSuperiorSearch() {
        if (typeof employeeSearchVM !== 'undefined') {
            employeeSearchVM.openModal('SuperiorSearch', (employee) => {
                $("#txtSuperiorEmpNo").val(employee.empNo);
                $("#txtSuperiorName").val(employee.name);
                $("#txtSuperiorDesignation").val(employee.designation);
            });
        }
    }

    openInjuredPersonSearch(index) {
        if (typeof employeeSearchVM !== 'undefined') {
            employeeSearchVM.openModal('InjuredSearch', (employee) => {
                $(`.injured-person-row[data-index="${index}"] .injured-emp-no`).val(employee.empNo);
                $(`.injured-person-row[data-index="${index}"] .injured-name`).val(employee.name);
                $(`.injured-person-row[data-index="${index}"] .injured-contact`).val(employee.contactNo);
            });
        }
    }

    openEyewitnessSearch(index) {
        if (typeof employeeSearchVM !== 'undefined') {
            employeeSearchVM.openModal('EyewitnessSearch', (employee) => {
                $(`.eyewitness-row[data-index="${index}"] .eyewitness-emp-no`).val(employee.empNo);
                $(`.eyewitness-row[data-index="${index}"] .eyewitness-name`).val(employee.name);
                $(`.eyewitness-row[data-index="${index}"] .eyewitness-contact`).val(employee.contactNo);
            });
        }
    }

    saveDraft() {
        console.log('Save draft functionality to be implemented');
        showInfoNotification("Save draft functionality will be implemented in the next phase.");
    }

    cancelForm() {
        if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
            window.location.href = "/Home/Index";
        }
    }
}

// Global functions for Kendo events
function onSbaChange(e) {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.onSbaChange(e);
    }
}

function onSbuChange(e) {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.onSbuChange(e);
    }
}

function onDivisionChange(e) {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.onDivisionChange(e);
    }
}

function onEyewitnessChange(e) {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.onEyewitnessChange(e);
    }
}

function openSuperiorSearch() {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.openSuperiorSearch();
    }
}

function openInjuredPersonSearch() {
    if (window.incidentCreateVM) {
        const index = $(event.target).data('index') || 0;
        window.incidentCreateVM.openInjuredPersonSearch(index);
    }
}

function openEyewitnessSearch() {
    if (window.incidentCreateVM) {
        const index = $(event.target).data('index') || 0;
        window.incidentCreateVM.openEyewitnessSearch(index);
    }
}

function addInjuredPerson() {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.addInjuredPerson();
    }
}

function removeInjuredPerson() {
    if (window.incidentCreateVM) {
        const index = $(event.target).data('index');
        window.incidentCreateVM.removeInjuredPerson(index);
    }
}

function addEyewitness() {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.addEyewitness();
    }
}

function removeEyewitness() {
    if (window.incidentCreateVM) {
        const index = $(event.target).data('index');
        window.incidentCreateVM.removeEyewitness(index);
    }
}

function saveDraft() {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.saveDraft();
    }
}

function cancelForm() {
    if (window.incidentCreateVM) {
        window.incidentCreateVM.cancelForm();
    }
}

// Initialize ViewModel
$(document).ready(function() {
    window.incidentCreateVM = new IncidentCreateViewModel();
});