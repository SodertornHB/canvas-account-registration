
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using AutoMapper;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Web.ViewModel;
using Logic.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore2;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using CanvasAccountRegistration.Logic.Settings;

namespace Web.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly RequestLocalizationOptions localizationOptions;
        private readonly IRequestedAttributeService requestedAttributeService;
        private readonly IAccountServiceExtended accountService;
        private readonly IMapper mapper;
        private readonly ApplicationSettings applicationSettings;

        public HomeController(IOptions<RequestLocalizationOptions> localizationOptions,
            IRequestedAttributeService requestedAttributeService,
            IAccountServiceExtended accountService,
            IMapper mapper,
            IOptions<ApplicationSettings> applicationSettingsOption)
        {
            this.localizationOptions = localizationOptions.Value;
            this.requestedAttributeService = requestedAttributeService;
            this.accountService = accountService;
            this.mapper = mapper;
            this.applicationSettings = applicationSettingsOption.Value;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index([FromQuery] string type, string role)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
            {
                var baseUrl = Url.Action(nameof(Index))!;

                var qs = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace(type)) qs["type"] = type!;
                if (!string.IsNullOrWhiteSpace(role)) qs["role"] = role!;

                var redirectUri = qs.Count == 0
                    ? baseUrl
                    : QueryHelpers.AddQueryString(baseUrl, qs);

                return Challenge(
                    new AuthenticationProperties { RedirectUri = redirectUri },
                    Saml2Defaults.Scheme
                );
            }

            type = NormalizeType(type);
            role = NormalizeRole(role);

            var collection = requestedAttributeService.GetRequestedAttributesFromLoggedInUser();
            var account = await accountService.NewRegister(collection, type, role);
            var viewModel = mapper.Map<RegistrationViewModel>(account);

            return View(viewModel);
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

        [AllowAnonymous]
        [HttpGet("no-auth")]
        public IActionResult NoAuth()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpGet("instructions")]
        public IActionResult Instructions()
        {
            return View();
        }

        [HttpGet("how-to-log-into-canvas")]
        public IActionResult CanvasInstruction()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet("notfound")]
        public IActionResult NotFound([FromQuery] string page)
        {
            return View("NotFound", page);
        }

        #region private

        private string NormalizeType(string? type)
        {
            var allowed = new HashSet<string>(applicationSettings.Types, StringComparer.OrdinalIgnoreCase);
            return string.IsNullOrWhiteSpace(type) || !allowed.Contains(type)
                ? applicationSettings.DefaultAccountType
                : type;
        }

        private string NormalizeRole(string role)
        {
            var allowed = new HashSet<string>(applicationSettings.Roles, StringComparer.OrdinalIgnoreCase);
            return string.IsNullOrWhiteSpace(role) || !allowed.Contains(role)
                ? applicationSettings.DefaultAccountRole
                : role;
        }

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
