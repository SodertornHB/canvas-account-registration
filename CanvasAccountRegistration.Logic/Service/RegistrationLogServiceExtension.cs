using AutoMapper;
using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IRegistrationLogServiceExtended : IRegistrationLogService
    {
        Task<RegistrationLog> NewRegister(RequestedAttributeCollection requestedAttributeCollection);
    }

    public partial class RegistrationLogServiceExtended : RegistrationLogService, IRegistrationLogServiceExtended
    {
        private readonly IRegistrationLogService registrationLogService;
        private readonly IMapper mapper;

        public RegistrationLogServiceExtended(ILogger<RegistrationLogService> logger,
           IRegistrationLogDataAccess dataAccess,
           IRegistrationLogService registrationLogService,
           IMapper mapper)
           : base(logger, dataAccess)
        {
            this.registrationLogService = registrationLogService;
            this.mapper = mapper;
        }

        public async Task<RegistrationLog> NewRegister(RequestedAttributeCollection requestedAttributeCollection)
        {
            var registrationLog = requestedAttributeCollection.ToRegistrationLog();
            registrationLog.CreatedOn = DateTime.UtcNow;
            return await Insert(registrationLog);
        }
    }
}
