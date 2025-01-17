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

        public AccountLog ToAccountLog()
        {
            var accountLog = new AccountLog();
            var mapping = new Dictionary<string, Action<string>>
            {
                { RequestedAttributeNameConstant.DisplayName, value => accountLog.displayName = value },
                { RequestedAttributeNameConstant.GivenName, value => accountLog.givenName = value },
                { RequestedAttributeNameConstant.Sn, value => accountLog.sn = value },
                { RequestedAttributeNameConstant.Mail, value => accountLog.mail = value },
                { RequestedAttributeNameConstant.EduPersonPrincipalName, value => accountLog.eduPersonPrincipalName = value },
                { RequestedAttributeNameConstant.EduPersonAssurance, value => accountLog.eduPersonAssurance = value }
            };

            foreach (var attr in this)
            {
                if (mapping.TryGetValue(attr.Name, out var setter))
                {
                    setter(attr.Value);
                }
            }

            return accountLog;
        }
    }
}