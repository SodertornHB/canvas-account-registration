﻿using AutoMapper;
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
using Logic.Http;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using CanvasAccountRegistration.Logic.DataAccess;

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
            services.Configure<CanvasSettings>(Configuration.GetSection("CanvasApiSettings"));
            services.Configure<Saml2Settings>(Configuration.GetSection("Authentication:Saml2"));
            services.AddTransient<IRequestedAttributeService, RequestedAttributeService>();
            services.AddTransient<IRegistrationLogServiceExtended, RegistrationLogServiceExtended>();
            services.AddTransient<IAccountServiceExtended, AccountServiceExtended>();
            services.AddTransient<IArchivedAccountServiceExtended, ArchivedAccountServiceExtended>();
            services.AddTransient<IAccountDataAccessExtended, AccountDataAccessExtended>();
            services.AddTransient<IPostCanvasAccountHttpService, PostCanvasAccountHttpService>();
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

            var saml2Options = Configuration.GetSection("Authentication:Saml2").Get<Saml2Settings>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Saml2Defaults.Scheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddSaml2(options =>
            {
                options.SPOptions.EntityId = new EntityId(saml2Options.EntityId);
                var certificate = string.IsNullOrEmpty(saml2Options.Certificate.Password) ? new X509Certificate2(saml2Options.Certificate.Path) : new X509Certificate2(saml2Options.Certificate.Path, saml2Options.Certificate.Password);
                options.SPOptions.ServiceCertificates.Add(certificate);

                options.IdentityProviders.Add(new IdentityProvider(
                 new EntityId(saml2Options.IdentityProvider.EntityId),
                 options.SPOptions)
                {
                    MetadataLocation = saml2Options.IdentityProvider.MetadataLocation,
                    LoadMetadata = true
                });
            });
            services.AddScoped<IPrincipal>(sp => sp.GetRequiredService<IHttpContextAccessor>().HttpContext.User);
            services.AddHttpContextAccessor();
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
            profile.CreateMap<Account, RegistrationViewModel>()
                .ForMember(x => x.IsApproved, opt => opt.MapFrom(x => x.GetIsApproved()))
                .ForMember(x => x.IsVerifiedWithId, opt => opt.MapFrom(x => x.GetIsVerifiedWithId()))
                .ForMember(x => x.IsIntegrated, opt => opt.MapFrom(x => x.GetIsIntegrated()))
                .ForMember(x => x.AssuranceLevels, opt => opt.MapFrom(x => x.GetAssuranceLevels()))
                .ForMember(x => x.SwamidAssuranceLevel, opt => opt.MapFrom(x => x.GetSwamidAssuranceLevel()))
                .ForMember(x => x.SwamidAssuranceLevel, opt => opt.MapFrom(x => x.GetIdentityAssuranceProfile()));
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