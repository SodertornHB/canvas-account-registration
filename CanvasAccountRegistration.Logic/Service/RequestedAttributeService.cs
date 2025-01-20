using CanvasAccountRegistration.Logic.Extensions;
using CanvasAccountRegistration.Logic.Model;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.Logging;

namespace Logic.Service
{
    public interface IRequestedAttributeService
    {
        RequestedAttributeCollection GetRequestedAttributesFromLoggedInUser();
    }
    public class RequestedAttributeService : IRequestedAttributeService
    {
        private readonly IPrincipal principal;
        private readonly ILogger<RequestedAttributeService> logger;

        public RequestedAttributeService(IPrincipal principal, 
            ILogger<RequestedAttributeService> logger)
        {
            this.principal = principal;
            this.logger = logger;
        }

        public RequestedAttributeCollection GetRequestedAttributesFromLoggedInUser()
        {
            if (principal is ClaimsPrincipal claimsPrincipal)
            {
                if (!claimsPrincipal.Identity.IsAuthenticated)
                {
                    throw new UnauthorizedAccessException();
                }
                logger.LogDebug("Claims");
                foreach (var attribute in claimsPrincipal.Claims)
                {
                    logger.LogDebug(attribute.ToString());
                }
                return claimsPrincipal.Claims.ToRequestedAttributeCollection();
            }
            return [];
        }
    }

    public class FakeRequestedAttributeService : IRequestedAttributeService
    {
        public RequestedAttributeCollection GetRequestedAttributesFromLoggedInUser()
        {
            return RequestedAttributeGenerator.GenerateRandomAttributes();
        }
    }
    public static class RequestedAttributeGenerator
    {
        private static readonly Random random = new Random();

        private static readonly List<string> FirstNames = new List<string>
        {
            "Alice", "Bob", "Charlie", "David", "Emma", "Fiona", "Gabriel", "Hannah", "Ivan", "Julia"
        };

        private static readonly List<string> LastNames = new List<string>
        {
            "Anderson", "Brown", "Clark", "Davis", "Evans", "Foster", "Garcia", "Harris", "Ibrahim", "Jones"
        };

        private static readonly List<string> Domains = new List<string>
        {
            "example.com", "test.org", "demo.net", "mail.dev", "random.io"
        };

        private static readonly List<string> AssuranceLevel = new List<string>
        {
            "http://www.swamid.se/policy/assurance/al1","http://www.swamid.se/policy/assurance/al2"
        };

        private static readonly List<string> AssuranceIap = new List<string>
        {
            "https://refeds.org/assurance/IAP/low","https://refeds.org/assurance/IAP/high"
        };

        public static RequestedAttributeCollection GenerateRandomAttributes()
        {
            var email = GetRandomEmail();
            return
            [
                new RequestedAttributeModel("urn:oid:2.16.840.1.113730.3.1.241", GetRandomFullName()),
                new RequestedAttributeModel("urn:oid:1.3.6.1.4.1.5923.1.1.1.6", email),
                new RequestedAttributeModel("urn:oid:2.5.4.42", GetRandomFirstName()),
                new RequestedAttributeModel("urn:oid:0.9.2342.19200300.100.1.3", email),
                new RequestedAttributeModel("urn:oid:2.5.4.4", GetRandomLastName()),
                new RequestedAttributeModel("urn:oid:1.3.6.1.4.1.5923.1.1.1.11", GetAssuranceLevel()),
                new RequestedAttributeModel("urn:oid:1.3.6.1.4.1.5923.1.1.1.11", "https://refeds.org/assurance"),
                new RequestedAttributeModel("urn:oid:1.3.6.1.4.1.5923.1.1.1.11", "https://refeds.org/assurance/ID/unique"),
                new RequestedAttributeModel("urn:oid:1.3.6.1.4.1.5923.1.1.1.11", "https://refeds.org/assurance/ID/eppn-unique-no-reassign"),
                new RequestedAttributeModel("urn:oid:1.3.6.1.4.1.5923.1.1.1.11", GetAssuranceIap())
            ];
        }

        private static string GetRandomFirstName()
        {
            return FirstNames[random.Next(FirstNames.Count)];
        }

        private static string GetRandomLastName()
        {
            return LastNames[random.Next(LastNames.Count)];
        }

        private static string GetRandomFullName()
        {
            return $"{GetRandomFirstName()} {GetRandomLastName()}";
        }

        private static string GetRandomEmail()
        {
            return $"{GetRandomFirstName().ToLower()}.{GetRandomLastName().ToLower()}@{Domains[random.Next(Domains.Count)]}";
        }

        private static string GetAssuranceLevel()
        {
            return AssuranceLevel[random.Next(AssuranceLevel.Count)];
        }

        private static string GetAssuranceIap()
        {
            return AssuranceIap[random.Next(AssuranceIap.Count)];
        }
    }

}
