CREATE TABLE [dbo].[PracticionerRole] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [OriginalId]     NVARCHAR (255)   NOT NULL,
    [Active]         BIT              NULL,
    [PeriodStart]    DATETIME         NULL,
    [PeriodEnd]      DATETIME         NULL,
    [Practicioner]   UNIQUEIDENTIFIER NOT NULL,
    [Organization]   UNIQUEIDENTIFIER NULL,
    [SDSJobRoleName] NVARCHAR (100)   NULL,
    [Speciality]     NVARCHAR (100)   NULL,
    [Location]       UNIQUEIDENTIFIER NULL,
    [Telecom]        NVARCHAR (50)    NULL,
    CONSTRAINT [PK_PracticionerRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PracticionerRole_Location] FOREIGN KEY ([Location]) REFERENCES [dbo].[Location] ([Id]),
    CONSTRAINT [FK_PracticionerRole_Organization] FOREIGN KEY ([Organization]) REFERENCES [dbo].[Organization] ([Id]),
    CONSTRAINT [FK_PracticionerRole_Practicioner] FOREIGN KEY ([Practicioner]) REFERENCES [dbo].[Practicioner] ([Id])
);

