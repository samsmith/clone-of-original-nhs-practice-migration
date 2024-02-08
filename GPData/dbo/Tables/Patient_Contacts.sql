CREATE TABLE [dbo].[Patient_Contacts] (
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [ContactId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Patient_Contacts_Contact] FOREIGN KEY ([ContactId]) REFERENCES [dbo].[Contact] ([Id]),
    CONSTRAINT [FK_Patient_Contacts_Patient] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[Patient] ([Id])
);

