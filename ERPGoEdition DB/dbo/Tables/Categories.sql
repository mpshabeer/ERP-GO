CREATE TABLE [dbo].[Categories] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (450) NOT NULL,
    [Code]          NVARCHAR (MAX) NULL,
    [IsActive]      BIT            DEFAULT ((1)) NOT NULL,
    [DefaultUnitId] INT            NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Categories_Units_DefaultUnitId] FOREIGN KEY ([DefaultUnitId]) REFERENCES [dbo].[Units] ([Id]),
    CONSTRAINT [UQ_Categories_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

