USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrGetUsedRounds') IS NOT NULL
	DROP PROCEDURE dbo.PrGetUsedRounds
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrGetUsedRounds
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament rounds by sanction and event
Value Values: 	Event (Slalom, Trick, Jump)
**************************************************************************** */
CREATE PROCEDURE dbo.PrGetUsedRounds @InSanctionId AS varchar(6), @InEvent AS varchar(12)
AS
BEGIN
	DECLARE @curSqlStmt NVARCHAR(MAX);

	SET @curSqlStmt = 'Select Distinct Round from ' + @InEvent + 'Score '
		+ 'Where SanctionId = ''' + @InSanctionId + ''' AND Round < 25'
		+ 'Order by Round';

	EXEC sp_executesql @curSqlStmt;
END
GO

GRANT EXECUTE ON dbo.PrGetUsedRounds TO PUBLIC
GO
