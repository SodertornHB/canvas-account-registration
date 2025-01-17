using CanvasAccountRegistration.Logic.Constans;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanvasAccountRegistration.Logic.Model
{
    public class RequestedAttributeCollection : List<RequestedAttributeModel>
    {
        public IEnumerable<RequestedAttributeModel> GetByIdentifier(string identifier)
        {
            return this.Where(attr => attr.Identifier == identifier);
        }

        public RequestedAttributeModel GetSingleByIdentifier(string identifier)
        {
            return this.FirstOrDefault(attr => attr.Identifier == identifier);
        }

        public RegistrationLog ToRegistrationLog()
        {
            var registrationLog = new RegistrationLog();
            var mapping = new Dictionary<string, Action<string>>
            {
                { RequestedAttributeNameConstant.DisplayName, value => registrationLog.displayName = value },
                { RequestedAttributeNameConstant.GivenName, value => registrationLog.givenName = value },
                { RequestedAttributeNameConstant.Sn, value => registrationLog.sn = value },
                { RequestedAttributeNameConstant.Mail, value => registrationLog.mail = value },
                { RequestedAttributeNameConstant.EduPersonPrincipalName, value => registrationLog.eduPersonPrincipalName = value },
                { RequestedAttributeNameConstant.EduPersonAssurance, value => registrationLog.eduPersonAssurance = value }
            };

            foreach (var attr in this)
            {
                if (mapping.TryGetValue(attr.Name, out var setter))
                {
                    setter(attr.Value);
                }
            }

            return registrationLog;
        }
    }
}