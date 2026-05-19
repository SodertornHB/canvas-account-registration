using CanvasAccountRegistration.Logic.Settings;
using Logic.Service;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CanvasAccountRegistration.Test
{
    public class RedirectLinkServiceTests
    {
        private RedirectLinkService CreateSut(string baseUrl = "https://redirect.example.com")
        {
            var options = Options.Create(new PostRegistrationRedirectSettings { BaseUrl = baseUrl });
            return new RedirectLinkService(options);
        }

        [Test]
        public void IsValidSlug_AcceptsLowercaseHyphenated()
        {
            var sut = CreateSut();
            Assert.That(sut.IsValidSlug("smob-grunder-i-kriminell-ekonomi"), Is.True);
        }

        [Test]
        public void IsValidSlug_AcceptsTripleHyphenSequences()
        {
            var sut = CreateSut();
            Assert.That(sut.IsValidSlug("smob---grunder-i-kriminell-ekonomi"), Is.True);
        }

        [Test]
        public void IsValidSlug_AcceptsDigits()
        {
            var sut = CreateSut();
            Assert.That(sut.IsValidSlug("course-101-intro"), Is.True);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("UPPER-case")]
        [TestCase("with space")]
        [TestCase("path/traversal")]
        [TestCase("../etc/passwd")]
        [TestCase("query?injection=1")]
        [TestCase("hash#frag")]
        [TestCase("javascript:alert(1)")]
        [TestCase("https://evil.example.com")]
        [TestCase("-leading-hyphen")]
        [TestCase("trailing-hyphen-")]
        [TestCase("under_score")]
        [TestCase("dot.notation")]
        public void IsValidSlug_RejectsInvalid(string slug)
        {
            var sut = CreateSut();
            Assert.That(sut.IsValidSlug(slug), Is.False);
        }

        [Test]
        public void IsValidSlug_RejectsTooLong()
        {
            var sut = CreateSut();
            var slug = new string('a', 201);
            Assert.That(sut.IsValidSlug(slug), Is.False);
        }

        [Test]
        public void Sanitize_TrimsAndReturnsValid()
        {
            var sut = CreateSut();
            Assert.That(sut.Sanitize("  smob-foo  "), Is.EqualTo("smob-foo"));
        }

        [Test]
        public void Sanitize_ReturnsNullForInvalid()
        {
            var sut = CreateSut();
            Assert.That(sut.Sanitize("../../passwd"), Is.Null);
        }

        [Test]
        public void BuildRedirectUrl_ConstructsExpectedPath()
        {
            var sut = CreateSut("https://redirect.example.com");
            var url = sut.BuildRedirectUrl("smob---grunder-i-kriminell-ekonomi");
            Assert.That(url, Is.EqualTo("https://redirect.example.com/smob---grunder-i-kriminell-ekonomi"));
        }

        [Test]
        public void BuildRedirectUrl_TrimsTrailingSlashOnBaseUrl()
        {
            var sut = CreateSut("https://redirect.example.com/");
            var url = sut.BuildRedirectUrl("foo");
            Assert.That(url, Is.EqualTo("https://redirect.example.com/foo"));
        }

        [Test]
        public void BuildRedirectUrl_ReturnsNullForInvalidSlug()
        {
            var sut = CreateSut();
            Assert.That(sut.BuildRedirectUrl("../etc/passwd"), Is.Null);
        }

        [Test]
        public void BuildRedirectUrl_ReturnsNullWhenBaseUrlMissing()
        {
            var sut = CreateSut(baseUrl: null);
            Assert.That(sut.BuildRedirectUrl("foo"), Is.Null);
        }
    }
}
