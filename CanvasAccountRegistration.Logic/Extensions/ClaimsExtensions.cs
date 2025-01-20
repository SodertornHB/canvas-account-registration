using CanvasAccountRegistration.Logic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CanvasAccountRegistration.Logic.Extensions
{
    public static class RequestedAttributeNames
    {
        public static RequestedAttributeCollection ToRequestedAttributeCollection(this IEnumerable<Claim> claims)
        {
            var collection = new RequestedAttributeCollection();
            var attributesByIdentifier = new Dictionary<string, RequestedAttributeModel>();

            foreach (var claim in claims)
            {
                if (!RequestedAttributeModel.NameIdentifierMappings.ContainsKey(claim.Type)) continue;

                //tyoe is OID for example "urn:oid:2.5.4.42"
                var identifier = RequestedAttributeModel.NameIdentifierMappings[claim.Type];

                if (!attributesByIdentifier.TryGetValue(identifier, out var attribute))
                {
                    attribute = new RequestedAttributeModel(claim.Type, claim.Value, identifier);
                    attributesByIdentifier[identifier] = attribute;
                }
                else
                {
                    attribute.AddValue(claim.Value);
                }
            }

            foreach (var attribute in attributesByIdentifier.Values)
            {
                collection.Add(attribute);
            }

            return collection;
        }

    }
}
