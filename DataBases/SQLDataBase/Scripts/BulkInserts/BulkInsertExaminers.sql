-- =============================================
-- Script Template
-- =============================================
USE master
IF DB_ID('SchoolOlympiads') IS NOT NULL
BEGIN
USE [SchoolOlympiads]
DELETE FROM Examiners
BULK INSERT Examiners
	FROM 'C:\Users\Deliany\Desktop\SQLDataBase\BulkInsert\Examiners.txt'
	WITH
	(
		FIELDTERMINATOR = ';',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	)
END
