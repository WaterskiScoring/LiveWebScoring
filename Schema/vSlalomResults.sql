USE LiveWebScoreboard
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  OBJECT_ID ('dbo.vSlalomResults') IS NOT NULL
	DROP VIEW dbo.vSlalomResults
GO

/* ****************************************************************************
Object:			View
Author:			David Allen
Create date:	January 30, 2024
Description:	Retrieve slalom scores
**************************************************************************** */
CREATE VIEW dbo.vSlalomResults AS 

SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
, ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder, '' as RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder
, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, SS.Score AS EventScore
, (Select RO.Score From SlalomScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
, CAST(SS.Score AS CHAR) AS Buoys
, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(SS.FinalLenOff) + ' (' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(SS.FinalLen) + 'm)' AS EventScoreDesc
, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(SS.FinalLen) + 'm' AS EventScoreDescMeteric
, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(SS.FinalLenOff) AS EventScoreDescImperial
, SS.InsertDate, SS.LastUpdateDate, SS.LastUpdateDate AS SortLastUpdateDate 
FROM TourReg AS TR 
INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Slalom' 
LEFT OUTER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup 
LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event 
GO

GRANT SELECT ON dbo.vSlalomResults TO PUBLIC
GO
