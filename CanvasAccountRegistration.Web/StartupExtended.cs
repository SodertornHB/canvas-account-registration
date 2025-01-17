using AutoMapper;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Logic.Settings;
using CanvasAccountRegistration.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Sustainsys.Saml2.AspNetCore2;
using Logic.Service;
using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Web.ViewModel;

namespace Web
{
    public class StartupExtended : Startup
    {
        public StartupExtended(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        { }

        protected override IList<CultureInfo> GetSupportedLanguages()
        {
            return new List<CultureInfo> {
                new CultureInfo("sv-se"),
                new CultureInfo("en-gb"),
            };
        }

        protected override RequestCulture GetDefaultCulture()
        {
            return new RequestCulture("sv-se");
        }

        protected override void CustomServiceConfiguration(IServiceCollection services)
        {
            services.Configure<CanvasApiSettings>(Configuration.GetSection("CanvasApiSettings"));
#if RELEASE
            services.AddTransient<IRequestedAttributeService, RequestedAttributeService>();
#else 
            services.AddTransient<IRequestedAttributeService, FakeRequestedAttributeService>();
#endif
            services.AddTransient<IRegistrationLogServiceExtended, RegistrationLogServiceExtended>();
            services.AddTransient<IAccountServiceExtended, AccountServiceExtended>();
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            if (Environment.IsProduction())
            {
                var saml2Options = Configuration.GetSection("Authentication:Saml2").Get<Saml2Settings>();

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "Saml2";
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddSaml2(options =>
                {
                    options.SPOptions.EntityId = new EntityId(saml2Options.EntityId);
                    options.SPOptions.ReturnUrl = new Uri(saml2Options.ReturnUrl, UriKind.RelativeOrAbsolute);

                    ConfigureIdentityProvider(options, saml2Options.IdentityProvider);
                    LoadCertificate(options, saml2Options.Certificate);
                });
            }
        }

        private static void ConfigureIdentityProvider(Saml2Options options, IdentityProviderSettings idpSettings)
        {
            options.IdentityProviders.Add(new IdentityProvider(
                new EntityId(idpSettings.EntityId),
                options.SPOptions)
            {
                MetadataLocation = idpSettings.MetadataLocation,
                LoadMetadata = true,
                AllowUnsolicitedAuthnResponse = idpSettings.AllowUnsolicitedAuthnResponse,
                RelayStateUsedAsReturnUrl = idpSettings.RelayStateUsedAsReturnUrl
            });
        }

        private static void LoadCertificate(Saml2Options options, CertificateSettings certSettings)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates.Find(
                    X509FindType.FindBySubjectName, certSettings.SubjectName, false);
                if (certificates.Count > 0)
                {
                    options.SPOptions.ServiceCertificates.Add(certificates[0]);
                }
                else
                {
                    throw new InvalidOperationException("Required certificate not found.");
                }
            }
            finally
            {
                store.Close();
            }
        }

        protected override void CustomConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        protected override void ConfigureExceptionHandler(IApplicationBuilder app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var e = context.Features.Get<IExceptionHandlerFeature>();
                    if (e == null) return;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/html";
                    var factory = builder.ApplicationServices.GetService<ILoggerFactory>();
                    var logger = factory.CreateLogger("ExceptionLogger");
                    logger.LogError(e.Error, e.Error.Message);
                    if (e.Error.Message.Contains("No connection could be made because the target machine actively refused it")) context.Response.Redirect("/no-auth");
                });
            });
        }

        public override IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                var profile = new MappingConfiguration();
                profile = AddAdditionalMappingConfig(profile);

                mc.AddProfile(profile);
            });

            return mapperConfig.CreateMapper();
        }

        public static MappingConfiguration AddAdditionalMappingConfig(MappingConfiguration profile)
        {
            profile.CreateMap<RequestedAttributeModel, RegistrationLog>();
            profile.CreateMap<RegistrationLog, Account>()
                .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.eduPersonPrincipalName))
                .ForMember(x => x.Email, opt => opt.MapFrom(x => x.mail))
                .ForMember(x => x.Surname, opt => opt.MapFrom(x => x.sn))
                .ForMember(x => x.AssuranceLevel, opt => opt.MapFrom(x => x.eduPersonAssurance))
                .ForMember(x => x.DisplayName, opt => opt.MapFrom(x => x.displayName))
                .ForMember(x => x.GivenName, opt => opt.MapFrom(x => x.givenName));

            return profile;
        }

    }

    public class CleanUpServiceExtended : CleanUpService
    {
        private const int WEEDING_TIME_IN_DAYS = -14;
        private readonly ILogService logService;

        public CleanUpServiceExtended(IOptions<ApplicationSettings> options,
            ILogService logService) : base(options)
        {
            this.logService = logService;
        }

        public override async Task ProcessCleanUp()
        {
            await base.ProcessCleanUp();
            await DeleteLogs();
        }

        private async Task DeleteLogs()
        {
            var all = await logService.GetAll();
            foreach (var item in all.Where(x => x.CreatedOn < DateTime.Now.AddDays(WEEDING_TIME_IN_DAYS)))
            {
                await logService.Delete(item.Id);
            }
        }
    }
}