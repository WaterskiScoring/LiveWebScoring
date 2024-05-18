USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrGetScoresOverall ') IS NOT NULL
	DROP PROCEDURE dbo.PrGetScoresOverall 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrGetScoresOverall 
Author:			David Allen
Create date: 	April 5, 2024
Description:	Retrieve tournament overall scores
**************************************************************************** */
CREATE PROCEDURE dbo.PrGetScoresOverall  @InSanctionId AS char(6), @InDiv as varchar(3) = 'All'
AS
BEGIN
	CREATE TABLE #OverallScores ( 
	SanctionId nchar(6) NOT NULL
	, MemberId nchar(9) NOT NULL
	, AgeGroup nvarchar(12) NOT NULL
	, [Round] tinyint NOT NULL
	, SkierName nvarchar(64) NOT NULL
	, TourClass nchar(1) NULL
	, ReadyForPlcmt nchar(1) NULL
	, NopsScoreSlalom numeric(7, 2) NULL
	, NopsScoreTrick numeric(7, 2) NULL
	, NopsScoreJump numeric(7, 2) NULL
	)

	DECLARE @curRounds int;
	DECLARE @curRoundsOverall int;
	DECLARE @curRoundsSlalom int;
	DECLARE @curRoundsTrick int;
	DECLARE @curRoundsJump int;
	Select @curRoundsSlalom = SlalomRounds, @curRoundsTrick = TrickRounds, @curRoundsJump = JumpRounds From Tournament Where SanctionId = @InSanctionId;

	SET @curRoundsOverall = 99;
	IF @curRoundsSlalom > 0 AND @curRoundsSlalom < @curRoundsOverall SET @curRoundsOverall = @curRoundsSlalom
	IF @curRoundsTrick > 0 AND @curRoundsTrick < @curRoundsOverall SET @curRoundsOverall = @curRoundsTrick
	IF @curRoundsJump > 0 AND @curRoundsJump < @curRoundsOverall SET @curRoundsOverall = @curRoundsJump
	
	DECLARE @curPointsMethod varchar(24);
	SET @curPointsMethod = (Select Upper(PropValue) From TourProperties Where SanctionId = @InSanctionId AND PropKey = 'MasterSummaryOverallPointsMethod')
	SET @curPointsMethod = (Select Coalesce(@curPointsMethod, 'NOPS'))

	SET @curRounds = 1
	
	IF @curPointsMethod = 'KBASE' BEGIN
		CREATE TABLE #BestEventScores ( 
			SanctionId nchar(6) NOT NULL
			, AgeGroup nvarchar(12) NOT NULL
			, MaxScoreSlalom numeric(7, 2) NULL
			, MaxScoreTrick numeric(7, 2) NULL
			, MaxScoreJump numeric(7, 2) NULL
		)

		Insert INTO #BestEventScores (
			SanctionId
			, AgeGroup
			, MaxScoreSlalom
			, MaxScoreTrick
			, MaxScoreJump
		)
		SELECT T.SanctionId, R.AgeGroup, Max(S.Score) as MaxSlalomScore, Max(K.Score) as MaxTrickScore, Max(J.ScoreMeters) as MaxJumpMeters
		From Tournament T
		INNER JOIN TourReg AS R ON R.SanctionId = T.SanctionId 
			LEFT OUTER JOIN SlalomScore AS S ON R.SanctionId = S.SanctionId AND R.MemberId = S.MemberId AND R.AgeGroup = S.AgeGroup AND S.Round < 25
			LEFT OUTER JOIN TrickScore AS K ON R.SanctionId = K.SanctionId AND R.MemberId = K.MemberId AND R.AgeGroup = K.AgeGroup AND K.Round < 25
			LEFT OUTER JOIN JumpScore AS J ON R.SanctionId = J.SanctionId AND R.MemberId = J.MemberId AND R.AgeGroup = J.AgeGroup AND J.Round < 25
		WHERE T.SanctionId = @InSanctionId AND R.ReadyForPlcmt = 'Y' AND (R.AgeGroup = @InDiv OR @InDiv = 'All') 
			AND EXISTS (Select 1 From EventReg AS E
				INNER JOIN CodeValueList AS L ON L.ListCode = E.AgeGroup 
				WHERE E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup
				GROUP BY E.MemberId, E.AgeGroup, L.MinValue
				HAVING COUNT(*) >= L.MinValue )
		Group By T.SanctionId, R.AgeGroup

		While @curRounds <= @curRoundsOverall BEGIN
			Insert INTO #OverallScores (
				SanctionId
				, MemberId
				, SkierName
				, AgeGroup
				, [Round]
				, TourClass
				, ReadyForPlcmt
				, NopsScoreSlalom
				, NopsScoreTrick
				, NopsScoreJump
			)
			SELECT R.SanctionId, R.MemberId, R.SkierName, R.AgeGroup, @curRounds AS OverallRound, T.Class, R.ReadyForPlcmt
				, CASE WHEN ( S.Score + SlalomScoreAdj ) <= 0 THEN 0 ELSE Round( ( ( S.Score + SlalomScoreAdj ) /  ( MaxScoreSlalom + SlalomScoreAdj ) ) * 1000, 1, 0) END AS SlalomOverallScore
				, CASE WHEN ( K.Score + TrickScoreAdj ) <= 0 THEN 0 ELSE Round( ( ( K.Score + TrickScoreAdj ) /  ( MaxScoreTrick + TrickScoreAdj ) ) * 1000, 1, 0) END AS TrickOverallScore
				, CASE WHEN ( J.ScoreMeters + JumpScoreAdj ) <= 0 THEN 0 ELSE Round( ( ( J.ScoreMeters + JumpScoreAdj ) /  ( MaxScoreJump + JumpScoreAdj ) ) * 1000, 1, 0) END AS JumpOverallScore
				From Tournament T
					INNER JOIN TourReg AS R ON R.SanctionId = T.SanctionId 
					INNER JOIN #BestEventScores B ON B.SanctionId = R.SanctionId AND B.AgeGroup = R.AgeGroup 
					INNER JOIN ( 
						Select S.ListCode, SUBSTRING(S.ListCode, 1, 2) as Div, S.MinValue as SlalomScoreAdj, T.MinValue as TrickScoreAdj, J.MinValue as JumpScoreAdj
						From CodeValueList AS S 
						INNER JOIN CodeValueList AS T ON T.ListName = S.ListName AND SUBSTRING(T.ListCode, 1, 2) = SUBSTRING(S.ListCode, 1, 2) AND T.ListCode like '%-T'
						INNER JOIN CodeValueList AS J ON J.ListName = S.ListName AND SUBSTRING(J.ListCode, 1, 2) = SUBSTRING(S.ListCode, 1, 2) AND J.ListCode like '%-J'
						Where S.ListName = 'IwwfOverallAdj' AND S.ListCode like '%-S'
					) AS OV ON OV.Div = R.AgeGroup
					LEFT OUTER JOIN SlalomScore AS S ON R.SanctionId = S.SanctionId AND R.MemberId = S.MemberId AND R.AgeGroup = S.AgeGroup AND S.Round = @curRounds
					LEFT OUTER JOIN TrickScore AS K ON R.SanctionId = K.SanctionId AND R.MemberId = K.MemberId AND R.AgeGroup = K.AgeGroup AND K.Round = @curRounds
					LEFT OUTER JOIN JumpScore AS J ON R.SanctionId = J.SanctionId AND R.MemberId = J.MemberId AND R.AgeGroup = J.AgeGroup AND J.Round = @curRounds
				WHERE R.SanctionId = @InSanctionId AND (R.AgeGroup = @InDiv OR @InDiv = 'All') 
					AND EXISTS (Select 1 From EventReg AS E
						INNER JOIN CodeValueList AS L ON L.ListCode = E.AgeGroup AND ListName = 'OverallEventsReqd'
						WHERE E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup
						GROUP BY E.MemberId, E.AgeGroup, L.MinValue
						HAVING COUNT(*) >= L.MinValue );

			SET @curRounds += 1;
		END

	END
	ELSE IF @curPointsMethod = 'NOPS' BEGIN
		While @curRounds <= @curRoundsOverall BEGIN
			Insert INTO #OverallScores (
				SanctionId
				, MemberId
				, SkierName
				, AgeGroup
				, [Round]
				, TourClass
				, ReadyForPlcmt
				, NopsScoreSlalom
				, NopsScoreTrick
				, NopsScoreJump
			)
			SELECT R.SanctionId, R.MemberId, R.SkierName, R.AgeGroup, @curRounds AS OverallRound, T.Class, R.ReadyForPlcmt, 
			S.NopsScore AS Slalom, K.NopsScore AS Trick, J.NopsScore AS Jump
			From Tournament T
				INNER JOIN TourReg AS R ON R.SanctionId = T.SanctionId 
				LEFT OUTER JOIN SlalomScore AS S ON R.SanctionId = S.SanctionId AND R.MemberId = S.MemberId AND R.AgeGroup = S.AgeGroup AND S.Round = @curRounds
				LEFT OUTER JOIN TrickScore AS K ON R.SanctionId = K.SanctionId AND R.MemberId = K.MemberId AND R.AgeGroup = K.AgeGroup AND K.Round = @curRounds
				LEFT OUTER JOIN JumpScore AS J ON R.SanctionId = J.SanctionId AND R.MemberId = J.MemberId AND R.AgeGroup = J.AgeGroup AND J.Round = @curRounds
			WHERE T.SanctionId = @InSanctionId AND (R.AgeGroup = @InDiv OR @InDiv = 'All') 
				AND EXISTS (Select 1 From EventReg AS E
					INNER JOIN CodeValueList AS L ON L.ListCode = E.AgeGroup AND ListName = 'OverallEventsReqd'
					WHERE E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup
					GROUP BY E.MemberId, E.AgeGroup, L.MinValue
					HAVING COUNT(*) >= L.MinValue )
				AND EXISTS (Select 1 From NopsData N 
					Where N.AgeGroup = R.AgeGroup AND SkiYear = SUBSTRING(@InSanctionId, 1, 2) AND Event = 'Slalom' AND RatingRec > 0);
	
			Insert INTO #OverallScores (
				SanctionId
				, MemberId
				, SkierName
				, AgeGroup
				, [Round]
				, TourClass
				, ReadyForPlcmt
				, NopsScoreSlalom
				, NopsScoreTrick
				, NopsScoreJump
			)
			SELECT T.SanctionId, R.MemberId, R.SkierName, R.AgeGroup, @curRounds AS OverallRound, T.Class, R.ReadyForPlcmt
				, CASE WHEN ( S.Score + SlalomScoreAdj ) <= 0 THEN 0 ELSE Round( ( ( S.Score + SlalomScoreAdj ) /  ( SlalomScoreBasis + SlalomScoreAdj ) ) * 1000, 1, 0) END AS SlalomOverallScore
				, CASE WHEN ( K.Score + TrickScoreAdj ) <= 0 THEN 0 ELSE Round( ( ( K.Score + TrickScoreAdj ) /  ( TrickScoreBasis + TrickScoreAdj ) ) * 1000, 1, 0) END AS TrickOverallScore
				, CASE WHEN ( J.ScoreMeters + JumpScoreAdj ) <= 0 THEN 0 ELSE Round( ( ( J.ScoreMeters + JumpScoreAdj ) /  ( JumpScoreBasis + JumpScoreAdj ) ) * 1000, 1, 0) END AS JumpOverallScore
				From Tournament T
					INNER JOIN TourReg AS R ON R.SanctionId = T.SanctionId 
					INNER JOIN ( Select S.Div, SlalomScoreBasis, TrickScoreBasis, JumpScoreBasis, SlalomScoreAdj, TrickScoreAdj, JumpScoreAdj
						From ( Select ListCode, SUBSTRING( ListCode, 1, 2 ) as Div, MaxValue as SlalomScoreBasis
							   From CodeValueList Where ListName = 'IwwfOverallWrlBasis' AND ListCode like '%-S' ) as S 
						INNER JOIN ( Select ListCode, SUBSTRING(ListCode, 1, 2) as Div, MaxValue as TrickScoreBasis
									 From CodeValueList Where ListName = 'IwwfOverallWrlBasis' AND ListCode like '%-T') as T
									 ON T.Div = S.Div 
						INNER JOIN ( Select ListCode, SUBSTRING(ListCode, 1, 2) as Div, MaxValue as JumpScoreBasis
									 From CodeValueList Where ListName = 'IwwfOverallWrlBasis' AND ListCode like '%-J') as J
									 ON J.Div = S.Div
						INNER JOIN (Select ListCode, SUBSTRING(ListCode, 1, 2) as Div, MinValue as SlalomScoreAdj
									 From CodeValueList Where ListName = 'IwwfOverallAdj') as S2
									 ON S2.ListCode = S.ListCode 
						INNER JOIN (Select ListCode, SUBSTRING(ListCode, 1, 2) as Div, MinValue as TrickScoreAdj
									 From CodeValueList Where ListName = 'IwwfOverallAdj') as T2
									 ON T2.ListCode = T.ListCode 
						INNER JOIN (Select ListCode, SUBSTRING(ListCode, 1, 2) as Div, MinValue as JumpScoreAdj
									 From CodeValueList Where ListName = 'IwwfOverallAdj') as J2
									 ON J2.ListCode = J.ListCode 
					) AS OV ON OV.Div = R.AgeGroup
					LEFT OUTER JOIN SlalomScore AS S ON R.SanctionId = S.SanctionId AND R.MemberId = S.MemberId AND R.AgeGroup = S.AgeGroup AND S.Round = @curRounds
					LEFT OUTER JOIN TrickScore AS K ON R.SanctionId = K.SanctionId AND R.MemberId = K.MemberId AND R.AgeGroup = K.AgeGroup AND K.Round = @curRounds
					LEFT OUTER JOIN JumpScore AS J ON R.SanctionId = J.SanctionId AND R.MemberId = J.MemberId AND R.AgeGroup = J.AgeGroup AND J.Round = @curRounds
				WHERE R.SanctionId = @InSanctionId AND (R.AgeGroup = @InDiv OR @InDiv = 'All') 
					AND EXISTS (Select 1 From EventReg AS E
						INNER JOIN CodeValueList AS L ON L.ListCode = E.AgeGroup AND ListName = 'OverallEventsReqd'
						WHERE E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup
						GROUP BY E.MemberId, E.AgeGroup, L.MinValue
						HAVING COUNT(*) >= L.MinValue );
				
			SET @curRounds += 1;
		END

	END

	Select SanctionId, MemberId, SkierName, AgeGroup, [Round], TourClass, ReadyForPlcmt
	, NopsScoreSlalom, NopsScoreTrick, NopsScoreJump
	, Coalesce(NopsScoreSlalom, 0) + Coalesce(NopsScoreTrick, 0) + Coalesce(NopsScoreJump, 0) AS NopsScoreOverall
	, CAST(L.MinValue AS INT) AS EventsReqd
	, CASE WHEN (
		CASE WHEN NopsScoreSlalom IS Null THEN 0 ELSE 1 END 
		+ CASE WHEN NopsScoreTrick IS Null THEN 0 ELSE 1 END 
		+ CASE WHEN NopsScoreJump IS Null THEN 0 ELSE 1 END
		) >= L.MinValue THEN 1 ELSE 0 END AS OverallQualified
	From #OverallScores 
	INNER JOIN CodeValueList AS L ON L.ListCode = AgeGroup AND ListName = 'OverallEventsReqd'
	ORDER BY SanctionId, AgeGroup, SkierName, MemberId, [Round];
END
GO

GRANT EXECUTE ON dbo.PrGetScoresOverall  TO PUBLIC
GO
