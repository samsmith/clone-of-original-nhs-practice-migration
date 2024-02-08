CREATE TABLE [dbo].[Patient_Links] (
    [PatientId]     UNIQUEIDENTIFIER NOT NULL,
    [LinkedPatient] UNIQUEIDENTIFIER NULL,
    [LinkedContact] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_Patient_Links_Contact] FOREIGN KEY ([LinkedContact]) REFERENCES [dbo].[Contact] ([Id]),
    CONSTRAINT [FK_Patient_Links_Patient] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patient] ([Id]),
    CONSTRAINT [FK_Patient_Links_Patient1] FOREIGN KEY ([LinkedPatient]) REFERENCES [dbo].[Patient] ([Id])
);

