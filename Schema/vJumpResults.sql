USE LiveWebScoreboard
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  OBJECT_ID ('dbo.vJumpResults') IS NOT NULL
	DROP VIEW dbo.vJumpResults
GO

/* ****************************************************************************
Object:			View
Author:			David Allen
Create date:	January 30, 2024
Description:	Retrieve Jump scores
**************************************************************************** */
CREATE VIEW dbo.vJumpResults AS 

SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
, ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder, '' as RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder
, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, SS.ScoreFeet AS EventScore, SS.ScoreFeet, ScoreMeters
, (Select RO.ScoreFeet From JumpScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
, TRIM(CAST(ROUND(SS.ScoreFeet, 0) AS CHAR)) + 'FT (' + TRIM(CAST(ROUND(SS.ScoreMeters, 1) AS CHAR)) + 'M)' AS EventScoreDesc
, SS.InsertDate, SS.LastUpdateDate, SS.LastUpdateDate AS SortLastUpdateDate
FROM TourReg AS TR 
INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Jump'
INNER JOIN JumpScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup 
LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event
GO

GRANT SELECT ON dbo.vJumpResults TO PUBLIC
GO
