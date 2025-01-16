
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IAccountLogService : IService<AccountLog>
    {
    }

    public partial class AccountLogService : Service<AccountLog>, IAccountLogService
    {
        public AccountLogService(ILogger<AccountLogService> logger,
           IAccountLogDataAccess dataAccess)
           : base(logger, dataAccess)
        { }
    }
}
