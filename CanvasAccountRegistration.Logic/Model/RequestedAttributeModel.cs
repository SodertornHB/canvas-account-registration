using System.Collections.Generic;
using System.Security.Claims;

namespace CanvasAccountRegistration.Logic.Model
{
    public class RequestedAttributeModel
    {
        public static readonly Dictionary<string, string> NameIdentifierMappings = new Dictionary<string, string>
        {
            { "urn:oid:2.16.840.1.113730.3.1.241", "displayName" },
            { "urn:oid:1.3.6.1.4.1.5923.1.1.1.6", "eduPersonPrincipalName" },
            { "urn:oid:2.5.4.42", "givenName" },
            { "urn:oid:0.9.2342.19200300.100.1.3", "mail" },
            { "urn:oid:2.5.4.4", "sn" },
            { "urn:oid:1.3.6.1.4.1.5923.1.1.1.11", "eduPersonAssurance" }
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