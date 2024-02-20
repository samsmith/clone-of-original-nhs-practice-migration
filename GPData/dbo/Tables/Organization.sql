CREATE TABLE [dbo].[Organization] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [OriginalId]     NVARCHAR (255)   NOT NULL,
    [ODSCode]        NVARCHAR (20)    NOT NULL,
    [PeriodStart]    DATETIME         NULL,
    [PeriodEnd]      DATETIME         NULL,
    [Type]           NVARCHAR (50)    NOT NULL,
    [Name]           NVARCHAR (255)   NOT NULL,
    [Telecom]        NVARCHAR (20)    NULL,
    [MainLocationID] UNIQUEIDENTIFIER NULL,
    [AddressID]      UNIQUEIDENTIFIER NULL,
    [PartOfID]       UNIQUEIDENTIFIER NULL,
    [ContactID]      UNIQUEIDENTIFIER NULL,
    [EntityId]       UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK__Organiza__3214EC07861ED8BE] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK__Organizat__Addre__5AB9788F] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([Id]),
    CONSTRAINT [FK__Organizat__Conta__5CA1C101] FOREIGN KEY ([ContactID]) REFERENCES [dbo].[Contact] ([Id]),
    CONSTRAINT [FK__Organizat__PartO__5BAD9CC8] FOREIGN KEY ([PartOfID]) REFERENCES [dbo].[Organization] ([Id]),
    CONSTRAINT [FK_Organization_Location] FOREIGN KEY ([MainLocationID]) REFERENCES [dbo].[Location] ([Id])
);

