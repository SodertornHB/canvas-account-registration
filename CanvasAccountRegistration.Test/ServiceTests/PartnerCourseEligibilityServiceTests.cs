using CanvasAccountRegistration.Logic.Constans;
using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.Services;
using CanvasAccountRegistration.Web.Service;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Test
{
    public class PartnerCourseEligibilityServiceTests
    {
        private const string Al2 = PartnerEligibilityService.SwamidAl2ClaimValue;
        private const string AssuranceType = RequestedAttributeObjectIdentifierConstant.EduPersonAssurance;

        private static IEnumerable<Claim> ClaimsWithAl2() => new[]
        {
            new Claim(AssuranceType, "http://www.swamid.se/policy/assurance/al1"),
            new Claim(AssuranceType, Al2),
            new Claim(AssuranceType, "https://refeds.org/assurance/IAP/medium")
        };

        private static IEnumerable<Claim> ClaimsWithoutAl2() => new[]
        {
            new Claim(AssuranceType, "http://www.swamid.se/policy/assurance/al1"),
            new Claim(AssuranceType, "https://refeds.org/assurance/IAP/medium")
        };

        private static PartnerEligibilityService CreateSut(
            params WhiteListedEmailDomain[] whitelist)
        {
            var mock = new Mock<IWhiteListedEmailDomainService>();
            mock.Setup(s => s.GetAll()).ReturnsAsync(whitelist);
            return new PartnerEligibilityService(mock.Object);
        }

        [Test]
        public async Task ReturnsPartner_WhenDomainMatchesAndAl2Present()
        {
            var sut = CreateSut(new WhiteListedEmailDomain
            {
                Domain = "example.com",
                PartnerOrganization = true
            });

            var partner = await sut.IsEligiblePartnerOrganization("user@example.com", ClaimsWithAl2());

            Assert.That(partner, Is.EqualTo(true));
        }

        [Test]
        public async Task ReturnsNull_WhenAl2Missing()
        {
            var sut = CreateSut(new WhiteListedEmailDomain
            {
                Domain = "example.com",
                PartnerOrganization = true
            });

            var partner = await sut.IsEligiblePartnerOrganization("user@example.com", ClaimsWithoutAl2());

            Assert.That(partner, Is.Null);
        }

        [Test]
        public async Task ReturnsNull_WhenDomainNotInWhitelist()
        {
            var sut = CreateSut(new WhiteListedEmailDomain
            {
                Domain = "other.com",
                PartnerOrganization = false
            });

            var partner = await sut.IsEligiblePartnerOrganization("user@example.com", ClaimsWithAl2());

            Assert.That(partner, Is.EqualTo(false));
        }


        [Test]
        public async Task MatchesDomain_StrippingLeadingAtAndIgnoringCase()
        {
            var sut = CreateSut(new WhiteListedEmailDomain
            {
                Domain = "@Example.COM",
                PartnerOrganization = true
            });

            var partner = await sut.IsEligiblePartnerOrganization("user@example.com", ClaimsWithAl2());

            Assert.That(partner, Is.EqualTo(true));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("not-an-email")]
        [TestCase("trailing@")]
        public async Task ReturnsNull_ForInvalidEmail(string email)
        {
            var sut = CreateSut(new WhiteListedEmailDomain
            {
                Domain = "example.com",
                PartnerOrganization = true
            });

            var partner = await sut.IsEligiblePartnerOrganization(email, ClaimsWithAl2());

            Assert.That(partner, Is.Null);
        }

        [Test]
        public async Task ReturnsNull_WhenClaimsNull()
        {
            var sut = CreateSut(new WhiteListedEmailDomain
            {
                Domain = "example.com",
                PartnerOrganization = true
            });

            var partner = await sut.IsEligiblePartnerOrganization("user@example.com", null);

            Assert.That(partner, Is.Null);
        }
    }
}
