
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IRegistrationLogService : IService<RegistrationLog>
    {
    }

    public partial class RegistrationLogService : Service<RegistrationLog>, IRegistrationLogService
    {
        public RegistrationLogService(ILogger<RegistrationLogService> logger,
           IRegistrationLogDataAccess dataAccess)
           : base(logger, dataAccess)
        { }
    }
}
