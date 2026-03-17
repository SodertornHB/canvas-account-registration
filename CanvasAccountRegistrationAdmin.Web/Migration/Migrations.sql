
if not exists (
    select distinct 1
    from information_schema.columns
    where table_name = 'RegistrationLog'
) begin CREATE TABLE RegistrationLog (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    displayName NVARCHAR(512) NOT NULL,
    givenName NVARCHAR(512),
    sn NVARCHAR(512) NOT NULL,
    eduPersonPrincipalName NVARCHAR(512),
    mail NVARCHAR(512),
    eduPersonAssurance NVARCHAR(512),
    CreatedOn DATETIME2 NOT NULL
)
end 

if not exists (
    select distinct 1
    from information_schema.columns
    where table_name = 'Account'
) begin CREATE TABLE Account (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    UserId NVARCHAR(512) NOT NULL UNIQUE,
    DisplayName NVARCHAR(512),
    GivenName NVARCHAR(512),
    Surname NVARCHAR(512),
    Email NVARCHAR(512),
    AssuranceLevel NVARCHAR(512),
    CreatedOn DATETIME2 NOT NULL,
    VerifiedOn DATETIME2 NULL,
    IntegratedOn DATETIME2 NULL
)
end if not exists (
    select distinct 1
    from information_schema.columns
    where table_name = 'Migration'
) begin CREATE TABLE [dbo].[Migration](
    [Id] [int] IDENTITY(1, 1) NOT NULL,
    [ClientVersion] [nvarchar](200) NULL,
    [DatabaseVersion] [nvarchar](200) NULL,
    [CreatedOn] [datetime] NULL,
    CONSTRAINT [PK_Migrations] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
insert [Migration] ([ClientVersion], [DatabaseVersion], [CreatedOn])
values ('1.0.0', '1.0.0', getdate())
end
go if not exists (
        select distinct 1
        from information_schema.columns
        where table_name = 'Log'
    ) begin CREATE TABLE [dbo].[Log](
        [Id] [int] IDENTITY(1, 1) NOT NULL,
        [Origin] [nvarchar](2000) NULL,
        [Message] [nvarchar](2000) NULL,
        [LogLevel] [nvarchar](2000) NULL,
        [CreatedOn] [datetime] NULL,
        [Exception] [nvarchar](4000) NULL,
        [Trace] [nvarchar](4000) NULL,
        CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC)
    ) ON [PRIMARY]
end

if not exists (
    select distinct 1
    from information_schema.columns
    where table_name = 'ArchivedAccount'
) begin 
CREATE TABLE ArchivedAccount (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
	InitialId INT NOT NULL UNIQUE,
    UserId NVARCHAR(512) NOT NULL,
    EmailDomain NVARCHAR(512),
    CreatedOn DATETIME2 NOT NULL,
    VerifiedOn DATETIME2 NULL,
    IntegratedOn DATETIME2 NULL,
    DeletedOn DATETIME2 NOT NULL
)
end

/** ADD AccountType and AccountRole */

SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRAN;

    IF NOT EXISTS (SELECT 1 FROM [Migration] WHERE DatabaseVersion = '1.1.0')
    BEGIN
        
        IF COL_LENGTH('dbo.Account', 'AccountType') IS NULL
        BEGIN
            ALTER TABLE dbo.Account
            ADD AccountType NVARCHAR(32) NOT NULL
                CONSTRAINT DF_Account_AccountType DEFAULT ('guest')
                WITH VALUES; 
        END;        

        IF COL_LENGTH('dbo.Account', 'AccountRole') IS NULL
        BEGIN
            ALTER TABLE dbo.Account
            ADD AccountRole NVARCHAR(32) NOT NULL
                CONSTRAINT DF_Account_AccountRole DEFAULT ('student')
                WITH VALUES; 
        END;
        

		IF COL_LENGTH('dbo.ArchivedAccount', 'ArchivedAccountType') IS NULL
        BEGIN
            ALTER TABLE dbo.ArchivedAccount
            ADD ArchivedAccountType NVARCHAR(32) NOT NULL
                CONSTRAINT DF_ArchivedAccount_ArchivedAccountType DEFAULT ('guest')
                WITH VALUES; 
        END;

        IF COL_LENGTH('dbo.ArchivedAccount', 'ArchivedAccountRole') IS NULL
        BEGIN
            ALTER TABLE dbo.ArchivedAccount
            ADD ArchivedAccountRole NVARCHAR(32) NOT NULL
                CONSTRAINT DF_ArchivedAccount_ArchivedAccountRole DEFAULT ('student')
                WITH VALUES; 
        END;

        INSERT INTO [Migration] ([ClientVersion], [DatabaseVersion], [CreatedOn])
        VALUES ('1.1.0', '1.1.0', SYSDATETIME());
    END;

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRAN;
    THROW;
END CATCH;

/** ADD WhiteListedEmailDomain */

SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRAN;

    IF NOT EXISTS (SELECT 1 FROM [Migration] WHERE DatabaseVersion = '1.2.0')
    BEGIN

        IF NOT EXISTS (
            SELECT distinct 1
            FROM information_schema.columns
            WHERE table_name = 'WhiteListedEmailDomain'
        )
        BEGIN
            CREATE TABLE WhiteListedEmailDomain (
                Id INT IDENTITY(1, 1) PRIMARY KEY,
                Domain NVARCHAR(256) NOT NULL,
                CreatedOn DATETIME2 NOT NULL
            );
        END;

        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'svensktnaringsliv.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('svensktnaringsliv.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'stockholmshandelskammare.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('stockholmshandelskammare.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'lo.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('lo.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'tco.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('tco.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'migrationsverket.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('migrationsverket.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'skatteverket.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('skatteverket.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'ekobrottsmyndigheten.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('ekobrottsmyndigheten.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'arbetsformedlingen.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('arbetsformedlingen.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'kronofogdemyndigheten.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('kronofogdemyndigheten.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'forsakringskassan.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('forsakringskassan.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'tullverket.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('tullverket.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'polisen.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('polisen.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = 'kronofogden.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('kronofogden.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = '@uhmynd.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('@uhmynd.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = '@slv.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('@slv.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = '@av.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('@av.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = '@bolagsverket.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('@bolagsverket.se', SYSDATETIME());
        IF NOT EXISTS (SELECT 1 FROM WhiteListedEmailDomain WHERE Domain = '@pensionsmyndigheten.se')
            INSERT INTO WhiteListedEmailDomain (Domain, CreatedOn) VALUES ('@pensionsmyndigheten.se', SYSDATETIME());

        INSERT INTO [Migration] ([ClientVersion], [DatabaseVersion], [CreatedOn])
        VALUES ('1.2.0', '1.2.0', SYSDATETIME());
    END;

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRAN;
    THROW;
END CATCH;