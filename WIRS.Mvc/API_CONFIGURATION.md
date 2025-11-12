# API Base Path Configuration

This document explains how to configure the API base path for different deployment environments.

## Overview

The application supports automatic API base path configuration for deployments in subdirectories (e.g., `hostname/WIRS`).

## Configuration Methods

### Method 1: Using appsettings.json (Recommended)

Configure the base path in your environment-specific settings files:

**appsettings.Development.json** (for localhost development):
```json
{
  "AppSettings": {
    "ApiBasePath": ""
  }
}
```

**appsettings.Production.json** (for UAT/Production deployment):
```json
{
  "AppSettings": {
    "ApiBasePath": "/WIRS"
  }
}
```

### Method 2: Automatic Detection (Fallback)

If no configuration is provided, the system will automatically detect:

1. **Localhost** (localhost, 127.0.0.1, 192.168.x.x): Base path = `""`
2. **Other domains**: Checks if first path segment is "WIRS" and sets base path to `/WIRS`

## Examples

### Development (localhost)
- Configuration: `ApiBasePath: ""`
- API Call: `/User/GetCurrentUser`
- Actual URL: `http://localhost:5000/User/GetCurrentUser`

### UAT/Production (subdirectory deployment)
- Configuration: `ApiBasePath: "/WIRS"`
- API Call: `/User/GetCurrentUser`
- Actual URL: `https://hostname/WIRS/User/GetCurrentUser`

## How It Works

1. Configuration is read from appsettings.json
2. Injected into `window.WIRS_CONFIG.basePath` in _Layout.cshtml
3. ApiConfig module automatically prefixes all API calls
4. Both jQuery AJAX and AngularJS $http calls are handled automatically

## Affected Components

All API calls in the following files are automatically handled:
- jQuery AJAX calls ($.ajax, $.get, $.post)
- AngularJS $http calls (via HTTP interceptor)
- Navigation URLs (window.location.href)

No code changes needed when deploying to different environments.
