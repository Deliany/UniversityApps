-- =============================================
-- Script Template
-- =============================================
USE [SchoolOlympiads] 
GO
IF OBJECT_ID ( 'GenerateReport', 'P' ) IS NOT NULL 
    DROP PROCEDURE GenerateReport;
GO

CREATE PROCEDURE [GenerateReport] (@olympID int = 1)
AS
	BEGIN

	--Check existing for @olympId parameter
	IF(EXISTS(SELECT * FROM Olympiads WHERE OlympiadID = @olympID))
		BEGIN

		--Drop views if they exists	
		IF OBJECT_ID('OlympiadSummary', 'V') IS NOT NULL
			DROP VIEW OlympiadSummary;
		IF OBJECT_ID('Winners', 'V') IS NOT NULL
			DROP VIEW Winners;
		/*=====================================*/

		--Create dynamic column names
		DECLARE @cols NVARCHAR(2000);
		SELECT @cols = COALESCE(@cols + ',[' +CONVERT(nvarchar,TaskNumber) +']',
										'['+CONVERT(nvarchar,TaskNumber)+']')
			FROM [Tasks]
			WHERE OlympiadID=@olympID;
			

		/*=====================================*/
		
		-- create view with dynamic number of columns 
		-- based on TaskNumber values
		DECLARE @query NVARCHAR(4000);

		SET @query = N'
					CREATE VIEW [OlympiadSummary]
					AS 
					SELECT ParticipantSurname,
					       ParticipantName,
					       ParticipantSecondName,
					       SchoolNumber,'
					       + @cols +',
					       SumScore
					FROM
					(
						SELECT ParticipantSurname,
						ParticipantName,
						ParticipantSecondName,
						t2.SchoolNumber as SchoolNumber,
						CONVERT(nvarchar,TaskNumber) as ColName,
						Score,
						(
							SELECT SUM(Score) 
							FROM OlympiadDetails 
							WHERE OlympiadDetails.ParticipantID=t1.ParticipantID
						) as SumScore
							FROM [OlympiadDetails] AS t1
							JOIN  [Participants] AS t2 ON t1.ParticipantID = t2.ParticipantID
							JOIN [Schools] AS t3 ON t2.SchoolNumber=t3.SchoolNumber
							WHERE OlympiadID=' + CONVERT(nvarchar, @olympID) + '
					) AS data 
					PIVOT
				    (	
						MAX([Score])
						FOR ColName IN
						( '+ @cols +' )
					) AS PVT';

					EXECUTE(@query);
					/*===============================================*/
					
					--Create view with olympiad winners
					 exec('CREATE VIEW [Winners]
					 AS
					 SELECT TOP 6 *
						FROM OlympiadSummary
						ORDER BY SumScore DESC');
	 
					END
	 END
GO
