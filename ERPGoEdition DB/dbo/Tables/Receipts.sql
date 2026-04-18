CREATE TABLE [dbo].[Receipts] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [ReceiptNo]         NVARCHAR (MAX)  NOT NULL,
    [Date]              DATETIME2 (7)   NOT NULL,
    [Amount]            DECIMAL (18, 2) NOT NULL,
    [Narration]         NVARCHAR (MAX)  NOT NULL,
    [CashBankAccountId] INT             NOT NULL,
    [PartyAccountId]    INT             NOT NULL,
    CONSTRAINT [PK_Receipts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Receipts_Accounts_CashBankAccountId] FOREIGN KEY ([CashBankAccountId]) REFERENCES [dbo].[Accounts] ([Id]),
    CONSTRAINT [FK_Receipts_Accounts_PartyAccountId] FOREIGN KEY ([PartyAccountId]) REFERENCES [dbo].[Accounts] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Receipts_CashBankAccountId]
    ON [dbo].[Receipts]([CashBankAccountId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Receipts_PartyAccountId]
    ON [dbo].[Receipts]([PartyAccountId] ASC);

