<?php
error_reporting(E_ALL);
ini_set("display_errors", 1);
include_once( "WfwInit.php" );

$eventGroups=array();

if (isset($_POST['sanctionID']) && isset($_POST['skiEvent'])) {
	$curSanctionID = $_POST['sanctionID'];
	$curSkiEvent = $_POST['skiEvent'];
	$curRegion = strtoupper(substr($curSanctionID, 2, 1));

	// Retrieve divisions currently scored for the specified tournament and event
	$QueryCmd = "SELECT DISTINCT EventGroup FROM EventReg WHERE SanctionID='" . $curSanctionID . "' AND Event='" . $curSkiEvent . "' ORDER BY EventGroup ASC";
	if ( $curRegion == "U") {
		$QueryCmd = "SELECT DISTINCT AgeGroup as EventGroup FROM EventReg WHERE SanctionID='" . $curSanctionID . "' AND Event='" . $curSkiEvent . "' ORDER BY AgeGroup ASC";
	}
	$QueryResult = $dbConnect->query($QueryCmd) or die ($dbConnect->error);
	$curRowCount = $QueryResult->num_rows;
	if ( $curRowCount != 0 ) {
		while ($curRow = $QueryResult->fetch_assoc()) {
			$eventGroups[] = $curRow['EventGroup'];
		}
		$eventGroups[] = 'All';
	} else {
		echo "<span class='noScores'>No groups have been defined yet.</span>" . $_POST['sanctionID'] . " - " . $_POST['skiEvent'];
	}
    /* close statement */
	$QueryResult->free();

	echo json_encode($eventGroups);
} else {
	echo "Tournament sanction and event information not provided";
}
?>