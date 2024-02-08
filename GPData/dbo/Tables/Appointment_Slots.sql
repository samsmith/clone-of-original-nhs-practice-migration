CREATE TABLE [dbo].[Appointment_Slots] (
    [AppointmentId] UNIQUEIDENTIFIER NOT NULL,
    [SlotId]        UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Appointment_Slots_Appointment] FOREIGN KEY ([AppointmentId]) REFERENCES [dbo].[Appointment] ([Id]),
    CONSTRAINT [FK_Appointment_Slots_Slot] FOREIGN KEY ([SlotId]) REFERENCES [dbo].[Slot] ([Id])
);

