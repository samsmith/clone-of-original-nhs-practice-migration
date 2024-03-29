﻿CREATE TABLE [dbo].[Appointment] (
    [Id]                            UNIQUEIDENTIFIER NOT NULL,
    [AppointmentCancellationReason] NVARCHAR (255)   NULL,
    [BookingOrganizationId]         UNIQUEIDENTIFIER NOT NULL,
    [PracticionerRole]              NVARCHAR (255)   NULL,
    [DeliveryChannel]               NVARCHAR (255)   NULL,
    [IdentifierId]                  UNIQUEIDENTIFIER NOT NULL,
    [Status]                        NVARCHAR (255)   NOT NULL,
    [ServiceCategory]               NVARCHAR (255)   NULL,
    [ServiceType]                   NVARCHAR (255)   NULL,
    [Speciality]                    NVARCHAR (255)   NULL,
    [Reason]                        NVARCHAR (255)   NULL,
    [Priority]                      INT              NULL,
    [Description]                   NVARCHAR (255)   NULL,
    [Start]                         DATETIME2 (7)    NOT NULL,
    [End]                           DATETIME2 (7)    NOT NULL,
    [MinutesDuration]               INT              NULL,
    [Created]                       DATETIME2 (7)    NULL,
    [Comment]                       NVARCHAR (255)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([IdentifierId]) REFERENCES [dbo].[Identifier] ([Id]),
    CONSTRAINT [FK__Appointme__Booki__17036CC0] FOREIGN KEY ([BookingOrganizationId]) REFERENCES [dbo].[Organization] ([Id])
);

