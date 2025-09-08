using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIRS.Services.Auth;
using WIRS.Services.Interfaces;

namespace WIRS.Services.Implementations
{
    public class UrlGeneratorService : IUrlGeneratorService
    {
        private readonly IAuthService _authService;
        private readonly IEncryptionService _encryptionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlGeneratorService(
            IAuthService authService,
            IEncryptionService encryptionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _encryptionService = encryptionService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateIncidentUrl(string incidentId, string userId, string action)
        {
            var urlHelper = GetUrlHelper();

            switch (action.ToLower())
            {
                case "edit":
                    if (await _authService.CanEditIncidentAsync(incidentId))
                    {
                        return urlHelper.Action("Edit", "Incident", new { id = incidentId }) ?? string.Empty;
                    }
                    break;

                case "view":
                default:
                    var encryptedIncidentId = _encryptionService.Encrypt(incidentId);
                    return urlHelper.Action("View", "Incident", new { id = encryptedIncidentId }) ?? string.Empty;
            }

            return urlHelper.Action("Index", "Home") ?? string.Empty;
        }

        public async Task<string> GeneratePrintUrl(string incidentId, string userId)
        {
            if (!await _authService.HasPermissionAsync("PRINT_INCIDENTS"))
                return string.Empty;

            var urlHelper = GetUrlHelper();
            var encryptedIncidentId = _encryptionService.Encrypt(incidentId);
            var encryptedUserId = _encryptionService.Encrypt(userId);

            return urlHelper.Action("Print", "Incident", new
            {
                incident_id = encryptedIncidentId,
                ijpname = encryptedUserId
            }) ?? string.Empty;
        }

        public async Task<string> GenerateRedirectUrl(string pageId)
        {
            try
            {
                var decryptedData = _encryptionService.Decrypt(pageId);
                var parameters = ParseDecryptedPageData(decryptedData);

                var urlHelper = GetUrlHelper();

                if (await _authService.CanEditIncidentAsync(parameters.IncidentId))
                {
                    return urlHelper.Action("Edit", "Incident", new { id = parameters.IncidentId }) ?? string.Empty;
                }

                var encryptedIncidentId = _encryptionService.Encrypt(parameters.IncidentId);
                return urlHelper.Action("View", "Incident", new { id = encryptedIncidentId }) ?? string.Empty;
            }
            catch
            {
                var urlHelper = GetUrlHelper();
                return urlHelper.Action("Index", "Home") ?? string.Empty;
            }
        }

        private IUrlHelper GetUrlHelper()
        {
            var actionContext = new ActionContext(
                _httpContextAccessor.HttpContext!,
                _httpContextAccessor.HttpContext!.GetRouteData(),
                new ActionDescriptor());

            return new UrlHelper(actionContext);
        }

        private static PageRedirectParameters ParseDecryptedPageData(string decryptedData)
        {
            var parts = decryptedData.Split('|');
            return new PageRedirectParameters
            {
                IncidentId = parts[0].Split('=')[1],
                UserId = parts[1].Split('=')[1],
                StatusCode = parts[2].Split('=')[1],
                ModifyDate = parts[3].Split('=')[1]
            };
        }
    }
}