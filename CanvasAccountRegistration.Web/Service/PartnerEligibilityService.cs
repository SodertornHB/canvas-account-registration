using CanvasAccountRegistration.Logic.Constans;
using CanvasAccountRegistration.Logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Web.Service
{
    public interface IPartnerEligibilityService
    {
        Task<bool> IsEligiblePartnerOrganization(string email, IEnumerable<Claim> claims);
    }

    public class PartnerEligibilityService : IPartnerEligibilityService
    {
        public const string SwamidAl2ClaimValue = "http://www.swamid.se/policy/assurance/al2";

        private readonly IWhiteListedEmailDomainService whiteListedEmailDomainService;

        public PartnerEligibilityService(IWhiteListedEmailDomainService whiteListedEmailDomainService)
        {
            this.whiteListedEmailDomainService = whiteListedEmailDomainService;
        }

        public async Task<bool> IsEligiblePartnerOrganization(string email, IEnumerable<Claim> claims)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            if (claims == null) return false;

            if (!HasSwamidAl2(claims)) return false;

            var domain = ExtractDomain(email);
            if (string.IsNullOrEmpty(domain)) return false;

            var matches = await whiteListedEmailDomainService.GetAll();
            var match = matches.FirstOrDefault(x => DomainsEqual(x.Domain, domain));
            return match?.PartnerOrganization ?? false;
        }

        private static bool HasSwamidAl2(IEnumerable<Claim> claims)
        {
            return claims.Any(c =>
                c.Type == RequestedAttributeObjectIdentifierConstant.EduPersonAssurance &&
                string.Equals(c.Value, SwamidAl2ClaimValue, StringComparison.OrdinalIgnoreCase));
        }

        private static string ExtractDomain(string email)
        {
            var at = email.LastIndexOf('@');
            if (at < 0 || at == email.Length - 1) return null;
            return email.Substring(at + 1).Trim().ToLowerInvariant();
        }

        private static bool DomainsEqual(string stored, string emailDomain)
        {
            if (string.IsNullOrWhiteSpace(stored)) return false;
            var normalized = stored.Trim().TrimStart('@').ToLowerInvariant();
            return normalized == emailDomain;
        }
    }
}
