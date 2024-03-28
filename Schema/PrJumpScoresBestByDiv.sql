USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrJumpScoresBestByDiv ') IS NOT NULL
	DROP PROCEDURE dbo.PrJumpScoresBestByDiv 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrJumpScoresBestByDiv 
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament jump scores sequenced by best score per division 
Value Values: 	Div (All or any valid age group code 
**************************************************************************** */
CREATE PROCEDURE dbo.PrJumpScoresBestByDiv  @InSanctionId AS varchar(6), @InDiv as varchar(3) = 'All'
AS
BEGIN
	CREATE TABLE #BestScore ( AgeGroup varchar(12), MemberId char(9), ScoreBest numeric(7,2) )
	Insert INTO #BestScore (AgeGroup, MemberId, ScoreBest)
		Select AgeGroup, MemberId, Max(ScoreFeet)
		From JumpScore
		Where SanctionId = @InSanctionId AND Round < 25 AND (@InDiv = 'All' OR AgeGroup = @InDiv)
		Group by AgeGroup, MemberId
	
	
	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
	, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
	, ER.RankingRating, ER.RankingScore
	, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, ScoreBest
	, (Select RO.ScoreFeet From JumpScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
	, TRIM(CAST(ROUND(SS.ScoreFeet, 0) AS CHAR)) + 'FT (' + TRIM(CAST(ROUND(SS.ScoreMeters, 1) AS CHAR)) + 'M)' AS EventScoreDesc
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Jump'
	INNER JOIN #BestScore AS BS ON BS.MemberId = TR.MemberId AND BS.AgeGroup = TR.AgeGroup 
	INNER JOIN JumpScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup 
		AND SS.PK = (Select Min(PK) From JumpScore PK Where PK.MemberId = BS.MemberId AND PK.SanctionId = TR.SanctionId AND PK.AgeGroup = BS.AgeGroup AND PK.Round < 25 AND PK.ScoreFeet = BS.ScoreBest)
	Where TR.SanctionId = @InSanctionId AND (@InDiv = 'All' OR TR.AgeGroup = @InDiv)
	Order by TR.AgeGroup ASC, ReadyForPlcmt ASC, ScoreBest DESC, SS.ScoreMeters DESC, ScoreRunoff DESC
	
END
GO

GRANT EXECUTE ON dbo.PrJumpScoresBestByDiv  TO PUBLIC
GO
