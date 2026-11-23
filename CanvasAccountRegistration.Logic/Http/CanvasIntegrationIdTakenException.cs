using Logic.HttpModel;
using System;
using System.Net;
using System.Net.Http;

namespace CanvasAccountRegistration.Logic.Http
{
    /// <summary>
    /// Thrown when Canvas rejects a user creation because the integration_id (the user's e-mail)
    /// is already taken by an existing (possibly soft deleted) Canvas user, and the account
    /// could not be revived automatically.
    /// </summary>
    public class CanvasIntegrationIdTakenException : HttpRequestException
    {
        public CanvasIntegrationIdTakenException(string integrationId, CanvasUserResponseModel existingUser, HttpRequestException inner)
            : base(BuildMessage(integrationId, existingUser, inner), inner, inner?.StatusCode ?? HttpStatusCode.BadRequest)
        {
            IntegrationId = integrationId;
            ExistingUser = existingUser;
        }

        /// <summary>The integration_id (e-mail) that Canvas reported as taken.</summary>
        public string IntegrationId { get; }

        /// <summary>The existing Canvas user found via lookup, or null if none was found.</summary>
        public CanvasUserResponseModel ExistingUser { get; }

        private static string BuildMessage(string integrationId, CanvasUserResponseModel existingUser, Exception inner)
        {
            var existing = existingUser == null
                ? "no existing Canvas user could be found"
                : $"existing Canvas user id={existingUser.Id}, sis_user_id='{existingUser.SisUserId}', login_id='{existingUser.LoginId}', name='{existingUser.Name}'";
            return $"Canvas integration_id '{integrationId}' is already taken ({existing}). {inner?.Message}";
        }
    }
}
