USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrScoreDetailByEventMember ') IS NOT NULL
	DROP PROCEDURE dbo.PrScoreDetailByEventMember 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrScoreDetailByEventMember 
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament score details for an event and member
Value Values: 	Event (Slalom, Trick, Jump)

**************************************************************************** */
CREATE PROCEDURE dbo.PrScoreDetailByEventMember  @InSanctionId AS varchar(6), @InMemberId AS Char(9), @InEvent AS varchar(12), @InRound as Int
AS
BEGIN
	IF @InEvent = 'Slalom' 
	BEGIN
		SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
			, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
			, ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder
			
			, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, SS.Score AS EventScore, SS.StartSpeed, SS.StartLen
			, (Select RO.Score From SlalomScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
			, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(SS.FinalLenOff) + ' (' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(SS.FinalLen) + 'm)' AS EventScoreDesc

			, SkierRunNum, BoatTime, TimeInTol, ScoreProt, Reride, ProtectedScore, BoatPathGood, PassLineLength, PassSpeedKph
			, SR.Score as PassScore, SR.Note as PassNote
			
			, SR.InsertDate, SR.LastUpdateDate, SR.RerideReason
		FROM TourReg AS TR 
		INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = @InEvent 
		INNER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
		INNER JOIN SlalomRecap AS SR ON SR.MemberId = TR.MemberId AND SR.SanctionId = TR.SanctionId AND SR.AgeGroup = TR.AgeGroup AND SR.Round = SS.Round
		Where TR.SanctionId = @InSanctionId AND TR.MemberId = @InMemberId AND (SS.Round = @InRound OR 0 = @InRound)
		Order by TR.SanctionId, SS.Round;
	END

	ELSE IF @InEvent = 'Jump' 
	BEGIN
		SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
			, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
			, ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder
			
			, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, SS.ScoreFeet AS EventScore, SS.ScoreFeet, SS.ScoreMeters
			, (Select RO.ScoreFeet From JumpScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
			, TRIM(CAST(ROUND(SS.ScoreFeet, 0) AS CHAR)) + 'FT (' + TRIM(CAST(ROUND(SS.ScoreMeters, 1) AS CHAR)) + 'M)' AS EventScoreDesc

			, PassNum, ReturnToBase, SR.RampHeight, SR.BoatSpeed, SR.SkierBoatPath, SR.BoatSplitTime as BoatSplitTime52, SR.BoatSplitTime2 as BoatSplitTime82, SR.BoatEndTime
			, TimeInTol, Split52TimeTol, Split82TimeTol, Split41TimeTol, ScoreProt, RerideIfBest, RerideCanImprove
			, SR.ScoreFeet as PassScoreFeet, SR.ScoreMeters as PassScoreMeters, SR.Results, SR.RerideReason
			
			, SR.InsertDate, SR.LastUpdateDate
		FROM TourReg AS TR 
		INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = @InEvent 
		INNER JOIN JumpScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
		INNER JOIN JumpRecap AS SR ON SR.MemberId = TR.MemberId AND SR.SanctionId = TR.SanctionId AND SR.AgeGroup = TR.AgeGroup AND SR.Round = SS.Round
		Where TR.SanctionId = @InSanctionId AND TR.MemberId = @InMemberId AND (SS.Round = @InRound OR 0 = @InRound)
		Order by TR.SanctionId, SS.Round, PassNum;
	END

	ELSE IF @InEvent = 'Trick' 
	BEGIN
		SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
			, ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt
			, ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder
			
			, SS.Round, COALESCE (SS.Status, 'TBD') AS Status, SS.NopsScore, SS.Score AS EventScore
			, (Select RO.Score From TrickScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff
			, TRIM(CAST(SS.Score AS CHAR)) + ' POINTS (P1:' + TRIM(CAST(SS.ScorePass1 AS CHAR)) + ' P2:' + TRIM(CAST(SS.ScorePass2 AS CHAR)) + ')' AS EventScoreDesc 

			, PassNum, Seq, Skis, SR.Score as TrickScore, Code as TrickCode, Results as TrickResults
			, TV.Pass1VideoUrl, TV.Pass2VideoUrl

			, SS.InsertDate, SR.LastUpdateDate
		FROM TourReg AS TR 
		INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = @InEvent 
		INNER JOIN TrickScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
		INNER JOIN TrickPass AS SR ON SR.MemberId = TR.MemberId AND SR.SanctionId = TR.SanctionId AND SR.AgeGroup = TR.AgeGroup AND SR.Round = SS.Round
		LEFT OUTER JOIN TrickVideo AS TV ON TV.SanctionId = TR.SanctionId AND TV.MemberId = TR.MemberId AND TV.AgeGroup = TR.AgeGroup AND TV.Round = SS.Round
		Where TR.SanctionId = @InSanctionId AND TR.MemberId = @InMemberId AND (SS.Round = @InRound OR 0 = @InRound)
		Order by TR.SanctionId, SS.Round, PassNum, Seq;
	END

	ELSE IF @InEvent = 'Overall' 
	BEGIN
		SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation
			, 'Overall' as Event, COALESCE(SS.EventClass, TS.EventClass, JS.EventClass) as EventClass, COALESCE(TR.ReadyForPlcmt, 'N') as ReadyForPlcmt
			, COALESCE(SS.Round, TS.Round, JS.Round) as Round
			
			, SS.NopsScore as SlalomNops, SS.Score AS SlalomScore
			, TS.NopsScore as TrickNops, TS.Score AS TrickScore
			, JS.NopsScore as JumpNops, JS.ScoreFeet AS JumpScore
			
			, TRIM(CAST(SS.FinalPassScore AS CHAR)) + ' @ ' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + 'mph ' + TRIM(SS.FinalLenOff) + ' (' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + 'kph ' + TRIM(SS.FinalLen) + 'm)' AS SlalomScoreDesc
			, TRIM(CAST(TS.Score AS CHAR)) + ' POINTS (P1:' + TRIM(CAST(TS.ScorePass1 AS CHAR)) + ' P2:' + TRIM(CAST(TS.ScorePass2 AS CHAR)) + ')' AS TrickScoreDesc 
			, TRIM(CAST(ROUND(JS.ScoreFeet, 0) AS CHAR)) + 'FT (' + TRIM(CAST(ROUND(JS.ScoreMeters, 1) AS CHAR)) + 'M)' AS JumpScoreDesc

		FROM TourReg AS TR 
		LEFT OUTER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup
		LEFT OUTER JOIN TrickScore AS TS ON TS.MemberId = TR.MemberId AND TS.SanctionId = TR.SanctionId AND TS.AgeGroup = TR.AgeGroup
		LEFT OUTER JOIN JumpScore AS JS ON JS.MemberId = TR.MemberId AND JS.SanctionId = TR.SanctionId AND JS.AgeGroup = TR.AgeGroup
		Where TR.SanctionId = @InSanctionId AND TR.MemberId = @InMemberId AND (SS.Round = @InRound OR 0 = @InRound)
		Order by TR.SanctionId, COALESCE(SS.Round, TS.Round, JS.Round) ;
	END
END
GO

GRANT EXECUTE ON dbo.PrScoreDetailByEventMember  TO PUBLIC
GO
