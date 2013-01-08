-- =============================================
-- Script Template
-- =============================================
USE master
IF DB_ID('SchoolOlympiads') IS NOT NULL
	DROP DATABASE [SchoolOlympiads]
	
CREATE DATABASE [SchoolOlympiads]
ON 
( NAME = Olympiads_dat,
    FILENAME = 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\DataBaseFiles\olympiads_dat.mdf',
    SIZE = 10MB,
    MAXSIZE = 50MB,
    FILEGROWTH = 5MB )
LOG ON
( NAME = Olympiads_log,
    FILENAME = 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\DataBaseFiles\olympiads_log.ldf',
    SIZE = 5MB,
    MAXSIZE = 25MB,
    FILEGROWTH = 5MB ) ;
GO

ALTER AUTHORIZATION ON DATABASE::[SchoolOlympiads] TO [NT AUTHORITY\SYSTEM]
GO

USE [SchoolOlympiads]
GO

IF DB_ID('SchoolOlympiads') IS NOT NULL
BEGIN
CREATE TABLE [Schools]
(
	SchoolNumber int NOT NULL UNIQUE CHECK(SchoolNumber > 0),
	Region nvarchar(255) NOT NULL CHECK(Region <> ''),
	PRIMARY KEY (SchoolNumber)
);
CREATE TABLE [Auditories]
(
	AuditoryNumber int NOT NULL UNIQUE CHECK(AuditoryNumber > 0),
	NumberOfComputers int NOT NULL CHECK(NumberOfComputers > 0),
	ComputersType nvarchar(255) NOT NULL CHECK(ComputersType <> ''),
	PRIMARY KEY (AuditoryNumber)
);
CREATE TABLE [Examiners]
(
	ExaminerID int NOT NULL IDENTITY,
	ExaminerName nvarchar(255) NOT NULL CHECK(ExaminerName <> ''),
	ExaminerSurname nvarchar(255) NOT NULL CHECK(ExaminerSurname <> ''),
	ExaminerSecondName nvarchar(255) NOT NULL CHECK(ExaminerSecondName <> ''),
	BirthDate date NOT NULL,
	Position nvarchar(255)NOT NULL CHECK(Position <> ''),
	PRIMARY KEY (ExaminerID)
);
CREATE TABLE [Participants]
(
	ParticipantID int NOT NULL IDENTITY,
	ParticipantName nvarchar(255) NOT NULL CHECK(ParticipantName <> ''),
	ParticipantSurname nvarchar(255) NOT NULL CHECK(ParticipantSurname <> ''),
	ParticipantSecondName nvarchar(255) NOT NULL CHECK(ParticipantSecondName <> ''),
	BirthDate date NOT NULL,
	SchoolNumber int NOT NULL,
	TeacherSurname nvarchar(255)NOT NULL CHECK(TeacherSurname <> ''),
	PRIMARY KEY (ParticipantID),
	FOREIGN KEY (SchoolNumber) REFERENCES Schools(SchoolNumber)
);
CREATE TABLE [Olympiads]
(
	OlympiadID int NOT NULL IDENTITY,
	Subject nvarchar(255) NOT NULL CHECK(Subject <> ''),
	Grade int NOT NULL CHECK(Grade > 0 AND Grade <=12),
	Date date NOT NULL,
	AuditoryNumber int NOT NULL,
	PRIMARY KEY (OlympiadID),
	FOREIGN KEY (AuditoryNumber) REFERENCES Auditories(AuditoryNumber)
);
CREATE TABLE [Tasks]
(
	OlympiadID int NOT NULL,
	TaskNumber int NOT NULL,
	Condition nvarchar(500) NOT NULL CHECK(Condition <> ''),
	MaximalScore real NOT NULL CHECK(MaximalScore > 0),
	FOREIGN KEY (OlympiadID) REFERENCES Olympiads(OlympiadID),
	PRIMARY KEY(OlympiadID,TaskNumber)
);
CREATE TABLE [OlympiadDetails]
(
	OlympiadID int NOT NULL,
	ParticipantID int NOT NULL,
	TaskNumber int NOT NULL,
	Score real NOT NULL CHECK(Score > 0),
	ExaminerID int NOT NULL,
	PRIMARY KEY (OlympiadID, ParticipantID, TaskNumber),
	FOREIGN KEY (OlympiadID) REFERENCES Olympiads(OlympiadID),
	FOREIGN KEY (ParticipantID) REFERENCES Participants(ParticipantID),
	FOREIGN KEY (ExaminerID) REFERENCES Examiners(ExaminerID),
	FOREIGN KEY (OlympiadID, TaskNumber) REFERENCES Tasks(OlympiadID, TaskNumber),
);
BULK INSERT Auditories
	FROM 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\BulkInsert\Auditories.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	);
BULK INSERT Examiners
	FROM 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\BulkInsert\Examiners.txt'
	WITH
	(
		FIELDTERMINATOR = ';',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	);
BULK INSERT OlympiadDetails
	FROM 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\BulkInsert\OlympiadDetails.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	);
BULK INSERT Olympiads
	FROM 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\BulkInsert\Olympiads.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	);
BULK INSERT Participants
	FROM 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\BulkInsert\Participants.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	);
BULK INSERT Schools
	FROM 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\BulkInsert\Schools.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	);
BULK INSERT Tasks
	FROM 'C:\Users\SiLenT\Desktop\DB\SQLDataBase\BulkInsert\Tasks.txt'
	WITH
	(
		FIELDTERMINATOR = ';',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	);
END