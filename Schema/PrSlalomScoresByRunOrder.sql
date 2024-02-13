USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrSlalomScoresByRunOrder') IS NOT NULL
	DROP PROCEDURE dbo.PrSlalomScoresByRunOrder
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrSlalomScoresByRunOrder
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament slalom scores sequenced by running order
Value Values: 	Event (Slalom, Trick, Jump)
				Div (All or any valid age group code 

**************************************************************************** */
CREATE PROCEDURE dbo.PrSlalomScoresByRunOrder @InSanctionId AS varchar(6), @InRound as Char(1), @InDiv as varchar(3) = 'All'
AS
BEGIN
	DECLARE @curSortCmd varchar(256);
	DECLARE @curPropKey varchar(256);
	DECLARE @curDivFilter varchar(256);
	DECLARE @curPropValue VARCHAR(MAX);
	DECLARE @curSqlStmt NVARCHAR(MAX);
	
	SET @curPropKey = 'RunningOrderSortSlalom';
	SET @curPropValue = (Select PropValue From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curPropKey)
	SET @curSortCmd = (Select COALESCE(@curPropValue, 'EventGroup ASC, DivOrder ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC') )

	IF @InDiv = 'All' 
		SET @curDivFilter = '';
	ELSE 
		SET @curDivFilter = 'AND TR.AgeGroup = ''' + @InDiv + ''' ';

	SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
		+ ', ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
		+ ', ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder, '''' as RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder'
		+ ', SS.Round, COALESCE (SS.Status, ''TBD'') AS Status, SS.NopsScore, SS.Score AS EventScore'
		+ ', (Select RO.Score From SlalomScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff'
		+ ', CAST(SS.Score AS CHAR) AS Buoys'
		+ ', TRIM(CAST(SS.FinalPassScore AS CHAR)) + '' @ '' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + ''mph '' + TRIM(SS.FinalLenOff) + '' ('' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + ''kph '' + TRIM(SS.FinalLen) + ''m)'' AS EventScoreDesc'
		+ ', SS.InsertDate, SS.LastUpdateDate, SS.LastUpdateDate AS SortLastUpdateDate '
		+ 'FROM TourReg AS TR '
		+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''Slalom'' '
		+ 'LEFT OUTER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND (' + @InRound + ' = 0 OR Round = ' + @InRound + ') '
		+ 'LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event '
		+ 'Where TR.SanctionId = ''' + @InSanctionId + ''' '
		+ @curDivFilter
		+ 'Order by ' + @curSortCmd;

	EXEC sp_executesql @curSqlStmt;
END
GO

GRANT EXECUTE ON dbo.PrSlalomScoresByRunOrder TO PUBLIC
GO
