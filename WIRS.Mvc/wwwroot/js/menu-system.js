const MenuSystem = {
    data: {
        menuItems: [],
        currentUser: null
    },


    init: function() {
        this.loadUserInfo();
        this.loadMenuItems();
    },

    loadUserInfo: function() {
        $.ajax({
            url: '/User/GetCurrentUser',
            type: 'GET',
            success: (response) => {
                if (response.success) {
                    this.data.currentUser = response.user;
                    $('.user-name').text(response.user.displayName || response.user.userName || 'User');
                    
                    // Update user display in navigation
                    const userNameElements = document.querySelectorAll('.user-display-name');
                    userNameElements.forEach(el => {
                        el.textContent = response.user.displayName || response.user.userName || 'User';
                    });
                }
            },
            error: () => {
                console.log('Failed to load user info');
                $('.user-name').text('User');
            }
        });
    },

    loadMenuItems: function() {
        $.ajax({
            url: '/Menu/GetUserMenu',
            type: 'GET',
            success: (response) => {
                console.log('Menu response:', response);
                if (response.success && response.menuItems && response.menuItems.length > 0) {
                    this.data.menuItems = response.menuItems;
                    this.renderMenu();
                } else {
                    console.log('Failed to load menu or empty menu:', response);
                    this.loadDefaultMenu();
                }
            },
            error: (xhr, status, error) => {
                console.log('Menu loading error:', xhr.responseText, error);
                this.loadDefaultMenu();
            }
        });
    },

    loadDefaultMenu: function() {
        console.log('Loading default menu');
        this.data.menuItems = [
            { menuId: 1, menuName: 'Home', menuUrl: '/Home', hasChildren: false },
            { menuId: 2, menuName: 'Incident', menuUrl: '#', hasChildren: true, 
                children: [
                    { menuId: 21, menuName: 'Create Incident Report', menuUrl: '/Incident/Create', hasChildren: false },
                    { menuId: 23, menuName: 'View Incident Report', menuUrl: '/Incident/View', hasChildren: false }
                ]
            },
            { menuId: 4, menuName: 'Maintenance', menuUrl: '#', hasChildren: true,
                children: [
                    { menuId: 41, menuName: 'Create User', menuUrl: '/User/Create', hasChildren: false },
                    { menuId: 42, menuName: 'Update User', menuUrl: '/User/Update', hasChildren: false }
                ]
            },
            { menuId: 5, menuName: 'Help', menuUrl: '#', hasChildren: true,
                children: [
                    { menuId: 51, menuName: 'User Guide', menuUrl: '/Help/UserGuide', hasChildren: false }
                ]
            },
            { menuId: 8, menuName: 'Logout', menuUrl: '/Login/Logout', hasChildren: false }
        ];
        this.renderMenu();
    },

    renderMenu: function() {
        const menuContainer = $('#mainMenu');
        const mobileContainer = $('#mainMenuMobile');
        
        menuContainer.empty();
        mobileContainer.empty();

        this.data.menuItems.forEach(item => {
            if (item.hasChildren) {
                const dropdownHtml = this.createDropdownMenu(item);
                const mobileDropdownHtml = this.createMobileDropdownMenu(item);
                menuContainer.append(dropdownHtml);
                mobileContainer.append(mobileDropdownHtml);
            } else {
                const linkHtml = this.createSimpleMenu(item);
                const mobileLinkHtml = this.createMobileSimpleMenu(item);
                menuContainer.append(linkHtml);
                mobileContainer.append(mobileLinkHtml);
            }
        });

        // Add click prevention for dropdown toggles
        this.initializeDropdowns();
    },

    initializeDropdowns: function() {
        // Prevent default click behavior for dropdown toggles but allow hover
        $(document).on('click', '[data-menu-dropdown]', function(e) {
            e.preventDefault();
            return false;
        });

        // Add explicit hover handlers for desktop dropdowns
        $(document).on('mouseenter', '.group', function() {
            const dropdown = $(this).find('.dropdown-content');
            if (dropdown.length) {
                dropdown.removeClass('opacity-0 invisible').addClass('opacity-100 visible');
            }
        });

        $(document).on('mouseleave', '.group', function() {
            const dropdown = $(this).find('.dropdown-content');
            if (dropdown.length) {
                dropdown.removeClass('opacity-100 visible').addClass('opacity-0 invisible');
            }
        });

        // Handle mobile dropdowns differently
        $(document).on('click', '.mobile-dropdown-toggle', function(e) {
            e.preventDefault();
            const dropdown = $(this).next('.mobile-dropdown-content');
            const arrow = $(this).find('.dropdown-arrow');
            
            if (dropdown.hasClass('hidden')) {
                dropdown.removeClass('hidden').slideDown(200);
                arrow.addClass('rotate-180');
            } else {
                dropdown.slideUp(200, function() {
                    $(this).addClass('hidden');
                });
                arrow.removeClass('rotate-180');
            }
        });
    },

    createSimpleMenu: function(item) {
        const isActive = window.location.pathname === item.menuUrl ? 'text-blue-400' : 'text-gray-300 hover:text-white';
        return `
            <a class="px-3 py-2 rounded-md text-sm font-medium transition-colors ${isActive}" href="${item.menuUrl}">
                ${item.menuName}
            </a>
        `;
    },

    createMobileSimpleMenu: function(item) {
        const isActive = window.location.pathname === item.menuUrl ? 'text-blue-400' : 'text-gray-300 hover:text-white';
        return `
            <a class="block px-3 py-2 text-sm font-medium transition-colors ${isActive}" href="${item.menuUrl}">
                ${item.menuName}
            </a>
        `;
    },

    createDropdownMenu: function(item) {
        const childrenHtml = item.children ? item.children.map(child => 
            `<a class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors" href="${child.menuUrl}">
                ${child.menuName}
            </a>`
        ).join('') : '';

        return `
            <div class="relative group">
                <a class="flex items-center px-3 py-2 rounded-md text-sm font-medium text-gray-300 hover:text-white hover:bg-slate-700 transition-colors cursor-pointer" href="#" data-menu-dropdown>
                    <span>${item.menuName}</span>
                    <svg class="w-4 h-4 ml-1 transition-transform group-hover:rotate-180" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd"/></svg>
                </a>
                <div class="dropdown-content absolute left-0 top-full mt-1 w-48 bg-white rounded-lg shadow-lg border border-gray-200 py-1 opacity-0 invisible transition-all duration-200 z-50">
                    ${childrenHtml}
                </div>
            </div>
        `;
    },

    createMobileDropdownMenu: function(item) {
        const childrenHtml = item.children ? item.children.map(child => 
            `<a class="block px-6 py-2 text-sm text-gray-300 hover:text-white transition-colors" href="${child.menuUrl}">
                ${child.menuName}
            </a>`
        ).join('') : '';

        return `
            <div class="mb-2">
                <button class="mobile-dropdown-toggle flex items-center justify-between w-full px-3 py-2 text-sm font-medium text-gray-300 hover:text-white transition-colors">
                    <span>${item.menuName}</span>
                    <svg class="w-4 h-4 dropdown-arrow transition-transform" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd"/>
                    </svg>
                </button>
                <div class="mobile-dropdown-content hidden ml-4 border-l border-gray-600 pl-2">
                    ${childrenHtml}
                </div>
            </div>
        `;
    },

    setActiveMenuItem: function(url) {
        $('.nav-link').removeClass('active');
        $(`.nav-link[href="${url}"]`).addClass('active');
    }
};

function logout() {
    if (confirm('Are you sure you want to logout?')) {
        $.post('/Login/Logout', function() {
            window.location.href = '/Login';
        }).fail(function() {
            window.location.href = '/Login';
        });
    }
}

$(document).ready(function() {
    MenuSystem.init();
    MenuSystem.setActiveMenuItem(window.location.pathname);
});