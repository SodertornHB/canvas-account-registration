using AutoMapper;
using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Extensions;
using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.Settings;
using Logic.Http;
using Logic.HttpModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IAccountServiceExtended : IAccountService
    {
        Task<IEnumerable<string>> GetAccountTypes();
        Task<Account> GetByUserId(string userId);
        Task<Account> NewRegister(RequestedAttributeCollection requestedAttributeCollection, string accountType, string role);
        Task<PostCanvasAccountResponseModel> IntegrateIntoCanvas(Account account);
    }

    public partial class AccountServiceExtended : AccountService, IAccountServiceExtended
    {
        private readonly IArchivedAccountServiceExtended archivedAccountService;
        private new readonly IAccountDataAccessExtended dataAccess;
        private readonly IRegistrationLogServiceExtended registrationLogService;
        private readonly IMapper mapper;
        private readonly IPostCanvasAccountHttpService postCanvasAccountHttpService;
        private readonly CanvasSettings canvasSettings;

        public AccountServiceExtended(ILogger<AccountService> logger,
           IAccountDataAccessExtended dataAccess,
           IArchivedAccountServiceExtended archivedAccountService,
           IRegistrationLogServiceExtended registrationLogService,
           IMapper mapper,
           IPostCanvasAccountHttpService postCanvasAccountHttpService,
           IOptions<CanvasSettings> options)
           : base(logger, dataAccess)
        {
            this.dataAccess = dataAccess;
            this.archivedAccountService = archivedAccountService;
            this.registrationLogService = registrationLogService;
            this.mapper = mapper;
            this.postCanvasAccountHttpService = postCanvasAccountHttpService;
            canvasSettings = options.Value;
            postCanvasAccountHttpService.OverrideDefaultBearerToken(canvasSettings.BearerToken);
        }

        public  async Task<Account> GetByUserId(string userId)
        {
            var accounts = await GetAll();
            var account = accounts.SingleOrDefault(x => x.UserId == userId);
            return account;
        }

        public async Task<PostCanvasAccountResponseModel> IntegrateIntoCanvas(Account account)
        {
            account.IntegratedOn = System.DateTime.UtcNow;
            if (account.VerifiedOn == null) account.VerifiedOn = System.DateTime.UtcNow;
            var postModel = mapper.Map<PostCanvasAccountRequestModel>(account);
            var response = await postCanvasAccountHttpService.Post($"{canvasSettings.ApiHost}/accounts/self/users",postModel);
            await Update(account);
            return response;
        }

        public async Task<Account> NewRegister(RequestedAttributeCollection requestedAttributeCollection, string accountType, string role)
        {
            logger.LogDebug("RequestedAttributeCollection");
            foreach (var attribute in requestedAttributeCollection)
            {
                logger.LogDebug(attribute.ToString());
            }
            var registrationLog = await registrationLogService.NewRegister(requestedAttributeCollection);
            logger.LogDebug("registrationLog: " + registrationLog.ToString());
            Account account = await GetByUserId(registrationLog.eduPersonPrincipalName);
            if (account != null)
            {
                await MapValuesAndUpdate(registrationLog, account);
                return account;
            }
            account = mapper.Map<Account>(registrationLog);
            account.AccountType = accountType;
            account.AccountRole= role;
            return await Insert(account);
        }

        public override async Task Delete(int id)
        {
            var account = await Get(id);
            if (account == null) return;
            var archivedAccount = await archivedAccountService.GetByInitialId(id);
            if (archivedAccount == null)
            {
                await ArchiveAccount(account);
            }
            await base.Delete(id);
        }

        public async Task<IEnumerable<string>> GetAccountTypes() => await dataAccess.GetAccountTypes();

        private async Task ArchiveAccount(Account account)
        {
            var archivedAccount = account.ToArchivedAccount();
            await archivedAccountService.Insert(archivedAccount);
        }

        private async Task MapValuesAndUpdate(RegistrationLog registrationLog, Account account)
        {
            registrationLog.MapAccount(account); 
            await Update(account);
        }
    }
}
