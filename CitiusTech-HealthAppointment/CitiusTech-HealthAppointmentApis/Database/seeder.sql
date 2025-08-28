------------------------------------------------
-- Identity: Roles (Id must be stable GUIDs for EF/Identity mapping)
------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
END
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'PROVIDER')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
    VALUES (NEWID(), 'Provider', 'PROVIDER', NEWID());
END
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'PATIENT')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
    VALUES (NEWID(), 'Patient', 'PATIENT', NEWID());
END

------------------------------------------------
-- Lookup: Gender
------------------------------------------------
MERGE Gender AS target
USING (VALUES 
    (1, 'Male'),
    (2, 'Female'),
    (3, 'Other')
) AS source (gender_id, gender_name)
ON target.gender_id = source.gender_id
WHEN NOT MATCHED THEN
    INSERT (gender_id, gender_name) VALUES (source.gender_id, source.gender_name);

------------------------------------------------
-- Lookup: Specialty
------------------------------------------------
MERGE Specialty AS target
USING (VALUES 
    (1, 'General Practitioner'),
    (2, 'Cardiology'),
    (3, 'Dermatology'),
    (4, 'Pediatrics')
) AS source (specialty_id, specialty_name)
ON target.specialty_id = source.specialty_id
WHEN NOT MATCHED THEN
    INSERT (specialty_id, specialty_name) VALUES (source.specialty_id, source.specialty_name);

------------------------------------------------
-- Lookup: Risk Types
------------------------------------------------
MERGE RiskType AS target
USING (VALUES 
    (1, 'Diabetes'),
    (2, 'Hypertension'),
    (3, 'Asthma')
) AS source (risk_type_id, risk_type_name)
ON target.risk_type_id = source.risk_type_id
WHEN NOT MATCHED THEN
    INSERT (risk_type_id, risk_type_name) VALUES (source.risk_type_id, source.risk_type_name);

------------------------------------------------
-- Lookup: Risk Levels
------------------------------------------------
MERGE RiskLevel AS target
USING (VALUES 
    (1, 'Low'),
    (2, 'Medium'),
    (3, 'High')
) AS source (risk_level_id, risk_level_name)
ON target.risk_level_id = source.risk_level_id
WHEN NOT MATCHED THEN
    INSERT (risk_level_id, risk_level_name) VALUES (source.risk_level_id, source.risk_level_name);

------------------------------------------------
-- Lookup: Appointment Status
------------------------------------------------
MERGE AppointmentStatus AS target
USING (VALUES 
    (1, 'Booked'),
    (2, 'Rescheduled'),
    (3, 'Cancelled'),
    (4, 'Completed'),
    (5, 'NoShow')
) AS source (status_id, status_name)
ON target.status_id = source.status_id
WHEN NOT MATCHED THEN
    INSERT (status_id, status_name) VALUES (source.status_id, source.status_name);

------------------------------------------------
-- Lookup: Appointment Type
------------------------------------------------
MERGE AppointmentType AS target
USING (VALUES 
    (1, 'Consultation'),
    (2, 'Follow-up'),
    (3, 'Telehealth'),
    (4, 'Emergency')
) AS source (type_id, type_name)
ON target.type_id = source.type_id
WHEN NOT MATCHED THEN
    INSERT (type_id, type_name) VALUES (source.type_id, source.type_name);

------------------------------------------------
-- Lookup: Delivery Status
------------------------------------------------
MERGE DeliveryStatus AS target
USING (VALUES 
    (1, 'Sent'),
    (2, 'Failed'),
    (3, 'Delivered')
) AS source (delivery_status_id, delivery_status_name)
ON target.delivery_status_id = source.delivery_status_id
WHEN NOT MATCHED THEN
    INSERT (delivery_status_id, delivery_status_name) VALUES (source.delivery_status_id, source.delivery_status_name);

------------------------------------------------
-- Seed Patients
------------------------------------------------
INSERT INTO Patient (full_name, dob, gender_id, email, phone, address)
VALUES 
('John Doe', '1985-01-01', 1, 'john.doe@example.com', '1234567890', '123 Main St'),
('Jane Smith', '1990-05-15', 2, 'jane.smith@example.com', '0987654321', '456 Elm St');

------------------------------------------------
------------------------------------------------
-- Seed Providers
------------------------------------------------
INSERT INTO Provider (full_name, specialty_id, contact_email, contact_phone)
VALUES 
('Dr. Alice Heart', 2, 'alice.heart@clinic.com', '1112223333'),
('Dr. Bob Skin', 3, 'bob.skin@clinic.com', '4445556666');

------------------------------------------------
-- Provider Schedule & Slots
------------------------------------------------
INSERT INTO ProviderSchedule (ProviderId, ScheduleDate, StartTime, EndTime, SlotDurationMinutes)
VALUES (1, CAST(GETUTCDATE() AS DATE), '09:00', '12:00', 60);

DECLARE @ScheduleId INT = SCOPE_IDENTITY();

-- 3 slots (9-10, 10-11, 11-12)
INSERT INTO ProviderSlots (ScheduleId, SlotStart, SlotEnd)
VALUES 
(@ScheduleId, '09:00', '10:00'),
(@ScheduleId, '10:00', '11:00'),
(@ScheduleId, '11:00', '12:00');

------------------------------------------------
-- Seed Appointment
------------------------------------------------
DECLARE @SlotId INT = (SELECT TOP 1 SlotId FROM ProviderSlots WHERE ScheduleId = @ScheduleId AND SlotStart='09:00');
INSERT INTO Appointment (patient_id, provider_id, slot_id, start_utc, end_utc, status_id, type_id, notes)
VALUES (1, 1, @SlotId, DATEADD(HOUR, 1, SYSUTCDATETIME()), DATEADD(HOUR, 2, SYSUTCDATETIME()), 1, 1, 'Initial consultation');

------------------------------------------------
-- Seed Notification
------------------------------------------------
INSERT INTO NotificationLog (patient_id, appointment_id, channel, message, delivery_status_id)
VALUES (1, 1, 'Email', 'Your appointment is confirmed.', 1);
