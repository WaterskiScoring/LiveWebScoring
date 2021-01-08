<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start(); // base page pulling this file in already starts session
include_once( "WfwInit.php" );

function checkReqVars() {
  return isset($_GET['sanctionID'],$_GET['groupID'], $_GET['skiEvent']);
}

if (checkReqVars()) {
	$thisSanc = $_GET['sanctionID'];
	$thisSkiEvent = $_GET['skiEvent'];
	$thisGroup = $_GET['groupID'];
	$curRegion = strtoupper(substr($thisSanc, 2, 1));

	$thisSortOrder = "TR.SanctionId, ER.Event, ER.EventGroup, ER.ReadyForPlcmt, ER.RunOrder, ER.RankingScore ";
	if ( $thisSkiEvent == "Jump" AND $curRegion != "U") {
		$thisSortOrder = "TR.SanctionId, ER.Event, ER.EventGroup, TR.JumpHeight, ER.ReadyForPlcmt, ER.RunOrder, ER.RankingScore ";
	}
	if ( $curRegion == "U") {
		$thisSortOrder = "TR.SanctionId, ER.Event, ER.AgeGroup, ER.EventGroup, ER.RunOrder, ER.RankingScore ";
	}

	if ($thisGroup == "All" ) {
		$QueryCmd = "Select TR.SanctionId, TR.MemberId, TR.SkierName, TR.AgeGroup "
		. ", ER.Event, ER.EventGroup, ER.ReadyForPlcmt, ER.EventClass, ER.RunOrder, ER.RankingScore, ER.TeamCode, LastSkied "
		. "From TourReg TR "
		. "Inner Join EventReg ER on ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND TR.AgeGroup = ER.AgeGroup "
		. "Left Outer Join "
		. "(Select SanctionId, MemberId, AgeGroup, Max(LastUpdateDate) as LastSkied From "
		. $thisSkiEvent . "Score Group by SanctionId, MemberId, AgeGroup) SS "
		. "on SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup "
		. "Where TR.SanctionId = '" .  $thisSanc . "' AND ER.Event = '" .  $thisSkiEvent . "' "
		. "Order by " . $thisSortOrder;
	} else {
		// Retrieve scores for single group and round
		$QueryCmd = "Select TR.SanctionId, TR.MemberId, TR.SkierName, TR.AgeGroup "
		. ", ER.Event, ER.EventGroup, ER.ReadyForPlcmt, ER.EventClass, ER.RunOrder, ER.RankingScore, ER.TeamCode, LastSkied "
		. "From TourReg TR "
		. "Inner Join EventReg ER on ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND TR.AgeGroup = ER.AgeGroup "
		. "Left Outer Join "
		. "(Select SanctionId, MemberId, AgeGroup, Max(LastUpdateDate) as LastSkied From "
		. $thisSkiEvent . "Score Group by SanctionId, MemberId, AgeGroup) SS "
		. "on SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup "
		. "Where TR.SanctionId = '" .  $thisSanc . "' AND ER.Event = '" .  $thisSkiEvent . "' ";
		if ( $curRegion == "U") {
			$QueryCmd = $QueryCmd . "AND ER.AgeGroup = '"  .  $thisGroup . "' ";
		} else {
			$QueryCmd = $QueryCmd . "AND ER.EventGroup = '"  .  $thisGroup . "' ";
		}
		$QueryCmd = $QueryCmd . "Order by " . $thisSortOrder;
	}

	$QueryResult = $dbConnect->query($QueryCmd) or die ($dbConnect->error);
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
	echo "All required values not sent!!! " . $thisSanc . ", " . $thisSkiEvent . ", ", $thisGroup . ":";
}

?>