-- =============================================
-- Script Template
-- =============================================
USE master
IF DB_ID('SchoolOlympiads') IS NOT NULL
BEGIN
USE [SchoolOlympiads]
DELETE FROM Participants
BULK INSERT Participants
	FROM 'C:\Users\Deliany\Desktop\SQLDataBase\BulkInsert\Participants.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	)
END
