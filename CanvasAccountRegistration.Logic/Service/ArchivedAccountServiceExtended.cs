
//--------------------------------------------------------------------------------------------------------------------
// Warning! This is an auto generated file. Changes may be overwritten. 
// Generator version: 0.0.1.0
//-------------------------------------------------------------------------------------------------------------------- 

using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IArchivedAccountServiceExtended : IArchivedAccountService 
    {
        Task<ArchivedAccount> GetByInitialId(int initialId);
    }

    public partial class ArchivedAccountServiceExtended : ArchivedAccountService, IArchivedAccountServiceExtended 
    {
        public ArchivedAccountServiceExtended(ILogger<ArchivedAccountService> logger,
           IArchivedAccountDataAccess dataAccess)
           : base(logger, dataAccess)
        { }

        public async Task<ArchivedAccount> GetByInitialId(int initialId)
        {
            var all = await GetAll();
            return all.FirstOrDefault(a => a.InitialId == initialId);
        }
    }
}
