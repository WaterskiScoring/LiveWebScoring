USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrCheckForCustomRunorder') IS NOT NULL
	DROP PROCEDURE dbo.PrCheckForCustomRunorder
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrCheckForCustomRunorder
Author:			David Allen
Create date: 	January 31, 2024
Description:	Check tournament for custom running order for individual rounds
Value Values: 	Event (Slalom, Trick, Jump)
**************************************************************************** */
CREATE PROCEDURE dbo.PrCheckForCustomRunorder @InSanctionId AS varchar(6), @InEvent AS varchar(12)
AS
BEGIN
	Select Distinct Round, EventGroup, AgeGroup
	From EventRunOrder
	Where SanctionId = @InSanctionId AND Event = @InEvent
	Order by Round, EventGroup, AgeGroup
END
GO

GRANT EXECUTE ON dbo.PrCheckForCustomRunorder TO PUBLIC
GO
