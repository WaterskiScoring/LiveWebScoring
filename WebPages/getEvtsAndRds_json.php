<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
include_once( "WfwInit.php" );

$allEventsRounds=array();

if (isset($_POST['sanctionID'])) {
	$curSanctionID = $_POST['sanctionID'];

	$QueryCmd = "SELECT SlalomRounds, TrickRounds, JumpRounds FROM Tournament WHERE SanctionID='" .  $curSanctionID . "'"; // Events planned
	$QueryResult = $dbConnect->query($QueryCmd) or die ($dbConnect->error);
	$curRowCount = $QueryResult->num_rows;
	if ( $curRowCount != 0 ) {
		while ($curRow = $QueryResult->fetch_assoc()) {
			if ($curRow['SlalomRounds'] >= 0 ) $allEventsRounds['Slalom'] = $curRow['SlalomRounds'];
			if ($curRow['TrickRounds'] >= 0 ) $allEventsRounds['Trick'] = $curRow['TrickRounds'];
			if ($curRow['JumpRounds'] >= 0 ) $allEventsRounds['Jump'] = $curRow['JumpRounds'];
		}
	} else {
		echo "<span class='noScores'>No events have been assigned to this Sanction ID.</span>";
	}
	$QueryResult->free();
	echo json_encode($allEventsRounds);
} else {
	echo "Sanction ID Data Not Received.";
}
?>