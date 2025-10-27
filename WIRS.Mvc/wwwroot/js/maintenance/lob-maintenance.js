var lobMaintenanceViewModel = (function () {

    var _currentMode = 'search';
    var _isEditMode = false;

    var _elements = {
        ddlSector: null,
        ddlFormSector: null,
        lobGrid: null,
        txtLOBCode: null,
        txtLOBName: null,
        dtpInactiveDate: null,
        searchSection: null,
        resultsSection: null,
        entryFormSection: null,
        formTitle: null
    };

    var _currentSectorCode = null;

    function init() {
        initializeElements();
        bindEvents();
        showSearchSection();
    }

    function initializeElements() {
        _elements.ddlSector = $("#ddlSector").data("kendoDropDownList");
        _elements.ddlFormSector = $("#ddlFormSector").data("kendoDropDownList");
        _elements.lobGrid = $("#lobGrid").data("kendoGrid");
        _elements.searchSection = $("#searchSection");
        _elements.resultsSection = $("#resultsSection");
        _elements.entryFormSection = $("#entryFormSection");
        _elements.formTitle = $("#formTitle");

        $('#lobForm').on('submit', function (e) {
            e.preventDefault();
            return false;
        });
    }

    function bindEvents() {
        $(document).on('click', '.edit-lob-btn', function (e) {
            e.preventDefault();
            var $this = $(this);
            var sbaCode = $this.data('sba-code');
            var sbuCode = $this.data('sbu-code');
            editLOBRecord(sbaCode, sbuCode);
        });
    }

    function showLoading() {
        kendo.ui.progress($("#lobGrid"), true);
    }

    function hideLoading() {
        kendo.ui.progress($("#lobGrid"), false);
    }

    function showNotification(message, type) {
        var notification = $("#notification").kendoNotification({
            position: {
                pinned: true,
                top: 30,
                right: 30
            },
            autoHideAfter: 3000,
            stacking: "down"
        }).data("kendoNotification");

        if (type === 'success') {
            notification.success(message);
        } else if (type === 'error') {
            notification.error(message);
        } else {
            notification.info(message);
        }
    }

    function clearValidationErrors() {
        $('.validation-error').text('');
    }

    function showValidationError(fieldId, message) {
        $('#' + fieldId).text(message);
    }

    function validateLOBForm() {
        clearValidationErrors();
        var isValid = true;

        var codeTextBox = $("#txtLOBCode").data("kendoTextBox");
        var nameTextBox = $("#txtLOBName").data("kendoTextBox");

        var lobName = nameTextBox ? nameTextBox.value().trim() : '';
        var lobCode = codeTextBox ? codeTextBox.value().trim() : '';

        if (!lobCode) {
            showValidationError('lobCodeError', 'LOB Code is required');
            isValid = false;
        }

        if (!lobName) {
            showValidationError('lobNameError', 'LOB Name is required');
            isValid = false;
        } else if (lobName.length > 100) {
            showValidationError('lobNameError', 'LOB Name cannot exceed 100 characters');
            isValid = false;
        }

        return isValid;
    }

    function onSectorChange() {
        if (!_elements.ddlSector) return;

        var sectorCode = _elements.ddlSector.value();
        _currentSectorCode = sectorCode;

        if (_elements.lobGrid) {
            _elements.lobGrid.dataSource.data([]);
        }

        if (sectorCode) {
            loadLOBList(sectorCode);
        }
    }

    function loadLOBList(sectorCode) {
        showLoading();

        $.ajax({
            url: '/Maintenance/GetLOBList',
            type: 'POST',
            data: { sbaCode: sectorCode },
            success: function (response) {
                hideLoading();

                if (response && Array.isArray(response)) {
                    if (_elements.lobGrid) {
                        var dataSource = new kendo.data.DataSource({
                            data: response,
                            pageSize: 15,
                            schema: {
                                model: {
                                    id: "uid",
                                    fields: {
                                        sbaCode: { type: "string" },
                                        sbaName: { type: "string" },
                                        sbuCode: { type: "string" },
                                        sbuName: { type: "string" },
                                        inactiveDate: { type: "string" },
                                        uid: { type: "string" }
                                    }
                                }
                            }
                        });

                        _elements.lobGrid.setDataSource(dataSource);
                    }
                } else {
                    showNotification('Failed to load LOB list', 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error loading LOB list: ' + error, 'error');
            }
        });
    }

    function showNewForm() {
        if (!_currentSectorCode) {
            showNotification('Please select a sector first', 'error');
            return;
        }

        _isEditMode = false;
        _currentMode = 'form';

        clearLOBForm();

        if (_elements.ddlFormSector) {
            _elements.ddlFormSector.value(_currentSectorCode);
        }

        if (_elements.formTitle) {
            _elements.formTitle.text('Create LOB');
        }

        showFormSection();

        setTimeout(function () {
            generateLOBCodeDirect();
        }, 200);
    }

    function generateLOBCodeDirect() {
        showLoading();

        $.ajax({
            url: '/Maintenance/GenerateLOBCode',
            type: 'POST',
            data: { sbaCode: _currentSectorCode },
            success: function (response) {
                hideLoading();

                if (response && response.success) {
                    var codeTextBox = $("#txtLOBCode").data("kendoTextBox");
                    var nameTextBox = $("#txtLOBName").data("kendoTextBox");

                    if (codeTextBox) {
                        codeTextBox.value(response.code);
                    }
                    if (nameTextBox && nameTextBox.element) {
                        nameTextBox.element.focus();
                    }
                } else {
                    showNotification('Error generating LOB code: ' + (response.message || 'Unknown error'), 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error generating LOB code: ' + error, 'error');
            }
        });
    }

    function editLOBRecord(sbaCode, sbuCode) {
        _isEditMode = true;
        _currentMode = 'form';

        if (_elements.formTitle) {
            _elements.formTitle.text('Edit LOB');
        }

        showFormSection();

        setTimeout(function () {
            var codeTextBox = $("#txtLOBCode").data("kendoTextBox");
            var nameTextBox = $("#txtLOBName").data("kendoTextBox");
            var datePicker = $("#txtInactiveDate").data("kendoDatePicker");

            if (!codeTextBox || !nameTextBox || !datePicker) {
                showNotification('Form controls not initialized', 'error');
                return;
            }

            loadLOBRecord(sbaCode, sbuCode);
        }, 200);
    }

    function loadLOBRecord(sbaCode, sbuCode) {
        showLoading();

        $.ajax({
            url: '/Maintenance/GetLOBByUid',
            type: 'POST',
            data: {
                sbaCode: sbaCode,
                sbuCode: sbuCode
            },
            success: function (response) {
                hideLoading();

                if (response) {
                    populateLOBFormDirect(response);
                } else {
                    showNotification('LOB record not found', 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error loading LOB record: ' + error, 'error');
            }
        });
    }

    function populateLOBFormDirect(data) {
        setTimeout(function () {
            var codeTextBox = $("#txtLOBCode").data("kendoTextBox");
            var nameTextBox = $("#txtLOBName").data("kendoTextBox");
            var datePicker = $("#txtInactiveDate").data("kendoDatePicker");
            var formSectorDdl = $("#ddlFormSector").data("kendoDropDownList");

            var sectorCode = data.SbaCode || data.sbaCode || '';
            var lobCode = data.SbuCode || data.sbuCode || '';
            var lobName = data.SbuName || data.sbuName || '';
            var inactiveDate = data.InactiveDate || data.inactiveDate;

            if (formSectorDdl && sectorCode) {
                formSectorDdl.value(sectorCode);
            }

            if (codeTextBox) {
                codeTextBox.value(lobCode);
            }

            if (nameTextBox) {
                nameTextBox.value(lobName);
            }

            if (datePicker && inactiveDate) {
                try {
                    var dateValue = new Date(inactiveDate);
                    if (!isNaN(dateValue.getTime())) {
                        datePicker.value(dateValue);
                    }
                } catch (e) {
                    console.log('Date parsing error:', e);
                }
            }

            if (nameTextBox && nameTextBox.element) {
                nameTextBox.element.focus();
            }
        }, 100);
    }

    function populateLOBForm(data) {
        var lobCode = data.SbuCode || data.sbuCode || '';
        var lobName = data.SbuName || data.sbuName || '';
        var inactiveDate = data.InactiveDate || data.inactiveDate;

        if (_elements.txtLOBCode) {
            _elements.txtLOBCode.value(lobCode);
        }
        if (_elements.txtLOBName) {
            _elements.txtLOBName.value(lobName);
        }

        if (inactiveDate && _elements.dtpInactiveDate) {
            try {
                var dateValue = new Date(inactiveDate);
                if (!isNaN(dateValue.getTime())) {
                    _elements.dtpInactiveDate.value(dateValue);
                }
            } catch (e) {
            }
        }
    }

    function clearLOBForm() {
        if (_elements.txtLOBCode) {
            _elements.txtLOBCode.value('');
        }
        if (_elements.txtLOBName) {
            _elements.txtLOBName.value('');
        }
        if (_elements.dtpInactiveDate) {
            _elements.dtpInactiveDate.value(null);
        }

        clearValidationErrors();
    }

    function saveLOBRecord() {
        if (!validateLOBForm()) {
            return;
        }

        var formSectorDdl = $("#ddlFormSector").data("kendoDropDownList");
        var sectorCode = formSectorDdl ? formSectorDdl.value() : _currentSectorCode;

        var formData = {
            SbaCode: sectorCode,
            SbuCode: _elements.txtLOBCode ? _elements.txtLOBCode.value() : '',
            SbuName: _elements.txtLOBName ? _elements.txtLOBName.value().trim() : '',
            InactiveDate: ''
        };

        if (_elements.dtpInactiveDate && _elements.dtpInactiveDate.value()) {
            formData.InactiveDate = kendo.toString(_elements.dtpInactiveDate.value(), 'dd/MM/yyyy');
        }

        showLoading();

        $.ajax({
            url: '/Maintenance/SaveLOB',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (response) {
                hideLoading();

                if (response && response.success) {
                    showNotification(response.message || 'LOB record saved successfully', 'success');
                    loadLOBList(sectorCode);
                    showSearchSection();
                } else {
                    showNotification('Save failed: ' + (response.message || 'Unknown error'), 'error');
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                showNotification('Error saving LOB record: ' + error, 'error');
            }
        });
    }

    function cancelLOBEdit() {
        if (_isEditMode || hasFormChanges()) {
            if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
                clearLOBForm();
                showSearchSection();
            }
        } else {
            clearLOBForm();
            showSearchSection();
        }
    }

    function hasFormChanges() {
        return !!(_elements.txtLOBCode && _elements.txtLOBCode.value()) ||
            !!(_elements.txtLOBName && _elements.txtLOBName.value());
    }

    function showSearchSection() {
        _currentMode = 'search';
        _isEditMode = false;
        if (_elements.entryFormSection) {
            _elements.entryFormSection.hide();
        }
        if (_elements.searchSection) {
            _elements.searchSection.show();
        }
        if (_elements.resultsSection) {
            _elements.resultsSection.show();
        }
    }

    function showFormSection() {
        _currentMode = 'form';
        if (_elements.searchSection) {
            _elements.searchSection.hide();
        }
        if (_elements.resultsSection) {
            _elements.resultsSection.hide();
        }
        if (_elements.entryFormSection) {
            _elements.entryFormSection.show();
        }
    }

    return {
        init: init,
        onSectorChange: onSectorChange,
        showNewForm: showNewForm,
        editLOBRecord: editLOBRecord,
        saveLOBRecord: saveLOBRecord,
        cancelLOBEdit: cancelLOBEdit
    };

})();

function onSectorChange() {
    lobMaintenanceViewModel.onSectorChange();
}

function newLOBRecord() {
    lobMaintenanceViewModel.showNewForm();
}

function editLOBRecord(sbaCode, sbuCode) {
    lobMaintenanceViewModel.editLOBRecord(sbaCode, sbuCode);
}

function saveLOBRecord() {
    lobMaintenanceViewModel.saveLOBRecord();
}

function cancelLOBEdit() {
    lobMaintenanceViewModel.cancelLOBEdit();
}