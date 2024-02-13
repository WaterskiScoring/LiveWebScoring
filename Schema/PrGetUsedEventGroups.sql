USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrGetUsedEventGroups') IS NOT NULL
	DROP PROCEDURE dbo.PrGetUsedEventGroups
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrGetUsedEventGroups
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament event groups by sanction and event
Value Values: 	Event (Slalom, Trick, Jump, All)
**************************************************************************** */
CREATE PROCEDURE dbo.PrGetUsedEventGroups @InSanctionId AS varchar(6), @InEvent AS varchar(12) = 'All'
AS
BEGIN
	DECLARE @curSqlStmt NVARCHAR(MAX);
	DECLARE @curFilter varchar(256);
	IF @InEvent = 'All' OR @InEvent = 'Overall' 
		SET @curFilter = '';
	ELSE 
		SET @curFilter = 'AND Event = ''' + @InEvent + ''' ';

	SET @curSqlStmt = 'Select Distinct EventGroup from EventReg '
		+ 'Where SanctionId = ''' + @InSanctionId + ''' '
		+ @curFilter
		+ 'Order by EventGroup';

	EXEC sp_executesql @curSqlStmt;
END
GO

GRANT EXECUTE ON dbo.PrGetUsedEventGroups TO PUBLIC
GO
