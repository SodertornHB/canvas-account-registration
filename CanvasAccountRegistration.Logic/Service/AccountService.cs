
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IAccountService : IService<Account>
    {
    }

    public partial class AccountService : Service<Account>, IAccountService
    {
        public AccountService(ILogger<AccountService> logger,
           IAccountDataAccess dataAccess)
           : base(logger, dataAccess)
        { }
    }
}
