if not exists (
    select distinct 1
    from information_schema.columns
    where table_name = 'Account'
) begin CREATE TABLE Account (
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    login_attribute NVARCHAR(512) NOT NULL,
    given_name NVARCHAR(512),
    surname NVARCHAR(512),
    display_name NVARCHAR(512),
    email NVARCHAR(512),
    assurance_level NVARCHAR(512),
    mail_verified BIT DEFAULT 0,
    create_date_time DATETIME2 NOT NULL,
    verification_date_time DATETIME2 NULL,
    integration_date_time DATETIME2 NULL
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