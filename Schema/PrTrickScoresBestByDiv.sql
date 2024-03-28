USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrTrickScoresBestByDiv ') IS NOT NULL
	DROP PROCEDURE dbo.PrTrickScoresBestByDiv 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrTrickScoresBestByDiv 
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament slalom scores sequenced by best score per division 
Value Values: 	Div (All or any valid age group code 

SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
, ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder, '' as RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder
, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, SS.Score AS EventScore
, TRIM(CAST(SS.Score AS CHAR)) + ' POINTS (P1:' + TRIM(CAST(SS.ScorePass1 AS CHAR)) + ' P2:' + TRIM(CAST(SS.ScorePass2 AS CHAR)) + ')' AS EventScoreDesc
, TV.Pass1VideoUrl, TV.Pass2VideoUrl
, (Select RO.Score From TrickScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
, SS.InsertDate, SS.LastUpdateDate, SS.LastUpdateDate AS SortLastUpdateDate
FROM TourReg AS TR 
INNER JOIN EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Trick'
INNER JOIN TrickScore AS SS ON SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup 

**************************************************************************** */
CREATE PROCEDURE dbo.PrTrickScoresBestByDiv  @InSanctionId AS varchar(6), @InDiv as varchar(3) = 'All'
AS
BEGIN
	CREATE TABLE #BestScore ( AgeGroup varchar(12), MemberId char(9), ScoreBest numeric(7,2) )
	Insert INTO #BestScore (AgeGroup, MemberId, ScoreBest)
		Select AgeGroup, MemberId, Max(Score)
		From TrickScore
		Where SanctionId = @InSanctionId AND Round < 25 AND (@InDiv = 'All' OR AgeGroup = @InDiv)
		Group by AgeGroup, MemberId
	
	
	SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
	, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
	, ER.RankingRating, ER.RankingScore
	, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, ScoreBest
	, TRIM(CAST(SS.Score AS CHAR)) + ' POINTS (P1:' + TRIM(CAST(SS.ScorePass1 AS CHAR)) + ' P2:' + TRIM(CAST(SS.ScorePass2 AS CHAR)) + ')' AS EventScoreDesc
	, (Select RO.Score From TrickScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
	, TV.Pass1VideoUrl, TV.Pass2VideoUrl
	FROM TourReg AS TR 
	INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Trick'
	INNER JOIN #BestScore AS BS ON BS.MemberId = TR.MemberId AND BS.AgeGroup = TR.AgeGroup 
	INNER JOIN TrickScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup 
		AND SS.PK = (Select Min(PK) From TrickScore PK Where PK.MemberId = BS.MemberId AND PK.SanctionId = TR.SanctionId AND PK.AgeGroup = BS.AgeGroup AND PK.Round < 25 AND PK.Score = BS.ScoreBest)
	LEFT OUTER JOIN TrickVideo AS TV ON TV.SanctionId = TR.SanctionId AND TV.MemberId = TR.MemberId AND TV.AgeGroup = TR.AgeGroup AND TV.Round = SS.Round
	Where TR.SanctionId = @InSanctionId AND (@InDiv = 'All' OR TR.AgeGroup = @InDiv)
	Order by TR.AgeGroup ASC, ReadyForPlcmt ASC, ScoreBest DESC, ScoreRunoff DESC
	
END
GO

GRANT EXECUTE ON dbo.PrTrickScoresBestByDiv  TO PUBLIC
GO
