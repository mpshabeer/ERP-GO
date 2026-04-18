CREATE TABLE [dbo].[Payments] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [PaymentNo]         NVARCHAR (MAX)  NOT NULL,
    [Date]              DATETIME2 (7)   NOT NULL,
    [Amount]            DECIMAL (18, 2) NOT NULL,
    [Narration]         NVARCHAR (MAX)  NOT NULL,
    [CashBankAccountId] INT             NOT NULL,
    [PartyAccountId]    INT             NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Payments_Accounts_CashBankAccountId] FOREIGN KEY ([CashBankAccountId]) REFERENCES [dbo].[Accounts] ([Id]),
    CONSTRAINT [FK_Payments_Accounts_PartyAccountId] FOREIGN KEY ([PartyAccountId]) REFERENCES [dbo].[Accounts] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Payments_CashBankAccountId]
    ON [dbo].[Payments]([CashBankAccountId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Payments_PartyAccountId]
    ON [dbo].[Payments]([PartyAccountId] ASC);

