USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrTournamentDelete') IS NOT NULL
	DROP PROCEDURE dbo.PrTournamentDelete
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrTournamentDelete
Author:			David Allen
Create date: 	January 31, 2024
Description:	Check tournament for custom running order for individual rounds
Value Values: 	Event (Slalom, Trick, Jump)
**************************************************************************** */
CREATE PROCEDURE dbo.PrTournamentDelete @InSanctionId AS varchar(6), @InDeleteConfirm char(1) = 'N'
AS
BEGIN
	DECLARE @curReadyToDelete char(1) = 'Y';
	DECLARE @curNumSlalomScores int;
	DECLARE @curNumTrickScores int;
	DECLARE @curNumJumpScores int;
	DECLARE @curNumPublishedReports int;
	DECLARE @curNumExportFiles int;

	SET @curNumSlalomScores = (Select Count(*) From SlalomScore Where SanctionId = @InSanctionId);
	SET @curNumTrickScores = (Select Count(*) From TrickScore Where SanctionId = @InSanctionId);
	SET @curNumJumpScores = (Select Count(*) From JumpScore Where SanctionId = @InSanctionId);
	SET @curNumPublishedReports = (Select Count(*) From PublishReport Where SanctionId = @InSanctionId AND ReportType != 'Export');
	SET @curNumExportFiles = (Select Count(*) From PublishReport Where SanctionId = @InSanctionId AND ReportType = 'Export');

	IF @curNumSlalomScores > 0 SET @curReadyToDelete = 'N'
	IF @curNumTrickScores > 0 SET @curReadyToDelete = 'N'
	IF @curNumJumpScores > 0 SET @curReadyToDelete = 'N'
	IF @curNumPublishedReports > 0 SET @curReadyToDelete = 'N'
	IF @curNumExportFiles > 0 SET @curReadyToDelete = 'N'

	IF Upper(@InDeleteConfirm) = 'Y' OR Upper(@curReadyToDelete) = 'Y' BEGIN
		Delete from Tournament where sanctionid = @InSanctionId;
		Delete From DivOrder Where SanctionId = @InSanctionId;
		Delete From TourProperties Where SanctionId = @InSanctionId;
		Delete FROM OfficialWork Where SanctionId = @InSanctionId;
		Delete FROM CodeValueList;

		Delete from TourReg where sanctionid = @InSanctionId;
		Delete from EventReg where sanctionid = @InSanctionId;
		Delete from EventRunOrder where sanctionid = @InSanctionId;
		Delete from EventRunOrderFilters where sanctionid = @InSanctionId;

		Delete from SlalomRecap where sanctionid = @InSanctionId;
		Delete from SlalomScore where sanctionid = @InSanctionId;

		Delete from JumpRecap where sanctionid = @InSanctionId;
		Delete from JumpScore where sanctionid = @InSanctionId;
		Delete from JumpMeasurement where sanctionid = @InSanctionId;

		Delete from TrickPass where sanctionid = @InSanctionId;
		Delete from TrickScore where sanctionid = @InSanctionId;
		Delete from TrickVideo where sanctionid = @InSanctionId;

		Delete from PublishReport where sanctionid = @InSanctionId;
		Delete from TeamScore where sanctionid = @InSanctionId;
		Delete from TeamScoreDetail where sanctionid = @InSanctionId;

		Delete from TeamList where sanctionid = @InSanctionId;
		Delete from TeamOrder where sanctionid = @InSanctionId;
		Delete from TeamScore where sanctionid = @InSanctionId;
		Delete from TeamScoreDetail where sanctionid = @InSanctionId;

		Delete from EventRunOrderFilters where sanctionid = @InSanctionId;

		Delete from OfficialWork where sanctionid = @InSanctionId;
		Delete from OfficialWorkAsgmt where sanctionid = @InSanctionId;

		Delete from TourBoatUse where sanctionid = @InSanctionId;
		Delete from TourProperties where sanctionid = @InSanctionId;

		Select 'Tournament ' + @InSanctionId + ' successfully deleted'
	END
	ELSE BEGIN
		Select 'Tournament has scores therefore confirmation is required to delete.  Must run the following: '
		+ ' EXEC PrTournamentDelete @InSanctionId=''' + @InSanctionId + '''' + ', ''@InDeleteConfirm= ''Y'''
	END

END
GO

GRANT EXECUTE ON dbo.PrTournamentDelete TO PUBLIC
GO
