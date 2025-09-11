namespace WIRS.Mvc.ViewModels
{
    public class EmployeeSearchViewModel
    {
        /// <summary>
        /// Unique identifier for the modal window
        /// </summary>
        public string ModalId { get; set; } = "employeeSearchModal";

        /// <summary>
        /// Unique identifier for the grid control
        /// </summary>
        public string GridId { get; set; } = "employeeSearchGrid";

        /// <summary>
        /// JavaScript callback function name when employee is selected
        /// </summary>
        public string OnSelectCallback { get; set; } = "onEmployeeSelected";

        /// <summary>
        /// Title of the modal window
        /// </summary>
        public string Title { get; set; } = "Employee Search";

        /// <summary>
        /// Subtitle/description text
        /// </summary>
        public string Subtitle { get; set; } = "Search by employee ID, name, email, or department";

        /// <summary>
        /// Enable advanced search filters panel
        /// </summary>
        public bool EnableAdvancedFilters { get; set; } = true;

        /// <summary>
        /// Enable export functionality
        /// </summary>
        public bool EnableExport { get; set; } = true;

        /// <summary>
        /// Enable department filter
        /// </summary>
        public bool EnableDepartmentFilter { get; set; } = true;

        /// <summary>
        /// Enable email filter
        /// </summary>
        public bool EnableEmailFilter { get; set; } = true;

        /// <summary>
        /// Enable status column
        /// </summary>
        public bool ShowStatusColumn { get; set; } = true;

        /// <summary>
        /// Initial page size for the grid
        /// </summary>
        public int PageSize { get; set; } = 25;

        /// <summary>
        /// Modal window width
        /// </summary>
        public int Width { get; set; } = 1000;

        /// <summary>
        /// Modal window height
        /// </summary>
        public int Height { get; set; } = 700;

        /// <summary>
        /// Allow modal to be resizable
        /// </summary>
        public bool Resizable { get; set; } = true;

        /// <summary>
        /// Show filters panel collapsed by default
        /// </summary>
        public bool FiltersCollapsed { get; set; } = false;

        /// <summary>
        /// Custom CSS class for styling
        /// </summary>
        public string CssClass { get; set; } = "employee-search-modal";

        /// <summary>
        /// Additional data attributes for customization
        /// </summary>
        public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Constructor with default values
        /// </summary>
        public EmployeeSearchViewModel()
        {
            // Set default HTML attributes
            HtmlAttributes.Add("data-component", "employee-search");
        }

        /// <summary>
        /// Constructor with custom modal and grid IDs
        /// </summary>
        /// <param name="modalId">Unique modal identifier</param>
        /// <param name="gridId">Unique grid identifier</param>
        /// <param name="onSelectCallback">JavaScript callback function</param>
        public EmployeeSearchViewModel(string modalId, string gridId, string onSelectCallback = null)
            : this()
        {
            ModalId = modalId;
            GridId = gridId;
            if (!string.IsNullOrEmpty(onSelectCallback))
                OnSelectCallback = onSelectCallback;
        }

        /// <summary>
        /// Create a minimal configuration for simple use cases
        /// </summary>
        /// <param name="modalId">Modal identifier</param>
        /// <param name="onSelectCallback">Selection callback</param>
        /// <returns>Configured view model</returns>
        public static EmployeeSearchViewModel CreateSimple(string modalId, string onSelectCallback)
        {
            return new EmployeeSearchViewModel(modalId, $"{modalId}Grid", onSelectCallback)
            {
                EnableAdvancedFilters = false,
                EnableExport = false,
                ShowStatusColumn = false,
                Width = 800,
                Height = 600,
                FiltersCollapsed = true
            };
        }

        /// <summary>
        /// Create a full-featured configuration for advanced use cases
        /// </summary>
        /// <param name="modalId">Modal identifier</param>
        /// <param name="onSelectCallback">Selection callback</param>
        /// <returns>Configured view model</returns>
        public static EmployeeSearchViewModel CreateAdvanced(string modalId, string onSelectCallback)
        {
            return new EmployeeSearchViewModel(modalId, $"{modalId}Grid", onSelectCallback)
            {
                EnableAdvancedFilters = true,
                EnableExport = true,
                ShowStatusColumn = true,
                EnableDepartmentFilter = true,
                EnableEmailFilter = true,
                Width = 1200,
                Height = 800,
                PageSize = 50,
                Resizable = true
            };
        }
    }
}