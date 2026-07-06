CREATE TABLE [dbo].[ProductType]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,

    CONSTRAINT [PK_ProductType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_ProductType_Name] UNIQUE ([Name])
)