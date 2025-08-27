
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IArchivedAccountService : IService<ArchivedAccount>
    {
    }

    public partial class ArchivedAccountService : Service<ArchivedAccount>, IArchivedAccountService
    {
        public ArchivedAccountService(ILogger<ArchivedAccountService> logger,
           IArchivedAccountDataAccess dataAccess)
           : base(logger, dataAccess)
        { }
    }
}
