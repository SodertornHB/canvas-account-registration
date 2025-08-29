using CanvasAccountRegistration.Logic.Model;
using System;

namespace CanvasAccountRegistration.Logic.Extensions
{
    public static class AccountExtensions
    {
        public static ArchivedAccount ToArchivedAccount(this Account account)
        {
            ArgumentNullException.ThrowIfNull(account, nameof(account));

            if (account.Id <= 0) throw new ArgumentOutOfRangeException(nameof(account.Id), "Account must have a persisted Id (> 0) before archiving.");

            if (account.CreatedOn is null) throw new InvalidOperationException("Unable to create archive account. CreatedOn is missing.");
            
            var emailDomain = GetEmailDomain(account);

            if (string.IsNullOrWhiteSpace(account.UserId)) throw new InvalidOperationException("Unable to create archive account. Lacking UserId.");

            return new ArchivedAccount
            {
                InitialId = account.Id,
                UserId = account.UserId,
                EmailDomain = emailDomain,
                CreatedOn = account.CreatedOn,
                VerifiedOn = account.VerifiedOn,
                IntegratedOn = account.IntegratedOn,
                DeletedOn = DateTime.UtcNow
            };
        }

        private static string GetEmailDomain(Account account)
        {
            var email = account.Email?.Trim();
            var at = email?.IndexOf('@') ?? -1;
            if (string.IsNullOrEmpty(email) || at <= 0 || at >= email.Length - 1) throw new InvalidOperationException("Unable to create archive account. Email is missing or invalid.");
            var emailDomain = email[(at + 1)..].ToLowerInvariant();
            return emailDomain;
        }
    }
}
