﻿CREATE TABLE [dbo].[ProcedureRequest] (
    [Id]                         UNIQUEIDENTIFIER NOT NULL,
    [Identifier]                 UNIQUEIDENTIFIER NULL,
    [Status]                     NVARCHAR (255)   NULL,
    [Assigner]                   UNIQUEIDENTIFIER NULL,
    [RequisitionAssigner]        UNIQUEIDENTIFIER NULL,
    [Category]                   NVARCHAR (255)   NULL,
    [Code]                       NVARCHAR (255)   NULL,
    [Subject]                    UNIQUEIDENTIFIER NULL,
    [Conext]                     UNIQUEIDENTIFIER NULL,
    [Encounter]                  UNIQUEIDENTIFIER NULL,
    [RequestingOrganization]     UNIQUEIDENTIFIER NULL,
    [RequestingPracticioner]     UNIQUEIDENTIFIER NULL,
    [OnBehalfOf]                 UNIQUEIDENTIFIER NULL,
    [PerformerOrganization]      UNIQUEIDENTIFIER NULL,
    [PerformerPracticioner]      UNIQUEIDENTIFIER NULL,
    [Reason]                     NVARCHAR (255)   NULL,
    [ReasonReferenceCondition]   UNIQUEIDENTIFIER NULL,
    [ReasonReferenceObservation] UNIQUEIDENTIFIER NULL,
    [SupportingInfo]             UNIQUEIDENTIFIER NULL,
    [Specimen]                   UNIQUEIDENTIFIER NULL,
    [BodySite]                   NVARCHAR (255)   NULL,
    [NoteText]                   NVARCHAR (MAX)   NULL,
    [NoteAuthored]               DATETIME         NULL,
    [NoteAuthorPatient]          UNIQUEIDENTIFIER NULL,
    [NoteAuthorPracticioner]     UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProcedureRequest_Assigner] FOREIGN KEY ([Assigner]) REFERENCES [dbo].[Organization] ([Id]),
    CONSTRAINT [FK_ProcedureRequest_ConextE] FOREIGN KEY ([Conext]) REFERENCES [dbo].[Entity] ([Id]),
    CONSTRAINT [FK_ProcedureRequest_Encounter] FOREIGN KEY ([Encounter]) REFERENCES [dbo].[Encounter] ([Id]),
    CONSTRAINT [FK_ProcedureRequest_Identifier] FOREIGN KEY ([Identifier]) REFERENCES [dbo].[Identifier] ([Id]),
    CONSTRAINT [FK_ProcedureRequest_OnBehalfOf] FOREIGN KEY ([OnBehalfOf]) REFERENCES [dbo].[Organization] ([Id]),
    CONSTRAINT [FK_ProcedureRequest_RequestingOrganization] FOREIGN KEY ([RequestingOrganization]) REFERENCES [dbo].[Organization] ([Id]),
    CONSTRAINT [FK_ProcedureRequest_RequestingPracticioner] FOREIGN KEY ([RequestingPracticioner]) REFERENCES [dbo].[Practicioner] ([Id]),
    CONSTRAINT [FK_ProcedureRequest_RequisitionAssigner] FOREIGN KEY ([RequisitionAssigner]) REFERENCES [dbo].[Organization] ([Id]),
    CONSTRAINT [FK_ProcedureRequest_Subject] FOREIGN KEY ([Subject]) REFERENCES [dbo].[Patient] ([Id])
);

