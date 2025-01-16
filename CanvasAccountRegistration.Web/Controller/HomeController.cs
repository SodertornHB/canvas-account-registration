
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Web.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly RequestLocalizationOptions localizationOptions;

        public HomeController(IOptions<RequestLocalizationOptions> localizationOptions)
        {
            this.localizationOptions = localizationOptions.Value;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("register")]
        public IActionResult Register()
        {
            var claims = User.Claims.Select(c => new ClaimViewModel
            {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            return View(claims);
        }

        [HttpPost]
        public IActionResult ToggleCulture(string returnUrl)
        {
            var culture = GetNextCultureFromSupportedCultures();
            SetCookie(culture);

            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet("info")]
        public IActionResult Info()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet("error")]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet("no-auth")]
        public IActionResult NoAuth()
        {
            return View();
        }

        #region private

        private CultureInfo GetNextCultureFromSupportedCultures()
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            int index = default;
            for (var i = 0; i < localizationOptions.SupportedCultures.Count(); i++)
            {
                if (localizationOptions.SupportedCultures[i].GetHashCode() == cultureInfo.GetHashCode())
                {
                    index = i;
                    break;
                }
            }
            var culture = index < localizationOptions.SupportedCultures.Count() - 1 ? localizationOptions.SupportedCultures[index + 1] : localizationOptions.SupportedCultures[0];
            return culture;
        }

        private void SetCookie(CultureInfo culture)
        {
            HttpContext.Response.Cookies.Append(
                            CookieRequestCultureProvider.DefaultCookieName,
                            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                            new CookieOptions { Path = Url.Content("~/") });
        }

        #endregion
    }
}
