CREATE TABLE [dbo].[AccountGroups] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (MAX) NOT NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_AccountGroups] PRIMARY KEY CLUSTERED ([Id] ASC)
);

