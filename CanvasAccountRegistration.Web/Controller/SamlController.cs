using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CanvasAccountRegistration.Logic.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Route("SAML")]
    public class SamlController : Controller
    {
        private readonly ILogger<Saml2Controller> logger;
        private readonly Saml2Settings samlSettings;

        public SamlController(ILogger<Saml2Controller> logger,
            IOptions<Saml2Settings> samlSettingsOptions)
        {
            this.logger = logger;
            this.samlSettings = samlSettingsOptions.Value;
        }


        [AllowAnonymous]
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
