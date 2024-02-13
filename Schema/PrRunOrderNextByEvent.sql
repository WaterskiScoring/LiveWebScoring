USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrRunOrderNextByEvent') IS NOT NULL
	DROP PROCEDURE dbo.PrRunOrderNextByEvent
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrRunOrderNextByEvent
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament running order by sanction and event
Value Values: 	Event (Slalom, Trick, Jump)
				Div (All or any valid age group code 
**************************************************************************** */
CREATE PROCEDURE dbo.PrRunOrderNextByEvent @InSanctionId AS varchar(6), @InEvent AS varchar(12), @InEventGroup as varchar(3) = 'All'
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

	IF @InEventGroup = 'All' 
		SET @curDivFilter = '';
	ELSE 
		BEGIN
			IF SUBSTRING(@InSanctionId, 3, 1) = 'U' 
				SET @curDivFilter = 'AND ER.AgeGroup = ''' + @InEventGroup + ''' ';
			ELSE 
				SET @curDivFilter = 'AND ER.EventGroup = ''' + @InEventGroup + ''' ';
		END

	SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
		+ ', ER.Event, ER.EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
		+ ', ER.RankingRating, ER.RankingScore, ER.Rotation, ER.RunOrder, '''' as RunOrderGroup, COALESCE(D.RunOrder, 999) as DivOrder, ER.LastUpdateDate '
		+ 'FROM TourReg AS TR '
		+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''' + @InEvent + ''' '
		+ 'LEFT OUTER JOIN DivOrder D ON D.SanctionId = ER.SanctionId AND D.AgeGroup = ER.AgeGroup AND D.Event = ER.Event '
		+ 'Where TR.SanctionId = ''' + @InSanctionId + ''' '
		+ 'AND Not Exists ( Select 1 From ' + @InEvent + 'Score AS SS Where SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup ) '
		+ @curDivFilter
		+ 'Order by ' + @curSortCmd;

	EXEC sp_executesql @curSqlStmt;
END
GO

GRANT EXECUTE ON dbo.PrRunOrderNextByEvent TO PUBLIC
GO
