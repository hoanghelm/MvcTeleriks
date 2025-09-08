using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WIRS.Mvc.ViewModels;
using WIRS.Services.Auth;
using WIRS.Services.Interfaces;
using WIRS.Services.Models;
using WIRS.Shared.Configuration;

namespace WIRS.Mvc.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly AppSettings _appSettings;

        public LoginController(
            IUserService userService,
            IAuthService authService,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _authService = authService;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string status = null, string loginid = null, string digest = null, string page_id = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ErrorMessage = GetStatusMessage(status),
                PageId = page_id
            };

            if (!string.IsNullOrEmpty(loginid) && !string.IsNullOrEmpty(digest))
            {
                await HandleSSOLogin(loginid, digest, page_id, model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var loginResult = await _authService.ValidateUserAsync(model.UserId, model.Password);

                if (loginResult.IsSuccess)
                {
                    await _authService.SignInUserAsync(loginResult.User, HttpContext);

                    if (loginResult.RequiresPasswordChange)
                    {
                        return RedirectToAction("ChangePassword", "Account");
                    }

                    if (!string.IsNullOrEmpty(model.PageId))
                    {
                        return RedirectToAction("ProcessPageRedirect", "Home", new { page_id = model.PageId });
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    model.ErrorMessage = GetLoginErrorMessage(loginResult);
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
                return View("Index", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync(HttpContext);
            return RedirectToAction("Index", "Login");
        }

        private string GetStatusMessage(string status)
        {
            return status switch
            {
                "sessionExpire" => "Session Expire, pls login again.",
                _ when !string.IsNullOrEmpty(status) => "Unknown status, pls login again.",
                _ => null
            };
        }

        private async Task HandleSSOLogin(string loginid, string digest, string pageId, LoginViewModel model)
        {
            try
            {
                var isValid = await _authService.ValidateSSO(loginid, digest);
                if (isValid)
                {
                    var userExists = await _userService.CheckUserExists(loginid);
                    if (userExists)
                    {
                        var user = await _userService.GetUserByEIP(loginid);
                        if (user != null)
                        {
                            await _authService.SignInUserAsync(user, HttpContext);
                            if (!string.IsNullOrEmpty(pageId))
                            {
                                RedirectToAction("ProcessPageRedirect", "Home", new { page_id = pageId });
                            }
                            else
                            {
                                RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
                else
                {
                    model.ErrorMessage = "You have No Access to this system. Pls check with HR.";
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
            }
        }

        private string GetLoginErrorMessage(LoginResult result)
        {
            return result.ErrorType switch
            {
                LoginErrorType.InvalidCredentials => "Invalid login. Please re-enter.",
                LoginErrorType.AccountLocked => "Account has been LOCKED, Please contact Administrator.",
                LoginErrorType.AccountInactive => "Account has been inactive, Please contact Administrator.",
                LoginErrorType.NoAccess => "You have No Access to this system.",
                _ => "Login failed. Please try again."
            };
        }
    }
}