USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrGetPublishReports') IS NOT NULL
	DROP PROCEDURE dbo.PrGetPublishReports
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrGetPublishReports
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament published report and export files
Value Values: 	InReportType (All, Exort, Other, Results, RunOrder, PDF (includes Other, Results, RunOrder)
**************************************************************************** */
CREATE PROCEDURE dbo.PrGetPublishReports @InSanctionId AS varchar(6), @InReportType as varchar(12) = 'All'
AS
BEGIN
	IF @InReportType = 'All' BEGIN
		Select PK, SanctionId, ReportType, Event, ReportTitle, ReportFilePath, LastUpdateDate
			, 'https://www.waterskiresults.com' + ReportFilePath AS ReportFileUri 
		From PublishReport
		Where SanctionId = @InSanctionId
		Order By ReportType, Event, ReportTitle
	END

	ELSE IF @InReportType = 'PDF' BEGIN
		Select PK, SanctionId, ReportType, Event, ReportTitle, ReportFilePath, LastUpdateDate
			, 'https://www.waterskiresults.com' + ReportFilePath AS ReportFileUri 
		From PublishReport
		Where SanctionId = @InSanctionId AND ReportType != 'Export'
		Order By ReportType, Event, ReportTitle
	END

	ELSE BEGIN
		Select PK, SanctionId, ReportType, Event, ReportTitle, ReportFilePath, LastUpdateDate
			, 'https://www.waterskiresults.com' + ReportFilePath AS ReportFileUri 
		From PublishReport
		Where SanctionId = @InSanctionId AND ReportType = @InReportType
		Order By ReportType, Event, ReportTitle
	END

END
GO

GRANT EXECUTE ON dbo.PrGetPublishReports TO PUBLIC
GO
