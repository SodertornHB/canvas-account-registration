using System.Collections.Generic;
using System.Security.Claims;

namespace CanvasAccountRegistration.Logic.Model
{
    public class RequestedAttributeModel
    {
        private static readonly Dictionary<string, string> NameIdentifierMappings = new Dictionary<string, string>
        {
            { "urn:oid:1.3.6.1.4.1.5923.1.1.1.6", "eduPersonPrincipalName" },
            { "urn:oid:2.5.4.42", "eduPersonPrincipalName" },
            { "urn:oid:0.9.2342.19200300.100.1.3", "givenName" },
            { "urn:oid:1.3.6.1.4.1.5923.1.1.1.6", "mail" },
            { "urn:oid:2.5.4.4", "sn" }
        };
        public RequestedAttributeModel(Claim claim)
        {
            Name = claim.Type;
            Value = claim.Value;
            Identifier = NameIdentifierMappings.TryGetValue(claim.Type, out var id) ? id : null;
        }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Value { get; set; }
       
    }
}