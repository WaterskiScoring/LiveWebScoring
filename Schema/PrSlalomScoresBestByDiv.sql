USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrSlalomScoresBestByDiv ') IS NOT NULL
	DROP PROCEDURE dbo.PrSlalomScoresBestByDiv 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrSlalomScoresBestByDiv 
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament slalom scores sequenced by best score per division 
Value Values: 	Div (All or any valid age group code 
**************************************************************************** */
CREATE PROCEDURE dbo.PrSlalomScoresBestByDiv  @InSanctionId AS varchar(6), @InDiv as varchar(3) = 'All'
AS
BEGIN
	CREATE TABLE #BestScore ( AgeGroup varchar(12), MemberId char(9), ScoreBest numeric(7,2) )
	Insert INTO #BestScore (AgeGroup, MemberId, ScoreBest)
		Select AgeGroup, MemberId, Max(Score)
		From SlalomScore
		Where SanctionId = @InSanctionId AND Round < 25 AND (@InDiv = 'All' OR AgeGroup = @InDiv)
		Group by AgeGroup, MemberId
	
	
	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
	, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
	, ER.RankingRating, ER.RankingScore
	, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, ScoreBest, CAST(SS.Score AS CHAR) AS Buoys
	, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(SS.FinalLenOff) + ' (' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(SS.FinalLen) + 'm)' AS EventScoreDesc
	, (Select RO.Score From SlalomScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Slalom'
	INNER JOIN #BestScore AS BS ON BS.MemberId = TR.MemberId AND BS.AgeGroup = TR.AgeGroup 
	INNER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup 
		AND SS.PK = (Select Min(PK) From SlalomScore PK Where PK.MemberId = BS.MemberId AND PK.SanctionId = TR.SanctionId AND PK.AgeGroup = BS.AgeGroup AND PK.Round < 25 AND PK.Score = BS.ScoreBest)
	Where TR.SanctionId = @InSanctionId AND (@InDiv = 'All' OR TR.AgeGroup = @InDiv)
	Order by TR.AgeGroup ASC, ReadyForPlcmt ASC, ScoreBest DESC, ScoreRunoff DESC
	
END
GO

GRANT EXECUTE ON dbo.PrSlalomScoresBestByDiv  TO PUBLIC
GO
