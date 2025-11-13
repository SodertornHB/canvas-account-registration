// This is an organization specific file 
using AutoMapper;
using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Logic.Settings;
using CanvasAccountRegistration.Web;
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Sh.Library.Authentication;
using Logic.HttpModel;
using Logic.Http;
using Sh.Library.MailSender;

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
            services.Configure<WhiteListedEmailDomainSettings>(Configuration.GetSection("WhiteListedMailDomains"));
            services.Configure<CanvasSettings>(Configuration.GetSection("Canvas"));
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
            services.AddLibraryAuthentication(authenticationHost: Configuration["Authentication:Host"]);
            services.AddLibraryMailSender(mailSenderHost: Configuration["MailSender:Host"], bearerToken: Configuration["MailSender:BearerToken"]);
        }

        protected override void CustomConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseLibraryAuthentication();
            app.UseLibraryApiAuthentication();
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
            profile.CreateMap<Account, CanvasAccountRegistration.Web.ViewModel.RegistrationViewModel>()
                .ForMember(x => x.AssuranceLevels, opt => opt.MapFrom(x => x.GetAssuranceLevels()))
                .ForMember(x => x.IsApproved, opt => opt.MapFrom(x => x.GetIsApproved()))
                .ForMember(x => x.IsVerifiedWithId, opt => opt.MapFrom(x => x.GetIsVerifiedWithId()))
                .ForMember(x => x.IsIntegrated, opt => opt.MapFrom(x => x.GetIsIntegrated()));

            profile.CreateMap<RequestedAttributeModel, RegistrationLog>();
            profile.CreateMap<RegistrationLog, Account>()
                .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.eduPersonPrincipalName))
                .ForMember(x => x.Email, opt => opt.MapFrom(x => x.mail))
                .ForMember(x => x.Surname, opt => opt.MapFrom(x => x.sn))
                .ForMember(x => x.AssuranceLevel, opt => opt.MapFrom(x => x.eduPersonAssurance))
                .ForMember(x => x.DisplayName, opt => opt.MapFrom(x => x.displayName))
                .ForMember(x => x.GivenName, opt => opt.MapFrom(x => x.givenName));

            profile.CreateMap<Account, PostCanvasAccountRequestModel>()
                .ForPath(x => x.user.Name, opt => opt.MapFrom(x => x.GetFullName()))
                .ForPath(x => x.user.Short_name, opt => opt.MapFrom(x => x.GetFullNameWithVerifiedIdPostfix()))
                .ForPath(x => x.user.Sortable_name, opt => opt.MapFrom(x => x.GetAsSortableName()))
                .ForPath(x => x.communication_channel.Address, opt => opt.MapFrom(x => x.Email))
                .ForPath(x => x.pseudonym.Sis_user_id, opt => opt.MapFrom(x => x.Email))
                .ForPath(x => x.pseudonym.Integration_id, opt => opt.MapFrom(x => x.Id))
                .ForPath(x => x.pseudonym.Unique_id, opt => opt.MapFrom(x => x.UserId));

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