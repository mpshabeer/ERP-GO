CREATE TABLE [dbo].[AppSettings] (
    [Key]   NVARCHAR (450) NOT NULL,
    [Value] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_AppSettings] PRIMARY KEY CLUSTERED ([Key] ASC)
);

