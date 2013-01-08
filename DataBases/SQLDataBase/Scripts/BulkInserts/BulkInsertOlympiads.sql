-- =============================================
-- Script Template
-- =============================================
USE master
IF DB_ID('SchoolOlympiads') IS NOT NULL
BEGIN
USE [SchoolOlympiads]
DELETE FROM Olympiads
BULK INSERT Olympiads
	FROM 'C:\Users\Deliany\Desktop\SQLDataBase\BulkInsert\Olympiads.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	)
END
