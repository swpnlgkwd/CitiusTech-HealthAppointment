-- Identity (ASP.NET Core Identity minimal tables)
CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    Name NVARCHAR(256),
    NormalizedName NVARCHAR(256),
    ConcurrencyStamp NVARCHAR(MAX)
);
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    UserName NVARCHAR(256),
    NormalizedUserName NVARCHAR(256),
    Email NVARCHAR(256),
    NormalizedEmail NVARCHAR(256),
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    PasswordHash NVARCHAR(MAX),
    SecurityStamp NVARCHAR(MAX),
    ConcurrencyStamp NVARCHAR(MAX),
    PhoneNumber NVARCHAR(MAX),
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnd DATETIMEOFFSET,
    LockoutEnabled BIT NOT NULL DEFAULT 1,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    DoctorId INT NULL,
    PatientId INT NULL
);
CREATE TABLE AspNetRoleClaims (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX),
    ClaimValue NVARCHAR(MAX),
    CONSTRAINT FK_RoleClaims_Roles FOREIGN KEY(RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);
CREATE TABLE AspNetUserClaims (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX),
    ClaimValue NVARCHAR(MAX),
    CONSTRAINT FK_UserClaims_Users FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
CREATE TABLE AspNetUserLogins (
    LoginProvider NVARCHAR(450) NOT NULL,
    ProviderKey NVARCHAR(450) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX),
    UserId NVARCHAR(450) NOT NULL,
    PRIMARY KEY(LoginProvider, ProviderKey),
    CONSTRAINT FK_UserLogins_Users FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY(UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY(RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);
CREATE TABLE AspNetUserTokens (
    UserId NVARCHAR(450) NOT NULL,
    LoginProvider NVARCHAR(450) NOT NULL,
    Name NVARCHAR(450) NOT NULL,
    Value NVARCHAR(MAX),
    PRIMARY KEY(UserId, LoginProvider, Name),
    CONSTRAINT FK_UserTokens_Users FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Refresh tokens (for Identity users)
CREATE TABLE RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Token NVARCHAR(500) NOT NULL,
    Expires DATETIME2 NOT NULL,
    Created DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByIp NVARCHAR(64) NOT NULL,
    Revoked DATETIME2 NULL,
    RevokedByIp NVARCHAR(64) NULL,
    ReplacedByToken NVARCHAR(500) NULL,
    UserId NVARCHAR(450) NOT NULL,
    CONSTRAINT FK_RefreshTokens_User FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- Minimal seed roles
INSERT INTO AspNetRoles (Id, Name, NormalizedName) VALUES
(NEWID(), 'Admin', 'ADMIN'),
(NEWID(), 'Provider', 'Provider'),
(NEWID(), 'Patient', 'PATIENT');

------------------------------------------------
-- Lookup Tables
------------------------------------------------
CREATE TABLE Gender (
    gender_id INT PRIMARY KEY,
    gender_name NVARCHAR(50) NOT NULL
);

CREATE TABLE Specialty (
    specialty_id INT PRIMARY KEY,
    specialty_name NVARCHAR(100) NOT NULL
);

CREATE TABLE RiskType (
    risk_type_id INT PRIMARY KEY,
    risk_type_name NVARCHAR(100) NOT NULL
);

CREATE TABLE RiskLevel (
    risk_level_id INT PRIMARY KEY,
    risk_level_name NVARCHAR(50) NOT NULL -- e.g., Low / Medium / High
);

CREATE TABLE AppointmentStatus (
    status_id INT PRIMARY KEY,
    status_name NVARCHAR(50) NOT NULL
);
/* Suggested values: (1=Booked, 2=Rescheduled, 3=Cancelled, 4=Completed, 5=NoShow) */

CREATE TABLE AppointmentType (
    type_id INT PRIMARY KEY,
    type_name NVARCHAR(50) NOT NULL
);
/* Suggested values: (1=Consultation, 2=Follow-up, 3=Telehealth, 4=Emergency) */

CREATE TABLE DeliveryStatus (
    delivery_status_id INT PRIMARY KEY,
    delivery_status_name NVARCHAR(50) NOT NULL
);
/* Suggested values: (1=Sent, 2=Failed, 3=Delivered) */


------------------------------------------------
-- Core Entities
------------------------------------------------
CREATE TABLE Patient (
    patient_id INT IDENTITY PRIMARY KEY,
    full_name NVARCHAR(150) NOT NULL,
    dob DATE NULL,
    gender_id INT NULL FOREIGN KEY REFERENCES Gender(gender_id),
    email NVARCHAR(150) NULL,
    phone NVARCHAR(50) NULL,
    address NVARCHAR(250) NULL,
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE Provider (
    provider_id INT IDENTITY PRIMARY KEY,
    full_name NVARCHAR(150) NOT NULL,
    specialty_id INT NULL FOREIGN KEY REFERENCES Specialty(specialty_id),
    contact_email NVARCHAR(150) NULL,
    contact_phone NVARCHAR(50) NULL,
    is_active BIT DEFAULT 1,
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
);


------------------------------------------------
-- Scheduling (create before Appointment so slot FK compiles)
------------------------------------------------
CREATE TABLE ProviderSchedule (
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    ProviderId INT NOT NULL,
    ScheduleDate DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    SlotDurationMinutes INT NOT NULL DEFAULT 60, -- 60-minute default
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
    CONSTRAINT FK_ProviderSchedule_Provider
        FOREIGN KEY (ProviderId) REFERENCES Provider(provider_id),
    -- Guards: positive duration and divisible by slot length
    CONSTRAINT CK_ProviderSchedule_Time
        CHECK (
            DATEDIFF(MINUTE, StartTime, EndTime) > 0
            AND DATEDIFF(MINUTE, StartTime, EndTime) % SlotDurationMinutes = 0
        )
);

CREATE TABLE ProviderSlots (
    SlotId INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleId INT NOT NULL,
    SlotStart TIME NOT NULL,
    SlotEnd TIME NOT NULL,
    IsBooked BIT NOT NULL DEFAULT 0,
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
    CONSTRAINT FK_ProviderSlots_Schedule
        FOREIGN KEY (ScheduleId) REFERENCES ProviderSchedule(ScheduleId)
            ON DELETE CASCADE,
    CONSTRAINT CK_ProviderSlots_Time
        CHECK (SlotEnd > SlotStart)
);

-- Ensure no duplicate slots per schedule
CREATE UNIQUE INDEX UX_ProviderSlots_Schedule_Slot
    ON ProviderSlots (ScheduleId, SlotStart, SlotEnd);


------------------------------------------------
-- Appointments (now tied to slots)
------------------------------------------------
CREATE TABLE Appointment (
    appointment_id INT IDENTITY PRIMARY KEY,
    patient_id INT NOT NULL,
    provider_id INT NOT NULL,
    slot_id INT NOT NULL,  -- Enforce booking against a concrete slot
    start_utc DATETIME2 NOT NULL,
    end_utc DATETIME2 NOT NULL,
    status_id INT NOT NULL,
    type_id INT NOT NULL,
    notes NVARCHAR(MAX) NULL,
    reminder_sent BIT DEFAULT 0,
    reminder_sent_at DATETIME2 NULL,
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
    CONSTRAINT FK_Appointment_Patient
        FOREIGN KEY (patient_id) REFERENCES Patient(patient_id),
    CONSTRAINT FK_Appointment_Provider
        FOREIGN KEY (provider_id) REFERENCES Provider(provider_id),
    CONSTRAINT FK_Appointment_Slot
        FOREIGN KEY (slot_id) REFERENCES ProviderSlots(SlotId),
    CONSTRAINT FK_Appointment_Status
        FOREIGN KEY (status_id) REFERENCES AppointmentStatus(status_id),
    CONSTRAINT FK_Appointment_Type
        FOREIGN KEY (type_id) REFERENCES AppointmentType(type_id),
    CONSTRAINT CK_Appointment_Time
        CHECK (end_utc > start_utc)
);

-- Helpful FKs indexes
CREATE INDEX IX_Appointment_patient_id ON Appointment(patient_id);
CREATE INDEX IX_Appointment_provider_id ON Appointment(provider_id);
CREATE INDEX IX_Appointment_slot_id ON Appointment(slot_id);
CREATE INDEX IX_Appointment_status_id ON Appointment(status_id);
CREATE INDEX IX_Appointment_type_id ON Appointment(type_id);


------------------------------------------------
-- Provider Exceptions (vacations, leave)
------------------------------------------------
CREATE TABLE ProviderException (
    exception_id INT IDENTITY PRIMARY KEY,
    provider_id INT NOT NULL FOREIGN KEY REFERENCES Provider(provider_id),
    start_utc DATETIME2 NOT NULL,
    end_utc DATETIME2 NOT NULL,
    reason NVARCHAR(200) NULL,
    CONSTRAINT CK_ProviderException_Time
        CHECK (end_utc > start_utc)
);


------------------------------------------------
-- Appointment History
------------------------------------------------
CREATE TABLE AppointmentHistory (
    history_id INT IDENTITY PRIMARY KEY,
    appointment_id INT NOT NULL FOREIGN KEY REFERENCES Appointment(appointment_id),
    old_start_utc DATETIME2 NULL,
    old_end_utc DATETIME2 NULL,
    old_status_id INT NULL FOREIGN KEY REFERENCES AppointmentStatus(status_id),
    new_start_utc DATETIME2 NULL,
    new_end_utc DATETIME2 NULL,
    new_status_id INT NULL FOREIGN KEY REFERENCES AppointmentStatus(status_id),
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0,
    changed_by_role_id NVARCHAR(450) NULL FOREIGN KEY REFERENCES AspNetRoles(Id)
);


------------------------------------------------
-- Patient Medical Data
------------------------------------------------
CREATE TABLE PatientHistory (
    history_id INT IDENTITY PRIMARY KEY,
    patient_id INT NOT NULL FOREIGN KEY REFERENCES Patient(patient_id),
    condition NVARCHAR(150) NOT NULL,
    notes NVARCHAR(MAX) NULL,
    diagnosed_on DATE NULL,
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE PatientRiskFactor (
    risk_id INT IDENTITY PRIMARY KEY,
    patient_id INT NOT NULL FOREIGN KEY REFERENCES Patient(patient_id),
    risk_type_id INT NOT NULL FOREIGN KEY REFERENCES RiskType(risk_type_id),
    risk_level_id INT NOT NULL FOREIGN KEY REFERENCES RiskLevel(risk_level_id),
    identified_on DATE DEFAULT CAST(SYSUTCDATETIME() AS DATE),
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
);

CREATE TABLE PatientRiskScore (
    score_id INT IDENTITY PRIMARY KEY,
    patient_id INT NOT NULL FOREIGN KEY REFERENCES Patient(patient_id),
    score INT NOT NULL,  -- numeric score (0-100)
    risk_level_id INT NOT NULL FOREIGN KEY REFERENCES RiskLevel(risk_level_id),
    reason NVARCHAR(MAX) NULL,
    calculated_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
);


------------------------------------------------
-- Notifications
------------------------------------------------
CREATE TABLE NotificationLog (
    notification_id INT IDENTITY PRIMARY KEY,
    patient_id INT NOT NULL FOREIGN KEY REFERENCES Patient(patient_id),
    appointment_id INT NULL FOREIGN KEY REFERENCES Appointment(appointment_id),
    channel NVARCHAR(50) NOT NULL, -- e.g., SMS, Email, App
    message NVARCHAR(MAX) NOT NULL,
    sent_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    delivery_status_id INT NULL FOREIGN KEY REFERENCES DeliveryStatus(delivery_status_id),
    created_at DATETIME2 DEFAULT SYSUTCDATETIME(),
    updated_at DATETIME2 NULL,
    is_deleted BIT NOT NULL DEFAULT 0
);

CREATE INDEX IX_NotificationLog_patient_id ON NotificationLog(patient_id);
CREATE INDEX IX_NotificationLog_appointment_id ON NotificationLog(appointment_id);
CREATE INDEX IX_NotificationLog_delivery_status_id ON NotificationLog(delivery_status_id);


------------------------------------------------
-- Drop constraints-dependent tables first
------------------------------------------------
--DROP TABLE IF EXISTS NotificationLog;
--DROP TABLE IF EXISTS PatientRiskScore;
--DROP TABLE IF EXISTS PatientRiskFactor;
--DROP TABLE IF EXISTS PatientHistory;
--DROP TABLE IF EXISTS AppointmentHistory;
--DROP TABLE IF EXISTS ProviderException;
--DROP TABLE IF EXISTS Appointment;
--DROP TABLE IF EXISTS ProviderSlots;
--DROP TABLE IF EXISTS ProviderSchedule;
--DROP TABLE IF EXISTS Provider;
--DROP TABLE IF EXISTS Patient;

------------------------------------------------
-- Lookup tables (if you want to drop them too)
-- Uncomment only if you created them in your schema
------------------------------------------------
-- DROP TABLE IF EXISTS Gender;
-- DROP TABLE IF EXISTS Specialty;
-- DROP TABLE IF EXISTS AppointmentStatus;
-- DROP TABLE IF EXISTS AppointmentType;
-- DROP TABLE IF EXISTS RiskType;
-- DROP TABLE IF EXISTS RiskLevel;
-- DROP TABLE IF EXISTS DeliveryStatus;
-- DROP TABLE IF EXISTS AspNetRoles;