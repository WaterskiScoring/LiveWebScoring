USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrGetOfficialsPanel') IS NOT NULL
	DROP PROCEDURE dbo.PrGetOfficialsPanel
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrGetOfficialsPanel
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament age groups used by sanction and event
Value Values: 	Event (Slalom, Trick, Jump, All)
**************************************************************************** */
CREATE PROCEDURE dbo.PrGetOfficialsPanel @InSanctionId AS varchar(6)
AS
BEGIN
	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Chief Judge' as Assignment, 11 as SortOrder, JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND JudgeChief = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Chief Driver' as Assignment, 12 as SortOrder, DriverSlalomRating, DriverTrickRating, DriverJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND DriverChief = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Chief Scorer' as Assignment, 13 as SortOrder, ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND ScoreChief = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Safety Director' as Assignment, 14 as SortOrder, SafetyOfficialRating, SafetyOfficialRating, SafetyOfficialRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND SafetyChief = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Chief Tech Controller' as Assignment, 15 as SortOrder, TechOfficialRating, TechOfficialRating, TechOfficialRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND TechChief = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Assist Chief Judge' as Assignment, 21 as SortOrder, JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND JudgeAsstChief = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Assist Chief Driver' as Assignment, 22 as SortOrder, DriverSlalomRating, DriverTrickRating, DriverJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND DriverAsstChief = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Assist Chief Scorer' as Assignment, 23 as SortOrder, ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND ScoreAsstChief = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Assist Chief Tech Controller' as Assignment, 24 as SortOrder, TechOfficialRating, TechOfficialRating, TechOfficialRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND TechAsstChief = 'Y'


	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Appointed Judge' as Assignment, 31 as SortOrder, JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND JudgeAppointed = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Appointed Driver' as Assignment, 32 as SortOrder, DriverSlalomRating, DriverTrickRating, DriverJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND DriverAppointed = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Appointed Scorer' as Assignment, 33 as SortOrder, ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND ScoreAppointed = 'Y'

	UNION

	Select TR.SkierName, TR.SanctionId, TR.MemberID, 'Appointed Safety' as Assignment, 34 as SortOrder, SafetyOfficialRating, SafetyOfficialRating, SafetyOfficialRating
	FROM TourReg TR 
	INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId
	Where TR.SanctionId = @InSanctionId AND SafetyAppointed = 'Y'
	Order by SortOrder, SkierName
END
GO

GRANT EXECUTE ON dbo.PrGetOfficialsPanel TO PUBLIC
GO
