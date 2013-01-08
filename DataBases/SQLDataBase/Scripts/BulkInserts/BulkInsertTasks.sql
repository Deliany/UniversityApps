-- =============================================
-- Script Template
-- =============================================
USE master
IF DB_ID('SchoolOlympiads') IS NOT NULL
BEGIN
USE [SchoolOlympiads]
DELETE FROM Tasks
BULK INSERT Tasks
	FROM 'C:\Users\Deliany\Desktop\SQLDataBase\BulkInsert\Tasks.txt'
	WITH
	(
		FIELDTERMINATOR = ';',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	)
END
