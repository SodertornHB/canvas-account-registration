using AutoMapper;
using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IAccountServiceExtended : IAccountService
    {
        Task<Account> GetByUserId(string userId);
        Task<Account> NewRegister(RequestedAttributeCollection requestedAttributeCollection);
    }

    public partial class AccountServiceExtended : AccountService, IAccountServiceExtended
    {
        private readonly IRegistrationLogServiceExtended registrationLogService;
        private readonly IMapper mapper;

        public AccountServiceExtended(ILogger<AccountService> logger,
           IAccountDataAccess dataAccess,
           IRegistrationLogServiceExtended registrationLogService,
           IMapper mapper)
           : base(logger, dataAccess)
        {
            this.registrationLogService = registrationLogService;
            this.mapper = mapper;
        }

        public  async Task<Account> GetByUserId(string userId)
        {
            var accounts = await GetAll();
            var account = accounts.SingleOrDefault(x => x.UserId == userId);
            return account;
        }

        public async Task<Account> NewRegister(RequestedAttributeCollection requestedAttributeCollection)
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
            return await Insert(account);
        }

        private async Task MapValuesAndUpdate(RegistrationLog registrationLog, Account account)
        {
            account.Surname = registrationLog.sn;
            account.GivenName = registrationLog.givenName;
            account.DisplayName = registrationLog.displayName;
            account.AssuranceLevel = registrationLog.eduPersonAssurance;
            account.Email = registrationLog.mail;
            account.UserId = registrationLog.eduPersonPrincipalName;
            await Update(account);
        }
    }
}
