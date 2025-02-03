namespace CanvasAccountRegistration.Logic.Settings
{
    public class Saml2Settings
    {
        public string EntityId { get; set; }
        public string ReturnUrl { get; set; }
        public string LogoutCallbackUrl { get; set; }
        public IdentityProviderSettings IdentityProvider { get; set; }
        public CertificateSettings Certificate { get; set; }
    }

    public class IdentityProviderSettings
    {
        public string EntityId { get; set; }
        public string MetadataLocation { get; set; }
        public bool AllowUnsolicitedAuthnResponse { get; set; }
        public bool RelayStateUsedAsReturnUrl { get; set; }
        public string SingleLogoutUrl { get; set; }
    }

    public class CertificateSettings
    {
        public string SubjectName { get; set; }
        public string Path { get; set; }
        public string Password { get; set; }
    }
}
