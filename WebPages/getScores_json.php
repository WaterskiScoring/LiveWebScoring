<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start(); // base page pulling this file in already starts session
include_once( "WfwInit.php" );

function checkReqVars() {
	$skiEvent = false;
	$sanctionID = false;

	if ( isset($_POST['skiEvent']) ) {
		$skiEvent = $_POST['skiEvent'];
		if ( $skiEvent == 'Recent') {
			return true;
		} else if ( $skiEvent == 'Team') {
			return isset($_POST['sanctionID']);
		} else {
			return isset($_POST['sanctionID'],$_POST['divisionID'], $_POST['skiEvent'], $_POST['changeRoundSelector']);
		}
	}
}
?>
<style>
.GroupHeader {
	Border: 5px #DDDDDD outset;
	Background: #396B9E;
	Color: White;
	font-size:1.1em;
	padding: 10px;
	margin: auto;
	text-align:center;
}
.RunOrderCount {
	font-size:.85em;
}

</style>
<?php
if (checkReqVars()) {
	$thisSanc = $_POST['sanctionID'];
	$thisSkiEvent = $_POST['skiEvent'];
	$thisDivision = '';

	if ( $thisSkiEvent == 'Recent') {
		$scoreField = "EventScore";
		$thisDivision = 'Recent';

		$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, tri.MemberId, tri.AgeGroup as AgeGroup, ssi.Score as EventScore, er.TeamCode, Round"
			. ", 'Slalom' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
			. ", CONCAT( CAST(ssi.Score as CHAR), ' buoys '"
			. ", CAST(FinalPassScore as CHAR), ' @ ', CAST(FinalSpeedMph as CHAR), 'mph ', FinalLenOff"
			. ", ' (', CAST(FinalSpeedKph as CHAR), 'kph ', FinalLen, 'm)') as EventScoreDesc "
			. "FROM TourReg tri "
			. "JOIN SlalomScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup = tri.AgeGroup "
			. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = 'Slalom' "
			. "WHERE ssi.SanctionID='" .  $thisSanc . "' AND ssi.LastUpdateDate >= CURDATE() "
			. "UNION "
			. "SELECT tri.SanctionId, tri.SkierName, tri.MemberId, tri.AgeGroup as AgeGroup, ssi.Score as EventScore, er.TeamCode, Round"
			. ", 'Trick' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
			. ", CONCAT(CAST(ssi.Score as CHAR), ' POINTS (P1:', CAST(ssi.ScorePass1 as CHAR), ' P2:', CAST(ssi.ScorePass2 as CHAR), ')' ) as EventScoreDesc "
			. "FROM TourReg tri "
			. "JOIN TrickScore ssi on ssi.MemberId=tri.MemberId AND ssi.SanctionId=tri.SanctionId AND ssi.AgeGroup = tri.AgeGroup "
			. "JOIN EventReg er on er.MemberId=tri.MemberId AND er.SanctionId=tri.SanctionId AND er.AgeGroup=tri.AgeGroup AND er.Event = 'Trick' "
			. "WHERE ssi.SanctionID='" .  $thisSanc . "' AND ssi.LastUpdateDate >= CURDATE() "
			. "UNION "
			. "SELECT tri.SanctionId, tri.SkierName, tri.MemberId, tri.AgeGroup as AgeGroup, ssi.ScoreFeet as EventScore, er.TeamCode, Round"
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
		$_SESSION['skiRound'] = $_POST['changeRoundSelector'];
		$_SESSION['skiDivision'] = $_POST['divisionID'];

		$thisRound = $_POST['changeRoundSelector'];
		$thisDivision = $_POST['divisionID'];
		$thisDivisionFilter = "";
		if ($thisDivision == "All" ) {
			$thisDivisionFilter = "";
		} else {
			$thisDivisionFilter = "AND TR.AgeGroup='" .  $thisDivision . "' ";
		}

		$QueryCmd = "SELECT TR.SanctionId, TR.SkierName, TR.MemberId, TR.AgeGroup, 'Overall' as Event "
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
		$_SESSION['skiRound'] = $_POST['changeRoundSelector'];
		$_SESSION['skiDivision'] = $_POST['divisionID'];

		$thisRound = $_POST['changeRoundSelector'];
		$thisDivision = $_POST['divisionID'];
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
			$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, ssi.MemberId, ssi.AgeGroup, Round"
				. ", '" . $thisSkiEvent . "' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
				. ", DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
				. ", ssi.Score as EventScore, er.TeamCode "
				. ", CONCAT( CAST(ssi.Score as CHAR), ' buoys '"
				. ", CAST(FinalPassScore as CHAR), ' @ ', CAST(FinalSpeedMph as CHAR), 'mph ', FinalLenOff"
				. ", ' (', CAST(FinalSpeedKph as CHAR), 'kph ', FinalLen, 'm)') as EventScoreDesc "
				. ", (SELECT IFNULL(ssi2.Score,0) "
				. "   FROM TourReg tri2 "
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
			$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, ssi.MemberId, ssi.AgeGroup, ssi.Round"
				. ", '" . $thisSkiEvent . "' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
				. ", DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
				. ", ssi.Score as EventScore, er.TeamCode "
				. ", CONCAT(CAST(ssi.Score as CHAR), ' POINTS (P1:', CAST(ssi.ScorePass1 as CHAR), ' P2:', CAST(ssi.ScorePass2 as CHAR), ')' ) as EventScoreDesc "
				. ", IFNULL(V.Pass1VideoUrl, '') as Pass1VideoUrl, IFNULL(V.Pass2VideoUrl, '') as Pass2VideoUrl "
				. ", (SELECT IFNULL(ssi2.Score,0) "
				. "FROM TourReg tri2 "
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
			$QueryCmd = "SELECT tri.SanctionId, tri.SkierName, ssi.MemberId, ssi.AgeGroup, Round"
				. ", '" . $thisSkiEvent . "' as Event, DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate "
				. ", DATE_FORMAT(ssi.LastUpdateDate, '%Y/%m/%d %h:%i %p') as LastUpdateDate, ssi.LastUpdateDate AS SortLastUpdateDate "
				. ", ssi.ScoreFeet as EventScore, er.TeamCode "
				. ", CONCAT(CAST(ROUND(ssi.ScoreFeet, 0) as CHAR), 'FT (', CAST(ROUND(ssi.ScoreMeters, 1) as CHAR), 'M)' ) as EventScoreDesc "
				. ", (SELECT IFNULL(ssi2.ScoreFeet,0) "
				. "FROM TourReg tri2 "
				. "JOIN JumpScore ssi2 on ssi2.MemberId=tri2.MemberId AND ssi2.SanctionId=tri2.SanctionId AND ssi2.AgeGroup = tri2.AgeGroup "
				. "WHERE ssi2.AgeGroup = ssi.AgeGroup "
				. "  AND ssi2.SanctionID = ssi.SanctionID "
				. "  AND ssi2.memberid = ssi.memberid "
				. "  AND ssi2.Round >= '25') AS RunOffScore "
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
			$prevGroup = '';

			echo "<h2><p class='centeredItalic'><br/>These scores are unofficial, repeat <span class='alertNotice'>UNOFFICIAL</span></p></h2>";

			if ( $thisSkiEvent == 'Overall') {
				while ($ScoresRow = $QueryResult->fetch_assoc()) {
					if ( $ScoresRow['AgeGroup'] != $prevGroup ) {
						if ( $prevGroup != '' ) {
							echo "\r\n</ul>";
						}

						echo "\r\n<div data-role='header' class='GroupHeader'>Division: " . $ScoresRow['AgeGroup'] . "</div>";
						echo "\r\n<ul data-role='listview' id='scoresID'>";

					}

					echo "<li>\r\n";
					echo "<a href='wfwShowScoreRecap.php?SanctionId=" . $ScoresRow['SanctionId'] . "&MemberId=" . $ScoresRow['MemberId'] . "&AgeGroup=" . $ScoresRow['AgeGroup'] . "&SkierName=" . str_replace("'","&#039;", $ScoresRow['SkierName']) . "&SkierRound=" . $ScoresRow['Round'] . "&SkierEvent=" . $ScoresRow['Event'] . "' "
					. "data-rel='dialog' data-transition='pop'>" . str_replace("'","&#039;", $ScoresRow['SkierName'])
					. "<span class='lastUpdateTime'> (" . $ScoresRow['AgeGroup'] . " - " . $ScoresRow['Event'] . ") </span>\r\n";
					echo "<span class='score'>" . $ScoresRow['OverallScore']  ." Points ("
						. $ScoresRow['SlalomNopsScore'] . ", " . $ScoresRow['TrickNopsScore'] . ", " . $ScoresRow['JumpNopsScore']
						. ") </span></a></li>\r\n";

					$prevGroup = $ScoresRow['AgeGroup'];
				}

			} else if ( $thisSkiEvent == 'Team') {
				$RowCount = 0;
				while ($ScoresRow = $QueryResult->fetch_assoc()) {
					if ( $ScoresRow['AgeGroup'] != $prevGroup || $RowCount == 0) {
						if ( $prevGroup != '' ) {
							echo "\r\n</ul>";
						}

						echo "\r\n<div data-role='header' class='GroupHeader'>Division: " . $ScoresRow['AgeGroup'] . "</div>";
						echo "\r\n<ul data-role='listview' id='scoresID'>";
					}

					echo "<li>\r\n";
					echo "<a href='wfwShowScoreRecapTeam.php?SanctionId=" . $ScoresRow['SanctionId'] . "&TeamCode=" . $ScoresRow['TeamCode'] . "&AgeGroup=" . $ScoresRow['AgeGroup'] . "&ReportFormat=" . $ScoresRow['ReportFormat'] . "' "
					. "data-rel='dialog' data-transition='pop'>" . $ScoresRow['TeamCode'] . "-" . $ScoresRow['Name']
					. "<span class='lastUpdateTime'> (" . $ScoresRow['AgeGroup'] . " - " . $ScoresRow['TeamCode'] . ") </span>\r\n";
					echo "<span class='score'>" . $ScoresRow['OverallScoreTeam']  ." Points ("
						. $ScoresRow['SlalomScoreTeam'] . ", " . $ScoresRow['TrickScoreTeam'] . ", " . $ScoresRow['JumpScoreTeam']
						. ") </span></a></li>\r\n";

					$prevGroup = $ScoresRow['AgeGroup'];
					$RowCount++;
				}
				echo "\r\n</ul>";


			} else {
				while ($ScoresRow = $QueryResult->fetch_assoc()) {
					if ($thisDivision == "Recent" || $thisSkiEvent == 'Recent' ) {
						if ( $prevGroup == '' ) {
							echo "\r\n<ul data-role='listview' id='scoresID'>";
						}
					} else {
						if ( $ScoresRow['AgeGroup'] != $prevGroup ) {
							if ( $prevGroup != '' ) {
								echo "\r\n</ul>";
							}
							$QueryCmd = "Select count(*) as SkierCount from EventReg R "
							. "WHERE R.SanctionID='" .  $thisSanc . "' And Event = '" . $thisSkiEvent . "' And AgeGroup = '" . $ScoresRow['AgeGroup'] . "' "
							. "And Not Exists (Select 1 From " . $eventTable . " S Where R.SanctionId = S.SanctionId And R.MemberId = S.MemberId "
							. "    And R.AgeGroup = S.AgeGroup And S.Round = '" . $thisRound . "') ";
							$CountQueryResult = $dbConnect->query($QueryCmd);
							if ($dbConnect->error) {
								echo "An error was encountered running a query: " . $dbConnect->error;
							} else {
								$curRowCount = $QueryResult->num_rows;
								if ( $curRowCount != 0 ) {
									$CountRow = $CountQueryResult->fetch_assoc();
									echo "\r\n<div data-role='header' class='GroupHeader'>Division: " . $ScoresRow['AgeGroup']
									. "<BR/><span class='RunOrderCount'>(". $CountRow['SkierCount'] . " skiers remaining in division)</span></div>";
								} else {
									echo "\r\n<div data-role='header' class='GroupHeader'>Division: " . $ScoresRow['AgeGroup'] . "</div>";
								}
								$CountQueryResult->free();

								echo "\r\n<ul data-role='listview' id='scoresID'>";
							}
						}
					}

					echo "<li>\r\n";
					echo "<a href='wfwShowScoreRecap.php?SanctionId=" . $ScoresRow['SanctionId'] . "&MemberId=" . $ScoresRow['MemberId'] . "&AgeGroup=" . $ScoresRow['AgeGroup']
						. "&SkierName=" . str_replace("'","&#039;", $ScoresRow['SkierName'])
						. "&SkierRound=" . $ScoresRow['Round']
						. "&SkierEvent=" . $ScoresRow['Event']
						. "' data-rel='dialog' data-transition='pop'>" . str_replace("'","&#039;", $ScoresRow['SkierName'])
						. "<span class='lastUpdateTime'> ("
						. $ScoresRow['AgeGroup'] . " " . $ScoresRow['TeamCode'] . " " . $ScoresRow['Event'] . ") - " . $ScoresRow['LastUpdateDate']
						. "</span>\r\n";

					if ($ScoresRow['Event'] == "Jump") {
						echo "<span class='score'>" . $ScoresRow['EventScoreDesc']  ."</span></a></li>\r\n";
					} else if ( ($ScoresRow['Event'] == "Trick") && ($thisSkiEvent == "Trick") ) {
						if ($ScoresRow['Pass1VideoUrl'] != "" || $ScoresRow['Pass2VideoUrl'] != "") {
							echo "<sup class='videoNote'>V</sup>" ;
						}

						echo "<span class='score'>" . $ScoresRow['EventScoreDesc']  ."</span></a></li>\r\n";
					} else if ($ScoresRow['Event'] == "Slalom") {
						echo "<span class='score'>" . $ScoresRow['EventScoreDesc']  ."</span></a></li>\r\n";
					} else {
						echo "<span class='score'>recent</span></a></li>\r\n";
					}

					$prevGroup = $ScoresRow['AgeGroup'];
				}
			}

			echo "</ul><!-- /listview -->";
			$QueryResult->free();
		} else {
			echo "<span class='noScores'>No scores available yet for " . $thisDivision . ".</span>";
		}
	}

} else {
	echo "All required values not sent!!!";
}

?>