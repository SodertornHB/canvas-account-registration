using CanvasAccountRegistration.Logic.Constans;
using System.Collections.Generic;
using System.Security.Claims;

namespace CanvasAccountRegistration.Logic.Model
{
    public class RequestedAttributeModel
    {
        public static readonly Dictionary<string, string> NameIdentifierMappings = new Dictionary<string, string>
        {
            { RequestedAttributeObjectIdentifierConstant.DisplayName, RequestedAttributeNameConstant.DisplayName },
            { RequestedAttributeObjectIdentifierConstant.EduPersonPrincipalName, RequestedAttributeNameConstant.EduPersonPrincipalName },
            { RequestedAttributeObjectIdentifierConstant.GivenName, RequestedAttributeNameConstant.GivenName },
            { RequestedAttributeObjectIdentifierConstant.Mail, RequestedAttributeNameConstant.Mail },
            { RequestedAttributeObjectIdentifierConstant.Sn, RequestedAttributeNameConstant.Sn },
            { RequestedAttributeObjectIdentifierConstant.EduPersonAssurance, RequestedAttributeNameConstant.EduPersonAssurance }
        };
        public RequestedAttributeModel(string identifier, string value)
        { 
            Identifier = identifier;
            Value = value;
            Name = NameIdentifierMappings[Identifier];
        }

        public RequestedAttributeModel(Claim claim)
        {
            Name = claim.Type;
            Value = claim.Value;
            //if (claim.Type != "urn:oid:1.3.6.1.4.1.5923.1.1.1.11" || claim.Value.StartsWith("http://www.swamid.se/policy/assurance/")) 
                Identifier = NameIdentifierMappings.TryGetValue(claim.Type, out var id) ? id : null;
        }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }
       
    }
}