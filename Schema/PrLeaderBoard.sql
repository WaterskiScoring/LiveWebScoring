USE LiveWebScoreboard
GO

/* ****************************************************************************
Drop StoredProcedure to allow it to be created again
**************************************************************************** */
IF OBJECT_ID ('dbo.PrLeaderBoard ') IS NOT NULL
	DROP PROCEDURE dbo.PrLeaderBoard 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ****************************************************************************
Object:			StoredProcedure dbo.PrLeaderBoard 
Author:			David Allen
Create date: 	January 31, 2024
Description:	Retrieve tournament slalom scores sequenced by score per division with score determind by requested format
Value Values: 	Div (All or any valid age group code 
				Event (Slalom, Trick, Jump)
				Format (Best, Final, First, Round)

EXEC PrLeaderBoard  '24U312', 'Slalom', 'BEST', 'All'
**************************************************************************** */
CREATE PROCEDURE dbo.PrLeaderBoard  @InSanctionId AS varchar(6), @InEvent AS varchar(12), @InFormat AS varchar(12), @InDiv as varchar(3) = 'All'
AS
BEGIN
	DECLARE @curSortCmd varchar(256);
	DECLARE @curScoreSelect varchar(1024);
	DECLARE @curDivFilter varchar(256);
	DECLARE @curSqlStmt NVARCHAR(MAX);
	DECLARE @curScoreFormat varchar(24);
	DECLARE @curNumPreLim varchar(2);
	DECLARE @curScoreAttr varchar(12);
	DECLARE @curTourProp varchar(24);
	DECLARE @curTourPropPrelim varchar(24);
	SET @curTourProp = @InEvent + 'SummaryDataType'
	SET @curTourPropPrelim = @InEvent + 'SummaryNumPrelim'

	SET @curScoreFormat = (Select Upper(PropValue) From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curTourProp)
	IF @curScoreFormat is null SET @curScoreFormat = Upper(@InFormat)
	IF Substring(@InSanctionId, 3, 1) = 'U' SET @curScoreFormat = 'NCWSA'

	SET @curScoreAttr = 'Score';
	IF @InEvent = 'Jump' SET @curScoreAttr = 'ScoreFeet';

	SET @curNumPreLim = '1';
	IF @curScoreFormat = 'FINAL' OR @curScoreFormat = 'H2H'
		SET @curNumPreLim = (Select PropValue From TourProperties Where SanctionId = @InSanctionId AND PropKey = @curTourPropPrelim)
	SET @curNumPreLim = (Select COALESCE(@curNumPreLim, '1'));
	
	IF @curScoreFormat = 'NCWSA'
		SET @curSortCmd = 'AgeGroup DESC, ReadyForPlcmt ASC, EventScore DESC, ScoreRunoff DESC'
	ELSE IF @curScoreFormat = 'BEST'
		SET @curSortCmd = 'AgeGroup ASC, ReadyForPlcmt ASC, EventScore DESC, ScoreRunoff DESC'
	ELSE IF @curScoreFormat = 'FINAL' OR @curScoreFormat = 'H2H'
		SET @curSortCmd = 'AgeGroup ASC, FinalRound DESC, ReadyForPlcmt ASC, EventScore DESC, ScoreRunoff DESC'
	ELSE IF @curScoreFormat = 'FIRST'
		SET @curSortCmd = 'AgeGroup ASC, Round ASC, ReadyForPlcmt ASC, EventScore DESC, ScoreRunoff DESC'
	ELSE IF @curScoreFormat = 'ROUND'
		SET @curSortCmd = 'AgeGroup ASC, Round ASC, ReadyForPlcmt ASC, EventScore DESC, ScoreRunoff DESC'
	ELSE
		SET @curSortCmd = 'AgeGroup ASC, ReadyForPlcmt ASC, EventScore DESC, ScoreRunoff DESC'

	IF @InDiv = 'All' 
		SET @curDivFilter = '';
	ELSE 
		SET @curDivFilter = 'AND TR.AgeGroup = ''' + @InDiv + ''' ';

	IF @curScoreFormat = 'BEST' AND (@InEvent = 'Slalom' OR @InEvent = 'Trick')
		SET @curScoreSelect = 'AND SS.PK = (Select Min(PK) From ' + @InEvent + 'Score PK '
			+ 'Where PK.SanctionId = TR.SanctionId AND PK.MemberId = SS.MemberId AND PK.AgeGroup = SS.AgeGroup AND PK.Round < 25 '
			+ 'AND PK.Score = ( Select Max(Score) From ' + @InEvent + 'Score BS Where BS.SanctionId = PK.SanctionId AND BS.MemberId = PK.MemberId AND BS.AgeGroup = PK.AgeGroup AND Round < 25) )'
	ELSE IF @curScoreFormat = 'BEST' AND @InEvent = 'Jump'
		SET @curScoreSelect = 'AND SS.PK = (Select Min(PK) From ' + @InEvent + 'Score PK '
			+ 'Where PK.SanctionId = TR.SanctionId AND PK.MemberId = SS.MemberId AND PK.AgeGroup = SS.AgeGroup AND PK.Round < 25 '
			+ 'AND PK.ScoreFeet = ( Select Max(ScoreFeet) From ' + @InEvent + 'Score BS Where BS.SanctionId = PK.SanctionId AND BS.MemberId = PK.MemberId AND BS.AgeGroup = PK.AgeGroup AND Round < 25) )'
	ELSE IF @curScoreFormat = 'Final' OR @curScoreFormat = 'H2H'
		SET @curScoreSelect = 'AND ( ( (Select Max(Round) as MaxRound From ' + @InEvent + 'Score SS2 Where SS2.SanctionId = SS.SanctionId AND SS2.MemberId = SS.MemberId AND SS2.AgeGroup = SS.AgeGroup AND SS2.Round < 25 ) > ' + @curNumPreLim
			+ ' AND SS.Round = (Select Max(Round) as MaxRound From ' + @InEvent + 'Score SS2 Where SS2.SanctionId = SS.SanctionId AND SS2.MemberId = SS.MemberId AND SS2.AgeGroup = SS.AgeGroup AND SS2.Round < 25 ) ) '
			+ 'OR ( '
			+ '(Select Max(Round) as MaxRound From ' + @InEvent + 'Score SS2 Where SS2.SanctionId = SS.SanctionId AND SS2.MemberId = SS.MemberId AND SS2.AgeGroup = SS.AgeGroup AND SS2.Round < 25 ) <= ' + @curNumPreLim
			+ ' AND SS.PK = (Select Min(PK) From ' + @InEvent + 'Score PK '
			+ 'Where PK.SanctionId = TR.SanctionId AND PK.MemberId = SS.MemberId AND PK.AgeGroup = SS.AgeGroup AND PK.Round < 25 '
			+ 'AND PK.' + @curScoreAttr + ' = ( Select Max(' + @curScoreAttr + ') From ' + @InEvent + 'Score BS Where BS.SanctionId = PK.SanctionId AND BS.MemberId = PK.MemberId AND BS.AgeGroup = PK.AgeGroup AND Round < 25) ) ) )';
	ELSE IF @curScoreFormat = 'FIRST'
		SET @curScoreSelect = 'AND SS.Round = (Select Min(Round) as MaxRound '
			+ 'From ' + @InEvent + 'Score SS2 '
			+ 'Where SS2.SanctionId = SS.SanctionId AND SS2.MemberId = SS.MemberId AND SS2.AgeGroup = SS.AgeGroup AND SS2.Round < 25 )'
	ELSE IF @curScoreFormat = 'ROUND'
		SET @curScoreSelect = ' '
	ELSE 
		SET @curScoreSelect = ' '

	IF @InEvent = 'Jump'
		SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
			+ ', ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
			+ ', ER.RankingRating, ER.RankingScore, ''' + @curScoreFormat + ''' AS PlcmtFormat, ' + @curNumPreLim + ' AS NumPreLim'
			+ ', SS.Round, (Select Max(Round) as MaxRound From JumpScore SS2 Where SS2.SanctionId = SS.SanctionId AND SS2.MemberId = SS.MemberId AND SS2.AgeGroup = SS.AgeGroup AND SS2.Round < 25 ) as FinalRound'
			+ ', COALESCE (SS.Status, ''TBD'') AS Status, SS.NopsScore, SS.ScoreFeet as EventScore, SS.ScoreMeters'
			+ ', (Select RO.ScoreFeet From JumpScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff'
			+ ', TRIM(CAST(ROUND(SS.ScoreFeet, 0) AS CHAR)) + ''FT ('' + TRIM(CAST(ROUND(SS.ScoreMeters, 1) AS CHAR)) + ''M)'' AS EventScoreDesc '

			+ 'FROM TourReg AS TR '
			+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''' + @InEvent + ''' '
			+ 'INNER JOIN JumpScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup '
			+ @curScoreSelect

			+ ' Where TR.SanctionId = ''' + @InSanctionId + ''' '
			+ @curDivFilter
			+ 'Order by ' + @curSortCmd;
	ELSE IF @InEvent = 'Trick'
		SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
			+ ', ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
			+ ', ER.RankingRating, ER.RankingScore, ''' + @curScoreFormat + ''' AS PlcmtFormat, ' + @curNumPreLim + ' AS NumPreLim'
			+ ', SS.Round, (Select Max(Round) as MaxRound From TrickScore SS2 Where SS2.SanctionId = SS.SanctionId AND SS2.MemberId = SS.MemberId AND SS2.AgeGroup = SS.AgeGroup AND SS2.Round < 25 ) as FinalRound'
			+ ', COALESCE (SS.Status, ''TBD'') AS Status, SS.NopsScore, SS.Score as EventScore'
			+ ', TRIM(CAST(SS.Score AS CHAR)) + '' POINTS (P1:'' + TRIM(CAST(SS.ScorePass1 AS CHAR)) + '' P2:'' + TRIM(CAST(SS.ScorePass2 AS CHAR)) + '')'' AS EventScoreDesc'
			+ ', (Select RO.Score From TrickScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff'
			+ ', TV.Pass1VideoUrl, TV.Pass2VideoUrl '

			+ 'FROM TourReg AS TR '
			+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''' + @InEvent + ''' '
			+ 'INNER JOIN TrickScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup '
			+ @curScoreSelect
			+ ' LEFT OUTER JOIN TrickVideo AS TV ON TV.SanctionId = TR.SanctionId AND TV.MemberId = TR.MemberId AND TV.AgeGroup = TR.AgeGroup AND TV.Round = SS.Round '
			+ ' Where TR.SanctionId = ''' + @InSanctionId + ''' '
			+ @curDivFilter
			+ 'Order by ' + @curSortCmd;
	ELSE 
		SET @curSqlStmt = 'SELECT TR.SkierName, TR.SanctionId, TR.MemberId, TR.SkiYearAge, TR.AgeGroup, TR.AgeGroup as Div, TR.Gender, TR.City, TR.State, TR.Federation'
			+ ', ER.Event, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.EventGroup, ER.TeamCode, COALESCE(ER.ReadyForPlcmt, ''N'') as ReadyForPlcmt'
			+ ', ER.RankingRating, ER.RankingScore, ''' + @curScoreFormat + ''' AS PlcmtFormat, ' + @curNumPreLim + ' AS NumPreLim'
			+ ', SS.Round, (Select Max(Round) as MaxRound From SlalomScore SS2 Where SS2.SanctionId = SS.SanctionId AND SS2.MemberId = SS.MemberId AND SS2.AgeGroup = SS.AgeGroup AND SS2.Round < 25 ) as FinalRound'
			+ ', COALESCE (SS.Status, ''TBD'') AS Status, SS.NopsScore, SS.Score as EventScore, CAST(SS.Score AS CHAR) AS Buoys'
			+ ', (Select RO.Score From SlalomScore RO Where RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round = 25) as ScoreRunoff'
			+ ', TRIM(CAST(SS.FinalPassScore AS CHAR)) + '' @ '' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + ''mph '' + TRIM(SS.FinalLenOff) + '' ('' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + ''kph '' + TRIM(SS.FinalLen) + ''m)'' AS EventScoreDesc'
			+ ', TRIM(CAST(SS.FinalPassScore AS CHAR)) + '' @ '' + TRIM(CAST(SS.FinalSpeedKph AS CHAR)) + ''kph '' + TRIM(SS.FinalLen) + ''m'' AS EventScoreDescMeteric'
			+ ', TRIM(CAST(SS.FinalPassScore AS CHAR)) + '' @ '' + TRIM(CAST(SS.FinalSpeedMph AS CHAR)) + ''mph '' + TRIM(SS.FinalLenOff) AS EventScoreDescImperial'
	
			+ ' FROM TourReg AS TR '
			+ 'INNER JOIN EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = ''' + @InEvent + ''' '
			+ 'INNER JOIN SlalomScore AS SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup '
			+ @curScoreSelect

			+ ' Where TR.SanctionId = ''' + @InSanctionId + ''' '
			+ @curDivFilter
			+ 'Order by ' + @curSortCmd;

	EXEC sp_executesql @curSqlStmt;
	--print @curScoreSelect;
	--print @curSqlStmt;

END
GO

GRANT EXECUTE ON dbo.PrLeaderBoard  TO PUBLIC
GO
