USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrSlalomScoresLastByDiv ') IS NOT NULL
	DROP PROCEDURE dbo.PrSlalomScoresLastByDiv 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrSlalomScoresLastByDiv 
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament slalom scores sequenced by a skiers last skied round score per division 
Value Values: 	Div (All or any valid age group code 
**************************************************************************** */
CREATE PROCEDURE dbo.PrSlalomScoresLastByDiv  @InSanctionId AS varchar(6), @InDiv as varchar(3) = 'All'
AS
BEGIN
	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
	, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
	, ER.RankingRating, ER.RankingScore
	, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, SS.Score as EventScore, CAST(SS.Score AS CHAR) AS Buoys
	, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(SS.FinalLenOff) + ' (' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(SS.FinalLen) + 'm)' AS EventScoreDesc
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Slalom'
	INNER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup 
		AND SS.Round = (Select Max(Round) as MaxRound 
			From SlalomScore SS2
			Where SS2.SanctionId = SS.SanctionId AND SS2.MemberId = SS.MemberId AND SS2.AgeGroup = SS.AgeGroup AND SS2.Round < 25
			)
	Where TR.SanctionId = @InSanctionId AND (@InDiv = 'All' OR TR.AgeGroup = @InDiv)
	Order by TR.AgeGroup ASC, ReadyForPlcmt ASC, SS.Round DESC, SS.Score DESC
	
END
GO

GRANT EXECUTE ON dbo.PrSlalomScoresLastByDiv  TO PUBLIC
GO
