using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;

namespace CanvasAccountRegistration.Logic.Services
{
    public interface IWhiteListedEmailDomainService : IService<WhiteListedEmailDomain> { }

    public class WhiteListedEmailDomainService : Service<WhiteListedEmailDomain>, IWhiteListedEmailDomainService
    {
        public WhiteListedEmailDomainService(ILogger<WhiteListedEmailDomainService> logger,
            IWhiteListedEmailDomainDataAccess dataAccess)
            : base(logger, dataAccess)
        { }
    }
}
