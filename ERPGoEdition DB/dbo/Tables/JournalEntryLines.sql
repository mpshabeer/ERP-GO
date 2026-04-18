CREATE TABLE [dbo].[JournalEntryLines] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [JournalEntryId] INT             NOT NULL,
    [AccountId]      INT             NOT NULL,
    [Debit]          DECIMAL (18, 2) NOT NULL,
    [Credit]         DECIMAL (18, 2) NOT NULL,
    [Description]    NVARCHAR (MAX)  NOT NULL,
    CONSTRAINT [PK_JournalEntryLines] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_JournalEntryLines_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_JournalEntryLines_JournalEntries_JournalEntryId] FOREIGN KEY ([JournalEntryId]) REFERENCES [dbo].[JournalEntries] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_JournalEntryLines_AccountId]
    ON [dbo].[JournalEntryLines]([AccountId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_JournalEntryLines_JournalEntryId]
    ON [dbo].[JournalEntryLines]([JournalEntryId] ASC);

