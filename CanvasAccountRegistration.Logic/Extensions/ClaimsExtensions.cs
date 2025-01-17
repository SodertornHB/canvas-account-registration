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
            var seenClaimTypes = new HashSet<string>();

            foreach (var claim in claims)
            {
                if (!RequestedAttributeModel.NameIdentifierMappings.ContainsKey(claim.Type)) continue;

                var attribute = new RequestedAttributeModel(claim);
                if (!string.IsNullOrEmpty(attribute.Identifier))
                {
                    collection.Add(attribute);
                }
            }

            return collection;
        }
    }
}
