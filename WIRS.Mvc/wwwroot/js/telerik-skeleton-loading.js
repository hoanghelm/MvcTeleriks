var TelerikSkeleton = (function() {
    
    function showDropdownSkeleton(dropdownElement) {
        if (!dropdownElement) return;
        
        var $dropdown = $(dropdownElement);
        var kendoDropdown = $dropdown.data("kendoDropDownList");
        
        if (kendoDropdown) {
            // Set loading state
            kendoDropdown.enable(false);
            kendoDropdown.text("Loading...");
            
            // Add skeleton class
            $dropdown.closest('.k-dropdown').addClass('k-loading');
        }
    }
    
    function hideDropdownSkeleton(dropdownElement) {
        if (!dropdownElement) return;
        
        var $dropdown = $(dropdownElement);
        var kendoDropdown = $dropdown.data("kendoDropDownList");
        
        if (kendoDropdown) {
            // Remove loading state
            kendoDropdown.enable(true);
            
            // Remove skeleton class
            $dropdown.closest('.k-dropdown').removeClass('k-loading');
        }
    }
    
    function showGridSkeleton(gridElement, rowCount) {
        if (!gridElement) return;
        
        rowCount = rowCount || 5;
        var $grid = $(gridElement);
        var kendoGrid = $grid.data("kendoGrid");
        
        if (kendoGrid) {
            // Create skeleton data
            var skeletonData = [];
            for (var i = 0; i < rowCount; i++) {
                skeletonData.push({ 
                    id: i,
                    skeleton: true 
                });
            }
            
            // Set skeleton data source
            kendoGrid.setDataSource(new kendo.data.DataSource({
                data: skeletonData
            }));
            
            // Add skeleton styling
            setTimeout(function() {
                $grid.find("tr").addClass("skeleton-row");
                $grid.find("td").each(function() {
                    var $cell = $(this);
                    if (!$cell.hasClass('k-command-cell')) {
                        $cell.html('<div class="skeleton-cell"></div>');
                    }
                });
            }, 100);
        }
    }
    
    function hideGridSkeleton(gridElement) {
        if (!gridElement) return;
        
        var $grid = $(gridElement);
        // Skeleton will be replaced when real data loads
        $grid.find("tr").removeClass("skeleton-row");
    }
    
    function showTextboxSkeleton(textboxElement) {
        if (!textboxElement) return;
        
        var $textbox = $(textboxElement);
        var kendoTextbox = $textbox.data("kendoTextBox");
        
        if (kendoTextbox) {
            kendoTextbox.enable(false);
            kendoTextbox.value("Loading...");
            $textbox.addClass('k-loading');
        }
    }
    
    function hideTextboxSkeleton(textboxElement) {
        if (!textboxElement) return;
        
        var $textbox = $(textboxElement);
        var kendoTextbox = $textbox.data("kendoTextBox");
        
        if (kendoTextbox) {
            kendoTextbox.enable(true);
            kendoTextbox.value("");
            $textbox.removeClass('k-loading');
        }
    }
    
    return {
        showDropdownSkeleton: showDropdownSkeleton,
        hideDropdownSkeleton: hideDropdownSkeleton,
        showGridSkeleton: showGridSkeleton,
        hideGridSkeleton: hideGridSkeleton,
        showTextboxSkeleton: showTextboxSkeleton,
        hideTextboxSkeleton: hideTextboxSkeleton
    };
    
})();

// Global functions
window.TelerikSkeleton = TelerikSkeleton;