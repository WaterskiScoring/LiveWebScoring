USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrJumpScoresByRunOrder') IS NOT NULL
	DROP PROCEDURE dbo.PrJumpScoresByRunOrder
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrJumpScoresByRunOrder
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament Jump scores sequenced by running order
Value Values: 	Div (All or any valid age group code 

**************************************************************************** */
CREATE PROCEDURE dbo.PrJumpScoresByRunOrder @InSanctionId AS varchar(6), @InRound as Char(1), @InDiv as varchar(3) = 'All', @InGroup as varchar(3) = 'All'
AS
BEGIN
	DECLARE @curSortCmd varchar(256);
	DECLARE @curPropKey varchar(256);
	DECLARE @curDivFilter varchar(256);
	DECLARE @curGroupFilter varchar(256);
	DECLARE @curRoundFilter varchar(256);
	DECLARE @curPropValue VARCHAR(MAX);
	DECLARE @curSqlStmt NVARCHAR(MAX);
	DECLARE @curCustomOrderCount INT = 0;
	
	IF @InDiv = 'All' 
		SET @curDivFilter = '';
	ELSE 
		SET @curDivFilter = 'AND TR.AgeGroup = ''' + @InDiv + ''' ';

	SET @curRoundFilter = ''
	IF @InRound > 0 BEGIN
		SET @curCustomOrderCount = (Select Count(*) as CustomRowCount From EventRunOrder Where SanctionId = @InSanctionId AND Event = 'Jump' AND Round = @InRound )
	END

	IF @InGroup = 'All' 
		SET @curGroupFilter = '';
	ELSE IF @InRound > 0 AND @curCustomOrderCount > 0
		SET @curGroupFilter = 'AND RO.EventGroup = ''' + @InGroup + ''' ';
	ELSE
		SET @curGroupFilter = 'AND ER.EventGroup = ''' + @InGroup + ''' ';


	IF @InRound = 0 OR @curCustomOrderCount = 0 BEGIN
		SET @curPropKey = 'RunningOrderSortJump';
		SET @curPropValue = (Select PropValue From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curPropKey)
		SET @curSortCmd = (Select COALESCE(@curPropValue, 'EventGroup ASC, DivOrder ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC') )
		IF @InRound > 0 SET @curRoundFilter = 'AND Round = ' + @InRound
		
		SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
			+ ', ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
			+ ', ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder, '''' as RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder'
			+ ', SS.Round, COALESCE (SS.Status, ''TBD'') AS Status, SS.NopsScore, SS.ScoreFeet AS EventScore, SS.ScoreFeet, ScoreMeters'
			+ ', (Select RO.ScoreFeet From JumpScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff'
			+ ', TRIM(CAST(ROUND(SS.ScoreFeet, 0) AS CHAR)) + ''FT ('' + TRIM(CAST(ROUND(SS.ScoreMeters, 1) AS CHAR)) + ''M)'' AS EventScoreDesc'
			+ ', SS.InsertDate, SS.LastUpdateDate, SS.LastUpdateDate AS SortLastUpdateDate '
			+ 'FROM TourReg AS TR '
			+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''Jump'' '
			+ 'LEFT OUTER JOIN JumpScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup ' + @curRoundFilter
			+ 'LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event '
			+ 'Where TR.SanctionId = ''' + @InSanctionId + ''' '
			+ @curDivFilter
			+ @curGroupFilter
			+ 'Order by ' + @curSortCmd;
	END
	ELSE BEGIN
		SET @curPropKey = 'RunningOrderSortJumpRound';
		SET @curPropValue = (Select PropValue From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curPropKey)
		SET @curSortCmd = (Select COALESCE(@curPropValue, 'EventGroup ASC, RunOrderGroup ASC, RunOrder ASC, RankingScore ASC, SkierName ASC') )

		SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
			+ ', ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
			+ ', RO.RankingScore, RO.EventGroup, RO.RunOrder, RO.RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder'
			+ ', SS.Round, COALESCE (SS.Status, ''TBD'') AS Status, SS.NopsScore, SS.ScoreFeet AS EventScore, SS.ScoreFeet, ScoreMeters'
			+ ', (Select RO.ScoreFeet From JumpScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff'
			+ ', TRIM(CAST(ROUND(SS.ScoreFeet, 0) AS CHAR)) + ''FT ('' + TRIM(CAST(ROUND(SS.ScoreMeters, 1) AS CHAR)) + ''M)'' AS EventScoreDesc'
			+ ', SS.LastUpdateDate '
			+ 'FROM TourReg AS TR '
			+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''Jump'' '
			+ 'INNER JOIN EventRunOrder as RO ON RO.MemberId = TR.MemberId AND RO.SanctionId = TR.SanctionId AND RO.AgeGroup = TR.AgeGroup AND RO.Event = ER.Event AND RO.Round = ' + @InRound + ' '
			+ 'LEFT OUTER JOIN JumpScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round = RO.Round '
			+ 'LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event '
			+ 'Where TR.SanctionId = ''' + @InSanctionId + ''' '
			+ @curDivFilter
			+ @curGroupFilter
			+ 'Order by ' + @curSortCmd;
	END

	EXEC sp_executesql @curSqlStmt;
END
GO

GRANT EXECUTE ON dbo.PrJumpScoresByRunOrder TO PUBLIC
GO
