using CanvasAccountRegistration.Logic.Constans;
using System.Collections.Generic;
using System.Linq;

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
        public static readonly Dictionary<string, string> NameToOidMappings = NameIdentifierMappings.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public RequestedAttributeModel(string name, string value)
        {
            Name = name;
            Values = new List<string> { value };
            Identifier = NameIdentifierMappings.TryGetValue(name, out var id) ? id : null;
        }
        public RequestedAttributeModel(string name, string value, string identifier)
        {
            Name = identifier;
            Values = new List<string> { value };
            Identifier = name;
        }

        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Value => Values.Count == 1 ? Values[0] : string.Join(", ", Values);

        public List<string> Values { get; set; }

        public void AddValue(string value)
        {
            if (!Values.Contains(value))
            {
                Values.Add(value);
            }
        }

        public override string ToString() => $"{Name} {Value} ({Identifier})";
    }
}