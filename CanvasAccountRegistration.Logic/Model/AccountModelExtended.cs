using System;
using System.Linq;

namespace CanvasAccountRegistration.Logic.Model
{
    public partial class Account 
    {
        public string[] GetAssuranceLevels() => AssuranceLevel?.Replace(" ", "").Split(',') ?? Array.Empty<string>();
        public virtual string GetSwamidAssuranceLevel() => string.Join(",", GetAssuranceLevels()
              .Where(x => x.StartsWith("http://www.swamid.se/policy/assurance/"))?
              .Select(x => x.Substring("http://www.swamid.se/policy/assurance/".Length)));
        public virtual string GetIdentityAssuranceProfile() => string.Join(",", GetAssuranceLevels()
                .Where(x => x.StartsWith("https://refeds.org/assurance/IAP/"))?
                .Select(x => x.Substring("https://refeds.org/assurance/IAP/".Length)));
        public bool GetIsVerifiedWithId() => GetSwamidAssuranceLevel().Contains("al2") && (GetIdentityAssuranceProfile().Contains("medium") || GetIdentityAssuranceProfile().Contains("high"));
        public bool GetIsApproved() => VerifiedOn != null;
        public bool GetIsIntegrated() => IntegratedOn != null;
        public string GetAsSortableName() => $"{Surname}, {GivenName}";
        public string GetFullName() => $"{GivenName} {Surname}";
        public string GetFullNameWithSmobPostfix() => $"{DisplayName} (smob)";
    }
} 