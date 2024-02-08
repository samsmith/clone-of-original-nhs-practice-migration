CREATE TABLE [dbo].[Medication] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Code]             UNIQUEIDENTIFIER NOT NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [IsBrand]          BIT              NULL,
    [Form]             UNIQUEIDENTIFIER NULL,
    [Quantity]         DECIMAL (18, 2)  NULL,
    [BatchNumber]      NVARCHAR (MAX)   NULL,
    [ExpirationDate]   DATETIME         NULL,
    [IsOverTheCounter] BIT              NULL,
    [Manufacturer]     UNIQUEIDENTIFIER NULL,
    [Ingredient]       UNIQUEIDENTIFIER NULL,
    [IngredientAmount] DECIMAL (18)     NULL,
    CONSTRAINT [PK__Medicati__3214EC076C92B62D] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Medication_Organization] FOREIGN KEY ([Manufacturer]) REFERENCES [dbo].[Organization] ([Id])
);

