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
    }
}