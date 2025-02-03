using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using CanvasAccountRegistration.Logic.Settings;
using Microsoft.Extensions.Options;

namespace Web.Controllers
{
    [AllowAnonymous]
    [Route("Saml2")]
    public class LoggedOutController : Controller
    {
        private readonly ILogger<LoggedOutController> logger;
        private readonly Saml2Settings samlSettings;

        public LoggedOutController(ILogger<LoggedOutController> logger,
            IOptions<Saml2Settings> samlSettingsOptions)
        {
            this.logger = logger;
            this.samlSettings = samlSettingsOptions.Value;
        }


        [AllowAnonymous]
        [HttpGet("Logout")]
        public IActionResult Logout([FromQuery] string SAMLResponse)
        {
            logger.LogInformation($"User logged out. Saml response: {SAMLResponse}");
            return View("Logout");
        }
    }
}
