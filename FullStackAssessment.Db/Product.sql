CREATE TABLE [dbo].[Product]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ProductTypeId] INT NOT NULL,
    [Name] NVARCHAR(150) NOT NULL,
    [Price] DECIMAL(18, 2) NOT NULL,
    [Description] NVARCHAR(1000) NULL,

    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Product_ProductType] FOREIGN KEY ([ProductTypeId]) 
        REFERENCES [dbo].[ProductType] ([Id]) 
        ON DELETE NO ACTION, -- SQL Server equivalent of RESTRICT, prevents deleting a ProductType if Products still rely on it
    
    CONSTRAINT [CK_Product_Price] CHECK ([Price] >= 0.00) -- ensures no negative pricing
)