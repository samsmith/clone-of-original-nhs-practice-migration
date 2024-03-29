﻿CREATE TABLE [dbo].[Dosage] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL,
    [Sequence]                 INT              NULL,
    [Text]                     NVARCHAR (MAX)   NULL,
    [Timing]                   UNIQUEIDENTIFIER NULL,
    [AdditionalInstruction]    NVARCHAR (MAX)   NULL,
    [PatientInstruction]       NVARCHAR (MAX)   NULL,
    [AsNeeded]                 BIT              NULL,
    [AsNeededCode]             NVARCHAR (MAX)   NULL,
    [Site]                     NVARCHAR (MAX)   NULL,
    [Route]                    NVARCHAR (MAX)   NULL,
    [Method]                   NVARCHAR (MAX)   NULL,
    [DoseRangeHigh]            INT              NULL,
    [DoseRangeLow]             INT              NULL,
    [Quantity]                 INT              NULL,
    [MaxDosePerPeriod]         DECIMAL (18, 4)  NULL,
    [MaxDosePerAdministration] INT              NULL,
    [MaxDosePerLifetime]       INT              NULL,
    [RateRatioNumerator]       DECIMAL (18, 4)  NULL,
    [RateRatioDenominator]     DECIMAL (18, 4)  NULL,
    [RateRangeLow]             INT              NULL,
    [RateRangeHigh]            INT              NULL,
    [RateQuantity]             INT              NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Dosage_Timing] FOREIGN KEY ([Timing]) REFERENCES [dbo].[Timing] ([Id])
);

