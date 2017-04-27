<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
session_start(); // base page pulling this file in already starts session
include_once( "WfwInit.php" );

function checkReqVars() {
  return isset($_POST['sanctionID'],$_POST['groupID'], $_POST['skiEvent']);
}

if (checkReqVars()) {
	$thisSanc = $_POST['sanctionID'];
	$thisSkiEvent = $_POST['skiEvent'];
	$thisGroup = $_POST['groupID'];
	$curRegion = strtoupper(substr($thisSanc, 2, 1));

	$thisSortOrder = "TR.SanctionId, ER.Event, ER.EventGroup, ER.RunOrder, ER.RankingScore ";
	if ( $curRegion == "U") {
		$thisSortOrder = "TR.SanctionId, ER.Event, ER.AgeGroup, ER.RunOrder, ER.RankingScore ";
	}

	if ($thisGroup == "All" ) {
		$QueryCmd = "Select TR.SanctionId, TR.MemberId, TR.SkierName, TR.AgeGroup "
		. ", ER.Event, ER.EventGroup, ER.EventClass, ER.RunOrder, ER.RankingScore, ER.TeamCode, LastSkied "
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
		. ", ER.Event, ER.EventGroup, ER.EventClass, ER.RunOrder, ER.RankingScore, ER.TeamCode, LastSkied "
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

	$curRowCount = $QueryResult->num_rows;
?>
<style>
.RunOrderGroup {
	Border: 5px #DDDDDD outset;
	Background: #396B9E;
	Color: White;
	font-size:1.1em;
	margin: auto;
	text-align:center;
}
.LastSkied {
	float:right;
	display:inline-block;
}

</style>
<?php
	if ( $curRowCount > 0 ) {
		$prevEventGroup = '';

		echo "\r\n<h2><p class='centeredItalic'>These are unofficial, repeat <span class='alertNotice'>UNOFFICIAL</span></p></h2>";
		echo "\r\n<ul data-role='listview' id='RunOrderID'>";
		while ($curDataRow = $QueryResult->fetch_assoc()) {
			if ( $curDataRow['EventGroup'] != $prevEventGroup ) {
				echo "\r\n<div data-role='header' class='RunOrderGroup'>Event Group: " . $curDataRow['EventGroup'] . "</div>";
			}

			echo "\r\n<li>";
			echo $curDataRow['SkierName'] . " (" . $curDataRow['AgeGroup'] . ")"
				. " Class: <Strong>" . $curDataRow['EventClass'] . "</Strong>"
				. " Order: <Strong>" . $curDataRow['RunOrder'] . "</Strong>"
				. " Rank: <Strong>" . number_format($curDataRow['RankingScore'],1) . "</Strong>"
				. " Team: <Strong>" . $curDataRow['TeamCode'] . "</Strong>";
			if ( $curDataRow['LastSkied'] == null ) {
				echo "\r\n<span class='LastSkied'> Last Skied: N/A";
			} else {
				echo "\r\n<span class='LastSkied'> Last Skied:" . date_format(date_create($curDataRow['LastSkied']),"m/d H:i");
			}
			echo "\r\n</span></li>";

			$prevEventGroup = $curDataRow['EventGroup'];
		}

		echo "\r\n</ul><!-- /listview -->";
		$QueryResult->free();
	} else {
		echo "<span class='noScores'>No running orders available yet for " . $thisGroup. ".</span>";
	}

} else {
	echo "All required values not sent!!!";
}

?>