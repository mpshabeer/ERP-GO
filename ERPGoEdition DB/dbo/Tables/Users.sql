CREATE TABLE [dbo].[Users] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Username]     NVARCHAR (100) NOT NULL,
    [Email]        NVARCHAR (255) NOT NULL,
    [FullName]     NVARCHAR (255) NOT NULL,
    [PasswordHash] NVARCHAR (MAX) NOT NULL,
    [Role]         NVARCHAR (MAX) NOT NULL,
    [IsActive]     BIT            NOT NULL,
    [CreatedAt]    DATETIME2 (7)  NOT NULL,
    [LastLoginAt]  DATETIME2 (7)  NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

