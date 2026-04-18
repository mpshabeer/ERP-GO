CREATE TABLE [dbo].[Expenses] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [ExpenseNo]         NVARCHAR (MAX)  NOT NULL,
    [Date]              DATETIME2 (7)   NOT NULL,
    [Amount]            DECIMAL (18, 2) NOT NULL,
    [Narration]         NVARCHAR (MAX)  NOT NULL,
    [CashBankAccountId] INT             NOT NULL,
    [ExpenseAccountId]  INT             NOT NULL,
    CONSTRAINT [PK_Expenses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Expenses_Accounts_CashBankAccountId] FOREIGN KEY ([CashBankAccountId]) REFERENCES [dbo].[Accounts] ([Id]),
    CONSTRAINT [FK_Expenses_Accounts_ExpenseAccountId] FOREIGN KEY ([ExpenseAccountId]) REFERENCES [dbo].[Accounts] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Expenses_CashBankAccountId]
    ON [dbo].[Expenses]([CashBankAccountId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Expenses_ExpenseAccountId]
    ON [dbo].[Expenses]([ExpenseAccountId] ASC);

