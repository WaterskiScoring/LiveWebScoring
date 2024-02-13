USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrRunOrderByEvent') IS NOT NULL
	DROP PROCEDURE dbo.PrRunOrderByEvent
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrRunOrderByEvent
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament running order by sanction and event
Value Values: 	Event (Slalom, Trick, Jump)
				Div (All or any valid age group code 
**************************************************************************** */
CREATE PROCEDURE dbo.PrRunOrderByEvent @InSanctionId AS varchar(6), @InEvent AS varchar(12), @InDiv as varchar(3) = 'All'
AS
BEGIN
	DECLARE @curSortCmd varchar(256);
	DECLARE @curPropKey varchar(256);
	DECLARE @curDivFilter varchar(256);
	DECLARE @curPropValue VARCHAR(MAX);
	DECLARE @curSqlStmt NVARCHAR(MAX);
	SET @curPropKey = 'RunningOrderSort' + @InEvent;

	SET @curPropValue = (Select PropValue From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curPropKey)
	SET @curSortCmd = (Select COALESCE(@curPropValue, 'EventGroup ASC, ReadyForPlcmt ASC, RunOrder ASC, RankingScore ASC, SkierName ASC') )

	IF @InDiv = 'All' 
		SET @curDivFilter = '';
	ELSE 
		SET @curDivFilter = 'AND AgeGroup = ''' + @InDiv + ''' ';

	SET @curSqlStmt = 'Select * From vSkiersEntered '
		+ 'Where SanctionId = ''' + @InSanctionId + ''' '
		+ 'AND Event = ''' + @InEvent + ''' '
		+ @curDivFilter
		+ 'Order by ' + @curSortCmd;
	EXEC sp_executesql @curSqlStmt;
END
GO

GRANT EXECUTE ON dbo.PrRunOrderByEvent TO PUBLIC
GO
