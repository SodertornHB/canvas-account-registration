using CanvasAccountRegistration.Logic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CanvasAccountRegistration.Logic.Extensions
{
    public static class ClaimsExtensions
    {
        public static RequestedAttributeCollection ToRequestedAttributeCollection(this IEnumerable<Claim> claims)
        {
            var collection = new RequestedAttributeCollection();
            collection.AddRange(claims.Select(c => new RequestedAttributeModel(c)));
            return collection;
        }
    }
}
