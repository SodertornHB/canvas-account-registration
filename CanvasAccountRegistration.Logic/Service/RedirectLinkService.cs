using CanvasAccountRegistration.Logic.Settings;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Logic.Service
{
    public interface IRedirectLinkService
    {
        bool IsValidSlug(string slug);
        string Sanitize(string slug);
        string BuildRedirectUrl(string slug);
    }

    public class RedirectLinkService : IRedirectLinkService
    {
        // Slugs may only contain lowercase letters, digits, and hyphens.
        // Bounded length keeps an attacker from blowing up the URL.
        private static readonly Regex SlugRegex = new Regex(
            "^[a-z0-9]+(?:-+[a-z0-9]+)*$",
            RegexOptions.Compiled);

        private const int MaxSlugLength = 200;

        private readonly PostRegistrationRedirectSettings settings;

        public RedirectLinkService(IOptions<PostRegistrationRedirectSettings> settingsOptions)
        {
            settings = settingsOptions.Value;
        }

        public bool IsValidSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return false;
            if (slug.Length > MaxSlugLength) return false;
            return SlugRegex.IsMatch(slug);
        }

        public string Sanitize(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;
            var trimmed = slug.Trim();
            return IsValidSlug(trimmed) ? trimmed : null;
        }

        public string BuildRedirectUrl(string slug)
        {
            var safeSlug = Sanitize(slug);
            if (safeSlug == null) return null;
            if (string.IsNullOrWhiteSpace(settings?.BaseUrl)) return null;

            var basePath = settings.BaseUrl.TrimEnd('/');
            return $"{basePath}/{safeSlug}";
        }
    }
}
