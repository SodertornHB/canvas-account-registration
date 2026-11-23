using AutoMapper;
using CanvasAccountRegistration.Logic.DataAccess;
using CanvasAccountRegistration.Logic.Extensions;
using CanvasAccountRegistration.Logic.Http;
using CanvasAccountRegistration.Logic.Model;
using CanvasAccountRegistration.Logic.Settings;
using Logic.Http;
using Logic.HttpModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.Services
{
    public partial interface IAccountServiceExtended : IAccountService
    {
        Task<IEnumerable<string>> GetAccountTypes();
        Task<Account> GetByUserId(string userId);
        Task<Account> NewRegister(RequestedAttributeCollection requestedAttributeCollection, string accountType, string role);
        Task<PostCanvasAccountResponseModel> IntegrateIntoCanvas(Account account);
        Task<RepairIntegrationIdsResult> RepairIntegrationIds();
    }

    public partial class AccountServiceExtended : AccountService, IAccountServiceExtended
    {
        private readonly IArchivedAccountServiceExtended archivedAccountService;
        private new readonly IAccountDataAccessExtended dataAccess;
        private readonly IRegistrationLogServiceExtended registrationLogService;
        private readonly IMapper mapper;
        private readonly IPostCanvasAccountHttpService postCanvasAccountHttpService;
        private readonly CanvasSettings canvasSettings;
        private readonly IHttpClient httpClient;        

        public AccountServiceExtended(ILogger<AccountService> logger,
           IAccountDataAccessExtended dataAccess,
           IArchivedAccountServiceExtended archivedAccountService,
           IRegistrationLogServiceExtended registrationLogService,
           IMapper mapper,
           IPostCanvasAccountHttpService postCanvasAccountHttpService,
           IHttpClient httpClient,
           IOptions<CanvasSettings> options)
           : base(logger, dataAccess)
        {
            this.dataAccess = dataAccess;
            this.archivedAccountService = archivedAccountService;
            this.registrationLogService = registrationLogService;
            this.mapper = mapper;
            this.postCanvasAccountHttpService = postCanvasAccountHttpService;
            this.httpClient = httpClient;
            canvasSettings = options.Value;
            postCanvasAccountHttpService.OverrideDefaultBearerToken(canvasSettings.BearerToken);
            httpClient.SetBearerToken(canvasSettings.BearerToken);
        }

        public  async Task<Account> GetByUserId(string userId)
        {
            var accounts = await GetAll();
            var account = accounts.SingleOrDefault(x => x.UserId == userId);
            return account;
        }

        public async Task<PostCanvasAccountResponseModel> IntegrateIntoCanvas(Account account)
        {
            account.IntegratedOn = System.DateTime.UtcNow;
            if (account.VerifiedOn == null) account.VerifiedOn = System.DateTime.UtcNow;
            var postModel = mapper.Map<PostCanvasAccountRequestModel>(account);
            PostCanvasAccountResponseModel response;
            try
            {
                response = await postCanvasAccountHttpService.Post(CreateUserUrl, postModel);
            }
            catch (HttpRequestException e) when (IsIntegrationIdTaken(e))
            {
                // Canvas already has a user (possibly soft deleted) whose integration_id equals this e-mail.
                // enable_sis_reactivation only matches on sis_user_id, so look the existing user up and retry with its sis_user_id.
                response = await ReviveExistingCanvasUser(account, postModel, e);
            }
            await Update(account);
            return response;
        }

        public async Task<Account> NewRegister(RequestedAttributeCollection requestedAttributeCollection, string accountType, string role)
        {
            logger.LogDebug("RequestedAttributeCollection");
            foreach (var attribute in requestedAttributeCollection)
            {
                logger.LogDebug(attribute.ToString());
            }
            var registrationLog = await registrationLogService.NewRegister(requestedAttributeCollection);
            logger.LogDebug("registrationLog: " + registrationLog.ToString());
            Account account = await GetByUserId(registrationLog.eduPersonPrincipalName);
            if (account != null)
            {
                await MapValuesAndUpdate(registrationLog, account);
                return account;
            }
            account = mapper.Map<Account>(registrationLog);
            account.AccountType = accountType;
            account.AccountRole = role;
            return await Insert(account);
        }

        public override async Task Delete(int id)
        {
            var account = await Get(id);
            if (account == null) return;
            var archivedAccount = await archivedAccountService.GetByInitialId(id);
            if (archivedAccount == null)
            {
                await ArchiveAccount(account);
            }
            await base.Delete(id);
        }

        public async Task<RepairIntegrationIdsResult> RepairIntegrationIds()
        {
            var accounts = await GetAll();
            var integratedAccounts = accounts.Where(x => x.IntegratedOn != null).ToList();

            var result = new RepairIntegrationIdsResult();

            foreach (var account in integratedAccounts)
            {
                try
                {
                    var loginsUrl = $"{canvasSettings.ApiHost}/users/sis_user_id:{account.Id}/logins";
                    var getResponse = await httpClient.Get(new Uri(loginsUrl));
                    getResponse.CheckStatus();
                    var logins = JsonConvert.DeserializeObject<IEnumerable<CanvasLoginResponseModel>>(getResponse.Content);

                    foreach (var login in logins)
                    {
                        if (login.IntegrationId == account.Email)
                        {
                            result.Skipped++;
                            result.Entries.Add(new RepairIntegrationIdsEntry
                            {
                                AccountId = account.Id,
                                DisplayName = account.DisplayName,
                                Email = account.Email,
                                Status = "Skipped"
                            });
                            continue;
                        }

                        var putUrl = $"{canvasSettings.ApiHost}/accounts/self/logins/{login.Id}";
                        var putModel = new PutCanvasLoginRequestModel { Login = new PutCanvasLoginModel { IntegrationId = account.Email } };
                        var putContent = JsonConvert.SerializeObject(putModel);
                        var putResponse = await httpClient.Put(new Uri(putUrl), putContent);
                        putResponse.CheckStatus();
                        result.Fixed++;
                        result.Entries.Add(new RepairIntegrationIdsEntry
                        {
                            AccountId = account.Id,
                            DisplayName = account.DisplayName,
                            Email = account.Email,
                            Status = "Fixed"
                        });
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"RepairIntegrationIds failed for account {account.Id} ({account.Email}): {e.Message}");
                    result.Failed++;
                    result.Entries.Add(new RepairIntegrationIdsEntry
                    {
                        AccountId = account.Id,
                        DisplayName = account.DisplayName,
                        Email = account.Email,
                        Status = "Failed",
                        ErrorMessage = e.Message
                    });
                }
            }

            return result;
        }

        public async Task<IEnumerable<string>> GetAccountTypes() => await dataAccess.GetAccountTypes();

        #region Canvas revive (integration_id taken)

        private string CreateUserUrl => $"{canvasSettings.ApiHost}/accounts/self/users";

        /// <summary>
        /// True when Canvas answered 400 with errors.pseudonym.integration_id[*].type == "taken".
        /// </summary>
        private static bool IsIntegrationIdTaken(HttpRequestException e)
        {
            if (e.StatusCode != HttpStatusCode.BadRequest || string.IsNullOrEmpty(e.Message)) return false;
            var jsonStart = e.Message.IndexOf('{');
            if (jsonStart < 0) return false;
            try
            {
                var json = JObject.Parse(e.Message.Substring(jsonStart));
                var integrationIdErrors = json.SelectToken("errors.pseudonym.integration_id") as JArray;
                return integrationIdErrors?.Any(x => string.Equals(x.Value<string>("type"), "taken", StringComparison.OrdinalIgnoreCase)) ?? false;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        /// <summary>
        /// Called when Canvas answered "integration_id taken". Looks up the Canvas user holding the integration_id and then:
        ///  - active login with the same unique_id (eppn) as the registering user => same person: re-link the login's sis_user_id to this account (no new Canvas user),
        ///  - soft deleted user whose login is the same eppn and that has a sis_user_id (i.e. a user this app created earlier and that was deleted)
        ///                                                                        => retry the creation with that sis_user_id so enable_sis_reactivation revives user + login,
        ///  - anything else                                                       => throw <see cref="CanvasIntegrationIdTakenException"/> carrying the looked up user.
        /// Reviving is deliberately limited to the same eppn: enable_sis_reactivation reactivates the user even when the login part fails,
        /// so guessing on e-mail only can reactivate the wrong (old guest) user.
        /// </summary>
        private async Task<PostCanvasAccountResponseModel> ReviveExistingCanvasUser(Account account, PostCanvasAccountRequestModel postModel, HttpRequestException original)
        {
            logger.LogWarning("Canvas rejected integration of account {AccountId}: integration_id '{Email}' is already taken. Looking up existing Canvas user.", account.Id, account.Email);

            var existing = await FindCanvasUserByIntegrationId(account.Email, account.UserId);
            if (existing == null)
            {
                logger.LogWarning("No existing Canvas user found for integration_id '{Email}' / eppn '{Eppn}' (account {AccountId}).", account.Email, account.UserId, account.Id);
                throw new CanvasIntegrationIdTakenException(account.Email, null, original);
            }

            logger.LogWarning("Found existing Canvas user id={CanvasUserId}, sis_user_id='{SisUserId}', login_id='{LoginId}', name='{Name}', active login={HasLogin} for integration_id '{Email}' (account {AccountId}).",
                existing.Id, existing.SisUserId, existing.LoginId, existing.Name, existing.Login != null, account.Email, account.Id);

            if (existing.Login != null)
            {
                // An active login holds the integration_id. Only re-use it when it is the same identity (same eppn).
                if (!string.Equals(existing.Login.UniqueId, account.UserId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new CanvasIntegrationIdTakenException(account.Email, existing, original);
                }
                await RelinkCanvasLogin(existing.Login, account);
                logger.LogInformation("Linked account {AccountId} ({Email}) to existing active Canvas user id={CanvasUserId} via login {LoginId} (unique_id='{UniqueId}'); sis_user_id set to {SisUserId}.",
                    account.Id, account.Email, existing.Id, existing.Login.Id, existing.Login.UniqueId, account.Id);
                return new PostCanvasAccountResponseModel { id = existing.Id };
            }

            var sameEppn = existing.GetLoginIds().Any(l => string.Equals(l, account.UserId, StringComparison.OrdinalIgnoreCase));
            if (!sameEppn || string.IsNullOrWhiteSpace(existing.SisUserId) || existing.SisUserId == postModel.pseudonym.Sis_user_id)
            {
                // Not a deleted user of the same identity with a usable sis_user_id - do not guess.
                throw new CanvasIntegrationIdTakenException(account.Email, existing, original);
            }

            postModel.pseudonym.Sis_user_id = existing.SisUserId;
            postModel.Enable_sis_reactivation = true;
            try
            {
                var response = await postCanvasAccountHttpService.Post(CreateUserUrl, postModel);
                logger.LogInformation("Revived Canvas user id={CanvasUserId} (sis_user_id='{SisUserId}') for account {AccountId} ({Email}).", response?.id, existing.SisUserId, account.Id, account.Email);
                return response;
            }
            catch (HttpRequestException retryException)
            {
                throw new CanvasIntegrationIdTakenException(account.Email, existing, retryException);
            }
        }

        /// <summary>PUT /accounts/self/logins/{id} - point the existing login's sis_user_id at this account (integration_id stays = e-mail).</summary>
        private async Task RelinkCanvasLogin(CanvasLoginResponseModel login, Account account)
        {
            var putModel = new PutCanvasLoginRequestModel { Login = new PutCanvasLoginModel { SisUserId = account.Id.ToString(), IntegrationId = account.Email } };
            var response = await httpClient.Put(new Uri($"{canvasSettings.ApiHost}/accounts/self/logins/{login.Id}"), JsonConvert.SerializeObject(putModel));
            response.CheckStatus();
        }

        /// <summary>
        /// Finds the Canvas user that holds the integration_id (e-mail):
        /// 1. GET /accounts/self/users?search_term={email}&include_deleted_users=true (include_deleted_users makes soft deleted users visible).
        ///    The search does not expose per-login integration_ids, only comma separated login_ids/sis_user_ids.
        /// 2. For every returned user, GET /users/{id}/logins (lists active logins only) and pick the login whose integration_id equals the e-mail.
        /// 3. Otherwise (the integration_id sits on a deleted login) prefer a user whose login_ids contain the eppn, then one whose login_ids contain the e-mail;
        ///    in both cases one that has a sis_user_id, since that is what enable_sis_reactivation matches on.
        /// </summary>
        private async Task<CanvasUserResponseModel> FindCanvasUserByIntegrationId(string email, string eppn)
        {
            var url = $"{canvasSettings.ApiHost}/accounts/self/users?search_term={Uri.EscapeDataString(email)}&include_deleted_users=true&per_page=50";
            var response = await httpClient.Get(new Uri(url));
            response.CheckStatus();
            var users = (JsonConvert.DeserializeObject<IEnumerable<CanvasUserResponseModel>>(response.Content) ?? Enumerable.Empty<CanvasUserResponseModel>()).ToList();

            bool HasLogin(CanvasUserResponseModel u, string loginId) =>
                u.GetLoginIds().Any(l => string.Equals(l, loginId, StringComparison.OrdinalIgnoreCase));
            bool HasEppn(CanvasUserResponseModel u) => !string.IsNullOrWhiteSpace(eppn) && HasLogin(u, eppn);
            bool HasEmail(CanvasUserResponseModel u) => HasLogin(u, email) || string.Equals(u.IntegrationId, email, StringComparison.OrdinalIgnoreCase);

            logger.LogInformation("Canvas user search for '{Email}' returned {Total} users: {Ids}",
                email, users.Count, string.Join(", ", users.Select(u => $"{u.Id} (login_ids='{u.LoginIds}', sis_user_ids='{u.SisUserIds}')")));
            if (users.Count == 0) return null;

            foreach (var user in users.OrderByDescending(HasEppn).ThenByDescending(HasEmail))
            {
                var login = await FindCanvasLoginByIntegrationId(user.Id, email);
                if (login == null) continue;
                logger.LogInformation("Canvas user {CanvasUserId} has active login {LoginId} (unique_id='{UniqueId}', sis_user_id='{SisUserId}', workflow_state='{State}') holding integration_id '{Email}'.",
                    user.Id, login.Id, login.UniqueId, login.SisUserId, login.WorkflowState, email);
                user.Login = login;
                user.IntegrationId = login.IntegrationId;
                user.LoginId = login.UniqueId;
                user.SisUserId = !string.IsNullOrWhiteSpace(login.SisUserId) ? login.SisUserId : user.GetSisUserIds().FirstOrDefault();
                return user;
            }

            // No active login exposes the integration_id - it sits on a deleted login. Prefer the same identity (eppn).
            var fallback = users.FirstOrDefault(u => HasEppn(u) && u.GetSisUserIds().Any())
                        ?? users.FirstOrDefault(HasEppn)
                        ?? users.FirstOrDefault(u => HasEmail(u) && u.GetSisUserIds().Any())
                        ?? users.FirstOrDefault(HasEmail);
            if (fallback != null)
            {
                fallback.SisUserId = fallback.GetSisUserIds().FirstOrDefault();
                fallback.LoginId = fallback.GetLoginIds().FirstOrDefault();
            }
            return fallback;
        }

        private async Task<CanvasLoginResponseModel> FindCanvasLoginByIntegrationId(string canvasUserId, string email)
        {
            try
            {
                var response = await httpClient.Get(new Uri($"{canvasSettings.ApiHost}/users/{canvasUserId}/logins"));
                response.CheckStatus();
                var logins = JsonConvert.DeserializeObject<IEnumerable<CanvasLoginResponseModel>>(response.Content) ?? Enumerable.Empty<CanvasLoginResponseModel>();
                return logins.FirstOrDefault(l => string.Equals(l.IntegrationId, email, StringComparison.OrdinalIgnoreCase));
            }
            catch (HttpRequestException e)
            {
                logger.LogWarning(e, "Could not list logins for Canvas user {CanvasUserId}: {Message}", canvasUserId, e.Message);
                return null;
            }
        }

        #endregion

        private async Task ArchiveAccount(Account account)
        {
            var archivedAccount = account.ToArchivedAccount();
            await archivedAccountService.Insert(archivedAccount);
        }

        private async Task MapValuesAndUpdate(RegistrationLog registrationLog, Account account)
        {
            registrationLog.MapAccount(account); 
            await Update(account);
        }
    }
}
