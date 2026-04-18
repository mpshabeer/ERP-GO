CREATE TABLE [dbo].[AccountHeads] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (MAX) NOT NULL,
    [AccountGroupId] INT            NOT NULL,
    [IsActive]       BIT            NOT NULL,
    CONSTRAINT [PK_AccountHeads] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AccountHeads_AccountGroups_AccountGroupId] FOREIGN KEY ([AccountGroupId]) REFERENCES [dbo].[AccountGroups] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AccountHeads_AccountGroupId]
    ON [dbo].[AccountHeads]([AccountGroupId] ASC);

