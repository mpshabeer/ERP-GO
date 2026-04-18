CREATE TABLE [dbo].[Accounts] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (MAX)  NOT NULL,
    [Code]           NVARCHAR (MAX)  NOT NULL,
    [AccountHeadId]  INT             NOT NULL,
    [OpeningBalance] DECIMAL (18, 2) NOT NULL,
    [IsDefault]      BIT             NOT NULL,
    [IsActive]       BIT             NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Accounts_AccountHeads_AccountHeadId] FOREIGN KEY ([AccountHeadId]) REFERENCES [dbo].[AccountHeads] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Accounts_AccountHeadId]
    ON [dbo].[Accounts]([AccountHeadId] ASC);

