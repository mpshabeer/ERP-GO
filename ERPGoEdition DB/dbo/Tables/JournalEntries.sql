CREATE TABLE [dbo].[JournalEntries] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [VoucherDate] DATETIME2 (7)  NOT NULL,
    [VoucherNo]   NVARCHAR (MAX) NOT NULL,
    [VoucherType] NVARCHAR (MAX) NOT NULL,
    [ReferenceId] INT            NULL,
    [Narration]   NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_JournalEntries] PRIMARY KEY CLUSTERED ([Id] ASC)
);

