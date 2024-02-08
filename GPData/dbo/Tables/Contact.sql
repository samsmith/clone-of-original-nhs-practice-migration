CREATE TABLE [dbo].[Contact] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Title]          NVARCHAR (50)    NULL,
    [GivenName]      NVARCHAR (100)   NOT NULL,
    [MiddleName]     NVARCHAR (100)   NULL,
    [Surname]        NVARCHAR (100)   NOT NULL,
    [AddressID]      UNIQUEIDENTIFIER NULL,
    [OrganizationId] UNIQUEIDENTIFIER NULL,
    [Gender]         NVARCHAR (100)   NULL,
    [Relationship]   NVARCHAR (100)   NULL,
    [HomePhone]      NVARCHAR (100)   NULL,
    [MobilePhone]    NVARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([Id]),
    CONSTRAINT [FK_Contact_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([Id])
);

