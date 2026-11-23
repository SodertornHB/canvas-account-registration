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
            var mapping = new Dictionary<string, Action<RequestedAttributeModel>>
    {
                { RequestedAttributeNameConstant.DisplayName, attr => registrationLog.displayName = attr.Value },
                { RequestedAttributeNameConstant.GivenName, attr => registrationLog.givenName = attr.Value },
                { RequestedAttributeNameConstant.Sn, attr => registrationLog.sn = attr.Value },
                { RequestedAttributeNameConstant.Mail, attr => registrationLog.mail = attr.Value },
                { RequestedAttributeNameConstant.EduPersonPrincipalName, attr => registrationLog.eduPersonPrincipalName = attr.Value },
                { RequestedAttributeNameConstant.EduPersonAssurance, attr =>
                    {
                        if (string.IsNullOrEmpty(registrationLog.eduPersonAssurance))
                        {
                            registrationLog.eduPersonAssurance = attr.Value;
                        }
                        else
                        {
                            registrationLog.eduPersonAssurance += ", " + attr.Value;
                        }
                    }
                }
            };

            foreach (var attr in this)
            {
                if (mapping.TryGetValue(attr.Identifier, out var setter))
                {
                    setter(attr);
                }
            }

            return registrationLog;
        }
    }
}