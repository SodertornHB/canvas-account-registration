// This is an organization specific file 

using CanvasAccountRegistration.Logic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sh.Library.Authentication;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly RequestLocalizationOptions localizationOptions;
        private readonly IAccountServiceExtended accountService;

        public HomeController(IOptions<RequestLocalizationOptions> localizationOptions,
            IAccountServiceExtended accountService)
        {
            this.localizationOptions = localizationOptions.Value;
            this.accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var accountTypes = await accountService.GetAccountTypes();
            if (accountTypes.Count() == 0) return Redirect("List");
            if (accountTypes.Count() == 1) return RedirectToAction("List", new { type = accountTypes.First() });
            var stringArray = accountTypes.ToArray();
            return View(stringArray);
        }

        [NoLibraryAuth]
        [HttpPost]
        public IActionResult ToggleCulture(string returnUrl)
        {
            var culture = GetNextCultureFromSupportedCultures();
            SetCookie(culture);

            return Redirect(returnUrl);
        }

        [NoLibraryAuth]
        [HttpGet("error")]
        public IActionResult Error()
        {
            return View();
        }

        [NoLibraryAuth]
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
