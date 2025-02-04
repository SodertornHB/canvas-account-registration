using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CanvasAccountRegistration.Logic.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.AspNetCore2;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Route("Saml2")]
    public class Saml2Controller : Controller
    {
        private readonly ILogger<Saml2Controller> logger;
        private readonly Saml2Settings samlSettings;

        public Saml2Controller(ILogger<Saml2Controller> logger,
            IOptions<Saml2Settings> samlSettingsOptions)
        {
            this.logger = logger;
            this.samlSettings = samlSettingsOptions.Value;
        }


        [HttpGet("LogoutUser")]
        public IActionResult LogoutUser()
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
                RedirectUri = "/"
            };
            return SignOut(props, CookieAuthenticationDefaults.AuthenticationScheme, Saml2Defaults.Scheme);
        }


        [AllowAnonymous]
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
