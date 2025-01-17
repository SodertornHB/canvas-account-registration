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
                { "displayName", value => accountLog.displayName = value },
                { "givenName", value => accountLog.givenName = value },
                { "sn", value => accountLog.sn = value },
                { "mail", value => accountLog.mail = value },
                { "eduPersonPrincipalName", value => accountLog.eduPersonPrincipalName = value },
                { "eduPersonAssurance", value => accountLog.eduPersonAssurance = value }
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