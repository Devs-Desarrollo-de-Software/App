IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpAuditLogExcelFiles] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [FileName] nvarchar(256) NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpAuditLogExcelFiles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpAuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationName] nvarchar(96) NULL,
        [UserId] uniqueidentifier NULL,
        [UserName] nvarchar(256) NULL,
        [TenantId] uniqueidentifier NULL,
        [TenantName] nvarchar(64) NULL,
        [ImpersonatorUserId] uniqueidentifier NULL,
        [ImpersonatorUserName] nvarchar(256) NULL,
        [ImpersonatorTenantId] uniqueidentifier NULL,
        [ImpersonatorTenantName] nvarchar(64) NULL,
        [ExecutionTime] datetime2 NOT NULL,
        [ExecutionDuration] int NOT NULL,
        [ClientIpAddress] nvarchar(64) NULL,
        [ClientName] nvarchar(128) NULL,
        [ClientId] nvarchar(64) NULL,
        [CorrelationId] nvarchar(64) NULL,
        [BrowserInfo] nvarchar(512) NULL,
        [HttpMethod] nvarchar(16) NULL,
        [Url] nvarchar(256) NULL,
        [Exceptions] nvarchar(max) NULL,
        [Comments] nvarchar(256) NULL,
        [HttpStatusCode] int NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpAuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpBackgroundJobs] (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationName] nvarchar(96) NULL,
        [JobName] nvarchar(128) NOT NULL,
        [JobArgs] nvarchar(max) NOT NULL,
        [TryCount] smallint NOT NULL DEFAULT CAST(0 AS smallint),
        [CreationTime] datetime2 NOT NULL,
        [NextTryTime] datetime2 NOT NULL,
        [LastTryTime] datetime2 NULL,
        [IsAbandoned] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Priority] tinyint NOT NULL DEFAULT CAST(15 AS tinyint),
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpBackgroundJobs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpBlobContainers] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Name] nvarchar(128) NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpBlobContainers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpClaimTypes] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(256) NOT NULL,
        [Required] bit NOT NULL,
        [IsStatic] bit NOT NULL,
        [Regex] nvarchar(512) NULL,
        [RegexDescription] nvarchar(128) NULL,
        [Description] nvarchar(256) NULL,
        [ValueType] int NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpClaimTypes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpFeatureGroups] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpFeatureGroups] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpFeatures] (
        [Id] uniqueidentifier NOT NULL,
        [GroupName] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [ParentName] nvarchar(128) NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [Description] nvarchar(256) NULL,
        [DefaultValue] nvarchar(256) NULL,
        [IsVisibleToClients] bit NOT NULL,
        [IsAvailableToHost] bit NOT NULL,
        [AllowedProviders] nvarchar(256) NULL,
        [ValueType] nvarchar(2048) NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpFeatures] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpFeatureValues] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(128) NOT NULL,
        [ProviderName] nvarchar(64) NULL,
        [ProviderKey] nvarchar(64) NULL,
        CONSTRAINT [PK_AbpFeatureValues] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpLinkUsers] (
        [Id] uniqueidentifier NOT NULL,
        [SourceUserId] uniqueidentifier NOT NULL,
        [SourceTenantId] uniqueidentifier NULL,
        [TargetUserId] uniqueidentifier NOT NULL,
        [TargetTenantId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpLinkUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpOrganizationUnits] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ParentId] uniqueidentifier NULL,
        [Code] nvarchar(95) NOT NULL,
        [DisplayName] nvarchar(128) NOT NULL,
        [EntityVersion] int NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_AbpOrganizationUnits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [AbpOrganizationUnits] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpPermissionGrants] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Name] nvarchar(128) NOT NULL,
        [ProviderName] nvarchar(64) NOT NULL,
        [ProviderKey] nvarchar(64) NOT NULL,
        CONSTRAINT [PK_AbpPermissionGrants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpPermissionGroups] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpPermissionGroups] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpPermissions] (
        [Id] uniqueidentifier NOT NULL,
        [GroupName] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [ParentName] nvarchar(128) NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [IsEnabled] bit NOT NULL,
        [MultiTenancySide] tinyint NOT NULL,
        [Providers] nvarchar(128) NULL,
        [StateCheckers] nvarchar(256) NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpPermissions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpRoles] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Name] nvarchar(256) NOT NULL,
        [NormalizedName] nvarchar(256) NOT NULL,
        [IsDefault] bit NOT NULL,
        [IsStatic] bit NOT NULL,
        [IsPublic] bit NOT NULL,
        [EntityVersion] int NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpSecurityLogs] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ApplicationName] nvarchar(96) NULL,
        [Identity] nvarchar(96) NULL,
        [Action] nvarchar(96) NULL,
        [UserId] uniqueidentifier NULL,
        [UserName] nvarchar(256) NULL,
        [TenantName] nvarchar(64) NULL,
        [ClientId] nvarchar(64) NULL,
        [CorrelationId] nvarchar(64) NULL,
        [ClientIpAddress] nvarchar(64) NULL,
        [BrowserInfo] nvarchar(512) NULL,
        [CreationTime] datetime2 NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpSecurityLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpSessions] (
        [Id] uniqueidentifier NOT NULL,
        [SessionId] nvarchar(128) NOT NULL,
        [Device] nvarchar(64) NOT NULL,
        [DeviceInfo] nvarchar(64) NULL,
        [TenantId] uniqueidentifier NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ClientId] nvarchar(64) NULL,
        [IpAddresses] nvarchar(2048) NULL,
        [SignedIn] datetime2 NOT NULL,
        [LastAccessed] datetime2 NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpSessions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpSettingDefinitions] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [DisplayName] nvarchar(256) NOT NULL,
        [Description] nvarchar(512) NULL,
        [DefaultValue] nvarchar(2048) NULL,
        [IsVisibleToClients] bit NOT NULL,
        [Providers] nvarchar(1024) NULL,
        [IsInherited] bit NOT NULL,
        [IsEncrypted] bit NOT NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpSettingDefinitions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpSettings] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(2048) NOT NULL,
        [ProviderName] nvarchar(64) NULL,
        [ProviderKey] nvarchar(64) NULL,
        CONSTRAINT [PK_AbpSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpUserDelegations] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [SourceUserId] uniqueidentifier NOT NULL,
        [TargetUserId] uniqueidentifier NOT NULL,
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NOT NULL,
        CONSTRAINT [PK_AbpUserDelegations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpUsers] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [UserName] nvarchar(256) NOT NULL,
        [NormalizedUserName] nvarchar(256) NOT NULL,
        [Name] nvarchar(64) NULL,
        [Surname] nvarchar(64) NULL,
        [Email] nvarchar(256) NOT NULL,
        [NormalizedEmail] nvarchar(256) NOT NULL,
        [EmailConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PasswordHash] nvarchar(256) NULL,
        [SecurityStamp] nvarchar(256) NOT NULL,
        [IsExternal] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PhoneNumber] nvarchar(16) NULL,
        [PhoneNumberConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit),
        [IsActive] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL DEFAULT CAST(0 AS bit),
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL DEFAULT CAST(0 AS bit),
        [AccessFailedCount] int NOT NULL DEFAULT 0,
        [ShouldChangePasswordOnNextLogin] bit NOT NULL,
        [EntityVersion] int NOT NULL,
        [LastPasswordChangeTime] datetimeoffset NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_AbpUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [OpenIddictApplications] (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationType] nvarchar(50) NULL,
        [ClientId] nvarchar(100) NULL,
        [ClientSecret] nvarchar(max) NULL,
        [ClientType] nvarchar(50) NULL,
        [ConsentType] nvarchar(50) NULL,
        [DisplayName] nvarchar(max) NULL,
        [DisplayNames] nvarchar(max) NULL,
        [JsonWebKeySet] nvarchar(max) NULL,
        [Permissions] nvarchar(max) NULL,
        [PostLogoutRedirectUris] nvarchar(max) NULL,
        [Properties] nvarchar(max) NULL,
        [RedirectUris] nvarchar(max) NULL,
        [Requirements] nvarchar(max) NULL,
        [Settings] nvarchar(max) NULL,
        [ClientUri] nvarchar(max) NULL,
        [LogoUri] nvarchar(max) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_OpenIddictApplications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [OpenIddictScopes] (
        [Id] uniqueidentifier NOT NULL,
        [Description] nvarchar(max) NULL,
        [Descriptions] nvarchar(max) NULL,
        [DisplayName] nvarchar(max) NULL,
        [DisplayNames] nvarchar(max) NULL,
        [Name] nvarchar(200) NULL,
        [Properties] nvarchar(max) NULL,
        [Resources] nvarchar(max) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_OpenIddictScopes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpAuditLogActions] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [AuditLogId] uniqueidentifier NOT NULL,
        [ServiceName] nvarchar(256) NULL,
        [MethodName] nvarchar(128) NULL,
        [Parameters] nvarchar(2000) NULL,
        [ExecutionTime] datetime2 NOT NULL,
        [ExecutionDuration] int NOT NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpAuditLogActions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpAuditLogActions_AbpAuditLogs_AuditLogId] FOREIGN KEY ([AuditLogId]) REFERENCES [AbpAuditLogs] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpEntityChanges] (
        [Id] uniqueidentifier NOT NULL,
        [AuditLogId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ChangeTime] datetime2 NOT NULL,
        [ChangeType] tinyint NOT NULL,
        [EntityTenantId] uniqueidentifier NULL,
        [EntityId] nvarchar(128) NULL,
        [EntityTypeFullName] nvarchar(128) NOT NULL,
        [ExtraProperties] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpEntityChanges] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpEntityChanges_AbpAuditLogs_AuditLogId] FOREIGN KEY ([AuditLogId]) REFERENCES [AbpAuditLogs] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpBlobs] (
        [Id] uniqueidentifier NOT NULL,
        [ContainerId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Name] nvarchar(256) NOT NULL,
        [Content] varbinary(max) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_AbpBlobs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpBlobs_AbpBlobContainers_ContainerId] FOREIGN KEY ([ContainerId]) REFERENCES [AbpBlobContainers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpOrganizationUnitRoles] (
        [RoleId] uniqueidentifier NOT NULL,
        [OrganizationUnitId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpOrganizationUnitRoles] PRIMARY KEY ([OrganizationUnitId], [RoleId]),
        CONSTRAINT [FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationUnitId] FOREIGN KEY ([OrganizationUnitId]) REFERENCES [AbpOrganizationUnits] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AbpOrganizationUnitRoles_AbpRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AbpRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpRoleClaims] (
        [Id] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ClaimType] nvarchar(256) NOT NULL,
        [ClaimValue] nvarchar(1024) NULL,
        CONSTRAINT [PK_AbpRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpRoleClaims_AbpRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AbpRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpUserClaims] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ClaimType] nvarchar(256) NOT NULL,
        [ClaimValue] nvarchar(1024) NULL,
        CONSTRAINT [PK_AbpUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpUserClaims_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpUserLogins] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(64) NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [ProviderKey] nvarchar(196) NOT NULL,
        [ProviderDisplayName] nvarchar(128) NULL,
        CONSTRAINT [PK_AbpUserLogins] PRIMARY KEY ([UserId], [LoginProvider]),
        CONSTRAINT [FK_AbpUserLogins_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpUserOrganizationUnits] (
        [UserId] uniqueidentifier NOT NULL,
        [OrganizationUnitId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpUserOrganizationUnits] PRIMARY KEY ([OrganizationUnitId], [UserId]),
        CONSTRAINT [FK_AbpUserOrganizationUnits_AbpOrganizationUnits_OrganizationUnitId] FOREIGN KEY ([OrganizationUnitId]) REFERENCES [AbpOrganizationUnits] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AbpUserOrganizationUnits_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpUserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        CONSTRAINT [PK_AbpUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AbpUserRoles_AbpRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AbpRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AbpUserRoles_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpUserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(64) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AbpUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AbpUserTokens_AbpUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [OpenIddictAuthorizations] (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NULL,
        [CreationDate] datetime2 NULL,
        [Properties] nvarchar(max) NULL,
        [Scopes] nvarchar(max) NULL,
        [Status] nvarchar(50) NULL,
        [Subject] nvarchar(400) NULL,
        [Type] nvarchar(50) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_OpenIddictAuthorizations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [OpenIddictApplications] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [AbpEntityPropertyChanges] (
        [Id] uniqueidentifier NOT NULL,
        [TenantId] uniqueidentifier NULL,
        [EntityChangeId] uniqueidentifier NOT NULL,
        [NewValue] nvarchar(512) NULL,
        [OriginalValue] nvarchar(512) NULL,
        [PropertyName] nvarchar(128) NOT NULL,
        [PropertyTypeFullName] nvarchar(64) NOT NULL,
        CONSTRAINT [PK_AbpEntityPropertyChanges] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId] FOREIGN KEY ([EntityChangeId]) REFERENCES [AbpEntityChanges] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE TABLE [OpenIddictTokens] (
        [Id] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NULL,
        [AuthorizationId] uniqueidentifier NULL,
        [CreationDate] datetime2 NULL,
        [ExpirationDate] datetime2 NULL,
        [Payload] nvarchar(max) NULL,
        [Properties] nvarchar(max) NULL,
        [RedemptionDate] datetime2 NULL,
        [ReferenceId] nvarchar(100) NULL,
        [Status] nvarchar(50) NULL,
        [Subject] nvarchar(400) NULL,
        [Type] nvarchar(50) NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        CONSTRAINT [PK_OpenIddictTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OpenIddictTokens_OpenIddictApplications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [OpenIddictApplications] ([Id]),
        CONSTRAINT [FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId] FOREIGN KEY ([AuthorizationId]) REFERENCES [OpenIddictAuthorizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpAuditLogActions_AuditLogId] ON [AbpAuditLogActions] ([AuditLogId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpAuditLogActions_TenantId_ServiceName_MethodName_ExecutionTime] ON [AbpAuditLogActions] ([TenantId], [ServiceName], [MethodName], [ExecutionTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpAuditLogs_TenantId_ExecutionTime] ON [AbpAuditLogs] ([TenantId], [ExecutionTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpAuditLogs_TenantId_UserId_ExecutionTime] ON [AbpAuditLogs] ([TenantId], [UserId], [ExecutionTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpBackgroundJobs_IsAbandoned_NextTryTime] ON [AbpBackgroundJobs] ([IsAbandoned], [NextTryTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpBlobContainers_TenantId_Name] ON [AbpBlobContainers] ([TenantId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpBlobs_ContainerId] ON [AbpBlobs] ([ContainerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpBlobs_TenantId_ContainerId_Name] ON [AbpBlobs] ([TenantId], [ContainerId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpEntityChanges_AuditLogId] ON [AbpEntityChanges] ([AuditLogId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpEntityChanges_TenantId_EntityTypeFullName_EntityId] ON [AbpEntityChanges] ([TenantId], [EntityTypeFullName], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpEntityPropertyChanges_EntityChangeId] ON [AbpEntityPropertyChanges] ([EntityChangeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpFeatureGroups_Name] ON [AbpFeatureGroups] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpFeatures_GroupName] ON [AbpFeatures] ([GroupName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpFeatures_Name] ON [AbpFeatures] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpFeatureValues_Name_ProviderName_ProviderKey] ON [AbpFeatureValues] ([Name], [ProviderName], [ProviderKey]) WHERE [ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_TargetTenantId] ON [AbpLinkUsers] ([SourceUserId], [SourceTenantId], [TargetUserId], [TargetTenantId]) WHERE [SourceTenantId] IS NOT NULL AND [TargetTenantId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId] ON [AbpOrganizationUnitRoles] ([RoleId], [OrganizationUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpOrganizationUnits_Code] ON [AbpOrganizationUnits] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpOrganizationUnits_ParentId] ON [AbpOrganizationUnits] ([ParentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpPermissionGrants_TenantId_Name_ProviderName_ProviderKey] ON [AbpPermissionGrants] ([TenantId], [Name], [ProviderName], [ProviderKey]) WHERE [TenantId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpPermissionGroups_Name] ON [AbpPermissionGroups] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpPermissions_GroupName] ON [AbpPermissions] ([GroupName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpPermissions_Name] ON [AbpPermissions] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpRoleClaims_RoleId] ON [AbpRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpRoles_NormalizedName] ON [AbpRoles] ([NormalizedName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_Action] ON [AbpSecurityLogs] ([TenantId], [Action]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_ApplicationName] ON [AbpSecurityLogs] ([TenantId], [ApplicationName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_Identity] ON [AbpSecurityLogs] ([TenantId], [Identity]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSecurityLogs_TenantId_UserId] ON [AbpSecurityLogs] ([TenantId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSessions_Device] ON [AbpSessions] ([Device]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSessions_SessionId] ON [AbpSessions] ([SessionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpSessions_TenantId_UserId] ON [AbpSessions] ([TenantId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AbpSettingDefinitions_Name] ON [AbpSettingDefinitions] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AbpSettings_Name_ProviderName_ProviderKey] ON [AbpSettings] ([Name], [ProviderName], [ProviderKey]) WHERE [ProviderName] IS NOT NULL AND [ProviderKey] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserClaims_UserId] ON [AbpUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserLogins_LoginProvider_ProviderKey] ON [AbpUserLogins] ([LoginProvider], [ProviderKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserOrganizationUnits_UserId_OrganizationUnitId] ON [AbpUserOrganizationUnits] ([UserId], [OrganizationUnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUserRoles_RoleId_UserId] ON [AbpUserRoles] ([RoleId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUsers_Email] ON [AbpUsers] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUsers_NormalizedEmail] ON [AbpUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUsers_NormalizedUserName] ON [AbpUsers] ([NormalizedUserName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_AbpUsers_UserName] ON [AbpUsers] ([UserName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_OpenIddictApplications_ClientId] ON [OpenIddictApplications] ([ClientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type] ON [OpenIddictAuthorizations] ([ApplicationId], [Status], [Subject], [Type]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_OpenIddictScopes_Name] ON [OpenIddictScopes] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_OpenIddictTokens_ApplicationId_Status_Subject_Type] ON [OpenIddictTokens] ([ApplicationId], [Status], [Subject], [Type]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_OpenIddictTokens_AuthorizationId] ON [OpenIddictTokens] ([AuthorizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    CREATE INDEX [IX_OpenIddictTokens_ReferenceId] ON [OpenIddictTokens] ([ReferenceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250912234400_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250912234400_Initial', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250920001835_Added_Destinos'
)
BEGIN
    ALTER TABLE [AppDestinos] ADD [FechaUltimaActualizacion] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250920001835_Added_Destinos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250920001835_Added_Destinos', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250927001338_RemoveFechaUltimaActualizacionFromDestino'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppDestinos]') AND [c].[name] = N'FechaUltimaActualizacion');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [AppDestinos] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [AppDestinos] DROP COLUMN [FechaUltimaActualizacion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250927001338_RemoveFechaUltimaActualizacionFromDestino'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250927001338_RemoveFechaUltimaActualizacionFromDestino', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251002015701_AddedCoordenadaToDestino'
)
BEGIN
    ALTER TABLE [AppDestinos] ADD [Latitud] float NOT NULL DEFAULT 0.0E0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251002015701_AddedCoordenadaToDestino'
)
BEGIN
    ALTER TABLE [AppDestinos] ADD [Longitud] float NOT NULL DEFAULT 0.0E0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251002015701_AddedCoordenadaToDestino'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251002015701_AddedCoordenadaToDestino', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251022234652_Added_Calificacion'
)
BEGIN
    CREATE TABLE [AppCalificaciones] (
        [Id] uniqueidentifier NOT NULL,
        [Puntuacion] int NOT NULL,
        [Comentario] nvarchar(1000) NULL,
        [DestinoId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        CONSTRAINT [PK_AppCalificaciones] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AppCalificaciones_AppDestinos_DestinoId] FOREIGN KEY ([DestinoId]) REFERENCES [AppDestinos] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251022234652_Added_Calificacion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AppCalificaciones_DestinoId_UserId] ON [AppCalificaciones] ([DestinoId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251022234652_Added_Calificacion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251022234652_Added_Calificacion', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251025024404_RemoveForeignKeyDestinoCalificacion'
)
BEGIN
    ALTER TABLE [AppCalificaciones] DROP CONSTRAINT [FK_AppCalificaciones_AppDestinos_DestinoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251025024404_RemoveForeignKeyDestinoCalificacion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251025024404_RemoveForeignKeyDestinoCalificacion', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251026030645_AddUserFilterToCalificacion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251026030645_AddUserFilterToCalificacion', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251205181501_Add_Experiencias'
)
BEGIN
    CREATE TABLE [AppExperiencias] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [DestinoId] uniqueidentifier NOT NULL,
        [Titulo] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [Valoracion] int NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        CONSTRAINT [PK_AppExperiencias] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251205181501_Add_Experiencias'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251205181501_Add_Experiencias', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260107234418_Add_Usuarios'
)
BEGIN
    CREATE TABLE [AppUsuarios] (
        [Id] uniqueidentifier NOT NULL,
        [NombreCompleto] nvarchar(100) NOT NULL,
        [NombreUsuario] nvarchar(50) NOT NULL,
        [IdentityUserId] uniqueidentifier NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [FotoPerfilUrl] nvarchar(500) NULL,
        [RecibirEnPantalla] bit NOT NULL DEFAULT CAST(1 AS bit),
        [RecibirPorEmail] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Frecuencia] int NOT NULL DEFAULT 0,
        [Rol] int NOT NULL,
        [EstaActivo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_AppUsuarios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AppUsuarios_AbpUsers_IdentityUserId] FOREIGN KEY ([IdentityUserId]) REFERENCES [AbpUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260107234418_Add_Usuarios'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Email] ON [AppUsuarios] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260107234418_Add_Usuarios'
)
BEGIN
    CREATE INDEX [IX_Usuarios_EstaActivo] ON [AppUsuarios] ([EstaActivo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260107234418_Add_Usuarios'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_IdentityUserId] ON [AppUsuarios] ([IdentityUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260107234418_Add_Usuarios'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_NombreUsuario] ON [AppUsuarios] ([NombreUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260107234418_Add_Usuarios'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260107234418_Add_Usuarios', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260109221344_Add_Favoritos'
)
BEGIN
    CREATE TABLE [AppFavoritos] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [DestinoId] uniqueidentifier NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        CONSTRAINT [PK_AppFavoritos] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260109221344_Add_Favoritos'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AppFavoritos_UserId_DestinoId] ON [AppFavoritos] ([UserId], [DestinoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260109221344_Add_Favoritos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260109221344_Add_Favoritos', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    CREATE TABLE [AppNotificaciones] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [DestinoId] uniqueidentifier NOT NULL,
        [Titulo] nvarchar(200) NOT NULL,
        [Mensaje] nvarchar(1000) NOT NULL,
        [Tipo] int NOT NULL,
        [Leida] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaLectura] datetime2 NULL,
        [EnviadaPorMail] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaEnvioMail] datetime2 NULL,
        [NombreDestino] nvarchar(200) NOT NULL,
        [ExtraProperties] nvarchar(max) NOT NULL,
        [ConcurrencyStamp] nvarchar(40) NOT NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        [LastModificationTime] datetime2 NULL,
        [LastModifierId] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DeleterId] uniqueidentifier NULL,
        [DeletionTime] datetime2 NULL,
        CONSTRAINT [PK_AppNotificaciones] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    CREATE INDEX [IX_AppFavoritos_DestinoId] ON [AppFavoritos] ([DestinoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_CreationTime] ON [AppNotificaciones] ([CreationTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    EXEC(N'CREATE INDEX [IX_Notificaciones_EnviadaPorMail] ON [AppNotificaciones] ([EnviadaPorMail]) WHERE [EnviadaPorMail] = 0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_UserId] ON [AppNotificaciones] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_UserId_DestinoId] ON [AppNotificaciones] ([UserId], [DestinoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_UserId_Leida] ON [AppNotificaciones] ([UserId], [Leida]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    ALTER TABLE [AppFavoritos] ADD CONSTRAINT [FK_AppFavoritos_AppDestinos_DestinoId] FOREIGN KEY ([DestinoId]) REFERENCES [AppDestinos] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110141803_Add_Notificaciones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260110141803_Add_Notificaciones', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260117143747_Sync_ApiCityId_Column'
)
BEGIN
    ALTER TABLE [AppDestinos] ADD [ApiCityId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260117143747_Sync_ApiCityId_Column'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260117143747_Sync_ApiCityId_Column', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000227_IncreasePhotoUrlLength'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppUsuarios]') AND [c].[name] = N'FotoPerfilUrl');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AppUsuarios] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [AppUsuarios] ALTER COLUMN [FotoPerfilUrl] nvarchar(MAX) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122000227_IncreasePhotoUrlLength'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260122000227_IncreasePhotoUrlLength', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122023431_AddMetricasApiExternaTable'
)
BEGIN
    CREATE TABLE [AppMetricasApiExterna] (
        [Id] uniqueidentifier NOT NULL,
        [NombreApi] nvarchar(50) NOT NULL,
        [Endpoint] nvarchar(500) NOT NULL,
        [MetodoHttp] nvarchar(10) NOT NULL,
        [ParametrosConsulta] nvarchar(2000) NOT NULL,
        [CodigoEstadoHttp] int NOT NULL,
        [TiempoRespuestaMs] bigint NOT NULL,
        [Exitosa] bit NOT NULL,
        [MensajeError] nvarchar(1000) NOT NULL,
        [CantidadResultados] int NULL,
        [CreationTime] datetime2 NOT NULL,
        [CreatorId] uniqueidentifier NULL,
        CONSTRAINT [PK_AppMetricasApiExterna] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122023431_AddMetricasApiExternaTable'
)
BEGIN
    CREATE INDEX [IX_MetricasApiExterna_CreationTime] ON [AppMetricasApiExterna] ([CreationTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122023431_AddMetricasApiExternaTable'
)
BEGIN
    CREATE INDEX [IX_MetricasApiExterna_Exitosa] ON [AppMetricasApiExterna] ([Exitosa]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122023431_AddMetricasApiExternaTable'
)
BEGIN
    CREATE INDEX [IX_MetricasApiExterna_NombreApi] ON [AppMetricasApiExterna] ([NombreApi]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122023431_AddMetricasApiExternaTable'
)
BEGIN
    CREATE INDEX [IX_MetricasApiExterna_NombreApi_CreationTime] ON [AppMetricasApiExterna] ([NombreApi], [CreationTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260122023431_AddMetricasApiExternaTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260122023431_AddMetricasApiExternaTable', N'9.0.9');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126141743_RemoveEventosTable'
)
BEGIN
    DROP TABLE [AppEventos];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126141743_RemoveEventosTable'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppUsuarios]') AND [c].[name] = N'FotoPerfilUrl');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AppUsuarios] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [AppUsuarios] ALTER COLUMN [FotoPerfilUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126141743_RemoveEventosTable'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppMetricasApiExterna]') AND [c].[name] = N'ParametrosConsulta');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [AppMetricasApiExterna] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [AppMetricasApiExterna] ALTER COLUMN [ParametrosConsulta] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126141743_RemoveEventosTable'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppMetricasApiExterna]') AND [c].[name] = N'MensajeError');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AppMetricasApiExterna] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [AppMetricasApiExterna] ALTER COLUMN [MensajeError] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126141743_RemoveEventosTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260126141743_RemoveEventosTable', N'9.0.9');
END;

COMMIT;
GO

