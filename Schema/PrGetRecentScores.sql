USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrGetRecentScores') IS NOT NULL
	DROP PROCEDURE dbo.PrGetRecentScores
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrGetRecentScores
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament slalom scores sequenced by time skied regardless of event

**************************************************************************** */
CREATE PROCEDURE dbo.PrGetRecentScores @InSanctionId AS varchar(6), @InUseLastActive as bit = 0
AS
BEGIN
	DECLARE @curDateFilter varchar(20);
	IF @InUseLastActive = 1
		SET @curDateFilter = (Select TRIM(CAST( Convert(Date, Max(InsertDate)) AS CHAR)) + ' 00:00:00' as LastScoredDate From SlalomScore Where SanctionId =  @InSanctionId);
	ELSE
		SET @curDateFilter = (Select TRIM(CAST( CONVERT (date, GETDATE()) AS CHAR)) + ' 00:00:00');

	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
		, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
		, ER.RankingRating, ER.RankingScore
		, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore
		, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(SS.FinalLenOff) 
			+ ' (' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(SS.FinalLen) + 'm) ' 
			+ CAST(SS.Score AS CHAR)+ ' Buoys' AS EventScoreDesc
		, SS.InsertDate
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Slalom' 
	INNER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
	Where TR.SanctionId =  @InSanctionId
	  AND SS.InsertDate > @curDateFilter
	
	UNION

	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
		, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
		, ER.RankingRating, ER.RankingScore
		, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore
		, TRIM(CAST(SS.Score AS CHAR)) + ' POINTS (P1:' + TRIM(CAST(SS.ScorePass1 AS CHAR)) + ' P2:' + TRIM(CAST(SS.ScorePass2 AS CHAR)) + ')' AS EventScoreDesc
		, SS.InsertDate
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Trick' 
	INNER JOIN TrickScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
	Where TR.SanctionId =  @InSanctionId
	  AND SS.InsertDate > @curDateFilter

	UNION

	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
		, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
		, ER.RankingRating, ER.RankingScore
		, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore
		, TRIM(CAST(ROUND(SS.ScoreFeet, 0) AS CHAR)) + 'FT (' + TRIM(CAST(ROUND(SS.ScoreMeters, 1) AS CHAR)) + 'M)' AS EventScoreDesc
		, SS.InsertDate
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Jump' 
	INNER JOIN JumpScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
	Where TR.SanctionId =  @InSanctionId
	  AND SS.InsertDate > @curDateFilter

	Order by TR.SanctionId, SS.InsertDate DESC;

END
GO

GRANT EXECUTE ON dbo.PrGetRecentScores TO PUBLIC
GO
