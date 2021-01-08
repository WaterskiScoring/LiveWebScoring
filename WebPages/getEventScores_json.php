<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start(); // base page pulling this file in already starts session
include_once( "WfwInit.php" );

function checkReqVars() {
	$skiEvent = false;
	$sanctionID = false;

	if ( isset($_GET['skiEvent']) ) {
		$skiEvent = $_GET['skiEvent'];
		if ( $skiEvent == 'Recent') {
			return true;
		} else if ( $skiEvent == 'Team') {
			return isset($_GET['sanctionID']);
		} else {
			return isset($_GET['sanctionID'],$_GET['divisionID'], $_GET['skiEvent'], $_GET['Round']);
		}
	}
}

if (checkReqVars()) {
	$thisSanc = $_GET['sanctionID'];
	$thisSkiEvent = $_GET['skiEvent'];
	$thisShow = "US";
	if ( isset($_GET['show']) ) {
		$thisShow = $_GET['show'];
	}
	$thisDivision = '';

	if ( $thisSkiEvent == 'Recent') {
		$scoreField = "EventScore";
		$thisDivision = 'Recent';

		$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, tri.AgeGroup as AgeGroup, ssi.Score as EventScore, er.TeamCode, Round"
			. ", 'Slalom' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
			. ", CONCAT( CAST(ssi.Score as CHAR), ' buoys '"
			. ", CAST(FinalPassScore as CHAR), ' @ ', CAST(FinalSpeedMph as CHAR), 'mph ', FinalLenOff"
			. ", ' (', CAST(FinalSpeedKph as CHAR), 'kph ', FinalLen, 'm)') as EventScoreDesc "
			. "FROM TourReg tri "
			. "JOIN SlalomScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup = tri.AgeGroup "
			. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = 'Slalom' "
			. "WHERE ssi.SanctionID='" .  $thisSanc . "' AND ssi.LastUpdateDate >= CURDATE() "
			. "UNION "
			. "SELECT tri.SanctionId, tri.SkierName, tri.AgeGroup as AgeGroup, ssi.Score as EventScore, er.TeamCode, Round"
			. ", 'Trick' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
			. ", CONCAT(CAST(ssi.Score as CHAR), ' POINTS (P1:', CAST(ssi.ScorePass1 as CHAR), ' P2:', CAST(ssi.ScorePass2 as CHAR), ')' ) as EventScoreDesc "
			. "FROM TourReg tri "
			. "JOIN TrickScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup = tri.AgeGroup "
			. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = 'Trick' "
			. "WHERE ssi.SanctionID='" .  $thisSanc . "' AND ssi.LastUpdateDate >= CURDATE() "
			. "UNION "
			. "SELECT tri.SanctionId, tri.SkierName, tri.AgeGroup as AgeGroup, ssi.ScoreFeet as EventScore, er.TeamCode, Round"
			. ", 'Jump' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
			. ", CONCAT(CAST(ROUND(ssi.ScoreFeet, 0) as CHAR), 'FT (', CAST(ROUND(ssi.ScoreMeters, 1) as CHAR), 'M)' ) as EventScoreDesc "
			. "FROM TourReg tri "
			. "JOIN JumpScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup = tri.AgeGroup "
			. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = 'Jump' "
			. "WHERE ssi.SanctionID='" .  $thisSanc . "' AND ssi.LastUpdateDate >= CURDATE() "
			. "ORDER BY SortLastUpdateDate DESC, AgeGroup, Event";

	} else if ( $thisSkiEvent == 'Team') {
		$QueryCmd = "Select S.SanctionId, S.TeamCode, S.AgeGroup, Name, ReportFormat"
			. ", S.OverallPlcmt AS OverallPlcmtTeam, S.SlalomPlcmt AS SlalomPlcmtTeam, S.TrickPlcmt AS TrickPlcmtTeam, S.JumpPlcmt AS JumpPlcmtTeam"
			. ", S.OverallScore AS OverallScoreTeam, S.SlalomScore AS SlalomScoreTeam, S.TrickScore AS TrickScoreTeam, S.JumpScore AS JumpScoreTeam "
			. "From TeamScore S "
			. "Where S.SanctionId = '" . $thisSanc . "' "
			. "Order by S.AgeGroup, S.OverallPlcmt ";

	} else if ( $thisSkiEvent == 'Overall') {
		$_SESSION['skiRound'] = $_GET['Round'];
		$_SESSION['skiDivision'] = $_GET['divisionID'];

		$thisRound = $_GET['Round'];
		$thisDivision = $_GET['divisionID'];
		$thisDivisionFilter = "";
		if ($thisDivision == "All" ) {
			$thisDivisionFilter = "";
		} else {
			$thisDivisionFilter = "AND TR.AgeGroup='" .  $thisDivision . "' ";
		}

		$QueryCmd = "SELECT TR.SanctionId, TR.SkierName, TR.AgeGroup, 'Overall' as Event "
			. ", COALESCE(SS.NopsScore,0) + COALESCE(TS.NopsScore,0) + COALESCE(JS.NopsScore,0) as OverallScore "
			. ", SS.NopsScore as SlalomNopsScore, TS.NopsScore as TrickNopsScore, JS.NopsScore as JumpNopsScore "
			. ", COALESCE(SS.Round,COALESCE(TS.Round,COALESCE(JS.Round,0))) as Round "
			. ", DATE_FORMAT(SS.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, SS.LastUpdateDate AS SortLastUpdateDate "
			. "FROM TourReg TR "
			. "LEFT OUTER JOIN SlalomScore SS on SS.MemberId=TR.MemberId AND SS.SanctionId=TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round = '" . $thisRound . "' "
			. "LEFT OUTER JOIN TrickScore TS on TS.MemberId=TR.MemberId AND TS.SanctionId=TR.SanctionId AND TS.AgeGroup = TR.AgeGroup AND TS.Round = '" . $thisRound . "' "
			. "LEFT OUTER JOIN JumpScore JS on JS.MemberId=TR.MemberId AND JS.SanctionId=TR.SanctionId AND JS.AgeGroup = TR.AgeGroup AND JS.Round = '" . $thisRound . "' "
			. "WHERE TR.SanctionID='" .  $thisSanc . "' " . $thisDivisionFilter
			. "AND COALESCE(SS.Round,COALESCE(TS.Round,COALESCE(JS.Round,0))) > 0 "
			. "Order By TR.AgeGroup, TR.ReadyForPlcmt DESC, OverallScore DESC, TR.SkierName ";

			// Removed until I have a better way to determine overall eligibility
			//. "AND ((Select count(*) from EventReg ER Where ER.MemberId=TR.MemberId AND ER.SanctionId=TR.SanctionId AND ER.AgeGroup = TR.AgeGroup ) > 2 "
			//. "	OR (Select count(*) from EventReg ER Where ER.MemberId=TR.MemberId AND ER.SanctionId=TR.SanctionId AND ER.AgeGroup = TR.AgeGroup ) >= 2 "
			//. "		AND TR.AgeGroup in ('B1', 'G1', 'W8', 'W9', 'WA', 'WB', 'M8', 'M9', 'MA', 'MB')) "

	} else {
		$_SESSION['skiRound'] = $_GET['Round'];
		$_SESSION['skiDivision'] = $_GET['divisionID'];

		$thisRound = $_GET['Round'];
		$thisDivision = $_GET['divisionID'];
		$eventTable = $thisSkiEvent . "Score";
		if ($thisSkiEvent == "Jump") {
			$scoreField = "ScoreFeet";
		} else {
			$scoreField = "Score";
		}

		if ($thisDivision == "All" ) {
			$WhereDivCmd = "";
			$OrderCmd = "ORDER BY ssi.AgeGroup, er.ReadyForPlcmt DESC, ssi." . $scoreField . " DESC, RunOffScore DESC ";
		} else if ($thisDivision == "Recent" ) {
			$WhereDivCmd = "AND ssi.LastUpdateDate >= CURDATE() ";
			$OrderCmd = "ORDER BY ssi.LastUpdateDate DESC, ssi." . $scoreField . " DESC ";
		} else {
			$WhereDivCmd = "AND ssi.AgeGroup = '" .  $thisDivision . "' ";
			$OrderCmd = "ORDER BY ssi.AgeGroup, er.ReadyForPlcmt DESC, ssi." . $scoreField . " DESC, RunOffScore DESC ";
		}

		// Retrieve scores includes runoff score if available, assumes runoff applies to current round
		// Not sure how this is impacted for multi-round events
		if ($thisSkiEvent == "Slalom") {
			$ShowFinalPassScore = ", CONCAT( CAST(FinalPassScore as CHAR), ' @ ', CAST(FinalSpeedMph as CHAR), 'mph ', FinalLenOff) as LastPassScore";
			if ( $thisShow == "Metric" || $thisShow == "metric" ) {
				$ShowFinalPassScore = ", CONCAT( CAST(FinalPassScore as CHAR), ' @ ', CAST(FinalSpeedKph as CHAR), 'kph ', FinalLen, 'M') as LastPassScore";
			}

			$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, ssi.AgeGroup, Round"
				. ", '" . $thisSkiEvent . "' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
				. ", DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
				. ", ssi.Score as EventBouyCount"
				. $ShowFinalPassScore
				. ", (SELECT IFNULL(ssi2.Score,0) FROM TourReg tri2 "
				. "   JOIN SlalomScore ssi2 on ssi2.MemberId=tri2.MemberId AND ssi2.SanctionId=tri2.SanctionId AND ssi2.AgeGroup = tri2.AgeGroup "
				. "   WHERE ssi2.AgeGroup = ssi.AgeGroup AND ssi2.SanctionID = ssi.SanctionID AND ssi2.memberid = ssi.memberid AND ssi2.Round >= '25')"
				. "     AS RunOffScore "
				. "FROM TourReg tri "
				. "JOIN SlalomScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup=tri.AgeGroup "
				. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = '" . $thisSkiEvent . "' "
				. "WHERE ssi.SanctionID='" .  $thisSanc . "'  AND Round = '" . $thisRound . "' "
				. $WhereDivCmd
				. $OrderCmd;

		} else if ($thisSkiEvent == "Trick") {
			$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, ssi.AgeGroup, ssi.Round"
				. ", '" . $thisSkiEvent . "' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
				. ", DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
				. ", ssi.Score as EventScore, er.TeamCode "
				. ", CONCAT(CAST(ssi.Score as CHAR), ' POINTS (P1:', CAST(ssi.ScorePass1 as CHAR), ' P2:', CAST(ssi.ScorePass2 as CHAR), ')' ) as EventScoreDesc "
				. ", (SELECT IFNULL(ssi2.Score,0) FROM TourReg tri2 "
				. "JOIN TrickScore ssi2 on ssi2.MemberId=tri2.MemberId AND ssi2.SanctionId=tri2.SanctionId AND ssi2.AgeGroup = tri2.AgeGroup "
				. "WHERE ssi2.AgeGroup = ssi.AgeGroup "
				. "  AND ssi2.SanctionID = ssi.SanctionID "
				. "  AND ssi2.memberid = ssi.memberid "
				. "  AND ssi2.Round >= '25') AS RunOffScore "
				. "FROM TourReg tri "
				. "JOIN TrickScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup=tri.AgeGroup "
				. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = '" . $thisSkiEvent . "' "
				. "WHERE ssi.SanctionID='" .  $thisSanc . "'  AND ssi.Round = '" . $thisRound . "' "
				. $WhereDivCmd
				. $OrderCmd;

		} else if ($thisSkiEvent == "TrickVideo") {
			$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, ssi.AgeGroup, ssi.Round"
				. ", '" . $thisSkiEvent . "' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
				. ", DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
				. ", ssi.Score as EventScore, er.TeamCode "
				. ", CONCAT(CAST(ssi.Score as CHAR), ' POINTS (P1:', CAST(ssi.ScorePass1 as CHAR), ' P2:', CAST(ssi.ScorePass2 as CHAR), ')' ) as EventScoreDesc "
				. ", IFNULL(V.Pass1VideoUrl, '') as Pass1VideoUrl, IFNULL(V.Pass2VideoUrl, '') as Pass2VideoUrl "
				. ", (SELECT IFNULL(ssi2.Score,0) FROM TourReg tri2 "
				. "JOIN TrickScore ssi2 on ssi2.MemberId=tri2.MemberId AND ssi2.SanctionId=tri2.SanctionId AND ssi2.AgeGroup = tri2.AgeGroup "
				. "WHERE ssi2.AgeGroup = ssi.AgeGroup "
				. "  AND ssi2.SanctionID = ssi.SanctionID "
				. "  AND ssi2.memberid = ssi.memberid "
				. "  AND ssi2.Round >= '25') AS RunOffScore "
				. "FROM TourReg tri "
				. "JOIN TrickScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup=tri.AgeGroup "
				. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = '" . $thisSkiEvent . "' "
				. "Left Outer Join TrickVideo V ON V.SanctionId = ssi.SanctionId AND V.MemberId = ssi.MemberId AND V.AgeGroup = ssi.AgeGroup AND V.Round = ssi.Round "
				. "WHERE ssi.SanctionID='" .  $thisSanc . "'  AND ssi.Round = '" . $thisRound . "' "
				. $WhereDivCmd
				. $OrderCmd;

		} else if ($thisSkiEvent == "Jump") {
			$ShowScore = ", CONCAT(CAST(ROUND(ssi.ScoreFeet, 0) as CHAR), ' Feet' ) as JumpDistance ";
			if ( $thisShow == "Metric" || $thisShow == "metric" ) {
				$ShowScore = ", CONCAT(CAST(ROUND(ssi.ScoreMeters, 1) as CHAR), ' Meters' ) as JumpDistance ";
			}
			$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, ssi.AgeGroup, Round"
				. ", '" . $thisSkiEvent . "' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
				. ", DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
				. $ShowScore
				. ", (SELECT IFNULL(ssi2.ScoreFeet,0) FROM TourReg tri2 "
				. "   JOIN JumpScore ssi2 on ssi2.MemberId=tri2.MemberId AND ssi2.SanctionId=tri2.SanctionId AND ssi2.AgeGroup = tri2.AgeGroup "
				. "   WHERE ssi2.AgeGroup = ssi.AgeGroup AND ssi2.SanctionID = ssi.SanctionID AND ssi2.memberid = ssi.memberid AND ssi2.Round >= '25')"
				. "   AS RunOffScore "
				. "FROM TourReg tri "
				. "JOIN JumpScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup=tri.AgeGroup "
				. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = '" . $thisSkiEvent . "' "
				. "WHERE ssi.SanctionID='" .  $thisSanc . "'  AND Round = '" . $thisRound . "' "
				. $WhereDivCmd
				. $OrderCmd;

		}

	}

	$QueryResult = $dbConnect->query($QueryCmd);
	if ($dbConnect->error) {
		echo "An error was encountered running a query: " . $dbConnect->error;
		exit(500);
	} else {
		$curRowCount = $QueryResult->num_rows;
		if ( $curRowCount != 0 ) {
			$resultDataRows = array();
			while ($ScoresRow = $QueryResult->fetch_assoc()) {
				$resultDataRows[]=$ScoresRow;
			}
			echo json_encode($resultDataRows);
		}
	}

} else {
	echo "All required values not sent!!!";
}

?>