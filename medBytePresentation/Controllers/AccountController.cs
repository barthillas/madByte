using medBytePresentation.Models.AccountViewModels;
using medBytePresentation.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static medBytePresentation.Models.ApiModels;

namespace medBytePresentation.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IMedByteApiService _medByteApiService;
        private readonly ILogger _logger;


        public AccountController(
            IMedByteApiService medByteApiService,
            ILoggerFactory loggerFactory)
        {
            _medByteApiService = medByteApiService;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }
        public partial class ErrorLogin
        {
            [JsonProperty("model")]
            public LoginViewModel Model { get; set; }

            [JsonProperty("error")]
            public string Error { get; set; }
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)            {
                var httpResponse = await _medByteApiService.Login(model);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var content = await httpResponse.Content.ReadAsStringAsync();

                    ErrorLogin errorLogin = JsonConvert.DeserializeObject<ErrorLogin>(content);
                    if (errorLogin.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, "Kullanıcı adı ya da şifre hatalı!");
                        return View(model);
                    }

                    TokenModel tasks = JsonConvert.DeserializeObject<TokenModel>(content);
                    HttpContext.Session.Clear();
                    HttpContext.Session.SetString("Token", tasks.Token.ToString());
                    HttpContext.Session.SetString("RefreshToken", tasks.RefreshToken.ToString());
                    _logger.LogInformation(1, "User logged in.");

                    JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(tasks.Token);
                                                         
                    var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim("FullName", model.Email)
                    };
                    foreach (var item in token.Claims)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.Value));
                    }
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                         CookieAuthenticationDefaults.AuthenticationScheme,
                         new ClaimsPrincipal(claimsIdentity),
                         new AuthenticationProperties
                         {
                             IsPersistent = model.RememberMe,
                             ExpiresUtc = DateTime.UtcNow.AddMinutes(600)
                         });

                    return RedirectToLocal(returnUrl);
                }

                throw new Exception("Cannot retrieve tasks");

            }

            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var httpResponse = await _medByteApiService.Register(model);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var content = await httpResponse.Content.ReadAsStringAsync();
                    ErrorLogin errorLogin = JsonConvert.DeserializeObject<ErrorLogin>(content);
                    if (errorLogin.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, "Kayıt oluşturulurken bir hatayla karşılaşıldı şifrenizi büyük küçük harf sayı ve özel karakter içermek zorundadır!");
                        return View(model);
                    }
                    return RedirectToAction("Login", "Account");                  
                }           
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = "IAsyncDisposable"; //_signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
               
        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _medByteApiService.SendPasswordRecoveryMail(model);
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string t = null)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel() { Code = t };
            return t == null ? View("Error") : View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var httpResponse = await _medByteApiService.ResetPassword(model);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var _result = JsonConvert.DeserializeObject<dynamic>(content);
            if (_result.succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
