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
	$QueryResult = mysql_query($QueryCmd) or die (mysql_error());
	$curRowCount = mysql_num_rows($QueryResult);
	if ( $curRowCount != 0 ) {
		while ($curRow = mysql_fetch_assoc($QueryResult)) {
			$eventGroups[] = $curRow['EventGroup'];
		}
		$eventGroups[] = 'All';
	} else {
		echo "<span class='noScores'>No groups have been defined yet.</span>" . $_POST['sanctionID'] . " - " . $_POST['skiEvent'];
	}
    /* close statement */
	mysql_free_result($QueryResult);

	echo json_encode($eventGroups);
} else {
	echo "Tournament sanction and event information not provided: SanctionId=" . $_POST['sanctionID'] . " - Evemt=" . $_POST['skiEvent'];
}
?>