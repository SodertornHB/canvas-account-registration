using AutoMapper;
using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IAccountLogServiceExtended : IAccountLogService
    {
        Task<AccountLog> NewRegister(RequestedAttributeCollection requestedAttributeCollection);
    }

    public partial class AccountLogServiceExtended : AccountLogService, IAccountLogServiceExtended
    {
        private readonly IAccountLogService accountLogService;
        private readonly IMapper mapper;

        public AccountLogServiceExtended(ILogger<AccountLogService> logger,
           IAccountLogDataAccess dataAccess,
           IAccountLogService accountLogService,
           IMapper mapper)
           : base(logger, dataAccess)
        {
            this.accountLogService = accountLogService;
            this.mapper = mapper;
        }

        public async Task<AccountLog> NewRegister(RequestedAttributeCollection requestedAttributeCollection)
        {
            var accountLog = requestedAttributeCollection.ToAccountLog();
            accountLog.CreatedOn = DateTime.UtcNow;
            return await Insert(accountLog);
        }
    }
}
