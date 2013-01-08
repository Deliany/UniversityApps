USE [SchoolOlympiads]
GO

IF OBJECT_ID ('SummaryLog', 'U') IS NOT NULL
   DROP TABLE SummaryLog;
GO

CREATE Table SummaryLog(
	TableName nvarchar(50),
	ActionName nvarchar(10),
	AffectedRow text,
	Date datetime
)

--------------- [Olympiads] triggers -----------------------
IF OBJECT_ID ('OlympiadsLogTrigger', 'TR') IS NOT NULL
   DROP TRIGGER OlympiadsLogTrigger;
GO

IF OBJECT_ID ('OlympiadsDuplicateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER OlympiadsDuplicateCheckTrigger;
GO

IF OBJECT_ID ('OlympiadsCascadeDeleteTrigger', 'TR') IS NOT NULL
   DROP TRIGGER OlympiadsCascadeDeleteTrigger;
GO

IF OBJECT_ID ('TasksCascadeDeleteTrigger', 'TR') IS NOT NULL
   DROP TRIGGER TasksCascadeDeleteTrigger;
GO
-----------------------------------------------------------

--------------- [Auditories] triggers -----------------------
IF OBJECT_ID ('AuditoriesLogTrigger', 'TR') IS NOT NULL
   DROP TRIGGER AuditoriesLogTrigger;
GO

IF OBJECT_ID ('AuditoriesDuplicateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER AuditoriesDuplicateCheckTrigger;
GO

IF OBJECT_ID ('AuditoriesUpdateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER AuditoriesUpdateCheckTrigger;
GO
--------------------------------------------------------------

--------------- [Schools] triggers -----------------------
IF OBJECT_ID ('SchoolsLogTrigger', 'TR') IS NOT NULL
   DROP TRIGGER SchoolsLogTrigger;
GO

IF OBJECT_ID ('SchoolsDuplicateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER SchoolsDuplicateCheckTrigger;
GO

IF OBJECT_ID ('SchoolsUpdateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER SchoolsUpdateCheckTrigger;
GO
--------------------------------------------------------------

--------------- [Examiners] triggers -----------------------
IF OBJECT_ID ('ExaminersLogTrigger', 'TR') IS NOT NULL
   DROP TRIGGER ExaminersLogTrigger;
GO

IF OBJECT_ID ('ExaminersDuplicateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER ExaminersDuplicateCheckTrigger;
GO
--------------------------------------------------------------

--------------- [Participants] triggers -----------------------
IF OBJECT_ID ('ParticipantsLogTrigger', 'TR') IS NOT NULL
   DROP TRIGGER ParticipantsLogTrigger;
GO

IF OBJECT_ID ('ParticipantsDuplicateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER ParticipantsDuplicateCheckTrigger;
GO
--------------------------------------------------------------

--------------- [Tasks] triggers -----------------------
IF OBJECT_ID ('TasksLogTrigger', 'TR') IS NOT NULL
   DROP TRIGGER TasksLogTrigger;
GO

IF OBJECT_ID ('TasksDuplicateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER TasksDuplicateCheckTrigger;
GO

IF OBJECT_ID ('TasksUpdateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER TasksUpdateCheckTrigger;
GO
--------------------------------------------------------------

--------------- [OlympiadDetails] triggers -----------------------
IF OBJECT_ID ('OlympiadDetailsLogTrigger', 'TR') IS NOT NULL
   DROP TRIGGER OlympiadDetailsLogTrigger;
GO

IF OBJECT_ID ('OlympiadDetailsDuplicateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER OlympiadDetailsDuplicateCheckTrigger;
GO

IF OBJECT_ID ('OlympiadDetailsUpdateCheckTrigger', 'TR') IS NOT NULL
   DROP TRIGGER OlympiadDetailsUpdateCheckTrigger;
GO
--------------------------------------------------------------

--------------- [Olympiads] triggers -----------------------
CREATE TRIGGER OlympiadsLogTrigger ON Olympiads AFTER INSERT, UPDATE, DELETE
AS
-- AFTER INSERT WRITE TO LOG
IF NOT EXISTS( SELECT * FROM Deleted )
	INSERT INTO SummaryLog
	SELECT 'Olympiads',
		'Insert',
		('Subject: ' + inserted.Subject + ' Grade: ' + CONVERT(nvarchar(5),inserted.Grade) + ' Date: ' + CONVERT(nvarchar(20),inserted.Date, 103)),
		GETDATE()
	FROM inserted
-- AFTER DELETE WRITE TO LOG
ELSE IF NOT EXISTS ( SELECT * FROM Inserted )
	INSERT INTO SummaryLog
	SELECT 'Olympiads',
		'Delete',
		('Subject: ' + deleted.Subject + ' Grade: ' + CONVERT(nvarchar(5),deleted.Grade) + ' Date: ' + CONVERT(nvarchar(20),deleted.Date, 103)),
		GETDATE()
	FROM deleted
-- AFTER UPDATE WRITE TO LOG
ELSE
	BEGIN
		INSERT INTO SummaryLog
		SELECT 'Olympiads',
		'Update',
		('Subject: ' + deleted.Subject +' -> ' + inserted.Subject + ' Grade: ' + CONVERT(nvarchar(5),deleted.Grade) + ' -> ' + CONVERT(nvarchar(5),inserted.Grade) + ' Date: ' + CONVERT(nvarchar(20),deleted.Date, 103) +' -> ' + CONVERT(nvarchar(20),inserted.Date, 103)),
		GETDATE()
		FROM inserted,deleted
	END
GO

CREATE TRIGGER OlympiadsDuplicateCheckTrigger ON Olympiads INSTEAD OF INSERT,UPDATE
AS
BEGIN
	IF EXISTS
	(
		SELECT 1
		FROM [Olympiads] O
		INNER JOIN Inserted I
		ON O.Subject = I.Subject
			AND O.Grade = I.Grade
			AND O.Date = I.Date
	)
	BEGIN
		-- Duplicate error
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate olympiad',10,1)
        return
	END

	-- APPLY INSERT
	IF NOT EXISTS( SELECT * FROM Deleted )
		INSERT INTO [Olympiads]
		SELECT Subject,Grade,Date,AuditoryNumber
		FROM Inserted
	-- APPLY UPDATE
	ELSE
		UPDATE [Olympiads]
		SET Subject = I.Subject, Grade=I.Grade, Date=I.Date, AuditoryNumber=I.AuditoryNumber
		FROM Inserted I, Deleted D
		WHERE [Olympiads].OlympiadID = D.OlympiadID
END
GO

CREATE TRIGGER OlympiadsCascadeDeleteTrigger ON Olympiads INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM Tasks
	WHERE OlympiadID IN (SELECT OlympiadID FROM Deleted)
	
	DELETE FROM OlympiadDetails
	WHERE OlympiadID IN (SELECT OlympiadID FROM Deleted)
	
	DELETE FROM Olympiads
	WHERE OlympiadID IN (SELECT OlympiadID FROM Deleted)
END
GO

CREATE TRIGGER TasksCascadeDeleteTrigger ON Tasks INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM OlympiadDetails
	WHERE OlympiadID IN (SELECT OlympiadID FROM Deleted) AND TaskNumber IN (SELECT TaskNumber FROM Deleted)
	
	DELETE FROM Tasks
	WHERE OlympiadID IN (SELECT OlympiadID FROM Deleted) AND TaskNumber IN (SELECT TaskNumber FROM Deleted)
END
GO
-------------------------------------------------------------

--------------- [Auditories] triggers -----------------------

CREATE TRIGGER AuditoriesLogTrigger ON Auditories AFTER INSERT, UPDATE, DELETE
AS
-- AFTER INSERT WRITE TO LOG
IF NOT EXISTS( SELECT * FROM Deleted )
	INSERT INTO SummaryLog
	SELECT 'Auditories',
		'Insert',
		('AuditoryNumber: ' + CONVERT(nvarchar(10),inserted.AuditoryNumber) + ' Number of computers: ' + CONVERT(nvarchar(5),inserted.NumberOfComputers) + ' ComputersType: ' + inserted.ComputersType),
		GETDATE()
	FROM inserted
-- AFTER DELETE WRITE TO LOG
ELSE IF NOT EXISTS ( SELECT * FROM Inserted )
	INSERT INTO SummaryLog
	SELECT 'Auditories',
		'Delete',
		('AuditoryNumber: ' + CONVERT(nvarchar(10),deleted.AuditoryNumber) + ' Number of computers: ' + CONVERT(nvarchar(5),deleted.NumberOfComputers) + ' ComputersType: ' + deleted.ComputersType),
		GETDATE()
	FROM deleted
-- AFTER UPDATE WRITE TO LOG
ELSE
	BEGIN
		INSERT INTO SummaryLog
		SELECT 'Auditories',
			'Update',
			('AuditoryNumber: ' + CONVERT(nvarchar(10),deleted.AuditoryNumber) + ' -> ' + CONVERT(nvarchar(10),inserted.AuditoryNumber) + ' Number of computers: ' + CONVERT(nvarchar(5),deleted.NumberOfComputers) + ' -> ' + CONVERT(nvarchar(5),inserted.NumberOfComputers) + ' ComputersType: ' + deleted.ComputersType + ' -> ' + inserted.ComputersType),
		GETDATE()
		FROM inserted,deleted	
	END
GO

CREATE TRIGGER AuditoriesDuplicateCheckTrigger ON Auditories INSTEAD OF INSERT
AS
BEGIN
	IF EXISTS
	(
		SELECT 1
		FROM [Auditories] O
		INNER JOIN Inserted I
		ON O.AuditoryNumber = I.AuditoryNumber
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate auditory',10,1)
        return
	END

	-- APPLY INSERT
		INSERT INTO [Auditories]
		SELECT AuditoryNumber,NumberOfComputers,ComputersType
		 FROM Inserted
END
GO

CREATE TRIGGER AuditoriesUpdateCheckTrigger ON Auditories INSTEAD OF UPDATE
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1
		FROM Inserted I
		INNER JOIN Deleted D
		ON I.AuditoryNumber = D.AuditoryNumber
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate auditory',10,1)
        return
	END

	-- APPLY UPDATE
		UPDATE [Auditories]
		SET AuditoryNumber = I.AuditoryNumber, NumberOfComputers = I.NumberOfComputers, ComputersType = I.ComputersType
		FROM Inserted I, Deleted D
		WHERE [Auditories].AuditoryNumber = D.AuditoryNumber
END
GO
-------------------------------------------------------------

--------------- [Schools] triggers -----------------------

CREATE TRIGGER SchoolsLogTrigger ON Schools AFTER INSERT, UPDATE, DELETE
AS
-- AFTER INSERT WRITE TO LOG
IF NOT EXISTS( SELECT * FROM Deleted )
	INSERT INTO SummaryLog
	SELECT 'Schools',
		'Insert',
		('SchoolNumber: ' + CONVERT(nvarchar(10),inserted.SchoolNumber) + ' Region: ' + inserted.Region),
		GETDATE()
	FROM inserted
-- AFTER DELETE WRITE TO LOG
ELSE IF NOT EXISTS ( SELECT * FROM Inserted )
	INSERT INTO SummaryLog
	SELECT 'Schools',
		'Delete',
		('SchoolNumber: ' + CONVERT(nvarchar(10),deleted.SchoolNumber) + ' Region: ' + deleted.Region),
		GETDATE()
	FROM deleted
-- AFTER UPDATE WRITE TO LOG
ELSE
	BEGIN
		INSERT INTO SummaryLog
		SELECT 'Schools',
		'Update',
		('SchoolNumber: ' + CONVERT(nvarchar(10),deleted.SchoolNumber) + ' -> ' + CONVERT(nvarchar(10),inserted.SchoolNumber) + ' Region: ' + deleted.Region + ' -> ' + inserted.Region),
		GETDATE()
		FROM inserted,deleted
	END
GO

CREATE TRIGGER SchoolsDuplicateCheckTrigger ON Schools INSTEAD OF INSERT
AS
BEGIN
	IF EXISTS
	(
		SELECT 1
		FROM [Schools] O
		INNER JOIN Inserted I
		ON O.SchoolNumber = I.SchoolNumber
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate school',10,1)
        return
	END

	-- APPLY INSERT
		INSERT INTO [Schools]
		SELECT SchoolNumber,Region
		 FROM Inserted
END
GO

CREATE TRIGGER SchoolsUpdateCheckTrigger ON Schools INSTEAD OF UPDATE
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1
		FROM Inserted I
		INNER JOIN Deleted D
		ON I.SchoolNumber = D.SchoolNumber
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate school',10,1)
        return
	END


	-- APPLY UPDATE
		UPDATE [Schools]
		SET SchoolNumber = I.SchoolNumber, Region = I.Region
		FROM Inserted I, Deleted D
		WHERE [Schools].SchoolNumber = D.SchoolNumber
END
GO
-------------------------------------------------------------

--------------- [Examiners] triggers -----------------------

CREATE TRIGGER ExaminersLogTrigger ON Examiners AFTER INSERT, UPDATE, DELETE
AS
-- AFTER INSERT WRITE TO LOG
IF NOT EXISTS( SELECT * FROM Deleted )
	INSERT INTO SummaryLog
	SELECT 'Examiners',
		'Insert',
		('ExaminerFullName: ' + inserted.ExaminerSurname + ' ' + inserted.ExaminerName + ' ' + inserted.ExaminerSecondName + ' BirthDate: ' + CONVERT(nvarchar(20),inserted.BirthDate) + ' Position: ' + inserted.Position),
		GETDATE()
	FROM inserted
-- AFTER DELETE WRITE TO LOG
ELSE IF NOT EXISTS ( SELECT * FROM Inserted )
	INSERT INTO SummaryLog
	SELECT 'Examiners',
		'Delete',
		('ExaminerFullName: ' + deleted.ExaminerSurname + ' ' + deleted.ExaminerName + ' ' + deleted.ExaminerSecondName + ' BirthDate: ' + CONVERT(nvarchar(20),deleted.BirthDate) + ' Position: ' + deleted.Position),
		GETDATE()
	FROM deleted
-- AFTER UPDATE WRITE TO LOG
ELSE
	BEGIN
		INSERT INTO SummaryLog
		SELECT 'Examiners',
		'Update',
		('ExaminerFullName: ' + deleted.ExaminerSurname + ' ' + deleted.ExaminerName + ' ' + deleted.ExaminerSecondName + ' -> ' + inserted.ExaminerSurname + ' ' + inserted.ExaminerName + ' ' + inserted.ExaminerSecondName + ' BirthDate: ' + CONVERT(nvarchar(20),deleted.BirthDate) + ' -> ' + CONVERT(nvarchar(20),inserted.BirthDate) + ' Position: ' + deleted.Position + ' -> ' + inserted.Position),
		GETDATE()
		FROM inserted,deleted
	END
GO

CREATE TRIGGER ExaminersDuplicateCheckTrigger ON Examiners INSTEAD OF INSERT,UPDATE
AS
BEGIN
	IF EXISTS
	(
		SELECT 1
		FROM [Examiners] O
		INNER JOIN Inserted I
		ON O.ExaminerName = I.ExaminerName AND
			O.ExaminerSurname = I.ExaminerSurname AND
			O.ExaminerSecondName = I.ExaminerSecondName
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate examiner',10,1)
        return
	END

	-- APPLY INSERT
	IF NOT EXISTS( SELECT * FROM Deleted )
		INSERT INTO [Examiners]
		SELECT ExaminerName,ExaminerSurname,ExaminerSecondName,BirthDate,Position
		 FROM Inserted
	-- APPLY UPDATE
	ELSE
		UPDATE [Examiners]
		SET ExaminerName = I.ExaminerName, ExaminerSurname = I.ExaminerSurname, ExaminerSecondName = I.ExaminerSecondName, BirthDate = I.BirthDate, Position = I.Position
		FROM Inserted I, Deleted D
		WHERE [Examiners].ExaminerID = D.ExaminerID
END
GO
-------------------------------------------------------------

--------------- [Participants] triggers -----------------------

CREATE TRIGGER ParticipantsLogTrigger ON Participants AFTER INSERT, UPDATE, DELETE
AS
-- AFTER INSERT WRITE TO LOG
IF NOT EXISTS( SELECT * FROM Deleted )
	INSERT INTO SummaryLog
	SELECT 'Participants',
		'Insert',
		('ParticipantFullName: ' + inserted.ParticipantSurname + ' ' + inserted.ParticipantName + ' ' + inserted.ParticipantSecondName + ' BirthDate: ' + CONVERT(nvarchar(20),inserted.BirthDate) + ' SchoolNumber: ' + CONVERT(nvarchar(10),inserted.SchoolNumber) + ' TeacherSurname: ' + inserted.TeacherSurname),
		GETDATE()
	FROM inserted
-- AFTER DELETE WRITE TO LOG
ELSE IF NOT EXISTS ( SELECT * FROM Inserted )
	INSERT INTO SummaryLog
	SELECT 'Participants',
		'Delete',
		('ParticipantFullName: ' + deleted.ParticipantSurname + ' ' + deleted.ParticipantName + ' ' + deleted.ParticipantSecondName + ' BirthDate: ' + CONVERT(nvarchar(20),deleted.BirthDate) + ' SchoolNumber: ' + CONVERT(nvarchar(10),deleted.SchoolNumber) + ' TeacherSurname: ' + deleted.TeacherSurname),
		GETDATE()
	FROM deleted
-- AFTER UPDATE WRITE TO LOG
ELSE
	BEGIN
		INSERT INTO SummaryLog
		SELECT 'Participants',
		'Update',
		('ParticipantFullName: ' + deleted.ParticipantSurname + ' ' + deleted.ParticipantName + ' ' + deleted.ParticipantSecondName + ' -> '  + inserted.ParticipantSurname + ' ' + inserted.ParticipantName + ' ' + inserted.ParticipantSecondName + ' BirthDate: ' + CONVERT(nvarchar(20),deleted.BirthDate) +'->' + CONVERT(nvarchar(20),inserted.BirthDate)  + ' SchoolNumber: ' + CONVERT(nvarchar(10),deleted.SchoolNumber) + ' -> ' + CONVERT(nvarchar(10),inserted.SchoolNumber) + ' TeacherSurname: ' + deleted.TeacherSurname + ' -> ' + inserted.TeacherSurname),
		GETDATE()
		FROM inserted,deleted
	END
GO

CREATE TRIGGER ParticipantsDuplicateCheckTrigger ON Participants INSTEAD OF INSERT,UPDATE
AS
BEGIN
	IF EXISTS
	(
		SELECT 1
		FROM [Participants] O
		INNER JOIN Inserted I
		ON O.ParticipantName = I.ParticipantName AND
			O.ParticipantSurname = I.ParticipantSurname AND
			O.ParticipantSecondName = I.ParticipantSecondName
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate participant',10,1)
        return
	END

	-- APPLY INSERT
	IF NOT EXISTS( SELECT * FROM Deleted )
		INSERT INTO [Participants]
		SELECT ParticipantName,ParticipantSurname,ParticipantSecondName,BirthDate,SchoolNumber,TeacherSurname
		 FROM Inserted
	-- APPLY UPDATE
	ELSE
		UPDATE [Participants]
		SET ParticipantName = I.ParticipantName, ParticipantSurname = I.ParticipantSurname, ParticipantSecondName = I.ParticipantSecondName, BirthDate = I.BirthDate, SchoolNumber = I.SchoolNumber, TeacherSurname = I.TeacherSurname
		FROM Inserted I, Deleted D
		WHERE [Participants].ParticipantID = D.ParticipantID
END
GO
-------------------------------------------------------------

--------------- [Tasks] triggers -----------------------

CREATE TRIGGER TasksLogTrigger ON Tasks AFTER INSERT, UPDATE, DELETE
AS
-- AFTER INSERT WRITE TO LOG
IF NOT EXISTS( SELECT * FROM Deleted )
	INSERT INTO SummaryLog
	SELECT 'Tasks',
		'Insert',
		('OlympiadID: ' + CONVERT(nvarchar(20),inserted.OlympiadID) + ' TaskNumber: ' + CONVERT(nvarchar(20),inserted.TaskNumber) + ' Condition: ' + inserted.Condition + ' MaximalScore: ' + CONVERT(nvarchar(20),inserted.MaximalScore)),
		GETDATE()
	FROM inserted
-- AFTER DELETE WRITE TO LOG
ELSE IF NOT EXISTS ( SELECT * FROM Inserted )
	INSERT INTO SummaryLog
	SELECT 'Tasks',
		'Delete',
		('OlympiadID: ' + CONVERT(nvarchar(20),deleted.OlympiadID) + ' TaskNumber: ' + CONVERT(nvarchar(20),deleted.TaskNumber) + ' Condition: ' + deleted.Condition + ' MaximalScore: ' + CONVERT(nvarchar(20),deleted.MaximalScore)),
		GETDATE()
	FROM deleted
-- AFTER UPDATE WRITE TO LOG
ELSE
	BEGIN
		INSERT INTO SummaryLog
		SELECT 'Tasks',
		'Update',
		('OlympiadID: ' + CONVERT(nvarchar(20),deleted.OlympiadID) + ' -> ' + CONVERT(nvarchar(20),inserted.OlympiadID) + ' TaskNumber: ' + CONVERT(nvarchar(20),deleted.TaskNumber) + ' -> ' + CONVERT(nvarchar(20),inserted.TaskNumber) + ' Condition: ' + deleted.Condition + ' -> ' + inserted.Condition + ' MaximalScore: ' + CONVERT(nvarchar(20),deleted.MaximalScore) + ' -> ' + CONVERT(nvarchar(20),inserted.MaximalScore)),
		GETDATE()
		FROM inserted,deleted
	END
GO

CREATE TRIGGER TasksDuplicateCheckTrigger ON Tasks INSTEAD OF INSERT
AS
BEGIN
	IF EXISTS
	(
		SELECT 1
		FROM [Tasks] O
		INNER JOIN Inserted I
		ON O.OlympiadID = I.OlympiadID AND
			O.TaskNumber = I.TaskNumber
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate tasks',10,1)
        return
	END

	-- APPLY INSERT
		INSERT INTO [Tasks]
		SELECT OlympiadID,TaskNumber,Condition,MaximalScore
		 FROM Inserted
END
GO

CREATE TRIGGER TasksUpdateCheckTrigger ON Tasks INSTEAD OF UPDATE
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1
		FROM Inserted I
		INNER JOIN Deleted D
		ON I.OlympiadID = D.OlympiadID AND
			I.TaskNumber = D.TaskNumber
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate tasks',10,1)
        return
	END

	-- APPLY UPDATE
		UPDATE [Tasks]
		SET OlympiadID = I.OlympiadID, TaskNumber = I.TaskNumber, Condition = I.Condition, MaximalScore = I.MaximalScore
		FROM Inserted I, Deleted D
		WHERE [Tasks].OlympiadID = D.OlympiadID AND
			[Tasks].TaskNumber = D.TaskNumber
END
GO
-------------------------------------------------------------	

--------------- [OlympiadDetails] triggers -----------------------

CREATE TRIGGER OlympiadDetailsLogTrigger ON OlympiadDetails AFTER INSERT, UPDATE, DELETE
AS
-- AFTER INSERT WRITE TO LOG
IF NOT EXISTS( SELECT * FROM Deleted )
	INSERT INTO SummaryLog
	SELECT 'OlympiadDetails',
		'Insert',
		('OlympiadID: ' + CONVERT(nvarchar(20),inserted.OlympiadID) + ' ParticipantID: ' + CONVERT(nvarchar(20),inserted.ParticipantID) +' TaskNumber: ' + CONVERT(nvarchar(20),inserted.TaskNumber) + ' Score: ' + CONVERT(nvarchar(20),inserted.Score) + ' ExaminerID: ' + CONVERT(nvarchar(20),inserted.ExaminerID)),
		GETDATE()
	FROM inserted
-- AFTER DELETE WRITE TO LOG
ELSE IF NOT EXISTS ( SELECT * FROM Inserted )
	INSERT INTO SummaryLog
	SELECT 'OlympiadDetails',
		'Delete',
		('OlympiadID: ' + CONVERT(nvarchar(20),deleted.OlympiadID) + ' ParticipantID: ' + CONVERT(nvarchar(20),deleted.ParticipantID) +' TaskNumber: ' + CONVERT(nvarchar(20),deleted.TaskNumber) + ' Score: ' + CONVERT(nvarchar(20),deleted.Score) + ' ExaminerID: ' + CONVERT(nvarchar(20),deleted.ExaminerID)),
		GETDATE()
	FROM deleted
-- AFTER UPDATE WRITE TO LOG
ELSE
	BEGIN
		INSERT INTO SummaryLog
		SELECT 'OlympiadDetails',
		'Update',
		('OlympiadID: ' + CONVERT(nvarchar(20),deleted.OlympiadID) + ' -> ' + CONVERT(nvarchar(20),inserted.OlympiadID) + ' ParticipantID: ' + CONVERT(nvarchar(20),deleted.ParticipantID) + ' -> ' + CONVERT(nvarchar(20),inserted.ParticipantID) +' TaskNumber: ' + CONVERT(nvarchar(20),deleted.TaskNumber) + ' -> ' + CONVERT(nvarchar(20),inserted.TaskNumber) + ' Score: ' + CONVERT(nvarchar(20),deleted.Score) + ' -> ' + CONVERT(nvarchar(20),inserted.Score) + ' ExaminerID: ' + CONVERT(nvarchar(20),deleted.ExaminerID) + ' -> ' + CONVERT(nvarchar(20),inserted.ExaminerID)),
		GETDATE()
		FROM inserted,deleted
	END
GO

CREATE TRIGGER OlympiadDetailsDuplicateCheckTrigger ON OlympiadDetails INSTEAD OF INSERT
AS
BEGIN
	IF EXISTS
	(
		SELECT 1
		FROM [OlympiadDetails] O
		INNER JOIN Inserted I
		ON O.OlympiadID = I.OlympiadID AND
			O.ParticipantID = I.ParticipantID AND
			O.TaskNumber = I.TaskNumber
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate olympiad details',10,1)
        return
	END

	IF EXISTS
	(
		SELECT 1
		FROM (SELECT Score,MaximalScore FROM Inserted I Join [Tasks] T ON I.TaskNumber = T.TaskNumber AND I.OlympiadID = T.OlympiadID) as X
		WHERE X.Score > X.MaximalScore
	)
	BEGIN
		 -- Bad score handling
        PRINT 'Bad score'
        raiserror('Cant submit score > maximal score',10,1)
        return
	END

	-- APPLY INSERT
		INSERT INTO [OlympiadDetails]
		SELECT OlympiadID,ParticipantID,TaskNumber,Score,ExaminerID
		 FROM Inserted
END
GO

CREATE TRIGGER OlympiadDetailsUpdateCheckTrigger ON OlympiadDetails INSTEAD OF UPDATE
AS
BEGIN
	IF NOT EXISTS
	(
		SELECT 1
		FROM Inserted I
		INNER JOIN Deleted D
		ON I.OlympiadID = D.OlympiadID AND
			I.ParticipantID = D.ParticipantID AND
			I.TaskNumber = D.TaskNumber
	)
	BEGIN
		 -- Duplicate handling
        PRINT 'Duplicate'
        raiserror('Cant submit duplicate olympiad details',10,1)
        return
	END

	IF EXISTS
	(
		SELECT 1
		FROM (SELECT Score,MaximalScore FROM Inserted I Join [Tasks] T ON I.TaskNumber = T.TaskNumber AND I.OlympiadID = T.OlympiadID) as X
		WHERE X.Score > X.MaximalScore
	)
	BEGIN
		 -- Bad score handling
        PRINT 'Bad score'
        raiserror('Cant submit score > maximal score',10,1)
        return
	END

	-- APPLY UPDATE
		UPDATE [OlympiadDetails]
		SET OlympiadID = I.OlympiadID, ParticipantID = I.ParticipantID, TaskNumber = I.TaskNumber, Score = I.Score, ExaminerID = I.ExaminerID
		FROM Inserted I, Deleted D
		WHERE [OlympiadDetails].OlympiadID = D.OlympiadID AND
			[OlympiadDetails].ParticipantID = D.ParticipantID AND
			[OlympiadDetails].TaskNumber = D.TaskNumber
END
GO
-------------------------------------------------------------