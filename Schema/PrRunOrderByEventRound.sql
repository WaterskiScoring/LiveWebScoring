USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrRunOrderByEventRound') IS NOT NULL
	DROP PROCEDURE dbo.PrRunOrderByEventRound
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrRunOrderByEventRound
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament running order by sanction and event and round (when custom running order for a round)
Value Values: 	Event (Slalom, Trick, Jump)
				Div (All or any valid age group code 

**************************************************************************** */
CREATE PROCEDURE dbo.PrRunOrderByEventRound @InSanctionId AS varchar(6), @InEvent AS varchar(12), @InRound as INT, @InDiv as varchar(3) = 'All'
AS
BEGIN
	DECLARE @curSortCmd varchar(256);
	DECLARE @curPropKey1 varchar(256);
	DECLARE @curPropKey2 varchar(256);
	DECLARE @curDivFilter varchar(256);
	DECLARE @curPropValue VARCHAR(MAX);
	DECLARE @curSqlStmt NVARCHAR(MAX);
	SET @curPropKey1 = 'RunningOrderSort' + @InEvent + 'Round';
	SET @curPropKey2 = 'RunningOrderSort' + @InEvent;

	SET @curPropValue = (Select PropValue From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curPropKey1)
	SET @curPropValue = (Select COALESCE(@curPropValue, PropValue) From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curPropKey2)
	SET @curSortCmd = (Select COALESCE(@curPropValue, 'EventGroup ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC') )

	IF @InDiv = 'All' 
		SET @curDivFilter = '';
	ELSE 
		SET @curDivFilter = 'AND AgeGroup = ''' + @InDiv + ''' ';

	SET @curSqlStmt = 'Select * From vSkiersEnteredRound '
		+ 'Where SanctionId = ''' + @InSanctionId + ''' '
		+ 'AND Event = ''' + @InEvent + ''' '
		+ @curDivFilter
		+ 'Order by ' + @curSortCmd;
	EXEC sp_executesql @curSqlStmt;
END
GO

GRANT EXECUTE ON dbo.PrRunOrderByEventRound TO PUBLIC
GO
