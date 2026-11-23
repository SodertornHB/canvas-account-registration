# Canvas Account Registration

A .NET 10 web application that allows users to register their [EduID](https://www.eduid.se/) accounts for use with [Canvas LMS](https://www.instructure.com/canvas). Users authenticate via SAML2/EduID, fill in their registration details, and have their accounts created in Canvas automatically.

The solution also includes an **admin web application** for managing accounts, viewing logs, and configuring whitelisted email domains.

---

## Solution Structure

```
canvas-account-registration/
├── CanvasAccountRegistration.Web/          # User-facing registration app
├── CanvasAccountRegistrationAdmin.Web/     # Admin management app
├── CanvasAccountRegistration.Logic/        # Shared business logic & data access
└── CanvasAccountRegistration.Test/         # Unit and integration tests
```

### Projects at a glance

| Project | Purpose |
|---|---|
| `CanvasAccountRegistration.Web` | Public-facing registration flow with SAML2/EduID authentication |
| `CanvasAccountRegistrationAdmin.Web` | Admin dashboard to manage accounts, logs, migrations, and whitelisted email domains |
| `CanvasAccountRegistration.Logic` | Shared services, Dapper-based data access, models, and Canvas API integration |
| `CanvasAccountRegistration.Test` | NUnit tests with Moq for mocking |

---

## Tech Stack

- **.NET 10** — ASP.NET Core MVC
- **SQL Server** — with [Dapper](https://github.com/DapperLib/Dapper) for data access
- **SAML2** — via [Sustainsys.Saml2](https://github.com/Sustainsys/Saml2) for EduID federation
- **AutoMapper** — object mapping between models and DTOs
- **NLog** — logging to database and files
- **Localization** — Swedish (sv-se) default, English (en-gb) supported

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (or SQL Server Express / LocalDB for local development)
- Access to a Canvas LMS instance with an API bearer token
- A SAML2 identity provider (for example eduID through SWAMID) and a service provider certificate (`.pfx`) for the registration app
- An SMTP server for the welcome e-mail from the admin app

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/SodertornHB/canvas-account-registration.git
cd canvas-account-registration
```

### 2. Set up the database

1. Open your database management tool (e.g. SQL Server Management Studio).
2. Create a new database.
3. Run the migration script to create all tables:
   ```
   CanvasAccountRegistrationAdmin.Web/Migration/Migrations.sql
   ```

### 3. Configure the applications

Each web project has a template. Copy it to `appsettings.json` in the same folder and fill in your values:

```bash
cp CanvasAccountRegistration.Web/appsettings.json.template CanvasAccountRegistration.Web/appsettings.json
cp CanvasAccountRegistrationAdmin.Web/appsettings.json.template CanvasAccountRegistrationAdmin.Web/appsettings.json
```

`appsettings.{Environment}.json` (for example `appsettings.Development.json`) overrides `appsettings.json`. Do not commit files that contain secrets.

Both applications use the same database.

#### Settings used by both applications

| Key | Description |
|---|---|
| `ConnectionStrings:Default` | Connection string to the SQL Server database |
| `Application:Name` | Display name of the application |
| `Application:KeepLogsInDays` | Number of days that log files are kept |
| `Authentication:UserAgent` | `User-Agent` header sent with every request to the Canvas API. Default: `CanvasAccountRegistration/1.0` |
| `Authentication:BearerToken` | Default bearer token of the HTTP client. The Canvas token below replaces it for calls to Canvas, so it can be empty |

#### Registration app (`CanvasAccountRegistration.Web`)

| Key | Description |
|---|---|
| `Application:Types` | Account types that are accepted in the `type` query parameter, for example `[ "guest" ]`. Required |
| `Application:DefaultAccountType` | Account type used when `type` is missing or not in `Types` |
| `Application:Roles` | Account roles that are accepted in the `role` query parameter. Required |
| `Application:DefaultAccountRole` | Role used when `role` is missing or not in `Roles` |
| `CanvasApiSettings:ApiHost` | Base URL of the Canvas API, for example `https://yourschool.instructure.com/api/v1` |
| `CanvasApiSettings:BearerToken` | Canvas API token. The token must be allowed to create users and to read and update logins |
| `PostRegistrationRedirect:BaseUrl` | Base URL of the link shown after an automatically approved registration. See [Redirect after registration](#redirect-after-registration). Leave empty to disable |
| `Authentication:Saml2:EntityId` | Entity ID of this service provider |
| `Authentication:Saml2:IdentityProvider:EntityId` | Entity ID of the identity provider (eduID in the template) |
| `Authentication:Saml2:IdentityProvider:MetadataLocation` | URL or path of the identity provider metadata |
| `Authentication:Saml2:IdentityProvider:RelayStateUsedAsReturnUrl` | Set to `true` so that the user returns to the page that started the login |
| `Authentication:Saml2:Certificate:Path` | Path to the service provider certificate (`.pfx`). Required |
| `Authentication:Saml2:Certificate:Password` | Password of the certificate file. Leave empty when the file has no password |

The role is stored with the account and shown in the admin app. It is not sent to Canvas.

#### Admin app (`CanvasAccountRegistrationAdmin.Web`)

| Key | Description |
|---|---|
| `Canvas:ApiHost` | Base URL of the Canvas API |
| `Canvas:Host` | Host of your Canvas instance |
| `Canvas:BearerToken` | Canvas API token, same requirements as for the registration app |
| `Smtp:Host` | SMTP server used for the welcome e-mail. The e-mail is sent when an administrator integrates an account into Canvas |
| `Smtp:Port` | SMTP port. Default: `25` |
| `Smtp:EnableSsl` | Set to `true` to use TLS |
| `Smtp:UserName`, `Smtp:Password` | Credentials for the SMTP server. Leave `UserName` empty when the server needs no login |
| `IPBlockOptions:BlockedIPs` | IP addresses that are refused by the admin app |

Note that the Canvas settings are in the section `CanvasApiSettings` in the registration app and in the section `Canvas` in the admin app.

To send e-mail through something other than SMTP, write your own implementation of `IMailService` (`CanvasAccountRegistration.Logic/Service/MailService.cs`) and register it in `StartupExtended.cs` of the admin app.

#### SAML2 metadata

`CanvasAccountRegistration.Web/saml_config/saml.xml.template` is a template for the service provider metadata that you register with your identity federation (for example SWAMID). Copy it to `saml.xml` and replace the placeholders in uppercase. The application does not read this file. Git ignores the folder `saml_config/`, apart from the templates.

The application reads these attributes from the identity provider: `displayName`, `givenName`, `sn`, `mail`, `eduPersonPrincipalName` and `eduPersonAssurance`.

#### Logging

Logging is configured in `nlog.config` in each web project. `nlog.{Environment}.config` is used instead when it exists. The admin app writes to log files and to the `Log` table in the database. The registration app writes to the console and to log files.

### 4. Restore dependencies

```bash
dotnet restore
```

### 5. Run the application

**Registration web app** (`https://localhost:5002`):
```bash
cd CanvasAccountRegistration.Web
dotnet run
```

**Admin web app** (`https://localhost:5003`):
```bash
cd CanvasAccountRegistrationAdmin.Web
dotnet run
```

---

## Running Tests

```bash
dotnet test
```

---

## Authentication in the Admin App

> **Warning:** `CanvasAccountRegistrationAdmin.Web` has **no authentication**. Every page and every endpoint under `/api/v1/` is open to anyone who can reach the application. The admin app shows personal data and creates accounts in Canvas.

You must add authentication and authorization before you deploy the admin app, for example OpenID Connect or SAML2 against your identity provider. The place to add it is `CustomServiceConfiguration` and `CustomConfiguration` in `CanvasAccountRegistrationAdmin.Web/StartupExtended.cs`.

---

## Organization-Specific Values

Values that differ between institutions are written as placeholders. Search the solution for these and replace them with your own values:

| Placeholder | Where | Meaning |
|---|---|---|
| `YOUR_ORGANIZATION` | `Resources/*.resx` | Name of your institution |
| `YOUR_DOMAIN` | `Resources/*.resx`, `Views/Shared/_Layout.cshtml`, `HomeControllerExtended.cs` (admin) | Your web domain (logo, favicon, shared CSS, footer, contact addresses) |
| `YOUR_CANVAS_HOST` | `Resources/*.resx`, `appsettings.json.template` | Host name of your Canvas instance |
| `YOUR_SMTP_HOST` | `appsettings.json.template` (admin) | SMTP server used for the welcome e-mail |
| `YOUR_REGISTRATION_HOST` | `HomeControllerExtended.cs` (admin) | Public host name of the registration app, used in the welcome e-mail |
| `YOUR_PROGRAMME`, `YOUR_PROGRAMME_DESCRIPTION` | `Resources/*.resx` | Name and description of the programme on the instructions page |

Files marked with the comment `This is an organization specific file` are the ones you are most likely to change.

The whitelist of e-mail domains is empty after the migration. Add your domains in the admin app.

An optional image per account type can be placed in `CanvasAccountRegistrationAdmin.Web/wwwroot/img/{accountType}.png`. The admin start page shows the account type as text when no image exists.

---

## Key Concepts

### Registration flow (user app)

1. The user opens the start page of the registration app. The link can have the query parameters `type`, `role` and `postRegistrationParam`.
1. A user who is not logged in is sent to the identity provider (SAML2, eduID in the template).
1. The app reads the SAML attributes and writes a registration log entry to the database.
1. The app creates the account in its own database, or updates it when the user (`eduPersonPrincipalName`) is already registered. A new account gets the account type and the role from the link.
1. The account is created in Canvas in one of two ways:
   - **Automatically**, when the user has SWAMID assurance level AL2 (verified identity) and the e-mail domain is on the whitelist and marked as a partner organization.
   - **By an administrator**, who approves the account and integrates it into Canvas in the admin app. The user then gets a welcome e-mail.
1. The start page shows the status of the registration.

When Canvas answers that the `integration_id` (the e-mail address) is already taken, the app looks up the existing Canvas user. It links or reactivates that user only when the login is the same `eduPersonPrincipalName`. In all other cases the integration fails with an error that names the existing user.

### Redirect after registration

A link to the registration app can carry a slug in `postRegistrationParam`, for example `https://YOUR_HOST/?postRegistrationParam=my-course`. When the user is from a partner organization, the page shows a link to `{PostRegistrationRedirect:BaseUrl}/my-course`. A slug can contain lowercase letters, digits and hyphens, and can have a maximum of 200 characters. Other slugs are ignored.

### Admin app

The admin app provides views and a RESTful API (`/api/v1/`) for:
- **Accounts** — browse, search, and manage registered Canvas accounts
- **Registration Logs** — view registration attempts and their outcomes
- **Logs** — view application logs
- **Migrations** — track database schema versions
- **Whitelisted Email Domains** — manage the e-mail domains of partner organizations. Accounts from a domain that is marked as partner organization can be created in Canvas automatically

The start page of the admin app lists the registered accounts per account type. An administrator can approve an account, integrate it into Canvas or remove it.

The admin app has no authentication. See [Authentication in the Admin App](#authentication-in-the-admin-app).

---

## Contributing

1. Fork the repository and clone your fork.
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes and add or update tests where relevant.
4. Commit with a descriptive message and push to your fork.
5. Open a pull request against the `main` branch of the original repository.

Please include a clear description of what the change does and why it is needed.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
