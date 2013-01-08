-- =============================================
-- Script Template
-- =============================================
USE master
IF DB_ID('SchoolOlympiads') IS NOT NULL
BEGIN
USE [SchoolOlympiads]
DELETE FROM Schools
BULK INSERT Schools
	FROM 'C:\Users\Deliany\Desktop\SQLDataBase\BulkInsert\Schools.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	)
END
