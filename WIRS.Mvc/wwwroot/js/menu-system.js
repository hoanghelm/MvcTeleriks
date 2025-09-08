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
            url: '/Home/GetCurrentUser',
            type: 'GET',
            success: (response) => {
                if (response.success) {
                    this.data.currentUser = response.user;
                    $('.user-name').text(response.user.userName || 'User');
                }
            },
            error: () => {
                console.log('Failed to load user info');
            }
        });
    },

    loadMenuItems: function() {
        $.ajax({
            url: '/Home/GetUserMenu',
            type: 'GET',
            success: (response) => {
                if (response.success) {
                    this.data.menuItems = response.menuItems;
                    this.renderMenu();
                }
            },
            error: () => {
                this.loadDefaultMenu();
            }
        });
    },

    loadDefaultMenu: function() {
        this.data.menuItems = [
            { menuId: 1, menuName: 'Dashboard', menuUrl: '/Home', icon: 'k-i-dashboard', hasChildren: false }
        ];
        this.renderMenu();
    },

    renderMenu: function() {
        const menuContainer = $('#mainMenu');
        menuContainer.empty();

        this.data.menuItems.forEach(item => {
            if (item.hasChildren) {
                const dropdownHtml = this.createDropdownMenu(item);
                menuContainer.append(dropdownHtml);
            } else {
                const linkHtml = this.createSimpleMenu(item);
                menuContainer.append(linkHtml);
            }
        });
    },

    createSimpleMenu: function(item) {
        const isActive = window.location.pathname === item.menuUrl ? 'active' : '';
        return `
            <li class="nav-item">
                <a class="nav-link ${isActive}" href="${item.menuUrl}">
                    <i class="k-icon ${item.icon} me-2"></i>
                    ${item.menuName}
                </a>
            </li>
        `;
    },

    createDropdownMenu: function(item) {
        const childrenHtml = item.children ? item.children.map(child => 
            `<li><a class="dropdown-item" href="${child.menuUrl}">
                <i class="k-icon ${child.icon} me-2"></i>
                ${child.menuName}
            </a></li>`
        ).join('') : '';

        return `
            <li class="nav-item dropdown">
                <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown">
                    <i class="k-icon ${item.icon} me-2"></i>
                    ${item.menuName}
                </a>
                <ul class="dropdown-menu">
                    ${childrenHtml}
                </ul>
            </li>
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