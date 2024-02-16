CREATE TABLE [dbo].[Qualification] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Use]        NVARCHAR (20)    NULL,
    [Type]       NVARCHAR (50)    NULL,
    [System]     NVARCHAR (50)    NULL,
    [Value]      NVARCHAR (255)   NULL,
    [FromDate]   DATETIME         NULL,
    [ToDate]     DATETIME         NULL,
    [AssignerID] UNIQUEIDENTIFIER NULL,
    [IssuerID]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK__Qualifica__Assig__76619304] FOREIGN KEY ([AssignerID]) REFERENCES [dbo].[Organization] ([Id]),
    CONSTRAINT [FK__Qualifica__Issue__756D6ECB] FOREIGN KEY ([IssuerID]) REFERENCES [dbo].[Organization] ([Id])
);

