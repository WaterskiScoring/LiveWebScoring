USE LiveWebScoreboard
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  OBJECT_ID ('dbo.vTrickResults') IS NOT NULL
	DROP VIEW dbo.vTrickResults
GO

/* ****************************************************************************
Object:			View
Author:			David Allen
Create date:	January 30, 2024
Description:	Retrieve trick scores
**************************************************************************** */
CREATE VIEW dbo.vTrickResults AS 

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
LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event
LEFT OUTER JOIN TrickVideo AS TV ON TV.SanctionId = TR.SanctionId AND TV.MemberId = TR.MemberId AND TV.AgeGroup = TR.AgeGroup AND TV.Round = SS.Round
GO

GRANT SELECT ON dbo.vTrickResults TO PUBLIC
GO
