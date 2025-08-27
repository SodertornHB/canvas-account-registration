
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IAccountArchiveService : IService<AccountArchive>
    {
    }

    public partial class AccountArchiveService : Service<AccountArchive>, IAccountArchiveService
    {
        public AccountArchiveService(ILogger<AccountArchiveService> logger,
           IAccountArchiveDataAccess dataAccess)
           : base(logger, dataAccess)
        { }
    }
}
