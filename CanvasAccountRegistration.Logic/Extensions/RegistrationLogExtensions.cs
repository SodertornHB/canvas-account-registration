using CanvasAccountRegistration.Logic.Model;

namespace CanvasAccountRegistration.Logic.Extensions
{
    public static class RegistrationLogExtensions
    {
        public static void MapAccount(this RegistrationLog registrationLog, Account account)
        {
            account.Surname = registrationLog.sn ?? string.Empty;
            account.GivenName = registrationLog.givenName ?? string.Empty;
            account.DisplayName = registrationLog.displayName ?? string.Empty;
            account.AssuranceLevel = registrationLog.eduPersonAssurance ?? string.Empty;
            account.Email = registrationLog.mail ?? string.Empty;
            account.UserId = registrationLog.eduPersonPrincipalName ?? string.Empty;
        }
    }
}
