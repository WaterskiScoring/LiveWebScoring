USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrGetMostRecentScores') IS NOT NULL
	DROP PROCEDURE dbo.PrGetMostRecentScores
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrGetMostRecentScores
Author:			David Allen
Create date: 	May 1, 2024
Description:	Retrieve tournament scores with a ski time in the last xx minutes of the system time.
				System time is adjusted for the tournament time zone 

**************************************************************************** */
CREATE PROCEDURE dbo.PrGetMostRecentScores @InSanctionId AS varchar(6), @InLastMinuteCheck as int = 0
AS
BEGIN
	DECLARE @curDateFilter varchar(20);
	DECLARE @curDateFilterSlalom varchar(20);
	DECLARE @curDateFilterTrick varchar(20);
	DECLARE @curDateFilterJump varchar(20);
	DECLARE @curTourTimeZone varchar(64);
	DECLARE @curSystemTimeZone varchar(64);
	SET @curSystemTimeZone = 'Eastern Standard Time';

	SET @curTourTimeZone = (Select CodeValue From Tournament INNER JOIN CodeValueList on ListName = 'TimeZoneByState' AND ListCode = Right(EventLocation, 2) Where SanctionId  = @InSanctionId);
	SET @curTourTimeZone = (Select COALESCE(@curTourTimeZone, @curSystemTimeZone));

	IF @InLastMinuteCheck = 0 BEGIN
		SET @curDateFilterSlalom = (Select Max(DATEADD (MINUTE, -1, InsertDate)) From SlalomScore Where SanctionId =  @InSanctionId);
		SET @curDateFilterTrick = (Select Max(DATEADD (MINUTE, -1, InsertDate)) From TrickScore Where SanctionId =  @InSanctionId);
		SET @curDateFilterJump = (Select Max(DATEADD (MINUTE, -1, InsertDate)) From JumpScore Where SanctionId =  @InSanctionId);
	END
	ELSE BEGIN
		DECLARE @curSystemDateAdj DateTime;
		SET @curSystemDateAdj = (Select DATEADD (MINUTE, @InLastMinuteCheck, GetDate() ) AT TIME ZONE @curSystemTimeZone AT TIME ZONE @curTourTimeZone );
		SET @curDateFilterSlalom = @curSystemDateAdj;
		SET @curDateFilterTrick = @curSystemDateAdj;
		SET @curDateFilterJump = @curSystemDateAdj;
	END

	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
		, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
		, ER.RankingRating, ER.RankingScore
		, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore
		, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(SS.FinalLenOff) 
			+ ' (' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(SS.FinalLen) + 'm) ' 
			+ CAST(SS.Score AS CHAR)+ ' Buoys' AS EventScoreDesc
		, SS.InsertDate, Convert(DateTime, @curDateFilterSlalom) as DateFilter
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Slalom' 
	INNER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
	Where TR.SanctionId =  @InSanctionId
	  AND SS.InsertDate > @curDateFilterSlalom
	
	UNION

	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
		, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
		, ER.RankingRating, ER.RankingScore
		, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore
		, TRIM(CAST(SS.Score AS CHAR)) + ' POINTS (P1:' + TRIM(CAST(SS.ScorePass1 AS CHAR)) + ' P2:' + TRIM(CAST(SS.ScorePass2 AS CHAR)) + ')' AS EventScoreDesc
		, SS.InsertDate, Convert(DateTime, @curDateFilterTrick) as DateFilter
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Trick' 
	INNER JOIN TrickScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
	Where TR.SanctionId =  @InSanctionId
	  AND SS.InsertDate > @curDateFilterTrick
	  
	UNION

	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
		, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
		, ER.RankingRating, ER.RankingScore
		, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore
		, TRIM(CAST(ROUND(SS.ScoreFeet, 0) AS CHAR)) + 'FT (' + TRIM(CAST(ROUND(SS.ScoreMeters, 1) AS CHAR)) + 'M)' AS EventScoreDesc
		, SS.InsertDate, Convert(DateTime, @curDateFilterJump) as DateFilter
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Jump' 
	INNER JOIN JumpScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
	Where TR.SanctionId =  @InSanctionId
	  AND SS.InsertDate > @curDateFilterJump

	Order by TR.SanctionId, SS.InsertDate DESC;

END
GO

GRANT EXECUTE ON dbo.PrGetMostRecentScores TO PUBLIC
GO
