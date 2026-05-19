# Canvas Account Registration

A .NET 8.0 web application that allows users to register their [EduID](https://www.eduid.se/) accounts for use with [Canvas LMS](https://www.instructure.com/canvas). Users authenticate via SAML2/EduID, fill in their registration details, and have their accounts created in Canvas automatically.

The solution also includes an **admin web application** for managing accounts, viewing logs, and configuring whitelisted email domains.

---

## Solution Structure

```
canvas-account-registration/
├── CanvasAccountRegistration.Web/          # User-facing registration app
├── CanvasAccountRegistrationAdmin.Web/     # Admin management app
├── CanvasAccountRegistration.Logic/        # Shared business logic & data access
├── CanvasAccountRegistration.Test/         # Unit and integration tests
└── organizational-specific/               # Organization-specific overrides (not in repo)
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

- **.NET 8.0** — ASP.NET Core MVC
- **SQL Server** — with [Dapper](https://github.com/DapperLib/Dapper) for data access
- **SAML2** — via [Sustainsys.Saml2](https://github.com/Sustainsys/Saml2) for EduID federation
- **AutoMapper** — object mapping between models and DTOs
- **NLog** — logging to database and files
- **Localization** — Swedish (sv-se) default, English (en-gb) supported

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (or SQL Server Express / LocalDB for local development)
- Access to a Canvas LMS instance with an API bearer token
- (Optional) A SAML2 identity provider for full authentication flow

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

### 3. Configure the application

A template configuration file is provided. Copy it and fill in your values:

```bash
cp CanvasAccountRegistration.Web/appsettings.json.template CanvasAccountRegistration.Web/appsettings.json
```

Open `appsettings.json` and update the following sections:

#### Database connection
```json
"ConnectionStrings": {
  "Default": "Server=SERVER_NAME;Database=DATABASE_NAME;..."
}
```

#### Application settings
| Key | Description |
|---|---|
| `Application:Name` | Display name for the application |
| `Application:KeepLogsInDays` | Number of days to retain log entries |
| `Application:KeysFolder` | Path to the folder for data protection keys |

#### Canvas LMS
| Key | Description |
|---|---|
| `Canvas:ApiHost` | Base URL of your Canvas API (e.g. `https://yourschool.instructure.com`) |
| `Canvas:Host` | Canvas host used for login links |
| `Canvas:BearerToken` | API token for authenticating requests to Canvas |

### 4. Restore dependencies

```bash
dotnet restore
```

### 5. Run the application

**Registration web app:**
```bash
cd CanvasAccountRegistration.Web
dotnet run
```

**Admin web app:**
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

## Organizational-Specific Folder

This project supports an `organizational-specific/` folder in the solution root for institution-specific files and settings. Files placed here are automatically copied into the web projects during build, so they won't be overwritten when pulling updates from GitHub.

### Structure

```
organizational-specific/
├── web/            # Files copied into CanvasAccountRegistration.Web/
└── admin-web/      # Files copied into CanvasAccountRegistrationAdmin.Web/
```

The folder structure inside `web/` and `admin-web/` mirrors the structure of the respective web projects.

The following file types are copied automatically: `.css`, `.js`, `.cs`, `.json`, `.csproj`, `.resx`, `.xml`, `.png`, `.cshtml`

### Example: local development settings

Place your `appsettings.Development.json` directly in `organizational-specific/web/`. It will be copied to the web project on build and picked up automatically in the `Development` environment.

> **Important:** Any file in the web project with the same name and path as a file in `organizational-specific/` will be overwritten on build. The `organizational-specific/` folder is not tracked by git — add it to `.gitignore` or manage it separately per institution.

---

## Key Concepts

### Registration flow (user app)

1. User navigates to the registration page.
1. User authenticates via SAML2/EduID.
1. The app reads SAML attributes (name, email, assurance level) and pre-fills the form.
1. The app validates the email domain against the whitelist and creates the account in Canvas via the Canvas API.
1. A registration log entry is written to the database.

### Admin app

The admin app provides views and a RESTful API (`/api/v1/`) for:
- **Accounts** — browse, search, and manage registered Canvas accounts
- **Registration Logs** — view registration attempts and their outcomes
- **Logs** — view application logs
- **Migrations** — track database schema versions
- **Whitelisted Email Domains** — manage which email domains are permitted

---

## Contributing

1. Fork the repository and clone your fork.
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes and add or update tests where relevant.
4. Commit with a descriptive message and push to your fork.
5. Open a pull request against the `main` branch of the original repository.

Please include a clear description of what the change does and why it is needed.
