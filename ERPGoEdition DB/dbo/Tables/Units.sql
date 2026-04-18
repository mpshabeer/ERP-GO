CREATE TABLE [dbo].[Units] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX) NOT NULL,
    [IsActive]      BIT            NOT NULL,
    [Code]          NVARCHAR (MAX) DEFAULT (N'') NOT NULL,
    [DecimalPlaces] INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED ([Id] ASC)
);

