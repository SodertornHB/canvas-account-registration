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
        private readonly IAccountLogServiceExtended accountLogService;
        private readonly IMapper mapper;

        public AccountServiceExtended(ILogger<AccountService> logger,
           IAccountDataAccess dataAccess,
           IAccountLogServiceExtended accountLogService,
           IMapper mapper)
           : base(logger, dataAccess)
        {
            this.accountLogService = accountLogService;
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
            var accountLog = await accountLogService.NewRegister(requestedAttributeCollection);
            Account account = await GetByUserId(accountLog.eduPersonPrincipalName);
            if (account != null) return account;
            account = mapper.Map<Account>(accountLog);
            return await Insert(account);
        }
    }
}
