CREATE TABLE [dbo].[Suppliers] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX) NOT NULL,
    [Code]          NVARCHAR (MAX) NOT NULL,
    [Address]       NVARCHAR (MAX) NOT NULL,
    [City]          NVARCHAR (MAX) NOT NULL,
    [Country]       NVARCHAR (MAX) NOT NULL,
    [Mobile]        NVARCHAR (MAX) NOT NULL,
    [Email]         NVARCHAR (MAX) NOT NULL,
    [TaxNumber]     NVARCHAR (MAX) NOT NULL,
    [ContactPerson] NVARCHAR (MAX) NOT NULL,
    [IsActive]      BIT            NOT NULL,
    [AccountId]     INT            NULL,
    CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Suppliers_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Suppliers_AccountId]
    ON [dbo].[Suppliers]([AccountId] ASC);

