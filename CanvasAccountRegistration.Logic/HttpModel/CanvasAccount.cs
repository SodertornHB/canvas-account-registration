using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Logic.HttpModel
{
    public class PostCanvasAccountRequestModel
    {
        public CanvasUserPostModel user { get; set; } = new CanvasUserPostModel();
        public PseudonymCanvasPostModel pseudonym { get; set; } = new PseudonymCanvasPostModel();
        public Communication_channelCanvasPostModel communication_channel { get; set; } = new Communication_channelCanvasPostModel();
        public bool Enable_sis_reactivation { get; set; } = true;
    }

    public class CanvasUserPostModel
    {
        public string Name { get; set; }
        public string Short_name { get; set; }
        public string Sortable_name { get; set; }
        public string Time_zone { get { return "Stockholm"; } }
        public string Locale { get; set; }
        public string Terms_of_use { get; set; }
        public bool Skip_registration { get { return true; } }
    }

    public class PseudonymCanvasPostModel
    {
        public string Unique_id { get; set; }
        //public string Password { get; set; }
        public string Sis_user_id { get; set; }
        public string Integration_id { get; set; }
        public bool Send_confirmation { get { return false; } }
        public bool Force_self_registration { get { return false; } }
        public string Authentication_provider_id { get; set; }
    }

    public class Communication_channelCanvasPostModel
    {
        public string Type { get { return "email"; } }
        public string Address { get; set; }
        public string Confirmation_url { get; set; }
        public bool Skip_confirmation { get { return true; } }
    }
    public class PostCanvasAccountResponseModel
    {
        public string id { get; set; }
    }

    public class CanvasLoginResponseModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string IntegrationId { get; set; }
        public string SisUserId { get; set; }
        public string UniqueId { get; set; }
        public string WorkflowState { get; set; }
    }

    /// <summary>
    /// Subset of the Canvas User object returned by e.g. GET /accounts/:id/users.
    /// sis_user_id, integration_id and login_id are only present when the API token has SIS permissions.
    /// </summary>
    public class CanvasUserResponseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SisUserId { get; set; }
        public string IntegrationId { get; set; }
        public string LoginId { get; set; }
        /// <summary>Comma separated login ids of all the user's pseudonyms (returned by the account user search).</summary>
        public string LoginIds { get; set; }
        /// <summary>Comma separated sis_user_ids of all the user's pseudonyms (returned by the account user search).</summary>
        public string SisUserIds { get; set; }
        /// <summary>The (active) login holding the integration_id, when found via GET /users/:id/logins. Null for soft deleted users.</summary>
        [JsonIgnore]
        public CanvasLoginResponseModel Login { get; set; }

        public IEnumerable<string> GetLoginIds() => Split(LoginIds, LoginId);
        public IEnumerable<string> GetSisUserIds() => Split(SisUserIds, SisUserId);

        private static IEnumerable<string> Split(string commaSeparated, string single)
        {
            var values = (commaSeparated ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            if (!string.IsNullOrWhiteSpace(single)) values.Add(single.Trim());
            return values.Distinct(StringComparer.OrdinalIgnoreCase);
        }
    }

    public class PutCanvasLoginRequestModel
    {
        public PutCanvasLoginModel Login { get; set; } = new PutCanvasLoginModel();
    }

    public class PutCanvasLoginModel
    {
        public string IntegrationId { get; set; }
        public string SisUserId { get; set; }
    }

    public class RepairIntegrationIdsResult
    {
        public int Fixed { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
        public List<RepairIntegrationIdsEntry> Entries { get; set; } = new List<RepairIntegrationIdsEntry>();
    }

    public class RepairIntegrationIdsEntry
    {
        public int AccountId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; } // "Fixed", "Skipped", "Failed"
        public string ErrorMessage { get; set; }
    }
}
