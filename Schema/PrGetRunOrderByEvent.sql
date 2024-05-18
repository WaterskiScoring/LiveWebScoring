USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrGetRunOrderByEvent') IS NOT NULL
	DROP PROCEDURE dbo.PrGetRunOrderByEvent
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrGetRunOrderByEvent
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament running order by sanction and event
Value Values: 	Event (Slalom, Trick, Jump)
				Div (All or any valid age group code 
**************************************************************************** */
CREATE PROCEDURE dbo.PrGetRunOrderByEvent @InSanctionId AS varchar(6), @InEvent AS varchar(12), @InDiv as varchar(3) = 'All', @InGroup as varchar(3) = 'All', @InRound as Char(1)
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
		SET @curCustomOrderCount = (Select Count(*) as CustomRowCount From EventRunOrder Where SanctionId = @InSanctionId AND Event = @InEvent AND Round = @InRound )
	END

	IF @InGroup = 'All' 
		SET @curGroupFilter = '';
	ELSE IF @InRound > 0 AND @curCustomOrderCount > 0
		SET @curGroupFilter = 'AND RO.EventGroup = ''' + @InGroup + ''' ';
	ELSE
		SET @curGroupFilter = 'AND ER.EventGroup = ''' + @InGroup + ''' ';

	IF @InRound = 0 OR @curCustomOrderCount = 0 BEGIN
		SET @curPropKey = 'RunningOrderSort' + @InEvent;
		SET @curPropValue = (Select PropValue From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curPropKey)
		SET @curSortCmd = (Select COALESCE(@curPropValue, 'EventGroup ASC, DivOrder ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC') )
		
		SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
			+ ', ER.Event, ER.EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
			+ ', ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder, '''' as RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder '
			+ 'FROM TourReg AS TR '
			+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''' + @InEvent + ''' '
			+ 'LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event '
			+ 'Where TR.SanctionId = ''' + @InSanctionId + ''' '
			+ @curDivFilter
			+ @curGroupFilter
			+ 'Order by ' + @curSortCmd;
	END
	ELSE BEGIN
		SET @curPropKey = 'RunningOrderSort' + @InEvent + 'Round';
		SET @curPropValue = (Select PropValue From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curPropKey)
		SET @curSortCmd = (Select COALESCE(@curPropValue, 'EventGroup ASC, RunOrderGroup ASC, RunOrder ASC, RankingScore ASC, SkierName ASC') )
		
		SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
			+ ', ER.Event, ER.EventClass, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
			+ ', RO.RankingScore, RO.EventGroup, RO.RunOrder, RO.RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder '
			+ 'FROM TourReg AS TR '
			+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''' + @InEvent + ''' '
			+ 'INNER JOIN EventRunOrder as RO ON RO.MemberId = TR.MemberId AND RO.SanctionId = TR.SanctionId AND RO.AgeGroup = TR.AgeGroup AND RO.Event = ER.Event AND RO.Round = ' + @InRound + ' '
			+ 'LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event '
			+ 'Where TR.SanctionId = ''' + @InSanctionId + ''' '
			+ @curDivFilter
			+ @curGroupFilter
			+ 'Order by ' + @curSortCmd;
	END

	EXEC sp_executesql @curSqlStmt;


END
GO

GRANT EXECUTE ON dbo.PrGetRunOrderByEvent TO PUBLIC
GO
