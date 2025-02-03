using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using CanvasAccountRegistration.Logic.Settings;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;
using Microsoft.AspNetCore.Http;

namespace Web.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ILogger<LoggedOutController> logger;
        private readonly Saml2Settings samlSettings;

        public AuthController(ILogger<LoggedOutController> logger,
            IOptions<Saml2Settings> samlSettingsOptions)
        {
            this.logger = logger;
            this.samlSettings = samlSettingsOptions.Value;
        }
              

        [HttpPost]
        public IActionResult ClearCookiesAndLogout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie, new CookieOptions
                {
                    Domain = "canvas-account-registration.shbiblioteket.se", 
                    Path = "/",
                    Secure = true,  
                    HttpOnly = true, 
                    SameSite = SameSiteMode.None 
                });
            }

            var props = new AuthenticationProperties
            {
                // new Uri($"https://canvas-account-registration.shbiblioteket.se/{samlSettings.LogoutCallbackUrl.TrimStart('/')}");
                RedirectUri = "/"
            };
            return SignOut(props, CookieAuthenticationDefaults.AuthenticationScheme, Saml2Defaults.Scheme);
        }

        [AllowAnonymous]
        public IActionResult Logout([FromQuery] string SAMLResponse)
        {
            logger.LogInformation($"User logged out. Saml response: {SAMLResponse}");
            return View(); 
        }
    }
}
