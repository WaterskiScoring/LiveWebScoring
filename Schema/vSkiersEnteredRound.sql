USE LiveWebScoreboard
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  OBJECT_ID ('dbo.vSkiersEnteredRound') IS NOT NULL
	DROP VIEW dbo.vSkiersEnteredRound
GO

/* ****************************************************************************
Object:			View
Author:			David Allen
Create date:	January 30, 2024
Description:	Retrieve running order when there is a run order for a round

**************************************************************************** */
CREATE VIEW dbo.vSkiersEnteredRound AS 

SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.City, TR.State, TR.Federation
, ER.Event, O.EventGroup, COALESCE(O.RunOrderGroup, '') as RunOrderGroup, O.RunOrder, COALESCE(D.RunOrder, 999) as DivOrder
, ER.EventClass, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt, ER.RankingRating, ER.RankingScore, ER.Rotation, TR.ReadyToSki, COALESCE(TR.ReadyForPlcmt, 'N') AS ReadyForPlcmtOverall
, ER.TeamCode as Team, TR.TrickBoat, TR.JumpHeight
, CASE 
	WHEN coalesce( (Select Min(PK) as TVPK FROM TrickVideo AS TV 
		WHERE TV.SanctionId = TR.SanctionId AND TV.MemberId = TR.MemberId AND TV.AgeGroup = TR.AgeGroup
			AND (TV.Pass1VideoUrl IS NOT NULL OR TV.Pass2VideoUrl IS NOT NULL) )
		, 0) > 0 
		THEN 'Y'
	ELSE 'N' END AS TrickVideoAvailable
FROM TourReg AS TR 
INNER JOIN EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup
INNER JOIN EventRunOrder O ON O.SanctionId = TR.SanctionId AND O.MemberId = TR.MemberId AND O.AgeGroup = TR.AgeGroup AND O.Event = ER.Event
LEFT OUTER JOIN DivOrder D ON D.SanctionId = TR.SanctionId AND D.AgeGroup = TR.AgeGroup AND D.Event = ER.Event
Where TR.Withdrawn IS NULL OR TR.Withdrawn = 'N'
GO

GRANT SELECT ON dbo.vSkiersEnteredRound TO PUBLIC
GO
