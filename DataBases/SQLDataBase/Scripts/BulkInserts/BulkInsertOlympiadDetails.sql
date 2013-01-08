-- =============================================
-- Script Template
-- =============================================
USE master
IF DB_ID('SchoolOlympiads') IS NOT NULL
BEGIN
USE [SchoolOlympiads]
DELETE FROM OlympiadDetails
BULK INSERT OlympiadDetails
	FROM 'C:\Users\Deliany\Desktop\SQLDataBase\BulkInsert\OlympiadDetails.txt'
	WITH
	(
		FIELDTERMINATOR = ',',
		ROWTERMINATOR = '\n',
		DATAFILETYPE = 'widechar'
	)
END
