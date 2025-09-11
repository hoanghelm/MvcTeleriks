class IncidentViewViewModel {
    constructor() {
        this.isEditMode = false;
        this.currentStage = 'A';
        this.incidentId = '';
        this.validation = new ValidationManager();
        this.init();
    }

    init() {
        this.incidentId = $("#IncidentId").val();
        this.setupValidation();
        this.setupEventHandlers();
        this.initializeStages();
    }

    setupValidation() {
        this.validation.addRule('IncidentDate', { required: true, message: 'Incident date is required' });
        this.validation.addRule('IncidentTime', { required: true, message: 'Incident time is required' });
        this.validation.addRule('IncidentDesc', { required: true, message: 'Incident description is required' });
    }

    setupEventHandlers() {
        $(document).ready(() => {
            this.bindEvents();
        });
    }

    bindEvents() {
        // Stage navigation
        $(".stage-tab").on("click", (e) => {
            e.preventDefault();
            const stage = $(e.currentTarget).data("stage");
            this.showStage(stage);
        });

        // Form submission
        $("#viewIncidentForm").on("submit", (e) => {
            e.preventDefault();
            return false;
        });

        // Workflow form submission
        $("#workflowActionForm").on("submit", (e) => {
            e.preventDefault();
            return false;
        });
    }

    initializeStages() {
        // Show the first available stage
        const activeTab = $(".stage-tab.active");
        if (activeTab.length > 0) {
            const stage = activeTab.data("stage");
            this.showStage(stage);
        } else {
            this.showStage('A');
        }
    }

    showStage(stage) {
        // Update active tab
        $(".stage-tab").removeClass("active");
        $(`.stage-tab[data-stage="${stage}"]`).addClass("active");

        // Show/hide content
        $(".stage-content").hide();
        $(`#part${stage}`).show();

        this.currentStage = stage;
        
        // Update page state
        this.updateStagePermissions(stage);
    }

    updateStagePermissions(stage) {
        // This would typically check server-side permissions
        // For now, we'll use the data from the view model
        console.log(`Current stage: ${stage}`);
    }

    toggleEditMode() {
        this.isEditMode = !this.isEditMode;
        
        if (this.isEditMode) {
            this.enableFormEditing();
            $("#btnEdit").html("Cancel Edit");
            showInfoNotification("Edit mode enabled. You can now modify the incident details.");
        } else {
            this.disableFormEditing();
            $("#btnEdit").html("Edit");
            showInfoNotification("Edit mode disabled.");
        }
    }

    enableFormEditing() {
        // Enable form fields based on current stage and permissions
        if (this.currentStage === 'A') {
            this.enablePartAFields();
        } else if (this.currentStage === 'B') {
            this.enablePartBFields();
        } else if (this.currentStage === 'C') {
            this.enablePartCFields();
        }
    }

    disableFormEditing() {
        // Disable all form fields
        $("#viewIncidentForm input, #viewIncidentForm textarea, #viewIncidentForm select").prop("disabled", true);
        
        // Disable Kendo components
        $("#viewIncidentForm").find("[data-role]").each(function() {
            const widget = kendo.widgetInstance($(this));
            if (widget && widget.enable) {
                widget.enable(false);
            }
        });
    }

    enablePartAFields() {
        // Enable fields that can be edited in Part A
        const editableFields = [
            "#dpIncidentDate",
            "#tpIncidentTime", 
            "#txtExactLocation",
            "#txtIncidentDesc",
            "#txtSuperiorEmpNo",
            "#txtSuperiorName",
            "#txtSuperiorDesignation",
            "#ddlAnyEyewitness",
            "#ddlIsWorkingOvertime",
            "#ddlIsJobrelated",
            "#ddlInjuredCaseType",
            "#txtDamageDescription",
            "#txtExaminedHospitalClinicName"
        ];

        editableFields.forEach(selector => {
            const element = $(selector);
            const widget = kendo.widgetInstance(element);
            if (widget && widget.enable) {
                widget.enable(true);
            } else {
                element.prop("disabled", false);
            }
        });
    }

    enablePartBFields() {
        showInfoNotification("Part B editing features will be implemented in the next phase.");
    }

    enablePartCFields() {
        showInfoNotification("Part C editing features will be implemented in the next phase.");
    }

    savePartA() {
        if (!this.validatePartA()) {
            showErrorNotification("Please correct the validation errors before saving.");
            return;
        }

        showLoadingNotification("Saving changes...");

        const formData = this.collectPartAData();

        $.ajax({
            url: '/Incident/UpdateIncident',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: (response) => {
                hideLoadingNotification();
                if (response.success) {
                    showSuccessNotification("Changes saved successfully!");
                    this.disableFormEditing();
                    this.isEditMode = false;
                    $("#btnEdit").html("Edit");
                    
                    // Reload the page to reflect changes
                    setTimeout(() => {
                        window.location.reload();
                    }, 1500);
                } else {
                    showErrorNotification(response.message || "Failed to save changes.");
                }
            },
            error: (xhr) => {
                hideLoadingNotification();
                console.error('Error saving incident:', xhr);
                showErrorNotification("An error occurred while saving changes.");
            }
        });
    }

    validatePartA() {
        // Validate required fields for Part A
        const requiredFields = [
            { selector: "#dpIncidentDate", message: "Incident date is required" },
            { selector: "#tpIncidentTime", message: "Incident time is required" },
            { selector: "#txtExactLocation", message: "Exact location is required" },
            { selector: "#txtIncidentDesc", message: "Incident description is required" },
            { selector: "#txtSuperiorEmpNo", message: "Superior employee number is required" },
            { selector: "#txtSuperiorName", message: "Superior name is required" }
        ];

        let isValid = true;
        
        requiredFields.forEach(field => {
            const element = $(field.selector);
            let value = '';
            
            const widget = kendo.widgetInstance(element);
            if (widget) {
                if (widget.value) {
                    value = widget.value();
                }
            } else {
                value = element.val();
            }

            if (!value || value.toString().trim() === '') {
                showErrorNotification(field.message);
                isValid = false;
                return false; // Break the loop
            }
        });

        return isValid;
    }

    collectPartAData() {
        return {
            Incident: {
                IncidentId: this.incidentId,
                IncidentDateTime: this.combineDateTime(),
                IncidentTime: $("#tpIncidentTime").data("kendoTimePicker")?.value(),
                IncidentDate: $("#dpIncidentDate").data("kendoDatePicker")?.value(),
                ExactLocation: $("#txtExactLocation").val(),
                IncidentDesc: $("#txtIncidentDesc").val(),
                SuperiorName: $("#txtSuperiorName").val(),
                SuperiorEmpNo: $("#txtSuperiorEmpNo").val(),
                SuperiorDesignation: $("#txtSuperiorDesignation").val(),
                AnyEyewitness: $("#ddlAnyEyewitness").data("kendoDropDownList")?.value(),
                DamageDescription: $("#txtDamageDescription").val(),
                IsWorkingOvertime: $("#ddlIsWorkingOvertime").data("kendoDropDownList")?.value(),
                IsJobrelated: $("#ddlIsJobrelated").data("kendoDropDownList")?.value(),
                ExaminedHospitalClinicName: $("#txtExaminedHospitalClinicName").val(),
                InjuredCaseType: $("#ddlInjuredCaseType").data("kendoDropDownList")?.value()
            }
        };
    }

    combineDateTime() {
        const datePicker = $("#dpIncidentDate").data("kendoDatePicker");
        const timePicker = $("#tpIncidentTime").data("kendoTimePicker");
        
        if (datePicker && timePicker) {
            const date = datePicker.value();
            const time = timePicker.value();
            
            if (date && time) {
                const combined = new Date(date);
                combined.setHours(time.getHours(), time.getMinutes(), time.getSeconds());
                return combined.toISOString();
            }
        }
        
        return null;
    }

    onWorkflowActionChange(e) {
        const selectedAction = e.sender.value();
        console.log('Workflow action selected:', selectedAction);
        
        // Enable/disable fields based on selected action
        this.updateWorkflowFormState(selectedAction);
    }

    updateWorkflowFormState(action) {
        const commentsField = $("#txtWorkflowComments");
        const submitButton = $("#btnSubmitWorkflow");

        if (action) {
            commentsField.prop("disabled", false);
            submitButton.prop("disabled", false);
        } else {
            commentsField.prop("disabled", true);
            submitButton.prop("disabled", true);
        }
    }

    submitWorkflowAction() {
        const actionDropdown = $("#ddlWorkflowAction").data("kendoDropDownList");
        const action = actionDropdown?.value();
        const comments = $("#txtWorkflowComments").val();

        if (!action) {
            showErrorNotification("Please select an action.");
            return;
        }

        if (!comments || comments.trim() === '') {
            showErrorNotification("Please enter comments for this action.");
            return;
        }

        showLoadingNotification("Submitting workflow action...");

        const workflowData = {
            IncidentId: this.incidentId,
            Action: action,
            Comments: comments,
            NextStage: this.determineNextStage(action)
        };

        $.ajax({
            url: '/Incident/SubmitWorkflow',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(workflowData),
            success: (response) => {
                hideLoadingNotification();
                if (response.success) {
                    showSuccessNotification("Workflow action submitted successfully!");
                    
                    // Reload the page to reflect workflow changes
                    setTimeout(() => {
                        window.location.reload();
                    }, 1500);
                } else {
                    showErrorNotification(response.message || "Failed to submit workflow action.");
                }
            },
            error: (xhr) => {
                hideLoadingNotification();
                console.error('Error submitting workflow:', xhr);
                showErrorNotification("An error occurred while submitting the workflow action.");
            }
        });
    }

    determineNextStage(action) {
        // Logic to determine next stage based on current stage and action
        switch (action) {
            case 'APPROVE':
                if (this.currentStage === 'A') return 'B';
                if (this.currentStage === 'B') return 'C';
                if (this.currentStage === 'C') return 'COMPLETED';
                break;
            case 'REJECT':
                return 'REJECTED';
            case 'SUBMIT_PARTC':
                return 'C';
            default:
                return this.currentStage;
        }
        return this.currentStage;
    }

    goBack() {
        if (this.isEditMode) {
            if (confirm("You have unsaved changes. Are you sure you want to go back?")) {
                window.history.back();
            }
        } else {
            window.history.back();
        }
    }

    printReport() {
        showInfoNotification("Print functionality will be implemented in the next phase.");
    }

    exportReport() {
        showInfoNotification("Export functionality will be implemented in the next phase.");
    }

    viewAttachments() {
        showInfoNotification("Attachment viewing will be implemented in the next phase.");
    }

    addAttachment() {
        showInfoNotification("File attachment functionality will be implemented in the next phase.");
    }
}

// Global functions for Kendo events and button clicks
function showStage(stage) {
    if (window.incidentViewVM) {
        window.incidentViewVM.showStage(stage);
    }
}

function toggleEditMode() {
    if (window.incidentViewVM) {
        window.incidentViewVM.toggleEditMode();
    }
}

function savePartA() {
    if (window.incidentViewVM) {
        window.incidentViewVM.savePartA();
    }
}

function onWorkflowActionChange(e) {
    if (window.incidentViewVM) {
        window.incidentViewVM.onWorkflowActionChange(e);
    }
}

function submitWorkflowAction() {
    if (window.incidentViewVM) {
        window.incidentViewVM.submitWorkflowAction();
    }
}

function goBack() {
    if (window.incidentViewVM) {
        window.incidentViewVM.goBack();
    }
}

function printReport() {
    if (window.incidentViewVM) {
        window.incidentViewVM.printReport();
    }
}

function exportReport() {
    if (window.incidentViewVM) {
        window.incidentViewVM.exportReport();
    }
}

// Initialize ViewModel
$(document).ready(function() {
    window.incidentViewVM = new IncidentViewViewModel();
});